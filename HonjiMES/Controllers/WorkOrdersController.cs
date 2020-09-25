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
        
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetWorkOrderHeadsRunById(int id)
        {
            var data = await _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && x.Status != 0 && x.Status != 4 && x.Status != 5 && x.Id == id).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 查詢工單 By 工單號
        /// </summary>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<MaterialBasic>> GetWorkOrderHeadByWorkOrderNo(string DataNo)
        {
            var WorkOrderHead = await _context.WorkOrderHeads.Where(x => x.WorkOrderNo == DataNo && x.DeleteFlag == 0).ToListAsync();
            if (WorkOrderHead.Count() == 0) {
                return Ok(MyFun.APIResponseError("查無工單號碼! [ " + DataNo + " ]"));
            }
            return Ok(MyFun.APIResponseOK(WorkOrderHead.FirstOrDefault()));
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
                var OrderDetailList = new List<OrderDetail>();
                foreach (var item in OrderData.OrderDetail)
                {
                    var CheckProductBasic = OrderDetailList.Where(x => x.ProductBasicId == item.ProductBasicId);
                    if (CheckProductBasic.Count() == 0) {
                        OrderDetailList.Add(item);
                    } else {
                        CheckProductBasic.FirstOrDefault().Quantity += item.Quantity;
                    }
                }

                foreach (var item in OrderDetailList)
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
                var OrderDetailList = new List<OrderDetail>();
                foreach (var item in OrderData.OrderDetail)
                {
                    var CheckProductBasic = OrderDetailList.Where(x => x.ProductBasicId == item.ProductBasicId);
                    if (CheckProductBasic.Count() == 0) {
                        OrderDetailList.Add(item);
                    } else {
                        CheckProductBasic.FirstOrDefault().Quantity += item.Quantity;
                    }
                }

                foreach (var item in OrderDetailList)
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
            var dt = DateTime.Now;
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var OWorkOrderHead = _context.WorkOrderHeads.Find(id);
            OWorkOrderHead.ReCount = OWorkOrderHead.ReCount + WorkOrderReportData.ReCount;
            OWorkOrderHead.UpdateTime = DateTime.Now;
            OWorkOrderHead.UpdateUser = WorkOrderReportData.CreateUser;

            //入庫
            var checkInfo = false;
            if (OWorkOrderHead.DataType == 1) // 原料
            {
                var MaterialBasic = await _context.MaterialBasics.Include(x => x.Materials).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
                foreach (var item in MaterialBasic.Materials)
                {
                    if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
                    {
                        item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
                        // item.UpdateTime = dt;
                        // item.UpdateUser = WorkOrderReportData.CreateUser;
                        item.MaterialLogs.Add(new MaterialLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = item.Quantity - WorkOrderReportData.ReCount,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        checkInfo = true;
                    }
                }

                // 如果沒有明細資訊，則自動新增。
                if (!checkInfo)
                {
                    MaterialBasic.Materials.Add(new Material
                    {
                        MaterialNo = MaterialBasic.MaterialNo,
                        Name = MaterialBasic.Name,
                        Quantity = WorkOrderReportData.ReCount,
                        Specification = MaterialBasic.Specification,
                        Property = MaterialBasic.Property,
                        Price = MaterialBasic.Price,
                        BaseQuantity = 2,
                        CreateTime = dt,
                        CreateUser = WorkOrderReportData.CreateUser,
                        WarehouseId = WorkOrderReportData.WarehouseId,
                        MaterialLogs = {new MaterialLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = 0,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        }}
                    });
                }
            }
            else if (OWorkOrderHead.DataType == 2) // 成品
            {
                var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
                foreach (var item in ProductBasic.Products)
                {
                    if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
                    {
                        item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
                        // item.UpdateTime = dt;
                        // item.UpdateUser = WorkOrderReportData.CreateUser;
                        item.ProductLogs.Add(new ProductLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = item.Quantity - WorkOrderReportData.ReCount,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        checkInfo = true;
                    }
                }

                // 如果沒有明細資訊，則自動新增。
                if (!checkInfo)
                {
                    ProductBasic.Products.Add(new Product
                    {
                        ProductNo = ProductBasic.ProductNo,
                        ProductNumber = ProductBasic.ProductNumber,
                        Name = ProductBasic.Name,
                        Quantity = WorkOrderReportData.ReCount,
                        Specification = ProductBasic.Specification,
                        Property = ProductBasic.Property,
                        Price = ProductBasic.Price,
                        MaterialRequire = 1,
                        CreateTime = dt,
                        CreateUser = WorkOrderReportData.CreateUser,
                        WarehouseId = WorkOrderReportData.WarehouseId,
                        ProductLogs = {new ProductLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = 0,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        }}
                    });
                }
            }
            else if (OWorkOrderHead.DataType == 3) // 半成品
            {
                var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
                foreach (var item in WiproductBasic.Wiproducts)
                {
                    if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
                    {
                        item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
                        // item.UpdateTime = dt;
                        // item.UpdateUser = WorkOrderReportData.CreateUser;
                        item.WiproductLogs.Add(new WiproductLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = item.Quantity - WorkOrderReportData.ReCount,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        checkInfo = true;
                    }
                }

                // 如果沒有明細資訊，則自動新增。
                if (!checkInfo)
                {
                    WiproductBasic.Wiproducts.Add(new Wiproduct
                    {
                        WiproductNo = WiproductBasic.WiproductNo,
                        WiproductNumber = WiproductBasic.WiproductNumber,
                        Name = WiproductBasic.Name,
                        Quantity = WorkOrderReportData.ReCount,
                        Specification = WiproductBasic.Specification,
                        Property = WiproductBasic.Property,
                        Price = WiproductBasic.Price,
                        MaterialRequire = 1,
                        CreateTime = dt,
                        CreateUser = WorkOrderReportData.CreateUser,
                        WarehouseId = WorkOrderReportData.WarehouseId,
                        WiproductLogs = {new WiproductLog
                        {
                            LinkOrder = OWorkOrderHead.WorkOrderNo,
                            Original = 0,
                            Quantity = WorkOrderReportData.ReCount,
                            Message = "品質檢驗入庫",
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        }}
                    });
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
                    item.UpdateUser = WorkOrderReportData.CreateUser;
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
                        CreateUser = WorkOrderReportData.CreateUser,
                    });

                    ////VVVVV  將內容自動回填至採購單內容 更新2020/08/18
                    var Warehouses = await _context.Warehouses.AsQueryable().Where(x => x.DeleteFlag == 0).ToListAsync();
                    var warehousesM = Warehouses.Where(x => x.Code == "101").FirstOrDefault(); // 原料內定代號 101
                    var warehousesW = Warehouses.Where(x => x.Code == "201").FirstOrDefault(); // 半成品內定代號 201
                    var warehousesP = Warehouses.Where(x => x.Code == "301").FirstOrDefault(); // 成品內定代號 301
                    var warehousesA = Warehouses.Where(x => x.Code == "201").FirstOrDefault(); // 半成品廠內內定代號 201
                    var warehousesB = Warehouses.Where(x => x.Code == "202").FirstOrDefault(); // 半成品廠外內定代號 202
                    var PurchaseHeads = await _context.PurchaseHeads.Include(x => x.PurchaseDetails).Where(x => x.Id == WorkOrderReportData.PurchaseId).FirstOrDefaultAsync();
                    var PurchaseDetails = PurchaseHeads.PurchaseDetails.Where(x =>
                        x.DataType == item.WorkOrderHead.DataType &&
                        x.DataId == item.WorkOrderHead.DataId &&
                        x.DeleteFlag == 0
                    ).ToList();
                    if (PurchaseDetails.Count() == 0)
                    { // 如果採購單明細沒有此成品，則回填(新增)採購單明細。
                        var BasicData = new BasicData();
                        var Warehouse201Check = 0;
                        decimal Warehouse201Stock = 0;
                        if (item.WorkOrderHead.DataType == 1) // 原料
                        {
                            var MaterialBasic = await _context.MaterialBasics.Include(x => x.Materials).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            BasicData.WarehouseId = warehousesP.Id;
                            BasicData.Specification = MaterialBasic.Specification;
                            BasicData.Price = MaterialBasic.Price;

                            var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }
                        else if (item.WorkOrderHead.DataType == 2) // 成品
                        {
                            var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            BasicData.WarehouseId = warehousesP.Id;
                            BasicData.Specification = ProductBasic.Specification;
                            BasicData.Price = ProductBasic.Price;

                            var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }
                        else if (item.WorkOrderHead.DataType == 3) // 半成品
                        {
                            var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            BasicData.WarehouseId = warehousesP.Id;
                            BasicData.Specification = WiproductBasic.Specification;
                            BasicData.Price = WiproductBasic.Price;

                            var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }

                        // 如採購單種類為[表處]，需要進行轉倉的動作 2020/09/09
                        if (PurchaseHeads.Type == 30)
                        {
                            var result = Warehouse201Fun(item, PurchaseHeads.PurchaseNo, Warehouse201Check, Warehouse201Stock, WorkOrderReportData, warehousesA.Id, warehousesB.Id);
                            if (!result.Result.success)
                            {
                                return Ok(await result);
                            }
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
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                    }
                    else // 如果已有相同成品，則增加數量並調整金額。
                    {
                        var Warehouse201Check = 0;
                        decimal Warehouse201Stock = 0;
                        if (item.WorkOrderHead.DataType == 1) // 原料
                        {
                            var MaterialBasic = await _context.MaterialBasics.Include(x => x.Materials).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }
                        else if (item.WorkOrderHead.DataType == 2) // 成品
                        {
                            var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }
                        else if (item.WorkOrderHead.DataType == 3) // 半成品
                        {
                            var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                        }

                        // 如採購單種類為[表處]，需要進行轉倉的動作 2020/09/09
                        if (PurchaseHeads.Type == 30)
                        {
                            var result = Warehouse201Fun(item, PurchaseHeads.PurchaseNo, Warehouse201Check, Warehouse201Stock, WorkOrderReportData, warehousesA.Id, warehousesB.Id);
                            if (!result.Result.success)
                            {
                                return Ok(await result);
                            }
                        }

                        PurchaseDetails.FirstOrDefault().Quantity += WorkOrderReportData.ReCount;
                        PurchaseDetails.FirstOrDefault().Price = PurchaseDetails.FirstOrDefault().Quantity * PurchaseDetails.FirstOrDefault().OriginPrice;
                        PurchaseDetails.FirstOrDefault().UpdateUser = WorkOrderReportData.CreateUser;
                    }
                    PurchaseHeads.PriceAll = PurchaseHeads.PurchaseDetails.Where(x => x.DeleteFlag == 0).Sum(x => x.Quantity * x.OriginPrice);
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
        private async Task<APIResponse> Warehouse201Fun(WorkOrderDetail itemData, string PurchaseNo, int Warehouse201Check, decimal Warehouse201Stock, WorkOrderReportData WorkOrderReportData, int warehousesA, int warehousesB)
        {
            if (Warehouse201Check == 0)
            {
                return MyFun.APIResponseError("品號 [ " + itemData.WorkOrderHead.DataNo + " ] 無庫存資訊(轉出)! 請重新確認!");
            }
            else
            {
                if (WorkOrderReportData.ReCount > Warehouse201Stock)
                {
                    return MyFun.APIResponseError("品號 [ " + itemData.WorkOrderHead.DataNo + " ] 轉出倉別庫存不足( 庫存 " + Warehouse201Stock + " / 需求 " + WorkOrderReportData.ReCount + " )! 請重新確認!");
                }
                else
                {
                    var dt = DateTime.Now;
                    if (itemData.WorkOrderHead.DataType == 1) // 原料
                    {
                        var MaterialBasic = _context.MaterialBasics.Find(itemData.WorkOrderHead.DataId);
                        var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().MaterialLogs.Add(new MaterialLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -WorkOrderReportData.ReCount,
                            Message = "表處轉倉",
                            CreateTime = dt.AddSeconds(-1),
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        Warehouse201.First().Quantity -= WorkOrderReportData.ReCount;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse202.First().MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = WorkOrderReportData.ReCount,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser
                            });
                            Warehouse202.First().Quantity += WorkOrderReportData.ReCount;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            MaterialBasic.Materials.Add(new Material
                            {
                                MaterialNo = MaterialBasic.MaterialNo,
                                Name = MaterialBasic.Name,
                                Quantity = WorkOrderReportData.ReCount,
                                Specification = MaterialBasic.Specification,
                                Property = MaterialBasic.Property,
                                Price = MaterialBasic.Price,
                                BaseQuantity = 2,
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser,
                                WarehouseId = warehousesB,
                                MaterialLogs = {new MaterialLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = WorkOrderReportData.ReCount,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = WorkOrderReportData.CreateUser
                                }}
                            });
                        }
                    }
                    else if (itemData.WorkOrderHead.DataType == 2) // 成品
                    {
                        var ProductBasic = _context.ProductBasics.Find(itemData.WorkOrderHead.DataId);
                        var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().ProductLogs.Add(new ProductLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -WorkOrderReportData.ReCount,
                            Message = "表處轉倉",
                            CreateTime = dt.AddSeconds(-1),
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        Warehouse201.First().Quantity -= WorkOrderReportData.ReCount;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse202.First().ProductLogs.Add(new ProductLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = WorkOrderReportData.ReCount,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser
                            });
                            Warehouse202.First().Quantity += WorkOrderReportData.ReCount;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            ProductBasic.Products.Add(new Product
                            {
                                ProductNo = ProductBasic.ProductNo,
                                ProductNumber = ProductBasic.ProductNumber,
                                Name = ProductBasic.Name,
                                Quantity = WorkOrderReportData.ReCount,
                                Specification = ProductBasic.Specification,
                                Property = ProductBasic.Property,
                                Price = ProductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser,
                                WarehouseId = warehousesB,
                                ProductLogs = {new ProductLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = WorkOrderReportData.ReCount,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = WorkOrderReportData.CreateUser
                                }}
                            });
                        }
                    }
                    else if (itemData.WorkOrderHead.DataType == 3) // 半成品
                    {
                        var WiproductBasic = _context.WiproductBasics.Find(itemData.WorkOrderHead.DataId);
                        var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().WiproductLogs.Add(new WiproductLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -WorkOrderReportData.ReCount,
                            Message = "表處轉倉",
                            CreateTime = dt.AddSeconds(-1),
                            CreateUser = WorkOrderReportData.CreateUser
                        });
                        Warehouse201.First().Quantity -= WorkOrderReportData.ReCount;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse202.First().WiproductLogs.Add(new WiproductLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = WorkOrderReportData.ReCount,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser
                            });
                            Warehouse202.First().Quantity += WorkOrderReportData.ReCount;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            WiproductBasic.Wiproducts.Add(new Wiproduct
                            {
                                WiproductNo = WiproductBasic.WiproductNo,
                                WiproductNumber = WiproductBasic.WiproductNumber,
                                Name = WiproductBasic.Name,
                                Quantity = WorkOrderReportData.ReCount,
                                Specification = WiproductBasic.Specification,
                                Property = WiproductBasic.Property,
                                Price = WiproductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser,
                                WarehouseId = warehousesB,
                                WiproductLogs = {new WiproductLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = WorkOrderReportData.ReCount,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = WorkOrderReportData.CreateUser
                                }}
                            });
                        }
                    }
                    return MyFun.APIResponseOK(itemData);
                }
            }
        }

        /// <summary>
        /// 工單結案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderReportData"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseHeads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> CloseWorkOrder(int id, WorkOrderReportData WorkOrderReportData)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var OWorkOrderHead = _context.WorkOrderHeads.Find(id);
            if (OWorkOrderHead.Status != 5)
            {
                OWorkOrderHead.Status = 5;//結案
                OWorkOrderHead.UpdateTime = DateTime.Now;
                OWorkOrderHead.UpdateUser = WorkOrderReportData.CreateUser;
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
                var Requisition = await _context.Requisitions.Where(x => x.WorkOrderHeadId == WorkOrderReportData.WorkOrderID).ToListAsync();
                if (Requisition.Count() == 0)
                {
                    return Ok(MyFun.APIResponseError("該工單尚未[領料]!"));
                }

                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    // if (WorkOrderDetails.FirstOrDefault().Status == 1)
                    // {
                    // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                    var checkLog = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0);
                    if (checkLog.Count() == 0)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        // WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        // WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                        if (WorkOrderDetails.FirstOrDefault().ActualStartTime == null)
                        {
                            WorkOrderDetails.FirstOrDefault().ActualStartTime = DateTime.Now;
                        }

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 1, // 開工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            // SupplierId = WorkOrderDetails.FirstOrDefault().SupplierId,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            CodeNo = WorkOrderReportData.CodeNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderReportData.ProducingMachine,
                            ReCount = 0,
                            // RePrice = WorkOrderDetails.FirstOrDefault().RePrice,
                            Message = WorkOrderReportData.Message,
                            StatusO = 1,
                            StatusN = 2,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = DateTime.Now,
                            ActualEndTime = null,
                            CreateTime = DateTime.Now,
                            CreateUser = WorkOrderReportData.CreateUser,
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportData.ProducingMachine + " ] 已經回報開工!"));
                    }

                    // }
                    // else
                    // {
                    //     return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    // }
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
                    // if (WorkOrderDetails.FirstOrDefault().Status == 2)
                    // {
                    // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                    var checkLogStart = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                    var checkLogEnd = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime != null && x.DeleteFlag == 0);
                    if ((checkLogStart.Count() - checkLogEnd.Count()) == 1)
                    {
                        // // Convert.ToInt32(((item?.ActualEndTime ?? dt) - (item?.ActualStartTime ?? dt)).TotalMinutes)
                        var ActualTimeDiff = DateTime.Now - (WorkOrderDetails.FirstOrDefault()?.ActualStartTime ?? DateTime.Now);
                        var Val = Convert.ToInt32(WorkOrderDetails.FirstOrDefault().ProcessTime * WorkOrderReportData.ReCount);
                        if(Convert.ToInt32(ActualTimeDiff.TotalMinutes) > Val){
                            WorkOrderDetails.FirstOrDefault().Status = 4;
                        }else{
                            WorkOrderDetails.FirstOrDefault().Status = 3;
                        }
                        WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        WorkOrderDetails.FirstOrDefault().ReCount = (WorkOrderDetails.FirstOrDefault()?.ReCount ?? 0) + WorkOrderReportData.ReCount;
                        WorkOrderDetails.FirstOrDefault().NgCount = (WorkOrderDetails.FirstOrDefault()?.NgCount ?? 0) + WorkOrderReportData.NgCount;
                        WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 2, // 完工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            SupplierId = WorkOrderDetails.FirstOrDefault().SupplierId,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            CodeNo = WorkOrderReportData.CodeNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderReportData.ProducingMachine,
                            ReCount = WorkOrderReportData.ReCount,
                            RePrice = WorkOrderReportData.RePrice,
                            NgCount = WorkOrderReportData.NgCount,
                            Message = WorkOrderReportData.Message,
                            StatusO = 2,
                            StatusN = 3,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = checkLogStart.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.FirstOrDefault().ActualEndTime,
                            CreateTime = DateTime.Now,
                            CreateUser = WorkOrderReportData.CreateUser,
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
                        return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportData.ProducingMachine + " ] 尚未回報開工!"));
                    }
                    // }
                    // else
                    // {
                    //     return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    // }
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
                    // if (WorkOrderDetails.FirstOrDefault().Status == 3)
                    // {
                    // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                    var checkLogStart = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0);
                    var checkLogEnd = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime != null && x.DeleteFlag == 0);
                    if (checkLogStart.Count() == checkLogEnd.Count())
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                        // WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        // WorkOrderDetails.FirstOrDefault().ReCount = WorkOrderReportData.ReCount;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 3, // 完工再開工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            CodeNo = WorkOrderReportData.CodeNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderReportData.ProducingMachine,
                            ReCount = 0,
                            Message = WorkOrderReportData.Message,
                            StatusO = 3,
                            StatusN = 2,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = DateTime.Now,
                            ActualEndTime = null,
                            CreateTime = DateTime.Now,
                            CreateUser = WorkOrderReportData.CreateUser,
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportData.ProducingMachine + " ] 已經回報開工!"));
                    }
                    // }
                    // else
                    // {
                    //     return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    // }
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
                var Requisition = await _context.Requisitions.Where(x => x.WorkOrderHeadId == WorkOrderDetails.WorkOrderHeadId).ToListAsync();
                if (Requisition.Count() == 0)
                {
                    return Ok(MyFun.APIResponseError("該工單尚未[領料]!"));
                }

                var WorkOrderReportData = new WorkOrderReportData
                {
                    ReCount = WorkOrderReportDataAll.ReportCount ?? 0,
                    NgCount = WorkOrderReportDataAll.ReportNgCount ?? 0,
                    Message = WorkOrderReportDataAll.Message,
                    ProducingMachine = WorkOrderReportDataAll.ProducingMachine,
                    WorkOrderID = WorkOrderDetails.WorkOrderHeadId,
                    WorkOrderSerial = WorkOrderDetails.SerialNumber,
                    CreateUser = WorkOrderReportDataAll.CreateUser,
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
                var StockInfo = "";
                var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).ToListAsync();
                if (DataType == 1)
                {

                }
                else if (DataType == 2)
                {
                    var BasicData = _context.ProductBasics.Find(OrderDetail.ProductBasicId);
                    BasicDataID = BasicData.Id;
                    BasicDataNo = BasicData.ProductNo;
                    BasicDataName = BasicData.Name;

                    StockInfo = "無庫存";
                    var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301");
                    if (WarehousesInfo.Count() != 0) {
                        var ProductsInfo = BasicData.Products.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                        if (ProductsInfo.Count() != 0) {
                            StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                        }
                    }
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

                var nWorkOrderHead = new WorkOrderHeadInfo
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
                    CreateUser = MyFun.GetUserID(HttpContext),
                    StockCount = StockInfo
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
                var checkData = OrderToWorkCheckData.WorkOrderHead.Where(x =>
                    x.OrderDetailId == nWorkOrderHead.OrderDetailId &&
                    x.DataType == nWorkOrderHead.DataType &&
                    x.DataId == nWorkOrderHead.DataId
                ).ToList();
                if (checkData.Count() == 1)
                {
                    nWorkOrderHead.Count = checkData.FirstOrDefault().Count;
                    foreach (var item in nWorkOrderHead.WorkOrderDetails)
                    {
                        item.Count = checkData.FirstOrDefault().Count;
                    }
                    _context.WorkOrderHeads.Add(nWorkOrderHead);
                }

                foreach (var item in Results)
                {
                    var checkDataTemp = OrderToWorkCheckData.WorkOrderHead.Where(x =>
                        x.OrderDetailId == item.OrderDetailId &&
                        x.DataType == item.DataType &&
                        x.DataId == item.DataId
                    ).ToList();
                    if (checkDataTemp.Count() == 1)
                    {
                        item.Count = checkDataTemp.FirstOrDefault().Count;
                        foreach (var item2 in item.WorkOrderDetails)
                        {
                            item2.Count = checkDataTemp.FirstOrDefault().Count;
                        }
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
                        var StockInfo ="";
                        var Warehouses = _context.Warehouses.Where(x => x.DeleteFlag == 0);
                        if (item.MaterialBasicId != null)
                        {
                            var BasicData = _context.MaterialBasics.Find(item.MaterialBasicId);
                            DataType = 1;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.MaterialNo;
                            BasicDataName = BasicData.Name;
                            
                            StockInfo = "無庫存";
                            var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "101");
                            if (WarehousesInfo.Count() != 0) {
                                var ProductsInfo = BasicData.Materials.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                                if (ProductsInfo.Count() != 0) {
                                    StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                                }
                            }
                        }
                        else if (item.ProductBasicId != null)
                        {
                            var BasicData = _context.ProductBasics.Find(item.ProductBasicId);
                            DataType = 2;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.ProductNo;
                            BasicDataName = BasicData.Name;
                            
                            StockInfo = "無庫存";
                            var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301");
                            if (WarehousesInfo.Count() != 0) {
                                var ProductsInfo = BasicData.Products.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                                if (ProductsInfo.Count() != 0) {
                                    StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                                }
                            }
                        }

                        var status = 0; // 工單是否已建立，和是否有MBOM (0否 1是 2無MBOM)
                        var CheckWorkOrderHeads = _context.WorkOrderHeads.Where(x => x.OrderDetailId == WorkOrderHead.OrderDetailId && x.DataType == DataType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToList();
                        if (CheckWorkOrderHeads.Count() > 0)
                        {
                            status = 1;
                        }

                        var nWorkOrderHead = new WorkOrderHeadInfo
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
                            CreateUser = MyFun.GetUserID(HttpContext),
                            StockCount = StockInfo
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
        public async Task<ActionResult<WorkOrderLog>> GetWorkOrderReportLogByNum(string machine)
        {
            var data = await _context.WorkOrderReportLogs.AsQueryable().Where(x => x.DeleteFlag == 0 && x.ProducingMachine == machine)
            .OrderByDescending(x => x.CreateTime).Include(x => x.WorkOrderDetail).ThenInclude(x => x.WorkOrderHead).ToListAsync();
            var WorkOrderLog = new List<WorkOrderLog>();
            foreach (var item in data)
            {
                var dt = DateTime.Now;
                var tempData = new WorkOrderLog
                {
                    WorkOrderNo = item.WorkOrderDetail.WorkOrderHead.WorkOrderNo,
                    ReportType = item.ReportType,
                    SerialNumber = item.WorkOrderDetail.SerialNumber,
                    Process = item.WorkOrderDetail.ProcessNo + '_' + item.WorkOrderDetail.ProcessName,
                    ProcessTime = Convert.ToInt32(((item?.ActualEndTime ?? dt) - (item?.ActualStartTime ?? dt)).TotalMinutes),
                    ReCount = item.ReCount,
                    Remarks = item.WorkOrderDetail.Remarks,
                    StatusO = item.StatusO,
                    StatusN = item.StatusN,
                    DueStartTime = item.DueStartTime,
                    DueEndTime = item.DueEndTime,
                    ActualStartTime = item.ActualStartTime,
                    ActualEndTime = item.ActualEndTime,
                    CreateTime = item.CreateTime
                };
                WorkOrderLog.Add(tempData);
            }
            return Ok(MyFun.APIResponseOK(WorkOrderLog));
        }

        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 查詢機台報工紀錄 By 機台名稱
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ResourceProcessData>> GetProcessByMachineName(string machine)
        {
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == machine && 
            x.WorkOrderHead.DeleteFlag == 0 && (x.WorkOrderHead.Status == 1 || x.WorkOrderHead.Status == 5))
            .Include(x => x.WorkOrderHead).ToListAsync();
            return Ok(MyFun.APIResponseOK(WorkOrderDetails));
        }    

        /// <summary>
        /// 產報工單PDF
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetPackingSlipPDF(int id)
        {
            var qcodesize = 70;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var ProcessReportVMList = new List<ProcessReportVM>();
            var WorkOrder = _context.WorkOrderHeads.Find(id);
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
