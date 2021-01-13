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
            foreach (var item in (List<WorkOrderDetail>)data)
            {
                newlist.Add(new WorkSchedulerVM
                {
                    Id = item.Id,
                    Text = item.WorkOrderHead.WorkOrderNo + " 工序：" + item.SerialNumber,
                    StartDate = item.DueStartTime.Value,
                    EndDate = item.DueEndTime ?? item.DueStartTime.Value,
                    AllDay = true,
                    Status = item.Status
                });
            }
            return newlist;
        }
        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 報工紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkSchedulerVM>> GetWorkOrderEstimate(
            [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderHead);
            var machine = _context.MachineInformations.AsEnumerable().Where(y => y.EnableState == 1).OrderBy(x => x.Name).ToList();
            var machineDefDate = _context.MachineWorkdates.Where(x => x.DeleteFlag == 0 && x.Setting == 1).OrderBy(x => x.WorkDate).FirstOrDefault();
            var machineSetDate = _context.MachineWorkdates.Where(x => x.DeleteFlag == 0 && x.Setting == 0).OrderBy(x => x.WorkDate).ToList();
            // data.FirstOrDefault().WorkOrderDetail.WorkOrderHead.WorkOrderNo 單獨抓取WorkOrderNo
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            var TimeData = GetTimeByMachine(FromQueryResult.data, machine);
            FromQueryResult.data = GetEstimateData(FromQueryResult.data, TimeData, machineDefDate , machineSetDate);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
        private List<ResoureAllocation> GetTimeByMachine(object data, List<MachineInformation> machine)
        {
            // _context.ChangeTracker.LazyLoadingEnabled = true;
            // var WorkOrderHeads = await _context.WorkOrderHeads.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderDetails).ToListAsync();
            var WorkOrderDetails = (List<WorkOrderDetail>)data;
            // var dataNew = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && (x.WorkOrderHead.Status == 0 || x.WorkOrderHead.Status == 4)); // 工單Head為[新建、轉單]
            var dataAssign = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && (x.WorkOrderHead.Status == 1 || (x.WorkOrderHead.Status == 2 && x.Status == 1))); // 工單Head為[已派工]
            var dataStart = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && (x.WorkOrderHead.Status == 2 && x.Status != 1)); // 工單Head為[已開工]
            // var dataReady = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.WorkOrderHead.Status == 3); // 工單Head為[完工]
            // var dataFinish = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.WorkOrderHead.Status == 5); // 工單Head為[結案]
            
            var Resource = new List<ResoureAllocation>();
            // var New = dataNew.Where(x => x.ProducingMachine == null);
            // var NewCount = New.Count();
            // var NewProcessTime = New.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
            var Assign = dataAssign.Where(x => x.ProducingMachine == null);
            var AssignCount = Assign.Count();
            var AssignProcessTime = Assign.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
            var Start = dataStart.Where(x => x.ProducingMachine == null);
            var StartCount = Start.Count();
            var StartProcessTime = Start.Where(x => x.SerialNumber != 1).Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
            // var Finish = dataFinish.Where(x => x.ProducingMachine == null);
            // var FinishCount = Finish.Count();
            // var FinishProcessTime = Finish.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
            Resource.Add(new ResoureAllocation
            {
                ProducingMachine = "<無>",
                // New = NewCount + " / " + NewProcessTime,
                // Assign = AssignCount + " / " + AssignProcessTime,
                // Start = StartCount + " / " + StartProcessTime,
                // Finish = FinishCount + " / " + FinishProcessTime,
                AllCount = AssignCount + StartCount,
                AllTime = AssignProcessTime + StartProcessTime
            });
            foreach (var item in machine)
            {
                // New = dataNew.Where(x => x.ProducingMachine == item.Name);
                // NewCount = New.Count();
                // NewProcessTime = New.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                Assign = dataAssign.Where(x => x.ProducingMachine == item.Name);
                AssignCount = Assign.Count();
                AssignProcessTime = Assign.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                Start = dataStart.Where(x => x.ProducingMachine == item.Name);
                StartCount = Start.Count();
                StartProcessTime = Start.Where(x => x.SerialNumber != 1).Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                // Finish = dataFinish.Where(x => x.ProducingMachine == item.Name);
                // FinishCount = Finish.Count();
                // FinishProcessTime = Finish.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                Resource.Add(new ResoureAllocation
                {
                    ProducingMachine = item.Name,
                    // New = NewCount + " / " + NewProcessTime,
                    // Assign = AssignCount + " / " + AssignProcessTime,
                    // Start = StartCount + " / " + StartProcessTime,
                    // Finish = FinishCount + " / " + FinishProcessTime,
                    AllCount = AssignCount + StartCount,
                    AllTime = AssignProcessTime + StartProcessTime
                });
            }
            // _context.ChangeTracker.LazyLoadingEnabled = false;
            return Resource;
        }
        private List<WorkSchedulerVM> GetEstimateData(object data, List<ResoureAllocation> TimeData, MachineWorkdate machineDefDate, List<MachineWorkdate> machineSetDate)
        {
            var index = 0;
            var DT_now = DateTime.Now;
            var DT_start = machineDefDate.WorkTimeStart; // 資料庫預設上班時間
            var DT_end = machineDefDate.WorkTimeEnd; // 資料庫預設下班時間
            var DT_min = (DT_end - DT_start).TotalMinutes - 60; // 資料庫預設[單日工作時間]，扣除中午休息時間
            var newlist = new List<WorkSchedulerVM>();
            foreach (var item in TimeData)
            {
                var tempAllTime = Decimal.ToDouble(item.AllTime ?? 0); // 機台的[總工序時間]
                var tempDayIndex = 0; // 日期遞增
                while (tempAllTime > 0) // 將[總工序時間]扣除[單日工作時間]
                {
                    var tempDT_min = DT_min;
                    var tempDT = DT_now.AddDays(tempDayIndex);
                    var showCheck = false;
                    var hasDate = machineSetDate.Where(x => x.WorkDate == tempDT.Date);
                    if (hasDate.Any()) { // 檢查資料庫是否有設定該日期的[單日工作時間]
                        tempDT_min = (hasDate.FirstOrDefault().WorkTimeEnd - hasDate.FirstOrDefault().WorkTimeStart).TotalMinutes - 60;
                        showCheck = true;
                    } else if (tempDT.DayOfWeek != DayOfWeek.Saturday && tempDT.DayOfWeek != DayOfWeek.Sunday) { // 如果資料庫沒設定，且又是[平日]則使用預設[單日工作時間]
                        showCheck = true;
                    }
                    
                    if (showCheck) { // 如果不是平日，且也沒特別設定工作日，就不計算該日期，遞延
                        var showVal = tempDT_min;
                        if (tempAllTime < tempDT_min) { // 如果[總工序時間]還有多，則顯示[單日工作時間]，否則顯示剩餘的[總工序時間]
                            showVal = tempAllTime;
                        }
                        newlist.Add(new WorkSchedulerVM
                        {
                            Id = index++,
                            Text = item.ProducingMachine + " ：" + showVal,
                            StartDate = tempDT,
                            EndDate = tempDT,
                            AllDay = true,
                            Status = item.ProducingMachine == "<無>" ? 0 : 1
                        });
                        tempAllTime = tempAllTime - tempDT_min; // 更新[總工序時間]
                    }
                    
                    tempDayIndex++;
                }
            }

            return newlist;
        }
        
        /// <summary>
        /// 查詢工作日全部資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetMachineWorkDate(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            // _context.ChangeTracker.LazyLoadingEnabled = true;
            var data = _context.MachineWorkdates.Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.Setting).ThenByDescending(x => x.WorkDate);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            // _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
        
        // PUT: api/WorkScheduler/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 修改工作日
        /// </summary>
        /// <param name="id"></param>
        /// <param name="MachineWorkDate"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineWorkDate(int id, MachineWorkdate MachineWorkDate)
        {
            MachineWorkDate.Id = id;
            var OMachineWorkDate = _context.MachineWorkdates.Find(id);
            var CMachineWorkDate = OMachineWorkDate;

            var Msg = MyFun.MappingData(ref OMachineWorkDate, MachineWorkDate);
            OMachineWorkDate.UpdateTime = DateTime.Now;
            OMachineWorkDate.UpdateUser = MyFun.GetUserID(HttpContext);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
               
            }

            return Ok(MyFun.APIResponseOK(OMachineWorkDate));
        }

        // POST: api/WorkScheduler
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 新增工作日
        /// </summary>
        /// <param name="MachineWorkDate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MaterialLog>> PostMachineWorkDate(MachineWorkdate MachineWorkDate)
        {
            //新增時檢查[日期]是否重複
            if (_context.MachineWorkdates.AsQueryable().Where(x => (x.WorkDate == MachineWorkDate.WorkDate) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("[設定日期] 已存在!", MachineWorkDate));
            }
            MachineWorkDate.CreateUser = MyFun.GetUserID(HttpContext);
            _context.MachineWorkdates.Add(MachineWorkDate);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(MachineWorkDate));
        }

        // DELETE: api/WorkScheduler/5
        /// <summary>
        /// 刪除工作日
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterialLog>> DeleteMachineWorkDate(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var MachineWorkDate = await _context.MachineWorkdates.FindAsync(id);
            if (MachineWorkDate == null)
            {
                return NotFound();
            }
            MachineWorkDate.DeleteFlag = 1;
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(MachineWorkDate));
        }

        /// <summary>
        /// 查詢所有工單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderHead>>> GetWorkOrderHeadSummarys(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var data = _context.WorkOrderHeads.Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderDetails)
            .OrderByDescending(x => x.CreateTime).Select(x => new WorkOrderHeadInfo
            {
                Id = x.Id,
                WorkOrderNo = x.WorkOrderNo,
                Count = x.Count,
                Status = x.Status,
                DueStartTime = x.DueStartTime,
                DueEndTime = x.DueEndTime,
                DeleteFlag = x.DeleteFlag,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                Total = x.WorkOrderDetails.Where(y => y.DeleteFlag == 0)
                .Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))))
            });
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/WorkOrderReportLogs
        /// <summary>
        /// 工序工時明細
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkSchedulerVM>> GetWorkOrderDetailSummary(int Id)
        {
            var data = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.WorkOrderHeadId == Id).ToListAsync();
            var total = data.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
            return Ok(MyFun.APIResponseOK(data));
        }

    }
}
