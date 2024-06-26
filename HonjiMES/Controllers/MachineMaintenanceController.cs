﻿using System;
using DevExtreme.AspNet.Mvc;
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
    /// 機台保養列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachineMaintenanceController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachineMaintenanceController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 查詢機台保養列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineMaintenance>>> GetMachineMaintenances(
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.MachineMaintenances.AsQueryable().Where(x => x.DeleteFlag == 0);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 使用ID查詢機台保養列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineMaintenance>> GetMachineMaintenance(int id)
        {
            var machinemaintenance = await _context.MachineMaintenances.FindAsync(id);

            if (machinemaintenance == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(machinemaintenance));
        }
        /// <summary>
        /// 修改機台保養列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="machinemaintenance"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMachineMaintenance(int id, MachineMaintenance machinemaintenance)
        {
            machinemaintenance.Id = id;
            var Omachinemaintenance = _context.MachineMaintenances.Find(id);
            var COmachinemaintenance = Omachinemaintenance;
            if (!string.IsNullOrWhiteSpace(machinemaintenance.Item))
            {
                COmachinemaintenance.Item = machinemaintenance.Item;
            }

            var Msg = MyFun.MappingData(ref Omachinemaintenance, machinemaintenance);
            Omachinemaintenance.UpdateTime = DateTime.Now;
            Omachinemaintenance.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineMaintenanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(machinemaintenance));
        }
        /// <summary>
        /// 新增機台保養列表
        /// </summary>
        /// <param name="machinemaintenance"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MachineMaintenance>> PostMachineMaintenance(MachineMaintenance machinemaintenance)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.MachineMaintenances.AsQueryable().Where(x => x.MachineId == machinemaintenance.MachineId 
            && x.Item == machinemaintenance.Item && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("該機台已存在!", machinemaintenance));
            }
            machinemaintenance.RecentTime = DateTime.Now;
            machinemaintenance.NextTime = machinemaintenance.RecentTime.AddMonths(machinemaintenance.CycleTime);
            machinemaintenance.CreateTime = DateTime.Now;
            machinemaintenance.CreateUser = MyFun.GetUserID(HttpContext);
            _context.MachineMaintenances.Add(machinemaintenance);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(machinemaintenance));
        }

        /// 機台保養紀錄

        /// <summary>
        /// 使用ID查詢機台保養紀錄列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceDetail>> GetMaintenanceDetails(int id)
        {
            var maintenancedetails = await _context.MaintenanceDetails.Where(x => x.MaintenanceId == id).OrderBy(x => x.RecentTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(maintenancedetails));
        }

        /// <summary>
        /// 修改機台保養紀錄列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maintenancedetail"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaintenanceDetail(int id, MaintenanceDetail maintenancedetail)
        {
            maintenancedetail.Id = id;
            var Omaintenancedetail = _context.MaintenanceDetails.Find(id);
            var COmaintenancedetail = Omaintenancedetail;
            var Msg = MyFun.MappingData(ref Omaintenancedetail, maintenancedetail);
            Omaintenancedetail.UpdateTime = DateTime.Now;
            Omaintenancedetail.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineMaintenanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(maintenancedetail));
        }
        /// <summary>
        /// 新增機台保養紀錄列表
        /// </summary>
        /// <param name="maintenancedetail"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MaintenanceDetail>> PostMaintenanceDetail(MaintenanceDetail maintenancedetail)
        {
            var machinemaintenance = _context.MachineMaintenances.Find(maintenancedetail.MaintenanceId);
            machinemaintenance.RecentTime = maintenancedetail.RecentTime;
            machinemaintenance.NextTime = machinemaintenance.RecentTime.AddMonths(machinemaintenance.CycleTime);
            maintenancedetail.CreateTime = DateTime.Now;
            maintenancedetail.CreateUser = MyFun.GetUserID(HttpContext);
            _context.MaintenanceDetails.Add(maintenancedetail);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(maintenancedetail));
        }
    

        

        private bool MachineMaintenanceExists(int id)
        {
            return _context.MachineMaintenances.Any(e => e.Id == id);
        }
    }
}
