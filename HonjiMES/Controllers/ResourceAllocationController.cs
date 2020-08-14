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
        public async Task<ActionResult<IEnumerable<ResoureAllocation>>> GetMachineReprt(int TimeType)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            // var WorkOrderHeads = await _context.WorkOrderHeads.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderDetails).ToListAsync();
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToListAsync();
            var machine = _context.WorkOrderDetails.AsEnumerable().Where(y => y.DeleteFlag == 0).GroupBy(x => x.ProducingMachine).ToList();
            var Resource = new List<ResoureAllocation>();
            foreach (var item in machine)
            {
                Resource.Add(new ResoureAllocation
                {
                    ProducingMachine = item.Key,
                    New = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 0).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Assign = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 1).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Start = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 2).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Ready = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 3).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    ToNew = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 4).Sum(y => y.ProcessTime + y.ProcessLeadTime),
                    Finish = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.ProducingMachine == item.Key && x.WorkOrderHead.Status == 5).Sum(y => y.ProcessTime + y.ProcessLeadTime),
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
