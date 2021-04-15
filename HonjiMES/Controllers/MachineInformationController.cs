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
    public class MachineInformationController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachineInformationController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;
        }
        /// <summary>
        /// 查詢機台列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineInformation>>> GetMachineInformations()
        {
            var data = _context.MachineInformations.AsQueryable().Where(x => x.EnableState == 1);
            var machineInformation = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(machineInformation));
        }

        /// <summary>
        /// 使用ID查詢機台列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineInformation>> GetMachineInformationById(int id)
        {
            var machineInformation = await _context.MachineInformations.FindAsync(id);

            if (machineInformation == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(machineInformation));
        }

        /// <summary>
        /// 新增機台列表
        /// </summary>
        /// <param name="machineInformation"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(MachineInformation machineInformation)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.MachineInformations.AsQueryable().Where(x => (x.Name == machineInformation.Name || x.Url == machineInformation.Url) && x.EnableState == 1).Any())
            {
                return Ok(MyFun.APIResponseError("機台的 [名稱] 或 [連線IP] 已存在!", machineInformation));
            }
            _context.MachineInformations.Add(machineInformation);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(machineInformation));
        }

        /// <summary>
        /// 修改機台列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="machineInformation"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, MachineInformation machineInformation)
        {
            machineInformation.Id = id;
            var OmachineInformation = _context.MachineInformations.Find(id);
            var CmachineInformation = OmachineInformation;
            if (!string.IsNullOrWhiteSpace(machineInformation.Url))
            {
                CmachineInformation.Url = machineInformation.Url;
            }
            if (!string.IsNullOrWhiteSpace(machineInformation.Name))
            {
                CmachineInformation.Name = machineInformation.Name;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.MachineInformations.AsQueryable().Where(x => x.Id != id && (x.Name == CmachineInformation.Name || x.Url == CmachineInformation.Url) && x.EnableState == 1).Any())
            {
                return Ok(MyFun.APIResponseError("客戶的的 [代號] 或 [名稱] 重複!", CmachineInformation));
            }

            var Msg = MyFun.MappingData(ref OmachineInformation, machineInformation);
            OmachineInformation.UpdateTime = DateTime.Now;
            // OmachineInformation.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineInformationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(machineInformation));
        }

        /// <summary>
        /// 刪除機台列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MachineInformation>> DeleteCustomer(int id)
        {
            var machineInformation = await _context.MachineInformations.FindAsync(id);
            if (machineInformation == null)
            {
                return NotFound();
            }
            machineInformation.EnableState = 0;
            // _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(machineInformation));
        }

        private bool MachineInformationExists(int id)
        {
            return _context.MachineInformations.Any(e => e.Id == id);
        }
    }
}
