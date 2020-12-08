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
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BillofPurchaseDetailsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public BillofPurchaseDetailsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 進貨單明細
        /// </summary>
        /// <returns></returns>
        // GET: api/BillofPurchaseDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillofPurchaseDetail>>> GetBillofPurchaseDetails()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.BillofPurchaseDetails.AsQueryable().ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 用ID查進貨單明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/BillofPurchaseDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BillofPurchaseDetail>> GetBillofPurchaseDetail(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var billofPurchaseDetail = await _context.BillofPurchaseDetails.FindAsync(id);

            if (billofPurchaseDetail == null)
            {
                return NotFound();
            }
            //return billofPurchaseDetail;
            return Ok(MyFun.APIResponseOK(billofPurchaseDetail));
        }
        /// <summary>
        /// 用父ID查進貨單明細
        /// </summary>
        /// <param name="Pid">父ID</param>
        /// <returns></returns>
        public async Task<ActionResult<BillofPurchaseDetail>> GetBillofPurchaseDetailByPId(int Pid)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.BillofPurchaseDetails.AsQueryable().Where(x => x.BillofPurchaseId == Pid && x.DeleteFlag == 0).Include(x => x.Purchase).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 修改進貨單明細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="billofPurchaseDetail"></param>
        /// <returns></returns>
        // PUT: api/BillofPurchaseDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillofPurchaseDetail(int id, BillofPurchaseDetail billofPurchaseDetail)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            billofPurchaseDetail.Id = id;
            var OldBillofPurchaseDetail = _context.BillofPurchaseDetails.Include(x => x.PurchaseDetail).Where(x => x.Id == id).FirstOrDefault();
            // if (billofPurchaseDetail.DataId != 0 && billofPurchaseDetail.DataId != OldBillofPurchaseDetail.DataId)
            // {
            //     var Material = _context.Materials.Find(billofPurchaseDetail.DataId);
            //     billofPurchaseDetail.DataName = Material.Name;
            //     billofPurchaseDetail.DataNo = Material.MaterialNo;
            //     billofPurchaseDetail.Specification = Material.Specification;
            // }

            if (billofPurchaseDetail.Quantity != 0)
            {
                var value = OldBillofPurchaseDetail.Quantity - billofPurchaseDetail.Quantity;
                OldBillofPurchaseDetail.PurchaseDetail.PurchaseCount -= value;
            }

            var Msg = MyFun.MappingData(ref OldBillofPurchaseDetail, billofPurchaseDetail);

            OldBillofPurchaseDetail.UpdateTime = DateTime.Now;
            OldBillofPurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
            OldBillofPurchaseDetail.PriceAll = OldBillofPurchaseDetail.Quantity * OldBillofPurchaseDetail.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillofPurchaseDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(billofPurchaseDetail));
        }
        /// <summary>
        /// 新增進貨單明細
        /// </summary>
        /// <param name="Pid">進貨單ID</param>
        /// <param name="billofPurchaseDetail"></param>
        /// <returns></returns>
        // POST: api/BillofPurchaseDetails
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseDetail>> PostBillofPurchaseDetail(int Pid, BillofPurchaseDetail billofPurchaseDetail)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.BillofPurchaseDetails.Add(billofPurchaseDetail);
            var Material = _context.Materials.Find(billofPurchaseDetail.DataId);
            billofPurchaseDetail.DataName = Material.Name;
            billofPurchaseDetail.DataNo = Material.MaterialNo;
            billofPurchaseDetail.Specification = Material.Specification;
            billofPurchaseDetail.CreateTime = DateTime.Now;
            billofPurchaseDetail.CreateUser = MyFun.GetUserID(HttpContext);
            var BillofPurchaseHead = _context.BillofPurchaseHeads.Find(Pid);
            if (BillofPurchaseHead == null)
            {
                return Ok(MyFun.APIResponseError("進貨主檔有錯誤"));
            }
            else
            {
                BillofPurchaseHead.BillofPurchaseDetails.Add(billofPurchaseDetail);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(MyFun.APIResponseError(ex.InnerException.Message));
            }

            return Ok(MyFun.APIResponseOK(billofPurchaseDetail));
        }
        /// <summary>
        /// 刪除進貨單明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/BillofPurchaseDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BillofPurchaseDetail>> DeleteBillofPurchaseDetail(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var billofPurchaseDetail = await _context.BillofPurchaseDetails.FindAsync(id);
            if (billofPurchaseDetail == null)
            {
                return NotFound();
            }
            billofPurchaseDetail.DeleteFlag = 1;
            billofPurchaseDetail.PurchaseDetail.PurchaseCount -= billofPurchaseDetail.Quantity;
            // _context.BillofPurchaseDetails.Remove(billofPurchaseDetail);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料

            return Ok(MyFun.APIResponseOK("OK"));
        }

        private bool BillofPurchaseDetailExists(int id)
        {
            return _context.BillofPurchaseDetails.Any(e => e.Id == id);
        }
    }
}
