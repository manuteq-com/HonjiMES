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
            var data = _context.Customers.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Customers = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Customers));
        }

        /// <summary>
        /// 使用ID查詢顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(customer));
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            customer.Id = id;
            var Ocustomer = _context.Customers.Find(id);
            var Ccustomer = Ocustomer;
            if (!string.IsNullOrWhiteSpace(customer.Code))
            {
                Ccustomer.Code = customer.Code;
            }
            if (!string.IsNullOrWhiteSpace(customer.Name))
            {
                Ccustomer.Name = customer.Name;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.Customers.Where(x => x.Id != id && (x.Name == Ccustomer.Name || x.Code == Ccustomer.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("客戶的的 [代號] 或 [名稱] 重複!", Ccustomer));
            }
            
            var Msg = MyFun.MappingData(ref Ocustomer, customer);
            Ocustomer.UpdateTime = DateTime.Now;
            Ocustomer.UpdateUser = 1;
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
            return Ok(MyFun.APIResponseOK(customer));
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
            //新增時檢查[代號][名稱]是否重複
            if (_context.Customers.Where(x => (x.Name == customer.Name || x.Code == customer.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("客戶的 [代號] 或 [名稱] 已存在!", customer));
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(customer));
        }
        /// <summary>
        /// 刪除顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.DeleteFlag = 1;
            // _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(customer));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
