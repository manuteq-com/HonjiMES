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
    /// 產品列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WorkOrdersController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
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
                    var CheckWorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.OrderDetailId == item.Id && x.DeleteFlag == 0).ToListAsync();
                    if (CheckWorkOrderHeads.Count() > 0)
                    {
                        NewResultMessage += "";
                    }
                    else
                    {
                        NewResultMessage += await this.NewWorkOrderByOrder(item);
                    }
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
                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    if (WorkOrderDetails.FirstOrDefault().Status == 1)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        WorkOrderDetails.FirstOrDefault().ActualStartTime = DateTime.Now;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 1, // 開工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderDetails.FirstOrDefault().ProducingMachine,
                            ReCount = WorkOrderDetails.FirstOrDefault().ReCount,
                            Remarks = WorkOrderReportData.Remarks,
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
                var WorkOrderDetails = WorkOrderHeads.WorkOrderDetails.Where(x => x.SerialNumber == WorkOrderReportData.WorkOrderSerial && x.DeleteFlag == 0).ToList();
                if (WorkOrderDetails.Count() == 1)
                {
                    if (WorkOrderDetails.FirstOrDefault().Status == 2)
                    {
                        WorkOrderDetails.FirstOrDefault().Status = 3;
                        WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        WorkOrderDetails.FirstOrDefault().ReCount = (WorkOrderDetails.FirstOrDefault()?.ReCount ?? 0) + WorkOrderReportData.ReCount;

                        WorkOrderDetails.FirstOrDefault().WorkOrderReportLogs.Add(new WorkOrderReportLog
                        {
                            WorkOrderDetailId = WorkOrderDetails.FirstOrDefault().Id,
                            ReportType = 2, // 完工回報
                            PurchaseId = WorkOrderDetails.FirstOrDefault().PurchaseId,
                            // PurchaseNo = WorkOrderDetails.FirstOrDefault().,
                            DrawNo = WorkOrderDetails.FirstOrDefault().DrawNo,
                            Manpower = WorkOrderDetails.FirstOrDefault().Manpower,
                            // ProducingMachineId = WorkOrderDetails.FirstOrDefault().,
                            ProducingMachine = WorkOrderDetails.FirstOrDefault().ProducingMachine,
                            ReCount = WorkOrderDetails.FirstOrDefault().ReCount,
                            Remarks = WorkOrderReportData.Remarks,
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
                        var statusCheck = true;
                        foreach (var item in WorkOrderHeads.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToList())
                        {
                            if (item.Status != 3)
                            {
                                statusCheck = false;
                            }
                        }
                        if (statusCheck)
                        {
                            WorkOrderHeads.Status = 3;
                        }
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
                            ProducingMachine = WorkOrderDetails.FirstOrDefault().ProducingMachine,
                            ReCount = WorkOrderDetails.FirstOrDefault().ReCount,
                            Remarks = WorkOrderReportData.Remarks,
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

        public async Task<string> NewWorkOrderByOrder(OrderDetail OrderDetail)
        {
            string sMessage = "";
            if (OrderDetail.ProductBasicId != 0)
            {
                //取得工單號
                var key = "WO";
                var WorkOrderNo = DateTime.Now.ToString("yyMMdd");
                var NoData = await _context.WorkOrderHeads.AsQueryable().Where(x => x.WorkOrderNo.Contains(key + WorkOrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.Id).ToListAsync();
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

                var DataType = 1;
                var BasicDataID = 0;
                var BasicDataNo = "";
                var BasicDataName = "";
                if (DataType == 0)
                {

                }
                else if (DataType == 1)
                {
                    var BasicData = _context.ProductBasics.Find(OrderDetail.ProductBasicId);
                    BasicDataID = BasicData.Id;
                    BasicDataNo = BasicData.ProductNo;
                    BasicDataName = BasicData.Name;
                }
                else if (DataType == 2)
                {

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
                    Status = 4, // 表示由訂單轉程的工單，需要再由人工確認該工單
                    CreateUser = 1
                };

                var billOfMaterial = await _context.MBillOfMaterials.AsQueryable().Where(x => x.ProductBasicId == OrderDetail.ProductBasicId).ToListAsync();
                if (billOfMaterial.Count() == 0)
                {
                    sMessage += "該品號尚未建立MBOM資訊 [ " + BasicDataNo + " ] !<br/>";
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
                        Count = OrderDetail.Quantity,
                        // PurchaseId
                        DrawNo = item.DrawNo,
                        Manpower = item.Manpower,
                        ProducingMachine = item.ProducingMachine,
                        Remarks = item.Remarks,
                        DueStartTime = DateTime.Now,
                        DueEndTime = DateTime.Now,
                        ActualStartTime = null,
                        ActualEndTime = null,
                        CreateUser = 1
                    };
                    nWorkOrderHead.WorkOrderDetails.Add(nWorkOrderDetail);
                }
                if (sMessage.Length == 0)
                {
                    _context.WorkOrderHeads.Add(nWorkOrderHead);
                    await _context.SaveChangesAsync();
                }
                // return Ok(MyFun.APIResponseOK("OK"));
            }
            else
            {
                sMessage += "";
            }
            return sMessage;
        }


    }
}
