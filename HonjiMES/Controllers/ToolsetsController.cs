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
    /// 製程刀具列表
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToolsetsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ToolsetsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;
        }

        // GET: api/Toolsets
        /// <summary>
        /// 製程刀具列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Toolset>>> GetToolsets()
        {
            var data = await _context.Toolsets.Where(x => x.DeleteFlag == 0).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 製程刀具列表依製程ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Toolset>>> GetToolsetsByProcessId(int id)
        {
            var data = await _context.Toolsets.Where(x => x.ProcessId == id && x.DeleteFlag == 0).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        // GET: api/Toolsets/5
        /// <summary>
        /// 製程刀具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Toolset>> GetToolset(int id)
        {
            var toolset = await _context.Toolsets.FindAsync(id);
            if (toolset == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(toolset));
        }

        // PUT: api/Toolsets/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// 修改製程刀具
        /// </summary>
        /// <param name="id"></param>
        /// <param name="toolset"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToolset(int id, Toolset toolset)
        {
            toolset.Id = id;
            var OldToolsets = _context.Toolsets.Find(id);
            var Msg = MyFun.MappingData(ref OldToolsets, toolset);

            OldToolsets.UpdateTime = DateTime.Now;
            OldToolsets.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolsetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(toolset));
        }

        // POST: api/Toolsets
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// 新增製程刀具
        /// </summary>
        /// <param name="toolset"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Toolset>> PostToolset(Toolset toolset)
        {
            _context.Toolsets.Add(toolset);
            toolset.CreateTime = DateTime.Now;
            toolset.CreateUser = MyFun.GetUserID(HttpContext);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(toolset));
        }

        // DELETE: api/Toolsets/5
        /// <summary>
        /// 刪除製程刀具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Toolset>> DeleteToolset(int id)
        {
            var toolset = await _context.Toolsets.FindAsync(id);
            if (toolset == null)
            {
                return NotFound();
            }

            toolset.DeleteFlag = 1;
            // _context.OrderHeads.Remove(orderHead);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(toolset));
        }

        private bool ToolsetExists(int id)
        {
            return _context.Toolsets.Any(e => e.Id == id);
        }
    }
}
