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
    /// <summary>
    /// 產品列表
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WiproductsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WiproductsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Wiproducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wiproduct>>> GetWiproducts()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Wiproducts.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Wiproducts = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Wiproducts));
            //return Ok(new { data = Wiproducts, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Wiproducts
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Wiproduct>>> GetWiproductsById(int? id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Wiproducts.AsQueryable().Where(x => x.DeleteFlag == 0);
            if (id.HasValue)
            {
                data = data.Where(x => x.WiproductBasicId == id);
            }
            var Wiproducts = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Wiproducts));
            //return Ok(new { data = Wiproducts, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Wiproducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WiproductBasic>>> GetWiproductBasics()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.WiproductBasics.AsQueryable().Where(x => x.DeleteFlag == 0);
            var WiproductBasics = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(WiproductBasics));
            //return Ok(new { data = Wiproducts, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 使用ID查詢產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Wiproducts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wiproduct>> GetWiproduct(int id)
        {
            // var wiproduct = await _context.Wiproducts.FindAsync(id);
            var wiproduct = await _context.Wiproducts.AsQueryable().Where(x => x.Id == id).Include(x => x.Warehouse).FirstAsync();

            if (wiproduct == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(wiproduct));
            //return Ok(new { data = wiproduct, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 修改產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wiproduct"></param>
        /// <returns></returns>
        // PUT: api/Wiproducts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWiproduct(int id, Wiproduct wiproduct)
        {
            wiproduct.Id = id;
            var Owiproduct = _context.Wiproducts.Find(id);
            var Cwiproduct = Owiproduct;
            if (!string.IsNullOrWhiteSpace(wiproduct.WiproductNo))
            {
                Cwiproduct.WiproductNo = wiproduct.WiproductNo;
            }
            if (wiproduct.WarehouseId != 0)
            {
                Cwiproduct.WarehouseId = wiproduct.WarehouseId;
            }
            //修改時檢查[品號][倉庫]是否重複
            if (_context.Wiproducts.AsQueryable().Where(x => x.Id != id && x.WiproductNo == Cwiproduct.WiproductNo  && x.WarehouseId == Cwiproduct.WarehouseId && x.DeleteFlag == 0).Any())
            {
                var warehouse = _context.Warehouses.Find(Cwiproduct.WarehouseId);
                return Ok(MyFun.APIResponseError("成品的品號 [" + Cwiproduct.WiproductNo + "] 與存放褲別 [" + warehouse.Name + "] 重複!", Cwiproduct));
            }
            
            var Msg = MyFun.MappingData(ref Owiproduct, wiproduct);
            Owiproduct.UpdateTime = DateTime.Now;
            Owiproduct.UpdateUser = 1;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WiproductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(Owiproduct));
            //return Ok(new { success = true, timestamp = DateTime.Now, message = "" });
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
                return Ok(MyFun.APIResponseError("[主件品號] 不存在，請確認資訊是否正確。"));
            } else {
                wiproduct.WiproductBasicId = WiproductsBasicData.Id;
            }

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

        /// <summary>
        /// 刪除產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Wiproducts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Wiproduct>> DeleteWiproduct(int id)
        {
            var wiproduct = await _context.Wiproducts.FindAsync(id);
            if (wiproduct == null)
            {
                return NotFound();
            }
            wiproduct.DeleteFlag = 1;
            // _context.Wiproducts.Remove(wiproduct);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(wiproduct));
            //return Ok(new { data = wiproduct, success = true, timestamp = DateTime.Now, message = "" });
        }

        private bool WiproductExists(int id)
        {
            return _context.Wiproducts.Any(e => e.Id == id);
        }
    }
}
