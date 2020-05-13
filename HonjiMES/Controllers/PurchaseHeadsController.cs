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
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PurchaseHeadsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public PurchaseHeadsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 採購單列表
        /// </summary>
        /// <returns></returns>
        // GET: api/PurchaseHeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseHead>>> GetPurchaseHeads()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.PurchaseHeads.AsQueryable().OrderByDescending(x=>x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 用ID取採購單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/PurchaseHeads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseHead>> GetPurchaseHead(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var purchaseHead = await _context.PurchaseHeads.FindAsync(id);

            if (purchaseHead == null)
            {
                return NotFound();
            }

            //return purchaseHead;
            return Ok(MyFun.APIResponseOK(purchaseHead));
        }
        /// <summary>
        /// 修改採購單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchaseHead"></param>
        /// <returns></returns>
        // PUT: api/PurchaseHeads/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseHead(int id, PurchaseHead purchaseHead)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            purchaseHead.Id = id;
            var OldPurchaseHead = _context.PurchaseHeads.Find(id);
            var Msg = MyFun.MappingData(ref OldPurchaseHead, purchaseHead);

            OldPurchaseHead.UpdateTime = DateTime.Now;
            OldPurchaseHead.UpdateUser = 1;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseHeadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(purchaseHead));
        }
        /// <summary>
        /// 新增採購單
        /// </summary>
        /// <param name="purchaseHead"></param>
        /// <returns></returns>
        // POST: api/PurchaseHeads
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PurchaseHead>> PostPurchaseHead(PurchaseHead purchaseHead)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.PurchaseHeads.Add(purchaseHead);
            purchaseHead.CreateTime = DateTime.Now;
            purchaseHead.CreateUser = 1;
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(purchaseHead));
        }
        /// <summary>
        /// 刪除採購單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/PurchaseHeads/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseHead>> DeletePurchaseHead(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var purchaseHead = await _context.PurchaseHeads.FindAsync(id);
            if (purchaseHead == null)
            {
                return NotFound();
            }

            _context.PurchaseHeads.Remove(purchaseHead);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(purchaseHead));
        }

        private bool PurchaseHeadExists(int id)
        {
            return _context.PurchaseHeads.Any(e => e.Id == id);
        }
    }
}
