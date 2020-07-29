using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 調整單
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdjustListController : ControllerBase
    {
        private readonly HonjiContext _context;

        public AdjustListController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/AdjustList
        /// <summary>
        /// 調整單列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllStockLog>>> GetAdjustList(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.AllStockLogs.Where(x => x.DeleteFlag == 0);
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
 
        // GET: api/AdjustList/5
        /// <summary>
        /// 查詢調整單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetAdjustList(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var allStockLog = await _context.AllStockLogs.FindAsync(id);

            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }

        // PUT: api/AdjustList/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 修改調整單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="materiallog"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdjustList(int id, MaterialLog materiallog)
        {
            materiallog.Id = id;
            var Omateriallog = _context.MaterialLogs.Find(id);
            var Cmateriallog = Omateriallog;
            if (!string.IsNullOrWhiteSpace(materiallog.AdjustNo))
            {
                Cmateriallog.AdjustNo = materiallog.AdjustNo;
            }
            //修改時檢查[單號]是否重複
            if (_context.MaterialLogs.AsQueryable().Where(x => x.Id != id && (x.AdjustNo == Cmateriallog.AdjustNo) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("調整單的 [調整單號] 重複!", Cmateriallog));
            }
            
            var Msg = MyFun.MappingData(ref Omateriallog, materiallog);
            Omateriallog.UpdateTime = DateTime.Now;
            Omateriallog.UpdateUser = MyFun.GetUserID(HttpContext);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdjustListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(Omateriallog));
        }

        // POST: api/AdjustList
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 新增調整單
        /// </summary>
        /// <param name="materiallog"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MaterialLog>> PostAdjustList(MaterialLog materiallog)
        {
            //新增時檢查[單號]是否重複
            if (_context.MaterialLogs.AsQueryable().Where(x => (x.AdjustNo == materiallog.AdjustNo) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("調整單的 [調整單號]  已存在!", materiallog));
            }
            _context.MaterialLogs.Add(materiallog);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materiallog));
        }

        // DELETE: api/AdjustList/5
        /// <summary>
        /// 刪除供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterialLog>> DeleteAdjustList(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var materiallog = await _context.MaterialLogs.FindAsync(id);
            if (materiallog == null)
            {
                return NotFound();
            }
            materiallog.DeleteFlag = 1;
            // _context.MaterialLogs.Remove(materiallog);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materiallog));
        }

        private bool AdjustListExists(int id)
        {
            return _context.MaterialLogs.Any(e => e.Id == id);
        }
    }
}
