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
    /// 顧客列表
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly HonjiContext _context;

        public CustomersController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢顧客列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var Customers = await _context.Customers.ToListAsync();
            return Ok(Fun.APIResponseOK(Customers));
        }

        /// <summary>
        /// 使用ID查詢顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            return Ok(Fun.APIResponseOK(customer));
        }
        /// <summary>
        /// 修改顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(Fun.APIResponseOK(customer));
        }
        /// <summary>
        /// 新增顧客列表
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(customer));
        }
        /// <summary>
        /// 刪除顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [HttpDelete]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(customer));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
