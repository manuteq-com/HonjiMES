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
using AutoMapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 產品列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly HonjiContext _context;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IMapper _mapper;
        public WorkOrdersController(HonjiContext context, IWebHostEnvironment environment, IMapper mapper)
        {
            _mapper = mapper;
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
            _context.ChangeTracker.LazyLoadingEnabled = true;

            //這裡只是為了快速的產生工序，之後看情況要拿掉就拿掉
            var WorkOrderHeads = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && !x.WorkOrderDetails.Where(y => y.DeleteFlag == 0 && (x.Status == 0 || x.Status == 4)).Any()).Include(x => x.WorkOrderDetails).ToList();// 工單Head為[新建、轉單] 才自動補回
            foreach (var item in WorkOrderHeads)
            {
                if (!item.WorkOrderDetails.Any())
                {
                    var billOfMaterial = _context.MBillOfMaterials.AsQueryable().Where(x => x.MaterialBasicId == item.DataId).OrderBy(x => x.SerialNumber).ToList();
                    if (billOfMaterial.Any())
                    {
                        foreach (var bomitem in billOfMaterial)
                        {
                            item.WorkOrderDetails.Add(new WorkOrderDetail
                            {
                                SerialNumber = bomitem.SerialNumber,
                                ProcessId = bomitem.ProcessId,
                                ProcessNo = bomitem.ProcessNo,
                                ProcessName = bomitem.ProcessName,
                                ProcessLeadTime = bomitem.ProcessLeadTime,
                                ProcessTime = bomitem.ProcessTime,
                                ProcessCost = bomitem.ProcessCost,
                                DrawNo = bomitem.DrawNo,
                                Manpower = bomitem.Manpower,
                                ProducingMachine = bomitem.ProducingMachine,
                                Remarks = bomitem.Remarks,
                                DeleteFlag = 0,
                                CreateUser = MyFun.GetUserID(HttpContext),
                            });
                        }
                        _context.SaveChanges();
                    }
                }
            }
            // var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).Include(x => x.OrderDetail).OrderByDescending(x => x.CreateTime);
            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).Include(x => x.OrderDetail).Include(x => x.WorkOrderDetails)
            .OrderByDescending(x => x.CreateTime).Select(x => new WorkOrderHeadInfo
            {
                Id = x.Id,
                WorkOrderNo = x.WorkOrderNo,
                OrderDetailId = x.OrderDetailId,
                MachineNo = x.MachineNo,
                DataType = x.DataType,
                DataId = x.DataId,
                DataNo = x.DataNo,
                DataName = x.DataName,
                Count = x.Count,
                ReCount = x.ReCount,
                Status = x.Status,
                TotalTime = x.TotalTime,
                DispatchTime = x.DispatchTime,
                DueStartTime = x.DueStartTime,
                DueEndTime = x.DueEndTime,
                ActualStartTime = x.ActualStartTime,
                ActualEndTime = x.ActualEndTime,
                DeleteFlag = x.DeleteFlag,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                WorkOrderDetails = x.WorkOrderDetails,
                OrderCount = (decimal)x.OrderCount,

            });

            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.WorkOrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            _context.ChangeTracker.LazyLoadingEnabled = false;
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
            _context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料
            // var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && x.Status != 0 && x.Status != 5 && x.Status != 7);
            //status:5(結案)、7(工序暫停)
            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0 && x.Status != 0 && x.Status != 5 && x.Status != 7).Include(x => x.OrderDetail).OrderByDescending(x => x.CreateTime).Select(x => new WorkOrderHeadInfo
            {
                Id = x.Id,
                WorkOrderNo = x.WorkOrderNo,
                OrderDetailId = x.OrderDetailId,
                MachineNo = x.MachineNo,
                DataType = x.DataType,
                DataId = x.DataId,
                DataNo = x.DataNo,
                DataName = x.DataName,
                Count = x.Count,
                ReCount = x.ReCount,
                Status = x.Status,
                TotalTime = x.TotalTime,
                DispatchTime = x.DispatchTime,
                DueStartTime = x.DueStartTime,
                DueEndTime = x.DueEndTime,
                ActualStartTime = x.ActualStartTime,
                ActualEndTime = x.ActualEndTime,
                DeleteFlag = x.DeleteFlag,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                OrderCount = x.OrderDetail.Quantity,
            });
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.WorkOrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            _context.ChangeTracker.LazyLoadingEnabled = false;
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
            if (WorkOrderHead.Count() == 0)
            {
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
            try
            {
                var workOrderHead = await _context.WorkOrderHeads.FindAsync(id);
                if (workOrderHead != null)
                {
                    var WorkOrderDetail = _context.WorkOrderDetails.Where(x => x.WorkOrderHeadId == id && x.DeleteFlag == 0).OrderBy(x => x.SerialNumber).ToList();
                    var WorkOrderDetailDatalist = _mapper.Map<List<WorkOrderDetailData>>(WorkOrderDetail);
                    foreach (var item in WorkOrderDetailDatalist)
                    {
                        item.ExpectedlTotalTime = (item.ProcessLeadTime + item.ProcessTime) * workOrderHead.Count;
                    }
                    var data = new WorkOrderData
                    {
                        WorkOrderHead = workOrderHead,
                        WorkOrderDetailData = WorkOrderDetailDatalist
                    };
                    return Ok(MyFun.APIResponseOK(data));
                }
                else
                {
                    return Ok(MyFun.APIResponseError("工單查詢失敗!"));
                }
            }
            catch (System.Exception ex)
            {

                throw;
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
        /// 查詢工單明細的報工紀錄
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetWorkOrderQCLogByWorkOrderDetailId(int id)
        {
            var WorkOrderReportLogs = await _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0 && x.WorkOrderDetailId == id).Include(x => x.Supplier).ToListAsync();
            var WorkOrderQcLogs = await _context.WorkOrderQcLogs.Where(x => x.DeleteFlag == 0 && x.WorkOrderDetailId == id).ToListAsync();
            var data = new List<WorkOrderReportLogData>();
            foreach (var item in WorkOrderReportLogs)
            {
                var QClog = WorkOrderQcLogs.Where(x => x.CreateTime == item.CreateTime).FirstOrDefault();
                data.Add(new WorkOrderReportLogData
                {
                    CreateTime = item.CreateTime,
                    CreateUser = item.CreateUser,
                    ReportType = item.ReportType,
                    // ProducingMachine = item.ProducingMachine,
                    // ReCount = item.ReCount,
                    // NgCount = item.NgCount,
                    // RePrice = item.RePrice,
                    // DrawNo = item.DrawNo,
                    // PurchaseNo = item.PurchaseNo,
                    // Supplier = item.Supplier,
                    // Message = item.Message,
                    QCReportType = QClog?.ReportType ?? 0,
                    QCReCount = QClog?.ReCount ?? 0,
                    QCCkCount = QClog?.CkCount ?? 0,
                    QCOkCount = QClog?.OkCount ?? 0,
                    QCNgCount = QClog?.NgCount ?? 0,
                    QCNcCount = QClog?.NcCount ?? 0,
                    // CheckResult = QClog?.CheckResult ?? 0,
                    QCMessage = QClog?.Message ?? null,
                    // DueStartTime = item.DueStartTime,
                    // DueEndTime = item.DueEndTime,
                    ActualStartTime = item.ActualStartTime,
                    ActualEndTime = item.ActualEndTime,
                });
            }

            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 查詢製程資訊By工單明細Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Process>>> GetWorkOrderProcessByWorkOrderDetailId(int id)
        {
            var data = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Id == id).Include(x => x.Process).Select(x => new Process
            {
                Id = x.Process.Id,
                Name = x.Process.Name,
                Code = x.Process.Code,
                Type = x.Process.Type,
                LeadTime = x.Process.LeadTime,
                WorkTime = x.Process.WorkTime,
                Cost = x.Process.Cost,
                DrawNo = x.Process.DrawNo,
                Manpower = x.Process.Manpower,
                ProducingMachine = x.Process.ProducingMachine,
                Remark = x.Process.Remark,
                CreateTime = x.Process.CreateTime,
                CreateUser = x.Process.CreateUser,
                UpdateTime = x.Process.UpdateTime,
                UpdateUser = x.Process.UpdateUser,
                DeleteFlag = x.Process.DeleteFlag,
            }).FirstOrDefaultAsync();
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
                    // var CheckWorkOrder = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DeleteFlag == 0).AnyAsync();
                    var CheckWorkOrder = await _context.OrderDetailAndWorkOrderHeads.Where(x => x.OrderDetailId == item.Id
                    && x.OrderDetail.DueDate == item.DueDate && x.DataId == item.MaterialBasicId && x.DeleteFlag == 0).AnyAsync();
                    var CheckMaterialBasic = OrderDetailList.Where(x => x.MaterialBasicId == item.MaterialBasicId && x.DeleteFlag == 0);
                    if (CheckMaterialBasic.Count() == 0 || CheckWorkOrder)
                    {
                        item.DeleteFlag = CheckWorkOrder ? 1 : 0; // 借用欄位，用來辨識是否要合併!
                        OrderDetailList.Add(item);
                    }
                    else
                    {
                        CheckMaterialBasic.FirstOrDefault().Quantity += item.Quantity;
                    }
                }

                foreach (var item in OrderDetailList)
                {
                    item.DeleteFlag = 0;
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
                var OrderDetailList = new List<ToWorksOrderDetail>();
                foreach (var item in OrderData.OrderDetail)
                {
                    // var CheckWorkOrder = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DeleteFlag == 0).AnyAsync();
                    var CheckWorkOrder = await _context.OrderDetailAndWorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DataId == item.MaterialBasicId && x.DeleteFlag == 0).AnyAsync();
                    var CheckMaterialBasic = OrderDetailList.Where(x => x.MaterialBasicId == item.MaterialBasicId && x.DeleteFlag == 0);
                    if (CheckMaterialBasic.Count() == 0 || CheckWorkOrder)
                    {
                        item.DeleteFlag = CheckWorkOrder ? 1 : 0; // 借用欄位，用來辨識是否要合併!
                        var OrderDetailIdList = new List<OrderDetailIdListInfo>();
                        OrderDetailIdList.Add(new OrderDetailIdListInfo { OrderDetailId = item.Id, Count = item.Quantity });
                        OrderDetailList.Add(new ToWorksOrderDetail
                        {
                            Id = item.Id,
                            OrderId = item.OrderId,
                            CustomerNo = item.CustomerNo,
                            Serial = item.Serial,
                            MaterialBasicId = item.MaterialBasicId,
                            MaterialId = item.MaterialId,
                            Quantity = item.Quantity,
                            OriginPrice = item.OriginPrice,
                            Discount = item.Discount,
                            DiscountPrice = item.DiscountPrice,
                            Price = item.Price,
                            Delivered = item.Delivered,
                            Unit = item.Unit,
                            DueDate = item.DueDate,
                            Remark = item.Remark,
                            ReplyDate = item.ReplyDate,
                            ReplyRemark = item.ReplyRemark,
                            MachineNo = item.MachineNo,
                            Drawing = item.Drawing,
                            Ink = item.Ink,
                            Label = item.Label,
                            Package = item.Package,
                            Reply = item.Reply,
                            SaleCount = item.SaleCount,
                            SaledCount = item.SaledCount,
                            DeleteFlag = item.DeleteFlag,
                            CreateTime = item.CreateTime,
                            CreateUser = item.CreateUser,
                            UpdateTime = item.UpdateTime,
                            UpdateUser = item.UpdateUser,
                            OrderDetailIdList = OrderDetailIdList
                        });
                    }
                    else
                    {
                        CheckMaterialBasic.FirstOrDefault().Quantity += item.Quantity;
                        CheckMaterialBasic.FirstOrDefault().OrderDetailIdList.Add(new OrderDetailIdListInfo { OrderDetailId = item.Id, Count = item.Quantity });
                    }
                }

                foreach (var item in OrderDetailList)
                {
                    item.DeleteFlag = 0;
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
                if (NewResultMessage.Length == 0)
                {
                    return Ok(MyFun.APIResponseOK("OK"));
                }
                else
                {
                    return Ok(MyFun.APIResponseOK("OK", NewResultMessage));
                }
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

            if (WorkOrderReportData.Type == 0) // 入庫種類為半成品入庫時，不會更新[工單量]
            {

            }
            else if (WorkOrderReportData.Type == 1)
            {
                // 入庫數量不能大於工單量 2020/10/05
                if ((OWorkOrderHead.ReCount + WorkOrderReportData.ReCount) > OWorkOrderHead.Count)
                {
                    return Ok(MyFun.APIResponseError("入庫數量已超過工單量!! ( 工單量:" + OWorkOrderHead.Count + "　已完工量:" + OWorkOrderHead.ReCount + " )"));
                }
                OWorkOrderHead.ReCount = OWorkOrderHead.ReCount + WorkOrderReportData.ReCount;
                OWorkOrderHead.UpdateTime = DateTime.Now;
                OWorkOrderHead.UpdateUser = WorkOrderReportData.CreateUser;

                // 入庫數量抵達工單量以後，工單自動結案 2020/10/05
                if (OWorkOrderHead.Count == OWorkOrderHead.ReCount)
                {
                    OWorkOrderHead.Status = 5;//結案
                }
            }

            //入庫
            Decimal tempOriginal = 0;
            Decimal tempQuantity = 0;
            var tempDataNo = "";
            var MessageType = "";
            if (WorkOrderReportData.Type == 0)
            {
                MessageType = "[入庫單]半成品入庫";
            }
            else if (WorkOrderReportData.Type == 1)
            {
                MessageType = "[入庫單]成品入庫";
            }
            var checkInfo = false;
            // if (OWorkOrderHead.DataType == 1) // 原料
            // {
            var MaterialBasic = await _context.MaterialBasics.Include(x => x.Materials).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
            tempDataNo = MaterialBasic.MaterialNo;
            foreach (var item in MaterialBasic.Materials)
            {
                if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
                {
                    tempOriginal += item.Quantity;
                    tempQuantity += WorkOrderReportData.ReCount;
                    item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
                    // item.UpdateTime = dt;
                    // item.UpdateUser = WorkOrderReportData.CreateUser;
                    item.MaterialLogs.Add(new MaterialLog
                    {
                        LinkOrder = OWorkOrderHead.WorkOrderNo,
                        Original = item.Quantity - WorkOrderReportData.ReCount,
                        Quantity = WorkOrderReportData.ReCount,
                        Message = MessageType,
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
                            Message = MessageType,
                            CreateTime = dt,
                            CreateUser = WorkOrderReportData.CreateUser
                        }}
                });
                tempQuantity = WorkOrderReportData.ReCount;
            }
            // }
            // else if (OWorkOrderHead.DataType == 2) // 成品
            // {
            //     var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
            //     tempDataNo = ProductBasic.ProductNo;
            //     foreach (var item in ProductBasic.Products)
            //     {
            //         if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
            //         {
            //             tempOriginal += item.Quantity;
            //             tempQuantity += WorkOrderReportData.ReCount;
            //             item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
            //             // item.UpdateTime = dt;
            //             // item.UpdateUser = WorkOrderReportData.CreateUser;
            //             item.ProductLogs.Add(new ProductLog
            //             {
            //                 LinkOrder = OWorkOrderHead.WorkOrderNo,
            //                 Original = item.Quantity - WorkOrderReportData.ReCount,
            //                 Quantity = WorkOrderReportData.ReCount,
            //                 Message = MessageType,
            //                 CreateTime = dt,
            //                 CreateUser = WorkOrderReportData.CreateUser
            //             });
            //             checkInfo = true;
            //         }
            //     }

            //     // 如果沒有明細資訊，則自動新增。
            //     if (!checkInfo)
            //     {
            //         ProductBasic.Products.Add(new Product
            //         {
            //             ProductNo = ProductBasic.ProductNo,
            //             ProductNumber = ProductBasic.ProductNumber,
            //             Name = ProductBasic.Name,
            //             Quantity = WorkOrderReportData.ReCount,
            //             Specification = ProductBasic.Specification,
            //             Property = ProductBasic.Property,
            //             Price = ProductBasic.Price,
            //             MaterialRequire = 1,
            //             CreateTime = dt,
            //             CreateUser = WorkOrderReportData.CreateUser,
            //             WarehouseId = WorkOrderReportData.WarehouseId,
            //             ProductLogs = {new ProductLog
            //             {
            //                 LinkOrder = OWorkOrderHead.WorkOrderNo,
            //                 Original = 0,
            //                 Quantity = WorkOrderReportData.ReCount,
            //                 Message = MessageType,
            //                 CreateTime = dt,
            //                 CreateUser = WorkOrderReportData.CreateUser
            //             }}
            //         });
            //         tempQuantity = WorkOrderReportData.ReCount;
            //     }
            // }
            // else if (OWorkOrderHead.DataType == 3) // 半成品
            // {
            //     var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == OWorkOrderHead.DataId && x.DeleteFlag == 0).FirstAsync();
            //     tempDataNo = WiproductBasic.WiproductNo;
            //     foreach (var item in WiproductBasic.Wiproducts)
            //     {
            //         if (item.WarehouseId == WorkOrderReportData.WarehouseId && item.DeleteFlag == 0)
            //         {
            //             tempOriginal += item.Quantity;
            //             tempQuantity += WorkOrderReportData.ReCount;
            //             item.Quantity = item.Quantity + WorkOrderReportData.ReCount;
            //             // item.UpdateTime = dt;
            //             // item.UpdateUser = WorkOrderReportData.CreateUser;
            //             item.WiproductLogs.Add(new WiproductLog
            //             {
            //                 LinkOrder = OWorkOrderHead.WorkOrderNo,
            //                 Original = item.Quantity - WorkOrderReportData.ReCount,
            //                 Quantity = WorkOrderReportData.ReCount,
            //                 Message = MessageType,
            //                 CreateTime = dt,
            //                 CreateUser = WorkOrderReportData.CreateUser
            //             });
            //             checkInfo = true;
            //         }
            //     }

            //     // 如果沒有明細資訊，則自動新增。
            //     if (!checkInfo)
            //     {
            //         WiproductBasic.Wiproducts.Add(new Wiproduct
            //         {
            //             WiproductNo = WiproductBasic.WiproductNo,
            //             WiproductNumber = WiproductBasic.WiproductNumber,
            //             Name = WiproductBasic.Name,
            //             Quantity = WorkOrderReportData.ReCount,
            //             Specification = WiproductBasic.Specification,
            //             Property = WiproductBasic.Property,
            //             Price = WiproductBasic.Price,
            //             MaterialRequire = 1,
            //             CreateTime = dt,
            //             CreateUser = WorkOrderReportData.CreateUser,
            //             WarehouseId = WorkOrderReportData.WarehouseId,
            //             WiproductLogs = {new WiproductLog
            //             {
            //                 LinkOrder = OWorkOrderHead.WorkOrderNo,
            //                 Original = 0,
            //                 Quantity = WorkOrderReportData.ReCount,
            //                 Message = MessageType,
            //                 CreateTime = dt,
            //                 CreateUser = WorkOrderReportData.CreateUser
            //             }}
            //         });
            //         tempQuantity = WorkOrderReportData.ReCount;
            //     }
            // }

            // 建立入庫單
            var key = "AS";
            var StockNo = DateTime.Now.ToString("yyMMdd");

            var NoData = await _context.StockHeads.AsQueryable().Where(x => x.StockNo.Contains(key + StockNo) && x.StockNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastStockNo = NoData.FirstOrDefault().StockNo;
                var NoLast = Int32.Parse(LastStockNo.Substring(LastStockNo.Length - 3, 3));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            //// 建立主檔
            var StockHead = new StockHead
            {
                StockNo = key + StockNo + NoCount.ToString("000"),
                LinkOrder = OWorkOrderHead.WorkOrderNo,
                Type = WorkOrderReportData.Type,
                Remarks = MessageType,
                CreateTime = dt,
                CreateUser = WorkOrderReportData.CreateUser
            };
            //// 建立明細
            StockHead.StockDetails.Add(new StockDetail
            {
                ItemType = OWorkOrderHead.DataType,
                ItemId = OWorkOrderHead.DataId,
                DataNo = tempDataNo,
                Original = tempOriginal,
                Quantity = tempQuantity,
                // Price = null,
                // PriceAll = null,
                // Unit = null,
                // UnitCount = null,
                // UnitPrice = null,
                // UnitPriceAll = null,
                // WorkPrice = null,
                // Reason = null,
                Message = "",
                CreateTime = dt,
                CreateUser = WorkOrderReportData.CreateUser
            });
            _context.StockHeads.Add(StockHead);

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
            // 如果[工單Head]狀態為[已派工]，則改為[以開工]
            if (OWorkOrderHead.Status == 1)
            {
                OWorkOrderHead.Status = 2;
            }
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
                        ProducingMachine = item.ProducingMachine == "" ? null : item.ProducingMachine,
                        MCount = WorkOrderReportData.MCount,
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

                    if (WorkOrderReportData.CheckResult == 1)
                    { // 增加可選擇按鈕(要回填/不回填)
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
                            // if (item.WorkOrderHead.DataType == 1) // 原料
                            // {
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
                            // }
                            // else if (item.WorkOrderHead.DataType == 2) // 成品
                            // {
                            //     var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            //     BasicData.WarehouseId = warehousesP.Id;
                            //     BasicData.Specification = ProductBasic.Specification;
                            //     BasicData.Price = ProductBasic.Price;

                            //     var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            //     if (Warehouse201.Count() != 0)
                            //     {
                            //         Warehouse201Check = Warehouse201.Count();
                            //         Warehouse201Stock = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            //     }
                            // }
                            // else if (item.WorkOrderHead.DataType == 3) // 半成品
                            // {
                            //     var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            //     BasicData.WarehouseId = warehousesP.Id;
                            //     BasicData.Specification = WiproductBasic.Specification;
                            //     BasicData.Price = WiproductBasic.Price;

                            //     var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            //     if (Warehouse201.Count() != 0)
                            //     {
                            //         Warehouse201Check = Warehouse201.Count();
                            //         Warehouse201Stock = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            //     }
                            // }

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
                                CreateUser = WorkOrderReportData.CreateUser,
                                WorkOrderLog = item.WorkOrderHead.WorkOrderNo + ",",
                            });

                        }
                        else // 如果已有相同成品，則增加數量並調整金額。
                        {
                            var Warehouse201Check = 0;
                            decimal Warehouse201Stock = 0;
                            // if (item.WorkOrderHead.DataType == 1) // 原料
                            // {
                            var MaterialBasic = await _context.MaterialBasics.Include(x => x.Materials).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            if (Warehouse201.Count() != 0)
                            {
                                Warehouse201Check = Warehouse201.Count();
                                Warehouse201Stock = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            }
                            // }
                            // else if (item.WorkOrderHead.DataType == 2) // 成品
                            // {
                            //     var ProductBasic = await _context.ProductBasics.Include(x => x.Products).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            //     var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            //     if (Warehouse201.Count() != 0)
                            //     {
                            //         Warehouse201Check = Warehouse201.Count();
                            //         Warehouse201Stock = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            //     }
                            // }
                            // else if (item.WorkOrderHead.DataType == 3) // 半成品
                            // {
                            //     var WiproductBasic = await _context.WiproductBasics.Include(x => x.Wiproducts).Where(x => x.Id == item.WorkOrderHead.DataId).FirstAsync();
                            //     var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).ToList();
                            //     if (Warehouse201.Count() != 0)
                            //     {
                            //         Warehouse201Check = Warehouse201.Count();
                            //         Warehouse201Stock = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA.Id && x.DeleteFlag == 0).First().Quantity;
                            //     }
                            // }

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
                            PurchaseDetails.FirstOrDefault().WorkOrderLog += OWorkOrderHead.WorkOrderNo + ",";
                        }
                        PurchaseHeads.PriceAll = PurchaseHeads.PurchaseDetails.Where(x => x.DeleteFlag == 0).Sum(x => x.Quantity * x.OriginPrice);
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
                    // if (itemData.WorkOrderHead.DataType == 1) // 原料
                    // {
                    var MaterialBasic = _context.MaterialBasics.Find(itemData.WorkOrderHead.DataId);
                    var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                    var Warehouse202 = MaterialBasic.Materials.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                    Warehouse201.First().MaterialLogs.Add(new MaterialLog
                    {
                        LinkOrder = PurchaseNo,
                        Original = Warehouse201.First().Quantity,
                        Quantity = -WorkOrderReportData.ReCount,
                        Message = "[轉倉]表處採購單",
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
                            Message = "[轉倉]表處採購單",
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
                                    Message = "[轉倉]表處採購單",
                                    CreateTime = dt,
                                    CreateUser = WorkOrderReportData.CreateUser
                                }}
                        });
                    }
                    // }
                    // else if (itemData.WorkOrderHead.DataType == 2) // 成品
                    // {
                    //     var ProductBasic = _context.ProductBasics.Find(itemData.WorkOrderHead.DataId);
                    //     var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                    //     var Warehouse202 = ProductBasic.Products.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                    //     Warehouse201.First().ProductLogs.Add(new ProductLog
                    //     {
                    //         LinkOrder = PurchaseNo,
                    //         Original = Warehouse201.First().Quantity,
                    //         Quantity = -WorkOrderReportData.ReCount,
                    //         Message = "[轉倉]表處採購單",
                    //         CreateTime = dt.AddSeconds(-1),
                    //         CreateUser = WorkOrderReportData.CreateUser
                    //     });
                    //     Warehouse201.First().Quantity -= WorkOrderReportData.ReCount;

                    //     if (Warehouse202.Count() != 0)
                    //     {
                    //         Warehouse202.First().ProductLogs.Add(new ProductLog
                    //         {
                    //             LinkOrder = PurchaseNo,
                    //             Original = Warehouse202.First().Quantity,
                    //             Quantity = WorkOrderReportData.ReCount,
                    //             Message = "[轉倉]表處採購單",
                    //             CreateTime = dt,
                    //             CreateUser = WorkOrderReportData.CreateUser
                    //         });
                    //         Warehouse202.First().Quantity += WorkOrderReportData.ReCount;
                    //     }
                    //     else // 如無倉別資訊，則自動建立
                    //     {
                    //         ProductBasic.Products.Add(new Product
                    //         {
                    //             ProductNo = ProductBasic.ProductNo,
                    //             ProductNumber = ProductBasic.ProductNumber,
                    //             Name = ProductBasic.Name,
                    //             Quantity = WorkOrderReportData.ReCount,
                    //             Specification = ProductBasic.Specification,
                    //             Property = ProductBasic.Property,
                    //             Price = ProductBasic.Price,
                    //             MaterialRequire = 1,
                    //             CreateTime = dt,
                    //             CreateUser = WorkOrderReportData.CreateUser,
                    //             WarehouseId = warehousesB,
                    //             ProductLogs = {new ProductLog
                    //             {
                    //                 LinkOrder = PurchaseNo,
                    //                 Original = 0,
                    //                 Quantity = WorkOrderReportData.ReCount,
                    //                 Message = "[轉倉]表處採購單",
                    //                 CreateTime = dt,
                    //                 CreateUser = WorkOrderReportData.CreateUser
                    //             }}
                    //         });
                    //     }
                    // }
                    // else if (itemData.WorkOrderHead.DataType == 3) // 半成品
                    // {
                    //     var WiproductBasic = _context.WiproductBasics.Find(itemData.WorkOrderHead.DataId);
                    //     var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesA && x.DeleteFlag == 0).ToList();
                    //     var Warehouse202 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == warehousesB && x.DeleteFlag == 0).ToList();

                    //     Warehouse201.First().WiproductLogs.Add(new WiproductLog
                    //     {
                    //         LinkOrder = PurchaseNo,
                    //         Original = Warehouse201.First().Quantity,
                    //         Quantity = -WorkOrderReportData.ReCount,
                    //         Message = "[轉倉]表處採購單",
                    //         CreateTime = dt.AddSeconds(-1),
                    //         CreateUser = WorkOrderReportData.CreateUser
                    //     });
                    //     Warehouse201.First().Quantity -= WorkOrderReportData.ReCount;

                    //     if (Warehouse202.Count() != 0)
                    //     {
                    //         Warehouse202.First().WiproductLogs.Add(new WiproductLog
                    //         {
                    //             LinkOrder = PurchaseNo,
                    //             Original = Warehouse202.First().Quantity,
                    //             Quantity = WorkOrderReportData.ReCount,
                    //             Message = "[轉倉]表處採購單",
                    //             CreateTime = dt,
                    //             CreateUser = WorkOrderReportData.CreateUser
                    //         });
                    //         Warehouse202.First().Quantity += WorkOrderReportData.ReCount;
                    //     }
                    //     else // 如無倉別資訊，則自動建立
                    //     {
                    //         WiproductBasic.Wiproducts.Add(new Wiproduct
                    //         {
                    //             WiproductNo = WiproductBasic.WiproductNo,
                    //             WiproductNumber = WiproductBasic.WiproductNumber,
                    //             Name = WiproductBasic.Name,
                    //             Quantity = WorkOrderReportData.ReCount,
                    //             Specification = WiproductBasic.Specification,
                    //             Property = WiproductBasic.Property,
                    //             Price = WiproductBasic.Price,
                    //             MaterialRequire = 1,
                    //             CreateTime = dt,
                    //             CreateUser = WorkOrderReportData.CreateUser,
                    //             WarehouseId = warehousesB,
                    //             WiproductLogs = {new WiproductLog
                    //             {
                    //                 LinkOrder = PurchaseNo,
                    //                 Original = 0,
                    //                 Quantity = WorkOrderReportData.ReCount,
                    //                 Message = "[轉倉]表處採購單",
                    //                 CreateTime = dt,
                    //                 CreateUser = WorkOrderReportData.CreateUser
                    //             }}
                    //         });
                    //     }
                    // }
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

                // 如果[工單Head]狀態為[已派工]，則改為[以開工]
                if (WorkOrderHeads.Status == 1)
                {
                    WorkOrderHeads.Status = 2;
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
                        WorkOrderDetails.FirstOrDefault().Status = 2; // 開工
                        // WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        // WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                        WorkOrderDetails.FirstOrDefault().ProducingMachine = WorkOrderReportData.ProducingMachine;
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
                            MCount = WorkOrderReportData.MCount,
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
                        var dt = DateTime.Now;
                        var Val = Convert.ToInt32(WorkOrderDetails.FirstOrDefault().ProcessTime * WorkOrderReportData.ReCount);
                        if (Val != 0)
                        {
                            // // Convert.ToInt32(((item?.ActualEndTime ?? dt) - (item?.ActualStartTime ?? dt)).TotalMinutes)
                            var ActualTimeDiff = dt - (WorkOrderDetails.FirstOrDefault()?.ActualStartTime ?? dt);
                            if (Convert.ToInt32(ActualTimeDiff.TotalMinutes) > Val)
                            {
                                WorkOrderDetails.FirstOrDefault().Status = 6;// 完工(超時)
                            }
                            else
                            {
                                WorkOrderDetails.FirstOrDefault().Status = 3;// 完工
                            }
                        }
                        else
                        {
                            WorkOrderDetails.FirstOrDefault().Status = 3; // 完工
                        }

                        WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                        WorkOrderDetails.FirstOrDefault().ActualEndTime = dt;
                        WorkOrderDetails.FirstOrDefault().ReCount = (WorkOrderDetails.FirstOrDefault()?.ReCount ?? 0) + WorkOrderReportData.ReCount;
                        WorkOrderDetails.FirstOrDefault().NgCount = (WorkOrderDetails.FirstOrDefault()?.NgCount ?? 0) + WorkOrderReportData.NgCount;
                        WorkOrderDetails.FirstOrDefault().NcCount = (WorkOrderDetails.FirstOrDefault()?.NcCount ?? 0) + WorkOrderReportData.NcCount;
                        WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                        WorkOrderDetails.FirstOrDefault().ProducingMachine = WorkOrderReportData.ProducingMachine;

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
                            MCount = WorkOrderReportData.MCount,
                            ReCount = WorkOrderReportData.ReCount,
                            RePrice = WorkOrderReportData.RePrice,
                            NgCount = WorkOrderReportData.NgCount,
                            NcCount = WorkOrderReportData.NcCount,
                            Message = WorkOrderReportData.Message,
                            StatusO = 2,
                            StatusN = 3,
                            DueStartTime = WorkOrderDetails.FirstOrDefault().DueStartTime,
                            DueEndTime = WorkOrderDetails.FirstOrDefault().DueEndTime,
                            ActualStartTime = checkLogStart.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.FirstOrDefault().ActualEndTime,
                            CreateTime = dt,
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

                        // 該製程為QC檢驗的話，須建立QC檢驗log。
                        if (WorkOrderDetails.FirstOrDefault().Process.Type == 20)
                        {
                            WorkOrderDetails.FirstOrDefault().WorkOrderQcLogs.Add(new WorkOrderQcLog
                            {
                                // WorkOrderHeadId = WorkOrderReportData.WorkOrderHeadId,
                                // WorkOrderDetailId = WorkOrderReportData.WorkOrderDetailId,
                                ReportType = WorkOrderReportData.ReportType,
                                // PurchaseHeadId = WorkOrderReportData.PurchaseHeadId,
                                // SaleHeadId = WorkOrderReportData.SaleHeadId,
                                // SupplierId = WorkOrderReportData.SupplierId,
                                DrawNo = WorkOrderReportData.DrawNo,
                                MCount = WorkOrderReportData.MCount,
                                ReCount = WorkOrderReportData.ReCount,
                                CkCount = WorkOrderReportData.CkCount,
                                OkCount = WorkOrderReportData.OkCount,
                                NgCount = WorkOrderReportData.NgCount,
                                NcCount = WorkOrderReportData.NcCount,
                                // CheckResult = WorkOrderReportData.CheckResult,
                                Message = WorkOrderReportData.Message,
                                // Remark = WorkOrderReportData.Remark,
                                CreateTime = dt,
                                CreateUser = WorkOrderReportData.CreateUser,
                            });
                        }
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
                        WorkOrderDetails.FirstOrDefault().Status = 2; // 開工
                        WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                        WorkOrderDetails.FirstOrDefault().ProducingMachine = WorkOrderReportData.ProducingMachine;
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
                            MCount = WorkOrderReportData.MCount,
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

        /// <summary>
        /// 工單批次作業 (暫停/回復)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="WorkOrderReportDataAll"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<WorkOrderData>> WorkOrderReportAllStopOrStart(int id, [FromBody] WorkOrderReportDataAll WorkOrderReportDataAll)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var WorkOrderDetails = await _context.WorkOrderDetails.FindAsync(id);
            if (WorkOrderDetails != null)
            {
                var Requisition = await _context.Requisitions.Where(x => x.WorkOrderHeadId == WorkOrderDetails.WorkOrderHeadId).ToListAsync();
                if (Requisition.Count() == 0)
                {
                    return Ok(MyFun.APIResponseError("該工單尚未[領料]!"));
                }

                if (WorkOrderDetails.Status == 2) // 回報[工序暫停]
                {
                    // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                    var checkLogStart = WorkOrderDetails.WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportDataAll.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                    var checkLogEnd = WorkOrderDetails.WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportDataAll.ProducingMachine && x.ActualEndTime != null && x.DeleteFlag == 0);
                    if ((checkLogStart.Count() - checkLogEnd.Count()) == 1)
                    {
                        WorkOrderDetails.Status = 7; // 暫停
                        WorkOrderDetails.CodeNo = WorkOrderReportDataAll.CodeNo;
                        WorkOrderDetails.ProducingMachine = WorkOrderReportDataAll.ProducingMachine;
                        WorkOrderDetails.ActualEndTime = DateTime.Now;
                        WorkOrderDetails.UpdateUser = WorkOrderReportDataAll.CreateUser;

                        WorkOrderDetails.WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.Id,
                            ReportType = 4, // 工序暫停
                            DrawNo = WorkOrderDetails.DrawNo,
                            CodeNo = WorkOrderReportDataAll.CodeNo,
                            ProducingMachine = WorkOrderReportDataAll.ProducingMachine,
                            Message = WorkOrderReportDataAll.Message,
                            StatusO = 2,
                            StatusN = WorkOrderDetails.Status,
                            DueStartTime = WorkOrderDetails.DueStartTime,
                            DueEndTime = WorkOrderDetails.DueEndTime,
                            ActualStartTime = checkLogStart.FirstOrDefault().ActualStartTime,
                            ActualEndTime = WorkOrderDetails.ActualEndTime,
                            CreateTime = DateTime.Now,
                            CreateUser = WorkOrderReportDataAll.CreateUser,
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportDataAll.ProducingMachine + " ] 回報異常[Code: 2TO7]!"));
                    }
                }
                else if (WorkOrderDetails.Status == 7) // 回報[回復加工]
                {
                    // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                    var checkLogStart = WorkOrderDetails.WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportDataAll.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0);
                    var checkLogEnd = WorkOrderDetails.WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportDataAll.ProducingMachine && x.ActualEndTime != null && x.DeleteFlag == 0);
                    if (checkLogStart.Count() == checkLogEnd.Count())
                    {
                        WorkOrderDetails.Status = 2; // 開工
                        WorkOrderDetails.CodeNo = WorkOrderReportDataAll.CodeNo;
                        WorkOrderDetails.ProducingMachine = WorkOrderReportDataAll.ProducingMachine;
                        WorkOrderDetails.UpdateUser = WorkOrderReportDataAll.CreateUser;

                        WorkOrderDetails.WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.Id,
                            ReportType = 5, // 完工再開工回報
                            DrawNo = WorkOrderDetails.DrawNo,
                            CodeNo = WorkOrderReportDataAll.CodeNo,
                            ProducingMachine = WorkOrderReportDataAll.ProducingMachine,
                            Message = WorkOrderReportDataAll.Message,
                            StatusO = 7,
                            StatusN = WorkOrderDetails.Status,
                            DueStartTime = WorkOrderDetails.DueStartTime,
                            DueEndTime = WorkOrderDetails.DueEndTime,
                            ActualStartTime = DateTime.Now,
                            ActualEndTime = null,
                            CreateTime = DateTime.Now,
                            CreateUser = WorkOrderReportDataAll.CreateUser,
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportDataAll.ProducingMachine + " ] 回報異常[Code: 7TO2]!"));
                    }
                }

                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單異常!"));
            }
        }

        public async Task<List<WorkOrderHead>> NewWorkOrderByOrderCheck(OrderDetail OrderDetail, int TempNo)
        {
            var WorkOrderHeadList = new List<WorkOrderHead>();
            if (OrderDetail.MaterialBasicId != 0)
            {
                //取得工單號
                var key = "HJ";
                var WorkOrderNo = DateTime.Now.ToString("yyMMdd");
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11).OrderByDescending(x => x.WorkOrderNo).ToListAsync();
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

                // var DataType = 0;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                var StockInfo = "";
                var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).ToListAsync();
                // if (DataType == 1)
                // {
                var BasicData = _context.MaterialBasics.Find(OrderDetail.MaterialBasicId);
                BasicDataID = BasicData.Id;
                BasicDataNo = BasicData.MaterialNo;
                BasicDataName = BasicData.Name;
                // DataType = BasicData.MaterialType ?? 0;

                StockInfo = "無庫存";
                var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301");
                if (WarehousesInfo.Count() != 0)
                {
                    var MaterialsInfo = BasicData.Materials.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                    if (MaterialsInfo.Count() != 0)
                    {
                        StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + MaterialsInfo.FirstOrDefault().Quantity;
                    }
                }
                // }
                // else if (DataType == 2)
                // {
                //     var BasicData = _context.ProductBasics.Find(OrderDetail.MaterialBasicId);
                //     BasicDataID = BasicData.Id;
                //     BasicDataNo = BasicData.ProductNo;
                //     BasicDataName = BasicData.Name;

                //     StockInfo = "無庫存";
                //     var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301");
                //     if (WarehousesInfo.Count() != 0)
                //     {
                //         var ProductsInfo = BasicData.Products.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                //         if (ProductsInfo.Count() != 0)
                //         {
                //             StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                //         }
                //     }
                // }
                // else if (DataType == 3)
                // {

                // }

                var status = 0; // 工單是否已建立，和是否有MBOM (0否 1是 2無MBOM)
                // var CheckWorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == OrderDetail.Id && x.DataType == DataType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToListAsync();
                var CheckWorkOrderHeads = await _context.OrderDetailAndWorkOrderHeads.Where(x => x.OrderDetailId == OrderDetail.Id && x.DataType == BasicData.MaterialType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToListAsync();
                if (CheckWorkOrderHeads.Count() > 0)
                {
                    status = 1;
                }
                // var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.MaterialBasicId == OrderDetail.MaterialBasicId).ToListAsync();
                // if (billOfMaterial.Count() == 0)
                // {
                //     status = 2;
                // }
                var BillOfMaterials = await _context.BillOfMaterials.AsQueryable().Where(x => x.ProductBasicId == OrderDetail.MaterialBasicId).ToListAsync();
                if (BillOfMaterials.Count() == 0)
                {
                    status = 2;
                }
                var nWorkOrderHead = new WorkOrderHeadInfo
                {
                    WorkOrderNo = workOrderNo,
                    OrderDetailId = OrderDetail.Id, // 
                    MachineNo = OrderDetail.MachineNo,
                    DataType = BasicData.MaterialType,
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
                    item.Count = item.Count * nWorkOrderHead.Count;
                    WorkOrderHeadList.Add(item);
                }
            }
            return WorkOrderHeadList;
        }

        public async Task<string> NewWorkOrderByOrder(OrderToWorkCheckData OrderToWorkCheckData)
        {
            string sMessage = "";
            if (OrderToWorkCheckData.OrderDetail.MaterialBasicId != 0)
            {
                // 取得工單號
                // var key = "HJ";
                // var WorkOrderNo = DateTime.Now.ToString("yyMMdd");
                // var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.WorkOrderNo.Length == 11 && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
                // var NoCount = NoData.Count() + 1;
                // if (NoCount != 1)
                // {
                //     var LastWorkOrderNo = NoData.FirstOrDefault().WorkOrderNo;
                //     var NoLast = Int32.Parse(LastWorkOrderNo.Substring(LastWorkOrderNo.Length - 3, 3));
                //     // if (NoCount <= NoLast) {
                //     NoCount = NoLast + 1;
                //     // }
                // }
                // var workOrderNo = key + WorkOrderNo + NoCount.ToString("000");


                // var DataType = 0;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                // if (DataType == 1)
                // {
                var BasicData = _context.MaterialBasics.Find(OrderToWorkCheckData.OrderDetail.MaterialBasicId);
                BasicDataID = BasicData.Id;
                BasicDataNo = BasicData.MaterialNo;
                BasicDataName = BasicData.Name;
                // DataType = BasicData.MaterialType ?? 0;
                // }
                // else if (DataType == 2)
                // {
                //     var BasicData = _context.ProductBasics.Find(OrderToWorkCheckData.OrderDetail.MaterialBasicId);
                //     BasicDataID = BasicData.Id;
                //     BasicDataNo = BasicData.ProductNo;
                //     BasicDataName = BasicData.Name;
                // }
                // else if (DataType == 3)
                // {

                // }
                var nWorkOrderHead = new WorkOrderHead
                {
                    WorkOrderNo = OrderToWorkCheckData.WorkOrderHead.FirstOrDefault().WorkOrderNo,
                    OrderDetailId = OrderToWorkCheckData.OrderDetail.Id, // 
                    MachineNo = OrderToWorkCheckData.OrderDetail.MachineNo,
                    DataType = BasicData.MaterialType,
                    DataId = BasicDataID,
                    DataNo = BasicDataNo,
                    DataName = BasicDataName,
                    DrawNo = OrderToWorkCheckData.WorkOrderHead.FirstOrDefault().DrawNo,
                    Count = OrderToWorkCheckData.OrderDetail.Quantity,
                    OrderCount = OrderToWorkCheckData.WorkOrderHead.FirstOrDefault().OrderCount,
                    Status = 4, // 表示由訂單轉程的工單，需要再由人工確認該工單
                    CreateUser = MyFun.GetUserID(HttpContext)
                };

                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.MaterialBasicId == OrderToWorkCheckData.OrderDetail.MaterialBasicId).ToListAsync();
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
                        ProducingMachine = item.ProducingMachine == "" ? null : item.ProducingMachine,
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

                    // 新增訂單明細&工單主檔的關聯 2020/10/08
                    foreach (var item in OrderToWorkCheckData.OrderDetail.OrderDetailIdList)
                    {
                        nWorkOrderHead.OrderDetailAndWorkOrderHeads.Add(new OrderDetailAndWorkOrderHead
                        {
                            OrderDetailId = item.OrderDetailId,
                            DataType = nWorkOrderHead.DataType,
                            DataId = OrderToWorkCheckData.OrderDetail.MaterialBasicId,
                            OrdeCount = item.Count,
                            CreateTime = DateTime.Now,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
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
                        // 新增訂單明細&工單主檔的關聯 2020/10/08
                        foreach (var item2 in OrderToWorkCheckData.OrderDetail.OrderDetailIdList)
                        {
                            item.OrderDetailAndWorkOrderHeads.Add(new OrderDetailAndWorkOrderHead
                            {
                                OrderDetailId = item2.OrderDetailId,
                                DataType = item.DataType,
                                DataId = item.DataId,
                                OrdeCount = item.Count * item2.Count,
                                CreateTime = DateTime.Now,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                        }

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
                        var BasicData = new MaterialBasic();
                        // var DataType = NULL;
                        var BasicDataID = 0;
                        var BasicDataNo = "";
                        var BasicDataName = "";
                        var StockInfo = "";
                        var Warehouses = _context.Warehouses.Where(x => x.DeleteFlag == 0);
                        if (item.MaterialBasicId != null)
                        {
                            BasicData = _context.MaterialBasics.Find(item.MaterialBasicId);
                            // DataType = BasicData.MaterialType;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.MaterialNo;
                            BasicDataName = BasicData.Name;

                            StockInfo = "無庫存";
                            var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "101");
                            if (WarehousesInfo.Count() != 0)
                            {
                                var ProductsInfo = BasicData.Materials.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                                if (ProductsInfo.Count() != 0)
                                {
                                    StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                                }
                            }
                        }
                        else if (item.ProductBasicId != null)
                        {
                            BasicData = _context.MaterialBasics.Find(item.ProductBasicId);
                            // DataType = BasicData.MaterialType;
                            BasicDataID = BasicData.Id;
                            BasicDataNo = BasicData.MaterialNo;
                            BasicDataName = BasicData.Name;

                            StockInfo = "無庫存";
                            var WarehousesInfo = Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301");
                            if (WarehousesInfo.Count() != 0)
                            {
                                var ProductsInfo = BasicData.Materials.Where(x => x.DeleteFlag == 0 && x.WarehouseId == WarehousesInfo.FirstOrDefault().Id).ToList();
                                if (ProductsInfo.Count() != 0)
                                {
                                    StockInfo = WarehousesInfo.FirstOrDefault().Code + WarehousesInfo.FirstOrDefault().Name + " " + ProductsInfo.FirstOrDefault().Quantity;
                                }
                            }
                        }

                        var status = 0; // 工單是否已建立，和是否有MBOM (0否 1是 2無MBOM)
                        // var CheckWorkOrderHeads = _context.WorkOrderHeads.Where(x => x.OrderDetailId == WorkOrderHead.OrderDetailId && x.DataType == DataType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToList();
                        var CheckWorkOrderHeads = _context.OrderDetailAndWorkOrderHeads.Where(x => x.OrderDetailId == WorkOrderHead.OrderDetailId && x.DataType == BasicData.MaterialType && x.DataId == BasicDataID && x.DeleteFlag == 0).ToList();
                        if (CheckWorkOrderHeads.Count() > 0)
                        {
                            status = 1;
                        }

                        var nWorkOrderHead = new WorkOrderHeadInfo
                        {
                            WorkOrderNo = WorkOrderHead.WorkOrderNo + "-" + index,
                            OrderDetailId = WorkOrderHead.OrderDetailId, // 
                            MachineNo = WorkOrderHead.MachineNo,
                            DataType = BasicData.MaterialType,
                            DataId = BasicDataID,
                            DataNo = BasicDataNo,
                            DataName = BasicDataName,
                            // Count = Decimal.ToInt32(WorkOrderHead.Count * item.ReceiveQty) + 1, // 注意!
                            Count = Decimal.ToInt32(Decimal.Round(item.ReceiveQty)), // 注意!
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
                                // Count = Decimal.ToInt32(WorkOrderHead.Count * item.ReceiveQty) + 1,
                                Count = Decimal.ToInt32(Decimal.Round(item.ReceiveQty)),
                                // PurchaseId
                                DrawNo = item2.DrawNo,
                                Manpower = item2.Manpower,
                                ProducingMachine = item2.ProducingMachine == "" ? null : item2.ProducingMachine,
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

        /// <summary>
        /// 報工紀錄列表 By 機台名稱
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="FromQuery"></param>
        /// <param name="detailfilter"></param>
        /// <returns></returns>
        // GET: api/WorkOrder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderLog>>> GetWorkOrderReportLogByNum(
                string machine,
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var machineValue = machine == "<無>" ? null : machine;
            var dt = DateTime.Now;
            var data = _context.WorkOrderReportLogs.Where(
                x => x.DeleteFlag == 0 &&
                x.ProducingMachine == machineValue)
                .Include(x => x.WorkOrderDetail)
                .ThenInclude(x => x.WorkOrderHead)
                .OrderByDescending(x => x.CreateTime)
                .Select(x => new WorkOrderLog
                {
                    WorkOrderNo = x.WorkOrderDetail.WorkOrderHead.WorkOrderNo,
                    ReportType = x.ReportType,
                    SerialNumber = x.WorkOrderDetail.SerialNumber,
                    Process = x.WorkOrderDetail.ProcessNo + '_' + x.WorkOrderDetail.ProcessName,
                    ProcessTime = x.ActualEndTime == null ? 0 : Convert.ToInt32(((x.ActualEndTime ?? dt) - (x.ActualStartTime ?? dt)).TotalMinutes),
                    ReCount = x.ReCount == 0 ? null : x.ReCount,
                    Remarks = x.WorkOrderDetail.Remarks,
                    StatusO = x.StatusO,
                    StatusN = x.StatusN,
                    DueStartTime = x.DueStartTime,
                    DueEndTime = x.DueEndTime,
                    ActualStartTime = x.ActualStartTime,
                    ActualEndTime = x.ActualEndTime,
                    CreateTime = x.CreateTime
                });

            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 查詢機台工序資訊 By 機台名稱
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="FromQuery"></param>
        /// <param name="detailfilter"></param>
        /// <returns></returns>
        // GET: api/WorkOrder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDetailForResourceal>>> GetProcessByMachineName(
                string machine,
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var machineValue = machine == "<無>" ? null : machine;
            var data = _context.WorkOrderDetails.Where(
                x => x.DeleteFlag == 0 &&
                x.ProducingMachine == machineValue &&
                x.WorkOrderHead.DeleteFlag == 0 &&
                (x.WorkOrderHead.Status == 1 || x.WorkOrderHead.Status == 5))
                .OrderByDescending(x => x.WorkOrderHead.WorkOrderNo)
                .ThenBy(x => x.SerialNumber)
                .Select(x => new WorkOrderDetailForResourceal
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    ProcessName = x.ProcessNo + "_" + x.ProcessName,
                    WorkOrderNo = x.WorkOrderHead.WorkOrderNo,
                    MaterialBasicName = x.WorkOrderHead.DataNo + "_" + x.WorkOrderHead.DataName,
                    WorkOrderHeadStatus = x.WorkOrderHead.Status,
                    WorkOrderDetailStatus = x.Status,
                    Count = x.Count,
                    ReCount = x.ReCount,
                    Remarks = x.Remarks,
                    DueStartTime = x.DueStartTime,
                    DueEndTime = x.DueEndTime,
                    ActualStartTime = x.ActualStartTime,
                    ActualEndTime = x.ActualEndTime,
                    CreateTime = x.CreateTime,
                }
            );
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // data = data.Include(x => x.WorkOrderHead);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 工單QC回報
        /// </summary>
        /// <param name="WorkOrderQcData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<WorkOrderQcLog>> WorkOrderReportQC(WorkOrderQcLog WorkOrderQcData)
        {
            if (WorkOrderQcData.ReCount == 0)
            {
                return Ok(MyFun.APIResponseError("注意! [回報數量]不能為 0"));
            }
            if (WorkOrderQcData.CkCount == 0)
            {
                return Ok(MyFun.APIResponseError("注意! [抽檢數量]不能為 0"));
            }
            if (WorkOrderQcData.OkCount == 0 && WorkOrderQcData.NgCount == 0)
            {
                return Ok(MyFun.APIResponseError("注意! [OK數量]與[NG數量]不能同時為 0"));
            }
            if (WorkOrderQcData.ReCount < WorkOrderQcData.CkCount || WorkOrderQcData.ReCount < (WorkOrderQcData.OkCount + WorkOrderQcData.NgCount))
            {
                return Ok(MyFun.APIResponseError("注意! 回報數量異常!"));
            }

            if (WorkOrderQcData.ReCount != 0 && WorkOrderQcData.CkCount != 0)
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = await _context.WorkOrderHeads.FindAsync(WorkOrderQcData.WorkOrderHeadId);
                if (WorkOrderHeads.Status == 5)
                {
                    return Ok(MyFun.APIResponseError("該工單狀態為[結案]!"));
                }
                var Requisition = await _context.Requisitions.Where(x => x.WorkOrderHeadId == WorkOrderQcData.WorkOrderHeadId).ToListAsync();
                if (Requisition.Count() == 0)
                {
                    return Ok(MyFun.APIResponseError("該工單尚未[領料]!"));
                }

                // var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                // if (WorkOrderDetails.Count() == 1)
                // {
                // if (WorkOrderDetails.FirstOrDefault().Status == 1)
                // {
                // 因工序可重複開工/完工，所以需檢查同機台是否重複報工 2020/09/09
                // var checkLog = WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Where(x => x.ProducingMachine == WorkOrderReportData.ProducingMachine && x.ActualEndTime == null && x.DeleteFlag == 0);
                // if (checkLog.Count() == 0)
                // {
                // WorkOrderDetails.FirstOrDefault().Status = 2; // 開工
                // // WorkOrderDetails.FirstOrDefault().SupplierId = WorkOrderReportData.SupplierId;
                // // WorkOrderDetails.FirstOrDefault().RePrice = WorkOrderReportData.RePrice;
                // WorkOrderDetails.FirstOrDefault().CodeNo = WorkOrderReportData.CodeNo;
                // WorkOrderDetails.FirstOrDefault().ProducingMachine = WorkOrderReportData.ProducingMachine;
                // if (WorkOrderDetails.FirstOrDefault().ActualStartTime == null)
                // {
                //     WorkOrderDetails.FirstOrDefault().ActualStartTime = DateTime.Now;
                // }

                WorkOrderHeads.WorkOrderQcLogs.Add(new WorkOrderQcLog
                {
                    WorkOrderHeadId = WorkOrderQcData.WorkOrderHeadId,
                    // WorkOrderDetailId = WorkOrderQcData.WorkOrderDetailId,
                    ReportType = WorkOrderQcData.ReportType,
                    // PurchaseHeadId = WorkOrderQcData.PurchaseHeadId,
                    // SaleHeadId = WorkOrderQcData.SaleHeadId,
                    // SupplierId = WorkOrderQcData.SupplierId,
                    DrawNo = WorkOrderQcData.DrawNo,
                    ReCount = WorkOrderQcData.ReCount,
                    CkCount = WorkOrderQcData.CkCount,
                    OkCount = WorkOrderQcData.OkCount,
                    NgCount = WorkOrderQcData.NgCount,
                    NcCount = WorkOrderQcData.NcCount,
                    CheckResult = WorkOrderQcData.CheckResult,
                    Message = WorkOrderQcData.Message,
                    // Remark = WorkOrderQcData.Remark,
                    CreateTime = DateTime.Now,
                    CreateUser = WorkOrderQcData.CreateUser,
                });
                // }
                // else
                // {
                //     return Ok(MyFun.APIResponseError("該機台 [ " + WorkOrderReportData.ProducingMachine + " ] 已經回報開工!"));
                // }

                // }
                // else
                // {
                //     return Ok(MyFun.APIResponseError("工單狀態異常!"));
                // }
                // }
                // else
                // {
                //     return Ok(MyFun.APIResponseError("工單數量不正確!"));
                // }
                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                return Ok(MyFun.APIResponseError("回報失敗!"));
            }
        }

        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// QC報工紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkOrderReportLog>> GetWorkOrderQcLog([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.WorkOrderQcLogs.Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderHead);
            // data.FirstOrDefault().WorkOrderDetail.WorkOrderHead.WorkOrderNo 單獨抓取WorkOrderNo
            // data = data.Include(x => x.WorkOrderHead);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 查詢品質相關報工紀錄
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkOrderReportLog>> GetWorkOrderQCLogs(
            [FromQuery] DataSourceLoadOptions FromQuery)
        {
            // var WorkOrderReportLogs = _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0 ).Include(x => x.Supplier);
            var WorkOrderQcLogs = _context.WorkOrderQcLogs.Where(x => x.DeleteFlag == 0);
            //leftjoin：
            // var gnWorkOrderReportLogs = _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0 )
            //   .GroupJoin(WorkOrderQcLogs, x=>x.CreateTime,y=>y.CreateTime,(x, y) => new {
            //       x,y
            //   }).SelectMany(x=>x.y.DefaultIfEmpty(),(x,y)=>new WorkOrderReportLogData{
            //              CreateTime = x.x.CreateTime,
            //                     CreateUser = x.x.CreateUser,
            //                     ReportType = x.x.ReportType,
            //                     QCReportType = y.ReportType,
            //                     QCReCount = y.ReCount ,
            //                     QCCkCount = y.CkCount ,
            //                     QCOkCount = y.OkCount ,
            //                     QCNgCount = y.NgCount ,
            //                     QCNcCount = y.NcCount,
            //                     QCMessage = y.Message,
            //                     ActualStartTime = x.x.ActualStartTime,
            //                     ActualEndTime = x.x.ActualEndTime,
            //   });

            var nWorkOrderReportLogs = _context.WorkOrderReportLogs.Where(x => x.DeleteFlag == 0)
            .Include(x => x.WorkOrderDetail).ThenInclude(x => x.WorkOrderHead)
            .Join(WorkOrderQcLogs, x => x.CreateTime, y => y.CreateTime, (x, y) => new WorkOrderReportLogData
            {
                Id = x.Id,
                WorkOrderNo = x.WorkOrderDetail.WorkOrderHead.WorkOrderNo,
                DataNo = x.WorkOrderDetail.WorkOrderHead.DataNo,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                ReportType = x.ReportType,
                QCReportType = y.ReportType,
                QCReCount = y.ReCount,
                QCCkCount = y.CkCount,
                QCOkCount = y.OkCount,
                QCNgCount = y.NgCount,
                QCNcCount = y.NcCount,
                QCMessage = y.Message,
                ActualStartTime = x.ActualStartTime,
                ActualEndTime = x.ActualEndTime,
            });
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(nWorkOrderReportLogs, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
        /// 取得品號主要用料By工單Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/WorkOrders
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BillOfMaterial>>> GetBomDataByWorkOrderId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;//停止關連，減少資料
            var WorkOrderHead = await _context.WorkOrderHeads.FindAsync(id);
            var BillOfMaterial = await _context.BillOfMaterials
                .Where(x => x.DeleteFlag == 0 && x.ProductBasicId == WorkOrderHead.DataId && x.Lv == 1 && x.Master == 1 && x.MaterialBasicId != null)
                // .Include(x => x.Order)
                // .Include(x => x.OrderDetail)
                // .Include(x => x.Sale)
                .OrderByDescending(x => x.MaterialBasic.MaterialNo).ThenBy(x => x.MaterialBasic.Name)
                .Select(x => new BillOfMaterialData
                {
                    DataNo = x.MaterialBasic.MaterialNo,
                    Quantity = x.Quantity
                }).ToListAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            return Ok(MyFun.APIResponseOK(BillOfMaterial));
        }

        /// <summary>
        /// 匯入加工單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public ActionResult<OrderHead> PostWorkOrdeByExcel1()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var myFile = Request.Form.Files;
            if (myFile.Any())
            {
                foreach (var Fileitem in myFile)
                {
                    try
                    {
                        using (var ms = new FileStream(Path.GetTempFileName(), FileMode.Create))
                        {
                            Fileitem.CopyTo(ms);
                            ms.Position = 0; // <-- Add this, to make it work
                            IWorkbook workBook = new XSSFWorkbook(ms);//xlsx格式
                            IFormulaEvaluator formulaEvaluator = new XSSFFormulaEvaluator(workBook); // Important!! 取公式值的時候會用到
                            foreach (ISheet sheet in workBook)
                            {
                                if (sheet.SheetName == "生產紀錄")
                                {
                                    for (var i = 1; i < sheet.LastRowNum; i++)//筆數
                                    {
                                        var WorkOrderNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));//工單號
                                        var CreateOrderTime = DateTime.Now;//工單號建立日期
                                        var sCreateOrderTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(2));
                                        DateTime.TryParse(sCreateOrderTime, out CreateOrderTime);
                                        var DataType = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(3));//類別
                                        var Supplier = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(4));//廠商
                                        var OrderNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(5)).Replace("-", "");//訂單號
                                        var DueEndTime = DateTime.Now;//預計完成日
                                        var sDueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(6));
                                        if (!string.IsNullOrWhiteSpace(sDueEndTime))
                                        {
                                            DateTime.TryParse(sDueEndTime, out DueEndTime);
                                        }
                                        var DataNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(7));//品號
                                        var BOM = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(8));//BOM
                                        var BOMName = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(9));//品名
                                        var OrderCount = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(10));//訂單數量
                                        var workCount = 0;//加工數量
                                        int.TryParse(DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(11)), out workCount);
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(12));//料件
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(13));//領料數
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(14));//領料日
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(15));//倉庫
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(16));//現場
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(17));//備註
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(18));//完工
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(19));//建檔人員
                                        var CreateTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(20));//建檔日期
                                                                                                                            // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(21));//修改人員
                                                                                                                            // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(22));//修改日期

                                        var BasicData = _context.MaterialBasics.Where(x => x.MaterialNo == DataNo).FirstOrDefault();//讀取品項檔
                                        if (BasicData != null)
                                        {
                                            var OrderDetails = _context.OrderDetails.Where(x => x.MaterialBasicId == BasicData.Id).OrderByDescending(XRAppearanceObject => XRAppearanceObject.Id).ToList();
                                            foreach (var OrderDetaillitem in OrderDetails)
                                            {
                                                if (!_context.WorkOrderHeads.Where(x => x.WorkOrderNo == WorkOrderNo).Any())//沒有加工單號才能加進來
                                                {
                                                    if (BasicData != null)
                                                    {
                                                        var nWorkOrderHead = new WorkOrderHead
                                                        {
                                                            WorkOrderNo = WorkOrderNo,
                                                            DataType = 2,
                                                            DataId = BasicData.Id,
                                                            DataName = BasicData.Name,
                                                            DataNo = BasicData.MaterialNo,
                                                            Count = workCount,
                                                            DueEndTime = DueEndTime,
                                                            CreateUser = 1,
                                                            CreateTime = CreateOrderTime
                                                        };
                                                        OrderDetaillitem.WorkOrderHeads.Add(nWorkOrderHead);
                                                        _context.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (sheet.SheetName == "生產工時")
                                {
                                    for (var i = 1; i < sheet.LastRowNum; i++)//筆數
                                    {
                                        var WorkOrderNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));//工單號
                                        var DataNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(3));//品號
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        return Ok(MyFun.APIResponseError(ex.Message));
                    }
                }
            }
            return Ok(MyFun.APIResponseOK(""));
        }
        /// <summary>
        /// 匯入加工單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public ActionResult<OrderHead> PostWorkOrdeByExcel()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var myFile = Request.Form.Files;
            var eid = "";
            if (myFile.Any())
            {
                foreach (var Fileitem in myFile)
                {
                    try
                    {
                        using (var ms = new FileStream(Path.GetTempFileName(), FileMode.Create))
                        {
                            Fileitem.CopyTo(ms);
                            ms.Position = 0; // <-- Add this, to make it work
                            IWorkbook workBook = new XSSFWorkbook(ms);//xlsx格式
                            IFormulaEvaluator formulaEvaluator = new XSSFFormulaEvaluator(workBook); // Important!! 取公式值的時候會用到
                            foreach (ISheet sheet in workBook)
                            {
                                if (sheet.SheetName == "生產紀錄")
                                {
                                    for (var i = 1; i < sheet.LastRowNum; i++)//筆數
                                    {
                                        var WorkOrderNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(0));//工單號
                                        var CreateOrderTime = DateTime.Now;//工單號建立日期
                                        var sCreateOrderTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));
                                        DateTime.TryParse(sCreateOrderTime, out CreateOrderTime);
                                        if (CreateOrderTime.Year < 2020)
                                        {
                                            CreateOrderTime = DateTime.Now;
                                        }
                                        var DataNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(2));//品號
                                        var BOM = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(7));//BOM
                                        // var BOMName = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(9));//品名
                                        var OrderCount = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(4));//訂單數量
                                        var workCount = 0;//加工數量
                                        int.TryParse(DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(5)), out workCount);
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(12));//料件
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(13));//領料數
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(14));//領料日
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(15));//倉庫
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(16));//現場
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(17));//備註
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(18));//完工
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(19));//建檔人員
                                        // var CreateTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(20));//建檔日期
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(21));//修改人員
                                        // var DueEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(22));//修改日期

                                        eid = WorkOrderNo;
                                        var BasicData = _context.MaterialBasics.Where(x => x.MaterialNo == DataNo).FirstOrDefault();//讀取品項檔
                                        if (BasicData != null)
                                        {
                                            var OrderDetails = _context.OrderDetails.Where(x => x.MaterialBasicId == BasicData.Id).OrderByDescending(XRAppearanceObject => XRAppearanceObject.Id).ToList();
                                            foreach (var OrderDetaillitem in OrderDetails)
                                            {
                                                if (!_context.WorkOrderHeads.Where(x => x.WorkOrderNo == WorkOrderNo).Any())//沒有加工單號才能加進來
                                                {
                                                    if (BasicData != null)
                                                    {
                                                        var nWorkOrderHead = new WorkOrderHead
                                                        {
                                                            WorkOrderNo = WorkOrderNo,
                                                            DataType = 2,
                                                            DataId = BasicData.Id,
                                                            DataName = BasicData.Name,
                                                            DataNo = BasicData.MaterialNo,
                                                            Count = workCount,
                                                            DueEndTime = CreateOrderTime,
                                                            CreateUser = 1,
                                                            CreateTime = CreateOrderTime
                                                        };
                                                        OrderDetaillitem.WorkOrderHeads.Add(nWorkOrderHead);
                                                        _context.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (sheet.SheetName == "生產工時")
                                {
                                    System.Globalization.CultureInfo culTW = new System.Globalization.CultureInfo("zh-TW", true);

                                    var sDtPattern = new string[]
                                    {
                "yyyy/M/d 上午 hh:mm:ss",
                "yyyy/M/d 下午 hh:mm:ss",
                "yyyy/M/d tt hh:mm:ss",
                "yyyy/MM/dd 上午 HH:mm:ss",
                "yyyy/MM/dd 下午 HH:mm:ss",
                "yyyy/MM/dd tt hh:mm:ss",
                "yyyy年M月d日 tt hh:mm:ss"
    };
                                    for (var i = 1; i < sheet.LastRowNum; i++)//筆數
                                    {
                                        var grow = sheet.GetRow(i);
                                        if (grow != null)
                                        {
                                            var WorkOrderNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(1));//工單號
                                            // var DataNo = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(3));//品號
                                            var ProducingMachine = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(5));//機台
                                            var sMachinedt = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(6));//開工日
                                            var ProcessNos = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(7));//製程
                                            var sMachineStartTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(8));//開始時間
                                            var sMachineEndTime = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(9));//完工時間
                                            var Realname = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(10));//報工人員
                                            var sReCount = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(12));//完工數
                                            var sNgCount = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(13));//NG數
                                            eid = WorkOrderNo;
                                            var WorkOrderHeads = _context.WorkOrderHeads.Where(x => x.WorkOrderNo == WorkOrderNo).ToList();
                                            if (WorkOrderHeads.Any())//有工單號才開始
                                            {
                                                var ProcessNoList = ProcessNos.Split(',');
                                                foreach (var WorkOrderHeaditem in WorkOrderHeads)
                                                {
                                                    foreach (var item in WorkOrderHeaditem.WorkOrderDetails.Where(x => x.DeleteFlag == 0))
                                                    {
                                                        foreach (var ProcessNo in ProcessNoList)
                                                        {

                                                            if (item.ProcessNo.ToUpper().Contains(ProcessNo.ToUpper()))
                                                            {
                                                                var MachineStartTime = DateTime.Now;
                                                                var MachineEndTime = DateTime.Now;
                                                                var Machinedt = DateTime.Now;
                                                                var ReCount = 0;
                                                                var NgCount = 0;
                                                                DateTime.TryParse(sMachinedt, out Machinedt);//2021/5/17 上午 12:00:00
                                                                DateTime.TryParseExact(sMachineStartTime, sDtPattern, culTW, DateTimeStyles.AllowWhiteSpaces, out MachineStartTime);
                                                                DateTime.TryParseExact(sMachineEndTime, sDtPattern, culTW, DateTimeStyles.AllowWhiteSpaces, out MachineEndTime);
                                                                var MachineStartTimedt = new DateTime(Machinedt.Year, Machinedt.Month, Machinedt.Day, MachineStartTime.Hour, MachineStartTime.Minute, MachineStartTime.Second);
                                                                var MachineEndTimedt = new DateTime(Machinedt.Year, Machinedt.Month, Machinedt.Day, MachineEndTime.Hour, MachineEndTime.Minute, MachineEndTime.Second);

                                                                if (MachineStartTimedt.Year < 2020)
                                                                {
                                                                    MachineStartTimedt = DateTime.Now;
                                                                }
                                                                if (MachineEndTimedt.Year < 2020)
                                                                {
                                                                    MachineEndTimedt = DateTime.Now;
                                                                }
                                                                int.TryParse(sReCount, out ReCount);
                                                                int.TryParse(sNgCount, out NgCount);
                                                                var Users = _context.Users.Where(x => x.Realname == Realname).FirstOrDefault();
                                                                var Machine = _context.MachineInformations.Where(x => x.Name == ProducingMachine).FirstOrDefault();
                                                                item.ActualStartTime = MachineStartTimedt;
                                                                item.ActualEndTime = MachineEndTimedt;
                                                                item.MachineStartTime = null;
                                                                item.MachineEndTime = null;
                                                                item.ReCount = ReCount;
                                                                item.NgCount = NgCount;
                                                                item.ProducingMachine = ProducingMachine;
                                                                if (!item.WorkOrderReportLogs.Any())
                                                                {
                                                                    item.WorkOrderReportLogs.Add(new WorkOrderReportLog
                                                                    {
                                                                        ReportType = 1,
                                                                        ReCount = 0,
                                                                        NgCount = 0,
                                                                        StatusO = 1,
                                                                        StatusN = 2,
                                                                        ProducingMachine = Machine?.Name,
                                                                        ProducingMachineId = Machine?.Id,
                                                                        ActualStartTime = MachineStartTimedt,
                                                                        MachineStartTime = null,
                                                                        MachinelEndTime = null,
                                                                        CreateUser = Users != null ? Users.Id : 1

                                                                    });
                                                                    item.WorkOrderReportLogs.Add(new WorkOrderReportLog
                                                                    {
                                                                        ReportType = 2,
                                                                        ReCount = ReCount,
                                                                        NgCount = NgCount,
                                                                        StatusO = 1,
                                                                        StatusN = 3,
                                                                        ProducingMachine = Machine?.Name,
                                                                        ProducingMachineId = Machine?.Id,
                                                                        ActualStartTime = MachineStartTimedt,
                                                                        ActualEndTime = MachineEndTimedt,
                                                                        MachineStartTime = null,
                                                                        MachinelEndTime = null,
                                                                        CreateUser = Users != null ? Users.Id : 1
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    foreach (var ReportLogitem in item.WorkOrderReportLogs)
                                                                    {
                                                                        if (!ReportLogitem.ProducingMachineId.HasValue)
                                                                        {
                                                                            ReportLogitem.ProducingMachine = Machine?.Name;
                                                                            ReportLogitem.ProducingMachineId = Machine?.Id;
                                                                            ReportLogitem.ActualStartTime = MachineStartTimedt;
                                                                            ReportLogitem.ActualEndTime = MachineEndTimedt;
                                                                            ReportLogitem.CreateUser = Users != null ? Users.Id : 1;
                                                                            ReportLogitem.MachineStartTime = null;
                                                                            ReportLogitem.MachinelEndTime = null;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            _context.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        return Ok(MyFun.APIResponseError(ex.Message));
                    }
                }
            }
            return Ok(MyFun.APIResponseOK(""));
        }
    }
}
