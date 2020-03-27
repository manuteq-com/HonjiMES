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
    /// 訂單名細
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public OrderDetailsController(HonjiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 查詢訂單名細
        /// </summary>
        /// <returns></returns>
        // GET: api/OrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            var OrderDetails = await _context.OrderDetails.ToListAsync();
            return Ok(Fun.APIResponseOK(OrderDetails));
        }

        /// <summary>
        /// 使用ID查詢訂單名細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/OrderDetails/5
        [HttpGet]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(Fun.APIResponseOK(orderDetail));
        }
        /// <summary>
        /// 修改訂單名細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        // PUT: api/OrderDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            orderDetail.Id = id;
            var OrderDetails = _context.OrderDetails.Find(id);
            var Msg = myfun.MappingData(ref OrderDetails, orderDetail);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(Fun.APIResponseOK(orderDetail));
        }

        /// <summary>
        /// 新增訂單名細
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        // POST: api/OrderDetails
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(orderDetail));
        }

        /// <summary>
        /// 刪除訂單名細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/OrderDetails/5
        [HttpDelete]
        public async Task<ActionResult<OrderDetail>> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return Ok(Fun.APIResponseOK(orderDetail));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
