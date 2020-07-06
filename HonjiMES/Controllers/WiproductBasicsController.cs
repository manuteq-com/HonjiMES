using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;

namespace HonjiMES.Controllers
{
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
        public async Task<ActionResult<IEnumerable<WiproductBasic>>> GetWiproductBasics()
        {
            var wiproductBasic = await _context.WiproductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.UpdateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(wiproductBasic));
        }

        // GET: api/WiproductBasics
        [HttpGet("{id}")]
        public async Task<ActionResult<WiproductBasic>> GetWiproductBasic(int id)
        {
            var wiproductBasic = await _context.WiproductBasics.FindAsync(id);

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
            OwiproductBasic.UpdateTime = DateTime.Now;
            OwiproductBasic.UpdateUser = 1;

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
                    CreateUser = 1
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
                        CreateUser = 1
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

            _context.WiproductBasics.Remove(wiproductBasic);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(wiproductBasic));
        }

        private bool WiproductBasicExists(int id)
        {
            return _context.WiproductBasics.Any(e => e.Id == id);
        }
    }
}
