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
            var key = "PO";
            var dt = DateTime.Now;
            var BillofPurchaseNo = dt.ToString("yyMMdd");

            var NoData = await _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(key + BillofPurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            var BillofPurchaseHeadData = new BillofPurchaseHead
            {
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
            if (CreateNoData != null)
            {
                var key = "PO";
                var BillofPurchaseNo = CreateNoData.CreateTime.ToString("yyMMdd");

                var NoData = await _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(key + BillofPurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                    var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                    if (NoCount <= NoLast)
                    {
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
            data = data.Include(x => x.BillofPurchaseDetails);
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
            OldBillofPurchaseHead.UpdateUser = MyFun.GetUserID(HttpContext);

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
            billofPurchaseHead.CreateUser = MyFun.GetUserID(HttpContext);
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
                var MaterialBasics = await _context.MaterialBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
                var ProductBasics = await _context.ProductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
                var WiproductBasics = await _context.WiproductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
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
                if (checkBillofPurchaseNo != 0)
                {
                    return Ok(MyFun.APIResponseError("[進貨單號]已存在! 請刷新單號!"));
                }
                Head.CreateTime = dt;
                Head.CreateUser = MyFun.GetUserID(HttpContext);
                var Details = new List<BillofPurchaseDetail>();
                foreach (var item in Detail)
                {
                    var PurchaseDetail = _context.PurchaseDetails.AsQueryable().Include(x => x.Purchase).Where(x => x.PurchaseId == item.PurchaseId && x.DataId == item.DataId && x.DeleteFlag == 0).FirstOrDefault();
                    if (item.DataType == 1)
                    {
                        var BasicData = MaterialBasics.Find(x => x.Id == item.DataId);
                        item.DataNo = BasicData.MaterialNo;
                        item.DataName = BasicData.Name;
                        item.Specification = BasicData.Specification;
                    }
                    else if (item.DataType == 2)
                    {
                        var BasicData = ProductBasics.Find(x => x.Id == item.DataId);
                        item.DataNo = BasicData.ProductNo;
                        item.DataName = BasicData.Name;
                        item.Specification = BasicData.Specification;
                    }
                    else if (item.DataType == 3)
                    {
                        var BasicData = WiproductBasics.Find(x => x.Id == item.DataId);
                        item.DataNo = BasicData.WiproductNo;
                        item.DataName = BasicData.Name;
                        item.Specification = BasicData.Specification;
                    }
                    item.PurchaseDetailId = PurchaseDetail.Id;
                    item.CreateTime = dt;
                    item.CreateUser = MyFun.GetUserID(HttpContext);
                    Details.Add(item);

                    PurchaseDetail.PurchaseCount += item.Quantity;
                    PurchaseDetail.Purchase.Status = 2;
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
        /// 藉由供應商 新增進貨單同時新明細
        /// </summary>
        /// <param name="PostBillofPurchaseHead_Detail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseHead>> PostBillofPurchaseHead_DetailBySupplier(PostBillofPurchaseHead_Detail PostBillofPurchaseHead_Detail)
        {
            try
            {
                var MaterialBasics = await _context.MaterialBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
                var ProductBasics = await _context.ProductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
                var WiproductBasics = await _context.WiproductBasics.Where(x => x.DeleteFlag == 0).ToListAsync();
                var PurchaseHeads = await _context.PurchaseHeads.Where(x => x.DeleteFlag == 0).ToListAsync();
                var Head = PostBillofPurchaseHead_Detail.BillofPurchaseHead;
                var Detail = PostBillofPurchaseHead_Detail.BillofPurchaseDetail;

                // 用類似GroupBy的方式區分採購種類。
                var PostBillofPurchaseBySupplier = new List<PostBillofPurchaseBySupplier>();
                foreach (var item in Detail)
                {
                    var PurchaseHeadData = PurchaseHeads.Find(x => x.Id == item.PurchaseId);
                    var dataCheck = PostBillofPurchaseBySupplier.Where(x => x.PurchaseHeadType == PurchaseHeadData.Type).ToList();
                    if (dataCheck.Count() == 1)
                    {
                        if (!dataCheck.FirstOrDefault().PurchaseHeadIdArray.Exists(x => x == item.PurchaseId))
                        {
                            dataCheck.FirstOrDefault().PurchaseHeadIdArray.Add(item?.PurchaseId ?? 0);
                        }
                    }
                    else
                    {
                        PostBillofPurchaseBySupplier.Add(new PostBillofPurchaseBySupplier
                        {
                            PurchaseHeadType = PurchaseHeadData.Type,
                            PurchaseHeadIdArray = new List<int> { item?.PurchaseId ?? 0 }
                        });
                    }
                }

                var dt = DateTime.Now;
                foreach (var item2 in PostBillofPurchaseBySupplier)
                {
                    var No = dt.ToString("yyMMdd");
                    var NoData = _context.BillofPurchaseHeads.AsQueryable().Where(x => x.BillofPurchaseNo.Contains(No) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                    var NoCount = NoData.Count() + 1;
                    if (NoCount != 1)
                    {
                        var LastBillofPurchaseNo = NoData.FirstOrDefault().BillofPurchaseNo;
                        var NoLast = Int32.Parse(LastBillofPurchaseNo.Substring(LastBillofPurchaseNo.Length - 3, 3));
                        if (NoCount <= NoLast)
                        {
                            NoCount = NoLast + 1;
                        }
                    }
                    var BillofPurchaseHead = new BillofPurchaseHead();
                    BillofPurchaseHead.BillofPurchaseNo = "BOP" + No + NoCount.ToString("000");//進貨單  BOP + 年月日(西元年後2碼) + 001(當日流水號)
                    BillofPurchaseHead.BillofPurchaseDate = dt;
                    BillofPurchaseHead.CreateTime = dt;
                    BillofPurchaseHead.CreateUser = MyFun.GetUserID(HttpContext);
                    var Details = new List<BillofPurchaseDetail>();
                    foreach (var item in Detail)
                    {
                        if (item2.PurchaseHeadIdArray.Exists(x => x == (item?.PurchaseId ?? 0)))
                        {
                            var PurchaseDetail = _context.PurchaseDetails.AsQueryable().Include(x => x.Purchase).Where(x => x.PurchaseId == item.PurchaseId && x.DataId == item.DataId && x.DeleteFlag == 0).FirstOrDefault();
                            if (item.DataType == 1)
                            {
                                var BasicData = MaterialBasics.Find(x => x.Id == item.DataId);
                                item.DataNo = BasicData.MaterialNo;
                                item.DataName = BasicData.Name;
                                item.Specification = BasicData.Specification;
                            }
                            else if (item.DataType == 2)
                            {
                                var BasicData = ProductBasics.Find(x => x.Id == item.DataId);
                                item.DataNo = BasicData.ProductNo;
                                item.DataName = BasicData.Name;
                                item.Specification = BasicData.Specification;
                            }
                            else if (item.DataType == 3)
                            {
                                var BasicData = WiproductBasics.Find(x => x.Id == item.DataId);
                                item.DataNo = BasicData.WiproductNo;
                                item.DataName = BasicData.Name;
                                item.Specification = BasicData.Specification;
                            }
                            item.PurchaseDetailId = PurchaseDetail.Id;
                            item.CreateTime = dt;
                            item.CreateUser = MyFun.GetUserID(HttpContext);
                            Details.Add(item);

                            PurchaseDetail.PurchaseCount += item.Quantity;
                            PurchaseDetail.Purchase.Status = 2;
                        }
                    }
                    BillofPurchaseHead.BillofPurchaseDetails = Details;
                    _context.BillofPurchaseHeads.Add(BillofPurchaseHead);
                }

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
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var billofPurchaseHead = await _context.BillofPurchaseHeads.FindAsync(id);
            if (billofPurchaseHead == null)
            {
                return NotFound();
            }
            billofPurchaseHead.DeleteFlag = 1;
            foreach (var item in billofPurchaseHead.BillofPurchaseDetails)
            {
                if (item.DeleteFlag == 0)
                {
                    item.DeleteFlag = 1;
                    item.PurchaseDetail.PurchaseCount -= item.Quantity;   
                }
            }
            // _context.BillofPurchaseHeads.Remove(billofPurchaseHead);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料

            return Ok(MyFun.APIResponseOK(billofPurchaseHead));
        }

        private bool BillofPurchaseHeadExists(int id)
        {
            return _context.BillofPurchaseHeads.Any(e => e.Id == id);
        }
    }
}
