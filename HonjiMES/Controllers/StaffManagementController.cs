using System;
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
    /// 人員管理列表
    /// </summary>
    //[JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StaffManagementController : ControllerBase
    {
        private readonly HonjiContext _context;

        public StaffManagementController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 查詢人員管理列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffManagement>>> GetStaffManagements(
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.StaffManagements.AsQueryable().Where(x => x.DeleteFlag == 0);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 使用ID查詢人員管理列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffManagement>> GetStaffManagement(int id)
        {
            var staffmanagement = await _context.StaffManagements.FindAsync(id);

            if (staffmanagement == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(staffmanagement));
        }
        /// <summary>
        /// 修改人員管理列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="staffmanagement"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaffManagement(int id, StaffManagement staffmanagement)
        {
            staffmanagement.Id = id;
            var Ostaffmanagement = _context.StaffManagements.Find(id);
            var COstaffmanagement = Ostaffmanagement;

            var Msg = MyFun.MappingData(ref Ostaffmanagement, staffmanagement);
            Ostaffmanagement.UpdateTime = DateTime.Now;
            Ostaffmanagement.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffManagementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(staffmanagement));
        }
        /// <summary>
        /// 新增人員管理列表
        /// </summary>
        /// <param name="staffmanagement"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<StaffManagement>> PostStaffManagement(StaffManagement staffmanagement)
        {
            _context.StaffManagements.Add(staffmanagement);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(staffmanagement));
        }

        /// <summary>
        /// 刪除人員管理列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StaffManagement>> DeleteStaffManagement(int id)
        {
            var staffmanagement = await _context.StaffManagements.FindAsync(id);
            if (staffmanagement == null)
            {
                return NotFound();
            }
            staffmanagement.DeleteFlag = 1;
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(staffmanagement));
        }
        /// <summary>
        /// 讀取人員管理資訊
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<WorkOrderDetailsByStaff>> GetStaffInformation(DateTime StartTime, DateTime EndTime)
        {
            StartTime = StartTime.Date;
            EndTime = EndTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var users = _context.Users.Where(x => x.DeleteFlag == 0).OrderBy(y => y.Username).ToList();
            var workOrderDetails = _context.WorkOrderDetails.AsEnumerable()
            .Where(y => y.DeleteFlag == 0 && y.DueEndTime>=StartTime && y.DueEndTime<= EndTime)
            .OrderBy(x => x.DueEndTime).ThenBy(x => x.SerialNumber).GroupBy(x => x.CreateUser).OrderBy(x => x.Key).ToList();
            var workOrderDetailsByStaff = new List<WorkOrderDetailsByStaff>();

            if (users == null)
            {
                return NotFound();
            }
            foreach (var user in users)
            {
                
                var workOrderDetailsByOne = new WorkOrderDetailsByStaff
                {
                    StaffName = user.Username,                    
                };
                var workOrderDetail = workOrderDetails.Where(x => x.Key == user.Id);
                if (workOrderDetail.Any())
                {
                    workOrderDetailsByOne.WorkOrderDetails = workOrderDetail.FirstOrDefault().ToList();
                    workOrderDetailsByOne.WorkTIme = workOrderDetail.FirstOrDefault().ToList().Sum(y => y.Count * (y.ProcessLeadTime + y.ProcessTime));
                }               
                workOrderDetailsByStaff.Add(workOrderDetailsByOne);
            }            
            return Ok(MyFun.APIResponseOK(workOrderDetailsByStaff));
        }

        private bool StaffManagementExists(int id)
        {
            return _context.MachineMaintenances.Any(e => e.Id == id);
        }
    }
}
