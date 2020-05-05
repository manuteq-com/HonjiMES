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
    /// 倉庫
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WarehousesController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        ///  倉庫列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Warehouses.Where(x => x.DeleteFlag == 0);
            var Warehouses = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Warehouses));
        }

        /// <summary>
        /// 用ID最倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(warehouse));
        }
        /// <summary>
        /// 新增倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        // PUT: api/Warehouses/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, Warehouse warehouse)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            warehouse.Id = id;
            var OldWarehouse = _context.Warehouses.Find(id);
            var Msg = MyFun.MappingData(ref OldWarehouse, warehouse);

            OldWarehouse.UpdateTime = DateTime.Now;
            OldWarehouse.UpdateUser = 1;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(warehouse));
        }
        /// <summary>
        /// 新增倉庫
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        // POST: api/Warehouses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse warehouse)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(warehouse));
        }
        /// <summary>
        /// 刪除倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Warehouse>> DeleteWarehouse(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(warehouse));
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }
    }
}
