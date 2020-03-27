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
    public class ProductsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProductsController(HonjiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var Products = await _context.Products.ToListAsync();
            return Ok(Fun.APIResponseOK(Products));
            //return Ok(new { data = Products, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 使用ID查詢產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Products/5
        [HttpGet]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(Fun.APIResponseOK(product));
            //return Ok(new { data = product, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 修改產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(Fun.APIResponseOK(product));
            //return Ok(new { success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 新增產品列表
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(product));
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }

        /// <summary>
        /// 刪除產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Products/5
        [HttpDelete]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(product));
            //return Ok(new { data = product, success = true, timestamp = DateTime.Now, message = "" });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
