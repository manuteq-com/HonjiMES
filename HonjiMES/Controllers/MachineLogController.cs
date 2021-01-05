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
    /// 機台警示列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MachineLogController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MachineLogController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢機台警示資料列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MachineLog>>> GetMachineLogs()
        {
            var data = _context.MachineLogs.AsQueryable().Where(x => x.DeleteFlag == 0);
            var machinelog = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(machinelog));
        }

        /// <summary>
        /// 使用ID查詢機台警示資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MachineLog>> GetMachineLog(int id)
        {
            var machinelog = await _context.MachineLogs.FindAsync(id);

            if (machinelog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(machinelog));
        }
        private bool MachineLogExists(int id)
        {
            return _context.MachineLogs.Any(e => e.Id == id);
        }
    }
}
