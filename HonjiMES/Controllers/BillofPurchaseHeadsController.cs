using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;

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
        public async Task<ActionResult<IEnumerable<BillofPurchaseHead>>> GetBillofPurchaseHeadsOld()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.BillofPurchaseHeads.AsQueryable().Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 進貨單號
        /// </summary>
        /// <returns></returns>
        // GET: api/BillofPurchaseHead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillofPurchaseHead>>> GetBillofPurchaseNumber()
        {
            var key = "BOP";
            var dt = DateTime.Now;
            var BillofPurchaseNo = dt.ToString("yyMMdd");

            var NoData = await _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(key + BillofPurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var BillofPurchaseHeadData = new BillofPurchaseHead{
                CreateTime = dt,
                BillofPurchaseNo = key + BillofPurchaseNo + NoCount.ToString("000")
            };
            return Ok(MyFun.APIResponseOK(BillofPurchaseHeadData));
        }

        /// <summary>
        /// 進貨單號
        /// </summary>
        /// <returns></returns>
        // POST: api/BillofPurchaseHead
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetBillofPurchaseNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null) {
                var key = "BOP";
                var BillofPurchaseNo = CreateNoData.CreateTime.ToString("yyMMdd");
                
                var NoData = await _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(key + BillofPurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1) {
                    var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                    var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                    if (NoCount <= NoLast) {
                        NoCount = NoLast + 1;
                    }
                }
                CreateNoData.CreateNumber = key + BillofPurchaseNo + NoCount.ToString("000");
                return Ok(MyFun.APIResponseOK(CreateNoData));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 進貨單列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <param name="detailfilter"></param>
        /// <returns></returns>
        // GET: api/BillofPurchaseHead
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillofPurchaseHead>>> GetBillofPurchaseHeads(
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.BillofPurchaseHeads.Where(x => x.DeleteFlag == 0);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            if (!string.IsNullOrWhiteSpace(qSearchValue.PurchaseNo))
            {
                data = data.Where(x => x.BillofPurchaseDetails.Where(y => y.Purchase.PurchaseNo.Contains(qSearchValue.PurchaseNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            if (!string.IsNullOrWhiteSpace(qSearchValue.SupplierCode))
            {
                data = data.Where(x => x.BillofPurchaseDetails.Where(y => y.Supplier.Code.Contains(qSearchValue.SupplierCode, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            if (!string.IsNullOrWhiteSpace(qSearchValue.MaterialNo))
            {
                data = data.Where(x => x.BillofPurchaseDetails.Where(y => y.DataNo.Contains(qSearchValue.MaterialNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
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
        /// 新增進貨單同時新明細
        /// </summary>
        /// <param name="PostBillofPurchaseHead_Detail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseHead>> PostBillofPurchaseHead_Detail(PostBillofPurchaseHead_Detail PostBillofPurchaseHead_Detail)
        {
            try
            {
                var Head = PostBillofPurchaseHead_Detail.BillofPurchaseHead;
                var Detail = PostBillofPurchaseHead_Detail.BillofPurchaseDetail;

                var dt = DateTime.Now;
                // var No = dt.ToString("yyMMdd");
                // var NoData = _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(No) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                // var NoCount = NoData.Count() + 1;
                // if (NoCount != 1) {
                //     var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                //     var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                //     if (NoCount <= NoLast) {
                //         NoCount = NoLast + 1;
                //     }
                // }
                // Head.BillofPurchaseNo = "BOP" + No + NoCount.ToString("000");//進貨單  BOP + 年月日(西元年後2碼) + 001(當日流水號)
                var checkBillofPurchaseNo = _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(Head.BillofPurchaseNo) && x.DeleteFlag == 0).Count();
                if (checkBillofPurchaseNo != 0) {
                    return Ok(MyFun.APIResponseError("[進貨單號]已存在! 請刷新單號!"));
                }
                Head.CreateTime = dt;
                Head.CreateUser = 1;
                var Details = new List<BillofPurchaseDetail>();
                foreach (var item in Detail)
                {
                    var PurchaseDetail = _context.PurchaseDetails.AsQueryable().Where(x => x.PurchaseId == item.PurchaseId && x.DataId == item.DataId).FirstOrDefault();
                    var Material = _context.Materials.Find(item.DataId);
                    item.PurchaseDetailId = PurchaseDetail.Id;
                    item.DataName = Material.Name;
                    item.DataNo = Material.MaterialNo;
                    item.Specification = Material.Specification;
                    item.CreateTime = dt;
                    item.CreateUser = 1;
                    Details.Add(item);
                }
                Head.BillofPurchaseDetails = Details;
                _context.BillofPurchaseHeads.Add(Head);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Head));
            }
            catch (Exception ex)
            {
                return Ok(MyFun.APIResponseError(ex.Message));
            }
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
            billofPurchaseHead.DeleteFlag = 1;
            // _context.BillofPurchaseHeads.Remove(billofPurchaseHead);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }

        private bool BillofPurchaseHeadExists(int id)
        {
            return _context.BillofPurchaseHeads.Any(e => e.Id == id);
        }
    }
}
