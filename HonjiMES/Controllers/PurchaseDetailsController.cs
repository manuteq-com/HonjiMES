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
    public class PurchaseDetailsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public PurchaseDetailsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 採購明細
        /// </summary>
        /// <returns></returns>
        // GET: api/PurchaseDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDetail>>> GetPurchaseDetails()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.PurchaseDetails.AsQueryable().ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 用ID取採購明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/PurchaseDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDetail>> GetPurchaseDetail(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var purchaseDetail = await _context.PurchaseDetails.FindAsync(id);

            if (purchaseDetail == null)
            {
                return NotFound();
            }

            //return purchaseDetail;
            return Ok(MyFun.APIResponseOK(purchaseDetail));
        }
        /// <summary>
        /// 用父ID查採購單明細
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseDetail>>> GetPurchaseDetailByPId(int Pid)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.PurchaseDetails.AsQueryable().Where(x => x.PurchaseId == Pid && x.DeleteFlag == 0).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        // GET: api/MaterialBasics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialBasic>> GetMaterialBasicsByPurchase(int id)
        {
            // MaterialBasic materialBasic = new MaterialBasic();
            var materialBasic = await _context.PurchaseDetails.AsQueryable()
            .Where(x => x.DeleteFlag == 0 && x.PurchaseId == id)
            .Join(_context.MaterialBasics, x => x.DataId, materialBasic => materialBasic.Id, (x, materialBasic) => new {
                Id = materialBasic.Id,
                MaterialNo = materialBasic.MaterialNo,
                MaterialName = x.DataName,
                Specification = x.Specification,
                Quantity = x.Quantity - x.PurchaseCount,
                OriginPrice = x.OriginPrice,
                Price = x.OriginPrice * (x.Quantity - x.PurchaseCount),
                WarehouseId = x.WarehouseId
            })
            .ToListAsync();

            if (materialBasic == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        /// <summary>
        /// 修改採購明細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchaseDetail"></param>
        /// <returns></returns>
            // PUT: api/PurchaseDetails/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for
            // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseDetail(int id, PurchaseDetail purchaseDetail)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            purchaseDetail.Id = id;
            var OldPurchaseDetail = _context.PurchaseDetails.Find(id);
            var Msg = MyFun.MappingData(ref OldPurchaseDetail, purchaseDetail);

            OldPurchaseDetail.UpdateTime = DateTime.Now;
            OldPurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
            OldPurchaseDetail.Price = OldPurchaseDetail.Quantity * OldPurchaseDetail.OriginPrice;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(purchaseDetail));

        }
        /// <summary>
        /// 新增採購明細
        /// </summary>
        /// <param name="purchaseDetail"></param>
        /// <returns></returns>
        // POST: api/PurchaseDetails
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PurchaseDetail>> PostPurchaseDetail(PurchaseDetail purchaseDetail)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            _context.PurchaseDetails.Add(purchaseDetail);
            purchaseDetail.CreateTime = DateTime.Now;
            purchaseDetail. CreateUser = MyFun.GetUserID(HttpContext);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(purchaseDetail));
        }
        /// <summary>
        /// 刪除採購明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/PurchaseDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PurchaseDetail>> DeletePurchaseDetail(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var purchaseDetail = await _context.PurchaseDetails.FindAsync(id);
            if (purchaseDetail == null)
            {
                return NotFound();
            }
            purchaseDetail.DeleteFlag = 1;
            // _context.PurchaseDetails.Remove(purchaseDetail);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(purchaseDetail));
        }

        private bool PurchaseDetailExists(int id)
        {
            return _context.PurchaseDetails.Any(e => e.Id == id);
        }
    }
}
