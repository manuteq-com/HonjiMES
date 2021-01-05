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
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToolManagementController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ToolManagementController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢刀具基本資料列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToolManagement>>> GetToolManagements()
        {
            var data = _context.ToolManagements.AsQueryable().Where(x => x.DeleteFlag == 0);
            var toolmanagement = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(toolmanagement));
        }

        /// <summary>
        /// 使用ID查詢刀具基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToolManagement>> GetToolManagement(int id)
        {
            var toolmanagement = await _context.ToolManagements.FindAsync(id);

            if (toolmanagement == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(toolmanagement));
        }
        /// <summary>
        /// 修改刀具基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="toolmanagement"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToolManagement(int id, ToolManagement toolmanagement)
        {
            toolmanagement.Id = id;
            var Otoolmanagement = _context.ToolManagements.Find(id);
            var COtoolmanagement = Otoolmanagement;
            if (!string.IsNullOrWhiteSpace(toolmanagement.ToolSerialno))
            {
                COtoolmanagement.ToolSerialno = toolmanagement.ToolSerialno;
            }
            if (!string.IsNullOrWhiteSpace(toolmanagement.ToolSpecification))
            {
                COtoolmanagement.ToolSpecification = toolmanagement.ToolSpecification;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.ToolManagements.AsQueryable().Where(x => x.Id != id && (x.ToolName == COtoolmanagement.ToolName 
            || x.ToolSpecification == COtoolmanagement.ToolSpecification) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("刀具的 [刀具名稱] 或 [刀具規格] 重複!", COtoolmanagement));
            }

            var Msg = MyFun.MappingData(ref Otoolmanagement, toolmanagement);
            Otoolmanagement.UpdateTime = DateTime.Now;
            Otoolmanagement.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToolManagementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(toolmanagement));
        }
        /// <summary>
        /// 新增刀具基本資料列表
        /// </summary>
        /// <param name="toolmanagement"></param>
        /// <returns></returns>
        // POST: api/Customers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ToolManagement>> PostToolManagement(ToolManagement toolmanagement)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.ToolManagements.AsQueryable().Where(x => (x.ToolSerialno == toolmanagement.ToolSerialno || x.ToolSpecification == toolmanagement.ToolSpecification) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("客戶的 [代號] 或 [名稱] 已存在!", toolmanagement));
            }
            _context.ToolManagements.Add(toolmanagement);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(toolmanagement));
        }
        /// <summary>
        /// 刪除刀具基本資料列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ToolManagement>> DeleteToolManagement(int id)
        {
            var toolmanagement = await _context.ToolManagements.FindAsync(id);
            if (toolmanagement == null)
            {
                return NotFound();
            }
            toolmanagement.DeleteFlag = 1;
            // _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(toolmanagement));
        }

        private bool ToolManagementExists(int id)
        {
            return _context.ToolManagements.Any(e => e.Id == id);
        }
    }
}
