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
    public class WorkOrdersController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WorkOrdersController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
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
                if (WorkOrderDetails.Count() == 1) {
                    if (WorkOrderDetails.FirstOrDefault().Status == 1) {
                        WorkOrderDetails.FirstOrDefault().Status = 2;
                        WorkOrderDetails.FirstOrDefault().ActualStartTime = DateTime.Now;
                    } else {
                        return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    }
                } else {
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
                if (WorkOrderDetails.Count() == 1) {
                    if (WorkOrderDetails.FirstOrDefault().Status == 2) {
                        WorkOrderDetails.FirstOrDefault().Status = 3;
                        WorkOrderDetails.FirstOrDefault().ActualEndTime = DateTime.Now;
                        WorkOrderDetails.FirstOrDefault().ReCount = WorkOrderReportData.ReCount;
                    } else {
                        return Ok(MyFun.APIResponseError("工單狀態異常!"));
                    }
                } else {
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

    }
}
