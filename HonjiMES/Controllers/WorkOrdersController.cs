using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;
using DevExtreme.AspNet.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using HonjiMES.Helper;
using System.Drawing;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.UI;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;
using NPOI.POIFS.NIO;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 產品列表
    /// </summary>
   // [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly HonjiContext _context;
        private readonly IWebHostEnvironment _IWebHostEnvironment;

        public WorkOrdersController(HonjiContext context, IWebHostEnvironment environment)
        {
            _IWebHostEnvironment = environment;
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 查詢所有工單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderHead>>> GetWorkOrderHeads(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.WorkOrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetWorkOrderHeadskey(int id)
        {
            var data = await _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && x.Status != 0 && x.Status != 4 && x.Status != 5 && x.Id == id).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 查詢已派工工單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetWorkOrderHeadsRun(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料

            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && x.Status != 0 && x.Status != 4 && x.Status != 5);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.WorkOrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 查詢工單明細by工單ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<WorkOrderData>>> GetWorkOrderDetailByWorkOrderHeadId(int id)
        {
            // var data = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.WorkOrderHeadId == id && x.DeleteFlag == 0).ToListAsync();
            // return Ok(MyFun.APIResponseOK(data));
            var workOrderHead = await _context.WorkOrderHeads.Where(x => x.Id == id && x.DeleteFlag == 0).ToListAsync();
            if (workOrderHead.Count == 1)
            {
                var WorkOrderDetail = _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == id && x.DeleteFlag == 0).OrderBy(x => x.SerialNumber).ToList();
                var data = new WorkOrderData
                {
                    WorkOrderHead = workOrderHead.FirstOrDefault(),
                    WorkOrderDetail = WorkOrderDetail
                };
                return Ok(MyFun.APIResponseOK(data));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單查詢失敗!"));
            }
        }

        /// <summary>
        /// 查詢工單明細by工單NO
        /// </summary>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> GetWorkOrderDetailByWorkOrderNo(SearchValue SearchValue)
        {
            var workOrderHead = await _context.WorkOrderHeads.Where(x => x.WorkOrderNo == SearchValue.WorkOrderNo && x.DeleteFlag == 0).ToListAsync();
            if (workOrderHead.Count == 1)
            {
                var WorkOrderDetail = _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == workOrderHead.FirstOrDefault().Id && x.DeleteFlag == 0).ToList();
                var data = new WorkOrderData
                {
                    WorkOrderHead = workOrderHead.FirstOrDefault(),
                    WorkOrderDetail = WorkOrderDetail
                };
                return Ok(MyFun.APIResponseOK(data));
            }
            else
            {
                return Ok(MyFun.APIResponseError("[ " + SearchValue.WorkOrderNo + " ] 查無資訊!"));
            }
        }

        /// <summary>
        /// 查詢工單明細的報工紀錄
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetWorkOrderLogByWorkOrderDetailId(int id)
        {
            var data = await _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0 && x.WorkOrderDetailId == id).Include(x => x.Supplier).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 訂單轉工單前，確認清單
        /// </summary>
        /// <param name="OrderData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> OrderToWorkOrderCheck(OrderData OrderData)
        {
            if (OrderData.OrderDetail.Count() != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var TempNo = 0;
                var WorkOrderHeadList = new List<WorkOrderHead>();
                foreach (var item in OrderData.OrderDetail)
                {
                    // var CheckWorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DeleteFlag == 0).ToListAsync();
                    // if (CheckWorkOrderHeads.Count() > 0)
                    // {
                    //     // var ProductBasics = _context.ProductBasics.Find(item.ProductBasicId);
                    //     // NewResultMessage += " [ " + ProductBasics.ProductNo + " ] 工單已建立!<br/>";
                    // }
                    // else
                    // {
                    var CheckResult = await this.NewWorkOrderByOrderCheck(item, TempNo);
                    foreach (var item2 in CheckResult)
                    {
                        WorkOrderHeadList.Add(item2);
                    }
                    TempNo++;
                    // }
                }
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(WorkOrderHeadList));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單派工失敗!"));
            }
        }

        /// <summary>
        /// 訂單轉工單
        /// </summary>
        /// <param name="OrderData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> OrderToWorkOrder(OrderData OrderData)
        {
            if (OrderData.OrderDetail.Count() != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                string NewResultMessage = "";
                foreach (var item in OrderData.OrderDetail)
                {
                    // var CheckWorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DeleteFlag == 0).ToListAsync();
                    // if (CheckWorkOrderHeads.Count() > 0)
                    // {
                    //     var ProductBasics = _context.ProductBasics.Find(item.ProductBasicId);
                    //     NewResultMessage += " [ " + ProductBasics.ProductNo + " ] 工單已建立!<br/>";
                    // }
                    // else
                    // {
                    var OrderToWorkCheckData = new OrderToWorkCheckData();
                    OrderToWorkCheckData.OrderDetail = item;
                    OrderToWorkCheckData.WorkOrderHead = OrderData.WorkOrderHead;
                    NewResultMessage += await this.NewWorkOrderByOrder(OrderToWorkCheckData);
                    // }
                }

                _context.ChangeTracker.LazyLoadingEnabled = false;
                // if (NewResultMessage.Length == 0) {
                return Ok(MyFun.APIResponseOK("OK"));
                // } else {
                //     return Ok(MyFun.APIResponseOK("OK", NewResultMessage));
                // }
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單派工失敗!"));
            }
        }

        /// <summary>
        /// 工單派工
        /// </summary>
        /// <param name="WorkOrderData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> toWorkOrder(WorkOrderData WorkOrderData)
        {
            if (WorkOrderData.WorkOrderHead.Id != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(WorkOrderData.WorkOrderHead.Id);
                foreach (var item in WorkOrderHeads.WorkOrderDetails)
                {
                    if (item.DeleteFlag == 0)
                    {
                        item.Status = 1;
                    }
                }
                WorkOrderHeads.Status = 1;
                WorkOrderHeads.DispatchTime = DateTime.Now;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單派工失敗!"));
            }
        }

        /// <summary>
        /// 工單入庫
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseHeads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> StockWorkOrder(int id, WorkOrderReportData WorkOrderReportData)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var OWorkOrderHead = _context.WorkOrderHeads.Find(id);
            OWorkOrderHead.ReCount = OWorkOrderHead.ReCount + WorkOrderReportData.ReCount;
            OWorkOrderHead.UpdateTime = DateTime.Now;
            OWorkOrderHead.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(MyFun.APIResponseOK(OWorkOrderHead));
        }

        /// <summary>
        /// 工單回報by採購單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseHeads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ReportWorkOrderByPurchase(int id, WorkOrderReportData WorkOrderReportData)
        {
            var OWorkOrderHead = await _context.WorkOrderHeads.Include(x => x.WorkOrderDetails).Where(x => x.Id == id).FirstAsync();
            foreach (var item in OWorkOrderHead.WorkOrderDetails)
            {
                if (item.SerialNumber == WorkOrderReportData.WorkOrderSerial)
                {
                    item.Status = 3; // 綁定採購單後即完成。
                    item.PurchaseId = WorkOrderReportData.PurchaseId;
                    item.ReCount = (item?.ReCount ?? 0) + WorkOrderReportData.ReCount;
                    item.UpdateTime = DateTime.Now;
                    item.UpdateUser = MyFun.GetUserID(HttpContext);
                    item.ActualStartTime = DateTime.Now;
                    item.ActualEndTime = DateTime.Now;
                    item.WorkOrderReportLogs.Add(new WorkOrderReportLog
                    {
                        WorkOrderDetailId = item.Id,
                        ReportType = 2, // 完工回報
                        PurchaseId = item.PurchaseId,
                        PurchaseNo = WorkOrderReportData.PurchaseNo,
                        DrawNo = item.DrawNo,
                        Manpower = item.Manpower,
                        // ProducingMachineId = item.,
                        ProducingMachine = item.ProducingMachine,
                        ReCount = WorkOrderReportData.ReCount,
                        Message = WorkOrderReportData.Message,
                        StatusO = 1,
                        StatusN = 3,
                        DueStartTime = item.DueStartTime,
                        DueEndTime = item.DueEndTime,
                        ActualStartTime = item.ActualStartTime,
                        ActualEndTime = item.ActualEndTime,
                        CreateTime = DateTime.Now,
                        CreateUser = MyFun.GetUserID(HttpContext),
                    });

                    ////VVVVV  將內容自動回填至採購單內容 更新2020/08/18
                    var Warehouses = await _context.Warehouses.AsQueryable().Where(x => x.DeleteFlag == 0).ToListAsync();
                    var warehousesM = Warehouses.Where(x => x.Code == "101").FirstOrDefault(); // 原料內定代號 101
                    var warehousesW = Warehouses.Where(x => x.Code == "201").FirstOrDefault(); // 半成品內定代號 201
                    var warehousesP = Warehouses.Where(x => x.Code == "301").FirstOrDefault(); // 成品內定代號 301
                    var PurchaseHeads = await _context.PurchaseHeads.Include(x => x.PurchaseDetails).Where(x => x.Id == WorkOrderReportData.PurchaseId).FirstOrDefaultAsync();
                    var PurchaseDetails = PurchaseHeads.PurchaseDetails.Where(x =>
                        x.DataType == item.WorkOrderHead.DataType &&
                        x.DataId == item.WorkOrderHead.DataId &&
                        x.DeleteFlag == 0
                    ).ToList();
                    if (PurchaseDetails.Count() == 0)
                    { // 如果採購單明細沒有此成品，則回填(新增)採購單明細。
                        var BasicData = new BasicData();
                        if (item.WorkOrderHead.DataType == 1)
                        {
                            var Materials = await _context.Materials.Where(x => x.MaterialBasicId == item.WorkOrderHead.DataId && x.DeleteFlag == 0).ToListAsync();
                            if (Materials.Where(x => x.WarehouseId == warehousesM.Id && x.DeleteFlag == 0).Any())
                            {
                                BasicData.WarehouseId = warehousesM.Id;
                            }
                            else
                            {
                                BasicData.WarehouseId = null;
                            }
                            BasicData.Specification = Materials.FirstOrDefault().Specification;
                            BasicData.Price = Materials.FirstOrDefault().Price;
                        }
                        else if (item.WorkOrderHead.DataType == 2)
                        {
                            var Products = await _context.Products.Where(x => x.ProductBasicId == item.WorkOrderHead.DataId && x.DeleteFlag == 0).ToListAsync();
                            if (Products.Where(x => x.WarehouseId == warehousesP.Id && x.DeleteFlag == 0).Any())
                            {
                                BasicData.WarehouseId = warehousesP.Id;
                            }
                            else
                            {
                                BasicData.WarehouseId = null;
                            }
                            BasicData.Specification = Products.FirstOrDefault().Specification;
                            BasicData.Price = Products.FirstOrDefault().Price;
                        }
                        else if (item.WorkOrderHead.DataType == 3)
                        {
                            var Wiproducts = await _context.Wiproducts.Where(x => x.WiproductBasicId == item.WorkOrderHead.DataId && x.DeleteFlag == 0).ToListAsync();
                            if (Wiproducts.Where(x => x.WarehouseId == warehousesW.Id && x.DeleteFlag == 0).Any())
                            {
                                BasicData.WarehouseId = warehousesW.Id;
                            }
                            else
                            {
                                BasicData.WarehouseId = null;
                            }
                            BasicData.Specification = Wiproducts.FirstOrDefault().Specification;
                            BasicData.Price = Wiproducts.FirstOrDefault().Price;
                        }

                        PurchaseHeads.PurchaseDetails.Add(new PurchaseDetail
                        {
                            PurchaseType = PurchaseHeads.Type,
                            SupplierId = PurchaseHeads.SupplierId,
                            DeliveryTime = DateTime.Now,
                            DataType = item.WorkOrderHead.DataType,
                            DataId = item.WorkOrderHead.DataId,
                            DataNo = item.WorkOrderHead.DataNo,
                            DataName = item.WorkOrderHead.DataName,
                            Specification = BasicData.Specification,
                            Quantity = WorkOrderReportData.ReCount,
                            OriginPrice = BasicData.Price,
                            Price = WorkOrderReportData.ReCount * BasicData.Price,
                            WarehouseId = BasicData.WarehouseId,
                            CreateTime = DateTime.Now,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                    }
                    else
                    { // 如果已有相同成品，則增加數量並調整金額。
                        PurchaseDetails.FirstOrDefault().Quantity += WorkOrderReportData.ReCount;
                        PurchaseDetails.FirstOrDefault().Price += PurchaseDetails.FirstOrDefault().Quantity * PurchaseDetails.FirstOrDefault().OriginPrice;
                        PurchaseDetails.FirstOrDefault().UpdateUser = MyFun.GetUserID(HttpContext);
                    }
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(MyFun.APIResponseOK(OWorkOrderHead));
        }

        /// <summary>
        /// 工單結案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderData"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseHeads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> CloseWorkOrder(int id, WorkOrderData WorkOrderData)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var OWorkOrderHead = _context.WorkOrderHeads.Find(id);
            if (OWorkOrderHead.Status != 5)
            {
                OWorkOrderHead.Status = 5;//結案
                OWorkOrderHead.UpdateTime = DateTime.Now;
                OWorkOrderHead.UpdateUser = MyFun.GetUserID(HttpContext);
            }
            else
            {
                return Ok(MyFun.APIResponseError("該工單已結案!"));
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(MyFun.APIResponseOK(OWorkOrderHead));
        }

        /// <summary>
        /// 工單開工回報
        /// </summary>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> WorkOrderReportStart(WorkOrderReportData WorkOrderReportData)
        {
            if (WorkOrderReportData.WorkOrderID != 0 && WorkOrderReportData.WorkOrderSerial != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(WorkOrderReportData.WorkOrderID);
                if (WorkOrderHeads.Status == 5)
                {
                    return Ok(MyFun.APIResponseError("該工單狀態為[結案]!"));
                }
                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    if (WorkOrderDetails.FirstOrDefault().Status == 1)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        // WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        // WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                        WorkOrderDetails.FirstOrDefault().ActualStartTime = DateTime.Now;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 1, // 開工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            // SupplierId = WorkOrderDetails.FirstOrDefault().SupplierId,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderDetails.FirstOrDefault().ProducingMachine,
                            ReCount = 0,
                            // RePrice = WorkOrderDetails.FirstOrDefault().RePrice,
                            Message = WorkOrderReportData.Message,
                            StatusO = 1,
                            StatusN = 2,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = WorkOrderDetails.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.FirstOrDefault().ActualEndTime,
                            CreateTime = DateTime.Now,
                            CreateUser = MyFun.GetUserID(HttpContext),
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    }
                }
                else
                {
                    return Ok(MyFun.APIResponseError("工單數量不正確!"));
                }
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("回報失敗!"));
            }
        }

        /// <summary>
        /// 工單完工回報
        /// </summary>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> WorkOrderReportEnd(WorkOrderReportData WorkOrderReportData)
        {
            if (WorkOrderReportData.WorkOrderID != 0 && WorkOrderReportData.WorkOrderSerial != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(WorkOrderReportData.WorkOrderID);
                if (WorkOrderHeads.Status == 5)
                {
                    return Ok(MyFun.APIResponseError("該工單狀態為[結案]!"));
                }
                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    if (WorkOrderDetails.FirstOrDefault().Status == 2)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 3;
                        WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        WorkOrderDetails.FirstOrDefault().ReCount = (WorkOrderDetails.FirstOrDefault()?.ReCount ?? 0) + WorkOrderReportData.ReCount;
                        WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 2, // 完工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            SupplierId = WorkOrderDetails.FirstOrDefault().SupplierId,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderReportData.ProducingMachine,
                            ReCount = WorkOrderReportData.ReCount,
                            RePrice = WorkOrderReportData.RePrice,
                            Message = WorkOrderReportData.Message,
                            StatusO = 2,
                            StatusN = 3,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = WorkOrderDetails.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.FirstOrDefault().ActualEndTime,
                            CreateTime = DateTime.Now,
                            CreateUser = MyFun.GetUserID(HttpContext),
                        });

                        //檢查工單是否全數完工
                        // var statusCheck = true;
                        // foreach (var item in WorkOrderHeads.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToList())
                        // {
                        //     if (item.Status != 3)
                        //     {
                        //         statusCheck = false;
                        //     }
                        // }
                        // if (statusCheck)
                        // {
                        //     WorkOrderHeads.Status = 3;
                        // }
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    }
                }
                else
                {
                    return Ok(MyFun.APIResponseError("工單數量不正確!"));
                }
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("回報失敗!"));
            }
        }

        /// <summary>
        /// 工單再開工回報
        /// </summary>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderData>> WorkOrderReportRestart(WorkOrderReportData WorkOrderReportData)
        {
            if (WorkOrderReportData.WorkOrderID != 0 && WorkOrderReportData.WorkOrderSerial != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(WorkOrderReportData.WorkOrderID);
                if (WorkOrderHeads.Status == 5)
                {
                    return Ok(MyFun.APIResponseError("該工單狀態為[結案]!"));
                }
                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    if (WorkOrderDetails.FirstOrDefault().Status == 3)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        // WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        // WorkOrderDetails.FirstOrDefault().ReCount = WorkOrderReportData.ReCount;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 3, // 完工再開工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderReportData.ProducingMachine,
                            ReCount = 0,
                            Message = WorkOrderReportData.Message,
                            StatusO = 3,
                            StatusN = 2,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = WorkOrderDetails.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.FirstOrDefault().ActualEndTime,
                            CreateTime = DateTime.Now,
                            CreateUser = MyFun.GetUserID(HttpContext),
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    }
                }
                else
                {
                    return Ok(MyFun.APIResponseError("工單數量不正確!"));
                }
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("回報失敗!"));
            }
        }

        /// <summary>
        /// 工單批次作業
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderReportDataAll"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<WorkOrderData>> WorkOrderReportAll(int id, [FromBody] WorkOrderReportDataAll WorkOrderReportDataAll)
        {
            var WorkOrderDetails = await _context.WorkOrderDetails.FindAsync(id);
            if (WorkOrderDetails != null)
            {
                var WorkOrderReportData = new WorkOrderReportData
                {
                    ReCount = WorkOrderReportDataAll.ReportCount ?? 0,
                    Message = WorkOrderReportDataAll.Message,
                    ProducingMachine = WorkOrderReportDataAll.ProducingMachine,
                    WorkOrderID = WorkOrderDetails.WorkOrderHeadId,
                    WorkOrderSerial = WorkOrderDetails.SerialNumber
                };
                if (WorkOrderDetails.Status == 1)
                    return await WorkOrderReportStart(WorkOrderReportData);
                else if (WorkOrderDetails.Status == 2)
                    return await WorkOrderReportEnd(WorkOrderReportData);
                else if (WorkOrderDetails.Status == 3)
                    return await WorkOrderReportRestart(WorkOrderReportData);
                else
                    return Ok(MyFun.APIResponseError("工單狀態異常!"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單異常!"));
            }
        }

        public async Task<List<WorkOrderHead>> NewWorkOrderByOrderCheck(OrderDetail OrderDetail, int TempNo)
        {
            var WorkOrderHeadList = new List<WorkOrderHead>();
            if (OrderDetail.ProductBasicId != 0)
            {
                //取得工單號
                var key = "WO";
                var WorkOrderNo = DateTime.Now.ToString("yyMMdd");
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                    var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                    // if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                    // }
                }
                var workOrderNo = key + WorkOrderNo + (NoCount + TempNo).ToString("000");

                var DataType = 2;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                if (DataType == 1)
                {

                }
                else if (DataType == 2)
                {
                    var BasicData = _context.ProductBasics.Find(OrderDetail.ProductBasicId);
                    BasicDataID = BasicData.Id;
                    BasicDataNo = BasicData.ProductNo;
                    BasicDataName = BasicData.Name;
                }
                else if (DataType == 3)
                {

                }

                var status = 0; // 工單是否已建立，和是否有MBOM (0否 1是 2無MBOM)
                var CheckWorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == OrderDetail.Id && x.DataType == DataType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToListAsync();
                if (CheckWorkOrderHeads.Count() > 0)
                {
                    status = 1;
                }
                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.ProductBasicId == OrderDetail.ProductBasicId).ToListAsync();
                if (billOfMaterial.Count() == 0)
                {
                    status = 2;
                }

                var nWorkOrderHead = new WorkOrderHead
                {
                    WorkOrderNo = workOrderNo,
                    OrderDetailId = OrderDetail.Id, // 
                    MachineNo = OrderDetail.MachineNo,
                    DataType = DataType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    Count = OrderDetail.Quantity,
                    Status = status, // 注意!原本用於表示該工單目前狀態，這裡借來表示[該工單是否已經建立]
                    CreateUser = MyFun.GetUserID(HttpContext)
                };

                //依照成品BOM內容檢查新增工序
                var Results = this.NewWorkOrderByBOM(nWorkOrderHead);

                WorkOrderHeadList.Add(nWorkOrderHead);
                foreach (var item in Results)
                {
                    WorkOrderHeadList.Add(item);
                }
            }
            return WorkOrderHeadList;
        }

        public async Task<string> NewWorkOrderByOrder(OrderToWorkCheckData OrderToWorkCheckData)
        {
            string sMessage = "";
            if (OrderToWorkCheckData.OrderDetail.ProductBasicId != 0)
            {
                //取得工單號
                var key = "WO";
                var WorkOrderNo = DateTime.Now.ToString("yyMMdd");
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                    var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                    // if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                    // }
                }
                var workOrderNo = key + WorkOrderNo + NoCount.ToString("000");

                var DataType = 2;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                if (DataType == 1)
                {

                }
                else if (DataType == 2)
                {
                    var BasicData = _context.ProductBasics.Find(OrderToWorkCheckData.OrderDetail.ProductBasicId);
                    BasicDataID = BasicData.Id;
                    BasicDataNo = BasicData.ProductNo;
                    BasicDataName = BasicData.Name;
                }
                else if (DataType == 3)
                {

                }
                var nWorkOrderHead = new WorkOrderHead
                {
                    WorkOrderNo = workOrderNo,
                    OrderDetailId = OrderToWorkCheckData.OrderDetail.Id, // 
                    MachineNo = OrderToWorkCheckData.OrderDetail.MachineNo,
                    DataType = DataType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    Count = OrderToWorkCheckData.OrderDetail.Quantity,
                    Status = 4, // 表示由訂單轉程的工單，需要再由人工確認該工單
                    CreateUser = MyFun.GetUserID(HttpContext)
                };

                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.ProductBasicId == OrderToWorkCheckData.OrderDetail.ProductBasicId).ToListAsync();
                if (billOfMaterial.Count() == 0)
                {
                    // sMessage += "該品號尚未建立MBOM資訊 [ " + BasicDataNo + " ] !<br/>";
                }
                foreach (var item in billOfMaterial)
                {
                    var ProcessInfo = _context.Processes.Find(item.ProcessId);
                    var nWorkOrderDetail = new WorkOrderDetail
                    {
                        SerialNumber = item.SerialNumber,
                        ProcessId = item.ProcessId,
                        ProcessNo = ProcessInfo.Code,
                        ProcessName = ProcessInfo.Name,
                        ProcessLeadTime = item.ProcessLeadTime,
                        ProcessTime = item.ProcessTime,
                        ProcessCost = item.ProcessCost,
                        Count = OrderToWorkCheckData.OrderDetail.Quantity,
                        // PurchaseId
                        DrawNo = item.DrawNo,
                        Manpower = item.Manpower,
                        ProducingMachine = item.ProducingMachine,
                        Remarks = item.Remarks,
                        DueStartTime = DateTime.Now,
                        DueEndTime = DateTime.Now,
                        ActualStartTime = null,
                        ActualEndTime = null,
                        CreateUser = MyFun.GetUserID(HttpContext)
                    };
                    nWorkOrderHead.WorkOrderDetails.Add(nWorkOrderDetail);
                }

                //依照成品BOM內容檢查新增工序
                var Results = this.NewWorkOrderByBOM(nWorkOrderHead);

                // if (sMessage.Length == 0)
                // {
                if (OrderToWorkCheckData.WorkOrderHead.Where(x =>
                    x.OrderDetailId == nWorkOrderHead.OrderDetailId &&
                    x.DataType == nWorkOrderHead.DataType &&
                    x.DataId == nWorkOrderHead.DataId &&
                    x.Count == nWorkOrderHead.Count
                ).Any())
                {
                    _context.WorkOrderHeads.Add(nWorkOrderHead);
                }

                foreach (var item in Results)
                {
                    if (OrderToWorkCheckData.WorkOrderHead.Where(x =>
                        x.OrderDetailId == item.OrderDetailId &&
                        x.DataType == item.DataType &&
                        x.DataId == item.DataId &&
                        x.Count == item.Count
                    ).Any())
                    {
                        _context.WorkOrderHeads.Add(item);
                    }
                }
                await _context.SaveChangesAsync();
                // }
                // return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                sMessage += "";
            }
            return sMessage;
        }

        public List<WorkOrderHead> NewWorkOrderByBOM(WorkOrderHead WorkOrderHead)
        {
            var WorkOrderHeads = new List<WorkOrderHead>();
            if (WorkOrderHead.DataId != 0)
            {
                var bomlist = new List<BomList>();
                var BillOfMaterials = _context.BillOfMaterials.Where(x => x.ProductBasicId == WorkOrderHead.DataId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToList();
                if (BillOfMaterials != null)
                {
                    bomlist.AddRange(MyFun.GetBomList(BillOfMaterials));
                }

                var index = 1;
                foreach (var item in bomlist)
                {
                    var billOfMaterial = _context.MBillOfMaterials.AsQueryable().Where(x => x.BomId == item.Id).ToList();
                    if (billOfMaterial.Count() != 0)
                    {
                        var DataType = 0;
                        var BasicDataID = 0;
                        var BasicDataNo = "";
                        var BasicDataName = "";
                        if (item.MaterialBasicId != null)
                        {
                            var BasicData = _context.MaterialBasics.Find(item.MaterialBasicId);
                            DataType = 1;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.MaterialNo;
                            BasicDataName = BasicData.Name;
                        }
                        else if (item.ProductBasicId != null)
                        {
                            var BasicData = _context.ProductBasics.Find(item.ProductBasicId);
                            DataType = 2;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.ProductNo;
                            BasicDataName = BasicData.Name;
                        }

                        var status = 0; // 工單是否已建立，和是否有MBOM (0否 1是 2無MBOM)
                        var CheckWorkOrderHeads = _context.WorkOrderHeads.Where(x => x.OrderDetailId == WorkOrderHead.OrderDetailId && x.DataType == DataType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToList();
                        if (CheckWorkOrderHeads.Count() > 0)
                        {
                            status = 1;
                        }

                        var nWorkOrderHead = new WorkOrderHead
                        {
                            WorkOrderNo = WorkOrderHead.WorkOrderNo + "-" + index,
                            OrderDetailId = WorkOrderHead.OrderDetailId, // 
                            MachineNo = WorkOrderHead.MachineNo,
                            DataType = DataType,
                            DataId = BasicDataID,
                            DataNo = BasicDataNo,
                            DataName = BasicDataName,
                            Count = Decimal.ToInt32(WorkOrderHead.Count * item.ReceiveQty) + 1, // 注意!
                            Status = status, // 注意!原本用於表示該工單目前狀態，這裡借來表示[該工單是否已經建立]
                            CreateUser = MyFun.GetUserID(HttpContext)
                        };

                        foreach (var item2 in billOfMaterial)
                        {
                            var ProcessInfo = _context.Processes.Find(item2.ProcessId);
                            var nWorkOrderDetail = new WorkOrderDetail
                            {
                                SerialNumber = item2.SerialNumber,
                                ProcessId = item2.ProcessId,
                                ProcessNo = ProcessInfo.Code,
                                ProcessName = ProcessInfo.Name,
                                ProcessLeadTime = item2.ProcessLeadTime,
                                ProcessTime = item2.ProcessTime,
                                ProcessCost = item2.ProcessCost,
                                Count = Decimal.ToInt32(WorkOrderHead.Count * item.ReceiveQty) + 1,
                                // PurchaseId
                                DrawNo = item2.DrawNo,
                                Manpower = item2.Manpower,
                                ProducingMachine = item2.ProducingMachine,
                                Remarks = item2.Remarks,
                                DueStartTime = DateTime.Now,
                                DueEndTime = DateTime.Now,
                                ActualStartTime = null,
                                ActualEndTime = null,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            };
                            nWorkOrderHead.WorkOrderDetails.Add(nWorkOrderDetail);
                        }
                        WorkOrderHeads.Add(nWorkOrderHead);
                        index++;
                    }
                }
            }
            return WorkOrderHeads;
        }

        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 報工紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkOrderReportLog>> GetWorkOrderReportLog([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderDetail).ThenInclude(x => x.WorkOrderHead);
            // data.FirstOrDefault().WorkOrderDetail.WorkOrderHead.WorkOrderNo 單獨抓取WorkOrderNo
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 報工紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkOrderReportLog>> GetWorkOrderReportLogByNum(string machine)
        {
            var data = await _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == machine).Include(x => x.WorkOrderDetail).ThenInclude(x => x.WorkOrderHead).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 產報工單PDF
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPackingSlipPDF()
        {
            var qcodesize = 70;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var ProcessReportVMList = new List<ProcessReportVM>();
            var WorkOrder = _context.WorkOrderHeads.Find(98);
            var txt = "";
            foreach (var item in WorkOrder.WorkOrderDetails)
            {
                txt = item.WorkOrderHead.WorkOrderNo;
                var dbQrCode = BarcodeHelper.CreateQrCode(txt + "-" + item.SerialNumber, qcodesize, qcodesize);
                ProcessReportVMList.Add(new ProcessReportVM
                {
                    SerialNumber = item.SerialNumber,
                    ProcessName = item.ProcessName,
                    ProducingMachine = item.ProducingMachine,
                    Status = item.Status.ToString(),
                    Type = item.Type.ToString(),
                    ReCount = item.ReCount,
                    ActualStartTime = item.ActualStartTime,
                    ActualEndTime = item.ActualEndTime,
                    Img = MyFun.ImgToBase64String(dbQrCode),
                });
            }
            ProcessReportVMList.AddRange(ProcessReportVMList);
            ProcessReportVMList.AddRange(ProcessReportVMList);
            var json = JsonConvert.SerializeObject(ProcessReportVMList);
            var bQrCode = BarcodeHelper.CreateQrCode(txt, qcodesize, qcodesize);
            var webRootPath = _IWebHostEnvironment.ContentRootPath;
            var ReportPath = Path.Combine(webRootPath, "Reports", "process.repx");
            var report = XtraReport.FromFile(ReportPath);
            report.RequestParameters = false;
            report.Parameters["WorkOrderNo"].Value = WorkOrder.WorkOrderNo;
            report.Parameters["DataName"].Value = WorkOrder.DataName;
            report.Parameters["DataNo"].Value = WorkOrder.DataNo;
            var PictureBox = (XRPictureBox)report.FindControl("Qrcode", true);
            PictureBox.Image = Image.FromStream(new MemoryStream(bQrCode));
            var jsonDataSource = new JsonDataSource();
            jsonDataSource.JsonSource = new CustomJsonSource(json);
            report.DataSource = jsonDataSource;
            report.CreateDocument(true);
            using (MemoryStream ms = new MemoryStream())
            {
                report.ExportToPdf(ms);
                return File(ms.ToArray(), "application/pdf", txt + "報工單.pdf");
            }

        }
    }
}
