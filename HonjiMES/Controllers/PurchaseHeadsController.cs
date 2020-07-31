using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    [JWTAuthorize]
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
        /// 採購單號
        /// </summary>
        /// <returns></returns>
        // GET: api/PurchaseHeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseHead>>> GetPurchaseNumber()
        {
            var key = "PI";
            var dt = DateTime.Now;
            var PurchaseNo = dt.ToString("yyMMdd");

            var NoData = await _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(key + PurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastPurchaseNo = NoData.FirstOrDefault().PurchaseNo;
                var NoLast = Int32.Parse(LastPurchaseNo.Substring(LastPurchaseNo.Length - 3, 3));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var PurchaseHeadData = new PurchaseHead{
                Type = 10,
                CreateTime = dt,
                SupplierId = 0,
                PurchaseNo = key + PurchaseNo + NoCount.ToString("000")
            };
            return Ok(MyFun.APIResponseOK(PurchaseHeadData));
        }

        /// <summary>
        /// 採購單號
        /// </summary>
        /// <returns></returns>
        // POST: api/PurchaseHeads
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetPurchaseNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null) {
                var key = CreateNoData.Type == 10 ? "PI" : "PO";
                var PurchaseNo = CreateNoData.CreateTime.ToString("yyMMdd");
                
                var NoData = await _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(key + PurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1) {
                    var LastPurchaseNo = NoData.FirstOrDefault().PurchaseNo;
                    var NoLast = Int32.Parse(LastPurchaseNo.Substring(LastPurchaseNo.Length - 3, 3));
                    if (NoCount <= NoLast) {
                        NoCount = NoLast + 1;
                    }
                }
                CreateNoData.CreateNumber = key + PurchaseNo + NoCount.ToString("000");
                return Ok(MyFun.APIResponseOK(CreateNoData));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 採購單列表
        /// </summary>
        /// <returns></returns>
        // GET: api/PurchaseHeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseHead>>> GetPurchaseHeadsOld()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.PurchaseHeads.AsQueryable().Where(x => x.DeleteFlag == 0);
            var PurchaseHeads = await data.OrderByDescending(x => x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(PurchaseHeads));
        }

        /// <summary>
        /// 進貨單列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <param name="detailfilter"></param>
        /// <returns></returns>
        // GET: api/BillofPurchaseHead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillofPurchaseHead>>> GetPurchaseHeads(
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.PurchaseHeads.Where(x => x.DeleteFlag == 0);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            if (!string.IsNullOrWhiteSpace(qSearchValue.MaterialNo))
            {
                data = data.Where(x => x.PurchaseDetails.Where(y => y.DataNo.Contains(qSearchValue.MaterialNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
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
        /// 採購單列表
        /// </summary>
        /// <param name="status">0:未完成，1:已結案</param>
        /// <returns></returns>
        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseHead>>> GetPurchasesByStatus(int? status)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var PurchaseHeads = _context.PurchaseHeads.AsQueryable();
            if (status.HasValue)
            {
                PurchaseHeads = PurchaseHeads.Where(x => x.Status == status && x.DeleteFlag == 0);
            }
            var data = await PurchaseHeads.OrderByDescending(x=>x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 用ID取採購單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/PurchaseHeads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseHead>> GetPurchasesBySupplier(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var purchaseHead = await _context.PurchaseHeads.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Status == 0 && x.PurchaseDetails.Where(y => y.DeleteFlag == 0 && y.SupplierId == id && y.Quantity != y.PurchaseCount).Any()).ToListAsync();

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
            OldPurchaseHead.UpdateUser = MyFun.GetUserID(HttpContext);

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
            purchaseHead. CreateUser = MyFun.GetUserID(HttpContext);
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
            purchaseHead.DeleteFlag = 1;
            // _context.PurchaseHeads.Remove(purchaseHead);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(purchaseHead));
        }

        private bool PurchaseHeadExists(int id)
        {
            return _context.PurchaseHeads.Any(e => e.Id == id);
        }
    }
}
