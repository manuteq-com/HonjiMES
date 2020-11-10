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

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 調整紀錄
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResourceAllocationController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ResourceAllocationController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/ResourceAllocation
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResoureAllocation>>> GetMachineReport(int TimeType)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            // var WorkOrderHeads = await _context.WorkOrderHeads.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderDetails).ToListAsync();
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToListAsync();
            var machine = _context.WorkOrderDetails.AsEnumerable().Where(y => y.DeleteFlag == 0).GroupBy(x => x.ProducingMachine).ToList();

            var dataNew = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 0);
            var dataAssign = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 1);
            var dataStart = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 2);
            var dataReady = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 3);
            var dataToNew = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 4);
            var dataFinish = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 5);

            var Resource = new List<ResoureAllocation>();
            foreach (var item in machine)
            {
                Resource.Add(new ResoureAllocation
                {
                    ProducingMachine = item.Key == null ? "<無>" : item.Key,
                    New = dataNew.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataNew.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Assign = dataAssign.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataAssign.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Start = dataStart.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataStart.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Ready = dataReady.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataReady.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    ToNew = dataToNew.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataToNew.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Finish = dataFinish.Where(x => x.ProducingMachine == item.Key).Count() + " / " + dataFinish.Where(x => x.ProducingMachine == item.Key).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                });
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(Resource));
        }


        // GET: api/Adjusts/5
        /// <summary>
        /// 查詢調整紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetAdjustLog(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var allStockLog = await _context.AllStockLogs.FindAsync(id);

            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }
    }
}
