using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonjiMES.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToPurchaseController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ToPurchaseController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 採購單轉進貨
        /// </summary>
        /// <param name="ToPurchase"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PurchaseHead>> BillToPurchase(ToPurchase ToPurchase)
        {
            var BillofPurchaseDetails = await _context.BillofPurchaseDetails.Where(x => ToPurchase.BillofPurchaseDetail.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            return Ok(MyFun.APIResponseOK("OK"));
        }
        /// <summary>
        /// 查詢採購單，供應商的採購品項
        /// </summary>
        /// <param name="Id">供應商ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseHead>> GetCanPurchase(int Id)
        {
            var BillofPurchaseDetails = await _context.BillofPurchaseDetails.Where(x => x.SupplierId == Id && x.Quantity > x.PurchaseCount).ToListAsync();
            return Ok(MyFun.APIResponseOK(BillofPurchaseDetails));
        }
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostPurchaseMaster_Detail(PostPurchaseMaster_Detail PostPurchaseMaster_Detail)
        {
            try
            {
                var dt = DateTime.Now;
                var PurchaseNo = dt.ToString("yyMMdd");
                var NoCount = _context.OrderHeads.Where(x => x.OrderNo.StartsWith(PurchaseNo)).Count() + 1;
                var purchaseHead = PostPurchaseMaster_Detail.PurchaseHead;
                var purchaseDetail = PostPurchaseMaster_Detail.PurchaseDetails;
                var DirName = purchaseHead.PurchaseNo;
                var key = purchaseHead.Type == 10 ? "PI" : "PO";
                purchaseHead.PurchaseNo = key + PurchaseNo + NoCount.ToString("000");
                purchaseHead.CreateTime = dt;
                purchaseHead.CreateUser = 1;
                var PurchaseDetail = new List<PurchaseDetail>();
                foreach (var item in purchaseDetail)
                {
                    var BillP = _context.BillofPurchaseDetails.Find(item.DataId);
                    item.DataNo = BillP.DataNo;
                    item.DataName = BillP.DataName;
                    item.Specification = BillP.Specification;
                    item.CreateTime = dt;
                    item.CreateUser = 1;
                    PurchaseDetail.Add(item);
                }
                purchaseHead.PurchaseDetails = PurchaseDetail.ToList();
                _context.PurchaseHeads.Add(purchaseHead);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(purchaseHead.PurchaseNo));
                //return Ok(new { success = true, timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "", data = true});
                //return CreatedAtAction("GetOrderHead", new { id = PostOrderMaster_Detail.OrderHead.Id }, PostOrderMaster_Detail.OrderHead);
            }
            catch (Exception ex)
            {

                return Ok(MyFun.APIResponseError(ex.Message));
            }
        }
    }
}