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
    /// 顧客列表
    /// </summary>
    // [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachineManagementController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachineManagementController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢顧客列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var data = _context.Customers.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Customers = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Customers));
        }

        public async Task<ActionResult<IEnumerable<machine>>> GetMachineData()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var WorkOrderDetails = await _context.WorkOrderDetails.Where(x => x.DeleteFlag == 0).ToListAsync();
            var machine = _context.WorkOrderDetails.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.Status == 1 || y.Status == 2).GroupBy(x => x.ProducingMachine).ToList();
            
            // 工單Head為[已派工]
            var dataAssign = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && (x.WorkOrderHead.Status == 1 || (x.WorkOrderHead.Status == 2 && x.Status == 1))); 
            // 工單Head為[已開工]
            var dataStart = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && (x.WorkOrderHead.Status == 2 && x.Status != 1)); 

            var machineList = new List <machine>();
            foreach (var item in machine)
            {
                var Assign = dataAssign.Where(x => x.ProducingMachine == item.Key);
                var AssignCount = Assign.Count();
                var AssignProcessTime = Assign.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                var Start = dataStart.Where(x => x.ProducingMachine == item.Key);
                var StartCount = Start.Count();
                var StartProcessTime = Start.Where(x => x.SerialNumber != 1).Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));
                
                var dt = DateTime.Now;
                var machineData = new machine();
                var x = item.FirstOrDefault();
                machineData.Id = x.Id;
                machineData.DataNo = x.WorkOrderHead.DataNo;
                machineData.ProcessName = x.ProcessNo + "_" + x.ProcessName;
                machineData.RemainingTime = Convert.ToInt32((dt - (x.DueStartTime ?? dt)).TotalMinutes);
                machineData.ProcessTotal = AssignCount + StartCount;
                machineData.TotalTime = AssignProcessTime + StartProcessTime;
                machineData.machineOrderList = new List<machineOrder>();
                foreach (var itemdata in item)
                {
                    machineData.machineOrderList.Add(new machineOrder{
                    Id = itemdata.Id,
                    WorkOrderNo = itemdata.WorkOrderHead.WorkOrderNo
                });
                }
                machineList.Add(machineData);
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(machineList));
        }

        public async Task<ActionResult<IEnumerable<machine>>> GetProcessDatas()
        {
            var Data = _context.WorkOrderDetails.Include( x => x.WorkOrderHead);
            return Ok(MyFun.APIResponseOK(Data));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
