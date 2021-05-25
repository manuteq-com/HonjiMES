using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 訂單明細
    /// </summary>
    [JWTAuthorize]
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
        public async Task<ActionResult<IEnumerable<OrderDetailData>>> GetOrderDetailsByOrderId(int? OrderId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var OrderDetails = _context.OrderDetails.Include(x => x.SaleDetailNews).Include(x => x.WorkOrderHeads)
            .Include(x => x.OrderDetailAndWorkOrderHeads).Include(x => x.PurchaseDetails).Include(x => x.MaterialBasic).AsQueryable();

            if (OrderId.HasValue)
            {
                OrderDetails = OrderDetails.Where(x => x.OrderId == OrderId).OrderBy(x => x.Serial);
            }
            var data = await OrderDetails.Where(x => x.DeleteFlag == 0).Select(x => new OrderDetailData
            {
                TotalCount = x.MaterialBasic.Materials.Where(y => y.DeleteFlag == 0 && y.Warehouse.Code == "301").Sum(y => y.Quantity),
                WorkOrderHeads = x.WorkOrderHeads,
                OrderDetailAndWorkOrderHeads = x.OrderDetailAndWorkOrderHeads.ToList(),
                Id = x.Id,
                OrderId = x.OrderId,
                MaterialBasicId = x.MaterialBasicId,
                MaterialId = x.MaterialId,
                CustomerNo = x.Order.CustomerNo,
                OrderNo = x.Order.OrderNo,
                Serial = x.Serial,
                MachineNo = x.MachineNo,
                DueDate = x.DueDate,
                ReplyDate = x.ReplyDate,
                Remark = x.Remark,
                ReplyRemark = x.ReplyRemark,
                ReplyPrice = x.ReplyPrice,
                Quantity = x.Quantity,
                OriginPrice = x.OriginPrice,
                Price = x.Price,
                SaleCount = x.SaleCount,
                SaledCount = x.SaledCount,
                SaleDetailNews = x.SaleDetailNews,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                DeleteFlag = x.DeleteFlag,
            }).ToListAsync();

            foreach (var Detailitem in data)
            {
                //取得銷貨單號
                foreach (var SaleDetailitem in Detailitem.SaleDetailNews.ToList())
                {
                    var SaleHeads = _context.SaleHeads.Find(SaleDetailitem.SaleId);
                    if (SaleHeads.DeleteFlag == 0)
                    {
                        SaleDetailitem.Sale = SaleHeads;
                    }
                    else
                    {
                        Detailitem.SaleDetailNews.Remove(SaleDetailitem);
                    }
                }
                //取得工單號
                // foreach (var WorkOrderHeaditem in Detailitem.WorkOrderHeads.ToList()) // 先清空所有工單關聯資料。
                // {
                //     Detailitem.WorkOrderHeads.Remove(WorkOrderHeaditem);
                // }
                foreach (var WorkOrderHeaditem in Detailitem.OrderDetailAndWorkOrderHeads.ToList()) // 重新取得工單關聯資料。
                {
                    var WorkOrderHead = _context.WorkOrderHeads.Find(WorkOrderHeaditem.WorkHeadId);
                    if (WorkOrderHeaditem.DeleteFlag == 0)
                    {
                        Detailitem.WorkOrderHeads.Add(WorkOrderHead);
                    }
                }
                // //取得工單號(舊方法)
                // foreach (var WorkOrderHeaditem in Detailitem.WorkOrderHeads.ToList())
                // {
                //     if (WorkOrderHeaditem.DeleteFlag != 0)
                //     {
                //         Detailitem.WorkOrderHeads.Remove(WorkOrderHeaditem);
                //     }
                // }
                //取得採購單號
                var tempCheck = new List<int>();
                foreach (var PurchaseDetailitem in Detailitem.PurchaseDetails.ToList())
                {
                    var PurchaseHeads = _context.PurchaseHeads.Find(PurchaseDetailitem.PurchaseId);
                    if (PurchaseHeads.DeleteFlag == 0 && !tempCheck.Contains(PurchaseDetailitem.PurchaseId))
                    {
                        tempCheck.Add(PurchaseDetailitem.PurchaseId);
                        PurchaseDetailitem.Purchase = PurchaseHeads;
                    }
                    else
                    {
                        Detailitem.PurchaseDetails.Remove(PurchaseDetailitem);
                    }
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
            OrderDetails.UpdateUser = MyFun.GetUserID(HttpContext);
            OrderDetails.Price = orderDetail.Quantity * OrderDetails.OriginPrice;
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
            orderDetail.CreateUser = MyFun.GetUserID(HttpContext);
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

        /// <summary>
        /// 使用成品basic ID查詢總庫存量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialBasicData>> GetStockCountByMaterialBasicId(int id)//OrderDetails.id
        {
            var OrderDetails = await _context.OrderDetails.FindAsync(id);//Find OrderDetails.MaterialBasicId
            var materialBasic = await _context.MaterialBasics.Where(x => x.DeleteFlag == 0 && x.Id == OrderDetails.MaterialBasicId).Select(x => new MaterialBasicData
            {
                TotalCount = x.Materials.Where(y => y.DeleteFlag == 0).Sum(y => y.Quantity),
                Materials = x.Materials.Where(y => y.DeleteFlag == 0).ToList()
            }).FirstOrDefaultAsync();
            if (materialBasic.Materials.Count() == 0)
            {
                return Ok(MyFun.APIResponseError("尚未建立品號明細!"));
            }
            else
            {
                return Ok(MyFun.APIResponseOK(materialBasic));
            }
        }
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
