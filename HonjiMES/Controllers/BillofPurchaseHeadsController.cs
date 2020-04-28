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
    public class BillofPurchaseHeadsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public BillofPurchaseHeadsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 進貨單列表
        /// </summary>
        /// <returns></returns>
        // GET: api/BillofPurchaseHeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillofPurchaseHead>>> GetBillofPurchaseHeads()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.BillofPurchaseHeads.OrderByDescending(x => x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 用ID查進貨單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillofPurchaseHeads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillofPurchaseHead>> GetBillofPurchaseHead(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var billofPurchaseHead = await _context.BillofPurchaseHeads.FindAsync(id);

            if (billofPurchaseHead == null)
            {
                return NotFound();
            }

            //return billofPurchaseHead;
            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }
        /// <summary>
        /// 修改進貨單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billofPurchaseHead"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseHeads/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillofPurchaseHead(int id, BillofPurchaseHead billofPurchaseHead)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            billofPurchaseHead.Id = id;
            var OldBillofPurchaseHead = _context.BillofPurchaseHeads.Find(id);
            var Msg = MyFun.MappingData(ref OldBillofPurchaseHead, billofPurchaseHead);

            OldBillofPurchaseHead.UpdateTime = DateTime.Now;
            OldBillofPurchaseHead.UpdateUser = 1;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillofPurchaseHeadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }
        /// <summary>
        /// 新增進貨單
        /// </summary>
        /// <param name="billofPurchaseHead"></param>
        /// <returns></returns>
        // POST: api/BillofPurchaseHeads
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseHead>> PostBillofPurchaseHead(BillofPurchaseHead billofPurchaseHead)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.BillofPurchaseHeads.Add(billofPurchaseHead);
            billofPurchaseHead.CreateTime = DateTime.Now;
            billofPurchaseHead.CreateUser = 1;
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }
        /// <summary>
        /// 刪除進貨單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/BillofPurchaseHeads/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BillofPurchaseHead>> DeleteBillofPurchaseHead(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var billofPurchaseHead = await _context.BillofPurchaseHeads.FindAsync(id);
            if (billofPurchaseHead == null)
            {
                return NotFound();
            }

            _context.BillofPurchaseHeads.Remove(billofPurchaseHead);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }

        private bool BillofPurchaseHeadExists(int id)
        {
            return _context.BillofPurchaseHeads.Any(e => e.Id == id);
        }
    }
}
