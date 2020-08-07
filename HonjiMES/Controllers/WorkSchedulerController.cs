using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Mvc;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonjiMES.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkSchedulerController : Controller
    {
        private readonly HonjiContext _context;

        public WorkSchedulerController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 報工紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkSchedulerVM>> GetWorkScheduler(
            [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.DueStartTime.HasValue).Include(x => x.WorkOrderHead);
            // data.FirstOrDefault().WorkOrderDetail.WorkOrderHead.WorkOrderNo 單獨抓取WorkOrderNo
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            FromQueryResult.data = GetScheduler(FromQueryResult.data);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        private List<WorkSchedulerVM> GetScheduler(object data)
        {
            var newlist = new List<WorkSchedulerVM>();
            foreach (var item in (List<WorkOrderDetail>) data)
            {
                newlist.Add(new WorkSchedulerVM
                {
                    Id = item.Id,
                    Text = item.WorkOrderHead.WorkOrderNo,
                    StartDate = item.DueStartTime.Value,
                    EndDate = item.DueEndTime ?? item.DueStartTime.Value,
                    AllDay = true
                });
            }
            return newlist;
        }
    }
}
