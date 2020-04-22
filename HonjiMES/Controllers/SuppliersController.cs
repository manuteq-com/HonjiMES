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
    /// 供應商
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly HonjiContext _context;

        public SuppliersController(HonjiContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        /// <summary>
        /// 供應商列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var Suppliers = await _context.Suppliers.ToListAsync();
            return Ok(MyFun.APIResponseOK(Suppliers));
        }

        // GET: api/Suppliers/5
        /// <summary>
        /// 查詢供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(supplier));
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 修改供應商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            supplier.Id = id;
            var Oldsupplier = _context.Suppliers.Find(id);
            var Msg = MyFun.MappingData(ref Oldsupplier, supplier);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(Oldsupplier));
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 新增供應商
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(supplier));
        }

        // DELETE: api/Suppliers/5
        /// <summary>
        /// 刪除供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Supplier>> DeleteSupplier(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
}
