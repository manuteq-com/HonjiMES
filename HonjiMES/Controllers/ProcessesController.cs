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
    /// 顧客列表
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProcessesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProcessesController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢顧客列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcesses()
        {
            var data = _context.Processes.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Processes = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Processes));
        }

        /// <summary>
        /// 使用ID查詢顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Processes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Process>> GetProcess(int id)
        {
            var process = await _context.Processes.FindAsync(id);

            if (process == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 修改顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        // PUT: api/Processes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcess(int id, Process process)
        {
            process.Id = id;
            var Oprocess = _context.Processes.Find(id);
            var Cprocess = Oprocess;
            if (!string.IsNullOrWhiteSpace(process.Code))
            {
                Cprocess.Code = process.Code;
            }
            if (!string.IsNullOrWhiteSpace(process.Name))
            {
                Cprocess.Name = process.Name;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.Processes.AsQueryable().Where(x => x.Id != id && (x.Name == Cprocess.Name || x.Code == Cprocess.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("製程的的 [代號] 或 [名稱] 重複!", Cprocess));
            }
            
            var Msg = MyFun.MappingData(ref Oprocess, process);
            Oprocess.UpdateTime = DateTime.Now;
            Oprocess.UpdateUser = 1;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcesseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 新增顧客列表
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        // POST: api/Processes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Process>> PostProcess(Process process)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.Processes.AsQueryable().Where(x => (x.Name == process.Name || x.Code == process.Code) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("製程的 [代號] 或 [名稱] 已存在!", process));
            }
            _context.Processes.Add(process);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(process));
        }
        /// <summary>
        /// 刪除顧客列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Processes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Process>> DeleteProcess(int id)
        {
            var process = await _context.Processes.FindAsync(id);
            if (process == null)
            {
                return NotFound();
            }
            process.DeleteFlag = 1;
            // _context.Processes.Remove(process);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(process));
        }

        private bool ProcesseExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
