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
    /// 訂單明細
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
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        ///// <summary>
        ///// 查詢所有訂單明細
        ///// </summary>
        ///// <returns></returns>
        //// GET: api/OrderDetails
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        //{
        //    var OrderDetails = await _context.OrderDetails.ToListAsync();
        //    return Ok(Fun.APIResponseOK(OrderDetails));
        //}

        /// <summary>
        /// 查詢訂單明細
        /// </summary>
        /// <param name="OrderId">訂單ID 非必填</param>
        /// <returns>訂單明細</returns>
        // GET: api/OrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetailsByOrderId(int? OrderId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var OrderDetails = _context.OrderDetails.Include(x => x.SaleDetailNews).AsQueryable();
            if (OrderId.HasValue)
            {
                OrderDetails = OrderDetails.Where(x => x.OrderId == OrderId).OrderBy(x => x.Serial);
            }
            var data = await OrderDetails.Where(x => x.DeleteFlag == 0).ToListAsync();
            foreach (var Detailitem in data)
            {
                foreach (var SaleDetailitem in Detailitem.SaleDetailNews)
                {
                    SaleDetailitem.Sale = _context.SaleHeads.Find(SaleDetailitem.SaleId);
                }
            }
            return Ok(MyFun.APIResponseOK(data));
        }



        /// <summary>
        /// 使用ID查詢訂單明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(orderDetail));
        }
        /// <summary>
        /// 修改訂單明細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        // PUT: api/OrderDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            orderDetail.Id = id;
            var OrderDetails = _context.OrderDetails.Find(id);
            var Msg = MyFun.MappingData(ref OrderDetails, orderDetail);
            OrderDetails.UpdateTime = DateTime.Now;
            OrderDetails.UpdateUser = 1;
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
            return Ok(MyFun.APIResponseOK(orderDetail));
        }

        /// <summary>
        /// 新增訂單明細
        /// </summary>
        /// <param name="PID">訂單ID</param>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        // POST: api/OrderDetails
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(int PID, OrderDetail orderDetail)
        {
            var Serial = 0;
            if (_context.OrderHeads.Find(PID).OrderDetails.Any())
            {
                Serial = _context.OrderHeads.Find(PID).OrderDetails.Max(x => x.Serial);
            }
            orderDetail.Serial = Serial + 1;
            orderDetail.OrderId = PID;
            orderDetail.CreateTime = DateTime.Now;
            orderDetail.CreateUser = 1;
            _context.OrderDetails.Add(orderDetail);
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(orderDetail));
        }

        /// <summary>
        /// 刪除訂單明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/OrderDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderDetail>> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            orderDetail.DeleteFlag = 1;
            // _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(orderDetail));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
