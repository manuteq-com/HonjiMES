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
            var machineName = _context.MachineInformations.Where(x => x.EnableState == 1).Select(x => x.Name).ToList();
            var machine = _context.WorkOrderDetails.AsEnumerable().Where(y => y.DeleteFlag == 0 && !string.IsNullOrWhiteSpace(y.ProducingMachine) && (y.Status == 1 || y.Status == 2))
            .OrderByDescending(x => x.Status).ThenBy(x => x.CreateTime).GroupBy(x => x.ProducingMachine).OrderBy(x => x.Key).ToList();

            // 工單Head為[已派工]
            var dataAssign = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 1);
            // 工單Head為[已開工]
            var dataStart = WorkOrderDetails.Where(x => x.DeleteFlag == 0 && x.Status == 2);

            // var no = 0;
            var machineList = new List<machine>();
            foreach (var items in machineName)
            {
                var machineData = new machine();
                machineList.Add(machineData);
                machineData.MachineName = items;
                foreach (var item in machine)
                {
                    if (items == item.Key)
                    {
                        var Assign = dataAssign.Where(x => x.ProducingMachine == item.Key);
                        var AssignCount = Assign.Count();
                        var AssignProcessTime = Assign.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                        var Start = dataStart.Where(x => x.ProducingMachine == item.Key);
                        var StartCount = Start.Count();
                        var StartProcessTime = Start.Sum(y => (y.ProcessTime + y.ProcessLeadTime) * (y.Count <= y.ReCount ? 0 : (y.Count - (y.ReCount ?? 0))));

                        var dt = DateTime.Now;

                        var x = item.FirstOrDefault();
                        //剩餘時間 = (前置時間+標準時間)*數量 - (現在時間- 實際開工時間)
                        var processtime = (x.ProcessTime + x.ProcessLeadTime) * (x.Count <= x.ReCount ? 0 : (x.Count - (x.ReCount ?? 0)));//工時
                        var tasktime = Convert.ToDecimal((dt - (x.ActualStartTime ?? dt)).TotalMinutes);//目前加工時間
                        var remain = processtime - tasktime;
                        // machineData.Id = no + 1;
                        if(item.Where(x => x.Status == 2).Any()){
                            machineData.Id = x.WorkOrderHead.Id;
                            machineData.No = x.WorkOrderHead.WorkOrderNo;
                            machineData.SerialNumber = x.SerialNumber;
                            machineData.DataNo = x.WorkOrderHead.DataNo;
                            machineData.ProcessName = x.ProcessNo + "_" + x.ProcessName;
                            if (Convert.ToDecimal(remain) >= 0)
                            {
                                machineData.RemainingTime = Convert.ToDecimal(remain);
                                machineData.DelayTime = 0;
                            }
                            else if (Convert.ToDecimal(remain) < 0)
                            {
                                machineData.RemainingTime = 0;
                                machineData.DelayTime = Math.Abs(Convert.ToDecimal(remain));
                            }
                        }
                        //  if(剩餘時間 >= 0){
                        //    工序剩餘時間 = 剩餘時間,抵累時間 = 0,總時間 = 已派工工序時間+已開工工序時間+剩餘時間
                        //  }elseif(剩餘時間 < 0 ){
                        //    工序剩餘時間 = 0,抵累時間 = |剩餘時間|,總時間 = 已派工工序時間+已開工工序時間
                        //  }
                        if (Convert.ToDecimal(remain) >= 0)
                        {
                            machineData.TotalTime = AssignProcessTime + StartProcessTime - processtime + Convert.ToDecimal(remain);
                        }
                        else if (Convert.ToDecimal(remain) < 0)
                        {
                            machineData.TotalTime = AssignProcessTime + StartProcessTime - processtime;
                        }
                        machineData.ProcessTotal = AssignCount;
                        machineData.machineOrderList = new List<machineOrder>();
                        foreach (var itemdata in item.Where(x=>x.Status == 1))
                        {
                            machineData.machineOrderList.Add(new machineOrder
                            {
                                Id = itemdata.WorkOrderHead.Id,
                                WorkOrderNo = itemdata.WorkOrderHead.WorkOrderNo + " / " + itemdata.ProcessNo + "_" + itemdata.ProcessName,
                                DetailSerialNumber = itemdata.SerialNumber
                            });
                        }

                    }
                }
            }

            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(machineList));
        }

        public async Task<ActionResult<IEnumerable<machine>>> GetProcessDatas()
        {
            var Data = _context.WorkOrderDetails.Include(x => x.WorkOrderHead);
            return Ok(MyFun.APIResponseOK(Data));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
