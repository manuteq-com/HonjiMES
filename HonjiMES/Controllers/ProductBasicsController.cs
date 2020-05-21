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
    public class ProductBasicsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProductBasicsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/ProductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasic>>> GetProductBasics()
        {
            var productBasic = await _context.ProductBasics.AsQueryable().ToListAsync();
            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // GET: api/ProductBasics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductBasic>> GetProductBasic(int id)
        {
            var productBasic = await _context.ProductBasics.FindAsync(id);

            if (productBasic == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // PUT: api/ProductBasics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductBasic(int id, ProductBasic productBasic)
        {
            if (id != productBasic.Id)
            {
                return BadRequest();
            }

            _context.Entry(productBasic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductBasicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // POST: api/ProductBasics
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProductBasic>> PostProductBasic(ProductBasic productBasic)
        {
            _context.ProductBasics.Add(productBasic);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // DELETE: api/ProductBasics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductBasic>> DeleteProductBasic(int id)
        {
            var productBasic = await _context.ProductBasics.FindAsync(id);
            if (productBasic == null)
            {
                return NotFound();
            }

            _context.ProductBasics.Remove(productBasic);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(productBasic));
        }

        private bool ProductBasicExists(int id)
        {
            return _context.ProductBasics.Any(e => e.Id == id);
        }
    }
}
