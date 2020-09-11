using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;
using DevExtreme.AspNet.Mvc;

namespace HonjiMES.Controllers
{
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WiproductBasicsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WiproductBasicsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/WiproductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasicData>>> GetWiproductBasics(
            [FromQuery] DataSourceLoadOptions FromQuery)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var wiproductBasic = _context.WiproductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.Wiproducts.OrderByDescending(y => y.UpdateTime).FirstOrDefault().UpdateTime).Include(x => x.Wiproducts).Select(x => new WiproductBasicData
            {
                TotalCount = x.Wiproducts.Where(y => y.DeleteFlag == 0).Sum(y => y.Quantity),
                Id = x.Id,
                WiproductNo = x.WiproductNo,
                WiproductNumber = x.WiproductNumber,
                Name = x.Name,
                Specification = x.Specification,
                Property = x.Property,
                Price = x.Price,
                SubInventory = x.SubInventory,
                SupplierId = x.SupplierId,
                Remarks = x.Remarks,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                DeleteFlag = x.DeleteFlag,
                Wiproducts = x.Wiproducts
            });
            
            // 排除預設排序 ID
            if (FromQuery.Sort != null)
            {
                var SortingInfoList = FromQuery.Sort.Where(x => x.Selector != "Id").ToArray();
                FromQuery.Sort = SortingInfoList;
            }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(wiproductBasic, FromQuery);
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/WiproductBasics
        [HttpGet("{id}")]
        public async Task<ActionResult<WiproductBasic>> GetWiproductBasic(int id)
        {
            var wiproductBasic = await _context.WiproductBasics.FindAsync(id);
            var wiproducts = _context.Wiproducts.Where(x => x.WiproductBasicId == id && x.DeleteFlag == 0).ToList();
            wiproductBasic.Wiproducts = wiproducts;

            if (wiproductBasic == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(wiproductBasic));
        }

        // PUT: api/WiproductBasics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWiproductBasic(int id, WiproductBasic wiproductBasic)
        {
            wiproductBasic.Id = id;
            var OwiproductBasic = _context.WiproductBasics.Find(id);
            var Cwiproduct = OwiproductBasic;
            if (!string.IsNullOrWhiteSpace(wiproductBasic.WiproductNo))
            {
                Cwiproduct.WiproductNo = wiproductBasic.WiproductNo;
            }

            var Msg = MyFun.MappingData(ref OwiproductBasic, wiproductBasic);
            OwiproductBasic.SupplierId = wiproductBasic.SupplierId;
            OwiproductBasic.UpdateTime = DateTime.Now;
            OwiproductBasic.UpdateUser = MyFun.GetUserID(HttpContext);

            //更新完basic後，同步更新底下資料
            var Wiproducts = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == OwiproductBasic.Id && x.DeleteFlag == 0);
            foreach (var item in Wiproducts)
            {
                item.WiproductNo = OwiproductBasic.WiproductNo;
                item.WiproductNumber = OwiproductBasic.WiproductNumber;
                item.Name = OwiproductBasic.Name;
                item.Specification = OwiproductBasic.Specification;
                item.Property = OwiproductBasic.Property;
                item.Price = OwiproductBasic.Price;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiproductBasicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(OwiproductBasic));
            //return Ok(new { success = true, timestamp = DateTime.Now, message = "" });
        }

        // POST: api/WiproductBasics
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WiproductBasic>> PostWiproductBasic(WiproductBasic wiproductBasic)
        {
            _context.WiproductBasics.Add(wiproductBasic);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(wiproductBasic));
        }

        /// <summary>
        /// 新增產品列表
        /// </summary>
        /// <param name="wiproduct"></param>
        /// <returns></returns>
        // POST: api/Wiproducts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Wiproduct>> PostWiproduct(WiproductW wiproduct)
        {
            if (wiproduct.wid == null) {
                return Ok(MyFun.APIResponseError("請選擇 [存放庫別]!", wiproduct));
            }

            //優先確認Basic是否存在
            var WiproductsBasicData = _context.WiproductBasics.AsQueryable().Where(x => x.WiproductNo == wiproduct.WiproductNo && x.DeleteFlag == 0).FirstOrDefault();
            if (WiproductsBasicData == null)
            {
                var WiproductBasics = new List<WiproductBasic>();
                _context.WiproductBasics.Add(new WiproductBasic
                {
                    WiproductNo = wiproduct.WiproductNo,
                    WiproductNumber = wiproduct.WiproductNumber,
                    Name = wiproduct.Name,
                    Specification = wiproduct.Specification,
                    Property = wiproduct.Property,
                    Price = wiproduct.Price,
                    SubInventory = wiproduct.SubInventory,
                    SupplierId = wiproduct.SupplierId,
                    Remarks = wiproduct.Remarks,
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                _context.SaveChanges();
                WiproductsBasicData = _context.WiproductBasics.AsQueryable().Where(x => x.WiproductNo == wiproduct.WiproductNo && x.DeleteFlag == 0).FirstOrDefault();
            } else {
                return Ok(MyFun.APIResponseError("[主件品號] 已存在!"));
            }
            wiproduct.WiproductBasicId = WiproductsBasicData.Id;

            string sRepeatWiproduct = null;
            var nWiproductlist = new List<Wiproduct>();
            foreach (var warehouseId in wiproduct.wid)
            {
                //新增時檢查主件品號是否重複
                if (_context.Wiproducts.AsQueryable().Where(x => x.WiproductNo == wiproduct.WiproductNo && x.WarehouseId == warehouseId && x.DeleteFlag == 0).Any())
                {
                    sRepeatWiproduct += "主件品號 [" + wiproduct.WiproductNo + "] 已經存在 [" + wiproduct.warehouseData[warehouseId - 1].Name + "] !<br/>";
                }
                else
                {
                    nWiproductlist.Add(new Wiproduct
                    {
                        WiproductNo = wiproduct.WiproductNo,
                        WiproductNumber = wiproduct.WiproductNumber,
                        Name = wiproduct.Name,
                        Quantity = wiproduct.Quantity,
                        QuantityLimit = wiproduct.QuantityLimit,
                        Specification = wiproduct.Specification,
                        Property = wiproduct.Property,
                        Price = wiproduct.Price,
                        MaterialId = wiproduct.MaterialId,
                        MaterialRequire = 1,
                        SubInventory = wiproduct.SubInventory,
                        WarehouseId = warehouseId,
                        WiproductBasicId = wiproduct.WiproductBasicId,
                         CreateUser = MyFun.GetUserID(HttpContext)
                    });
                }
            }

            if (sRepeatWiproduct == null)
            {
                _context.AddRange(nWiproductlist);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(wiproduct));
            }
            else
            {
                // return BadRequest("主件品號：" + wiproduct.WiproductNo + "重複");
                return Ok(MyFun.APIResponseError(sRepeatWiproduct, wiproduct));
            }
            //return Ok(new { data = CreatedAtAction("GetWiproduct", new { id = wiproduct.Id }, wiproduct), success = true });
        }

        // DELETE: api/WiproductBasics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WiproductBasic>> DeleteWiproductBasic(int id)
        {
            var wiproductBasic = await _context.WiproductBasics.FindAsync(id);
            if (wiproductBasic == null)
            {
                return NotFound();
            }
            wiproductBasic.DeleteFlag = 1;
            // _context.WiproductBasics.Remove(wiproductBasic);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(wiproductBasic));
        }

        [HttpGet]
        public async Task<ActionResult<WiproductBasic>> CheckWiproductNumber(string DataNo)
        {
            var wiproductBasicNo = await _context.WiproductBasics.Where(x => x.WiproductNo == DataNo && x.DeleteFlag == 0).AnyAsync();
            if (wiproductBasicNo) {
                return Ok(MyFun.APIResponseError("[成品品號]已存在!"));
            }
            return Ok(MyFun.APIResponseOK(""));
        }

        private bool WiproductBasicExists(int id)
        {
            return _context.WiproductBasics.Any(e => e.Id == id);
        }
    }
}
