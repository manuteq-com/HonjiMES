using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 採購API
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToPurchaseController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ToPurchaseController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 採購單轉進貨
        /// </summary>
        /// <param name="ToPurchase"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PurchaseHead>> BillToPurchase(ToPurchase ToPurchase)
        {
            var BillofPurchaseDetails = await _context.BillofPurchaseDetails.AsQueryable().Where(x => ToPurchase.BillofPurchaseDetail.Select(y => y.Id).Contains(x.Id)).ToListAsync();
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
            var BillofPurchaseDetails = await _context.BillofPurchaseDetails.AsQueryable().Where(x => x.SupplierId == Id && x.Quantity > x.PurchaseCount).ToListAsync();
            return Ok(MyFun.APIResponseOK(BillofPurchaseDetails));
        }

        /// <summary>
        /// 取得驗退單號
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<BillofPurchaseReturn>> GetBillofPurchaseReturnNo()
        {
            var ReturnNoName = "BOPR";
            var NoData = await _context.BillofPurchaseReturns.AsQueryable().Where(x => x.DeleteFlag == 0 && x.ReturnNo.Contains(ReturnNoName)).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastReturnNo = NoData.FirstOrDefault().ReturnNo;
                var LastLength = LastReturnNo.Length - ReturnNoName.Length;
                var NoLast = Int32.Parse(LastReturnNo.Substring(LastReturnNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var ReturnData = new BillofPurchaseReturn{
                ReturnNo = ReturnNoName + NoCount.ToString("000000")
            };
            return Ok(MyFun.APIResponseOK(ReturnData));
        }

        /// <summary>
        /// 新增採購單，同時新增明細
        /// </summary>
        /// <param name="PostPurchaseMaster_Detail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PurchaseHead>> PostPurchaseMaster_Detail(PostPurchaseMaster_Detail PostPurchaseMaster_Detail)
        {
            try
            {
                var purchaseHead = PostPurchaseMaster_Detail.PurchaseHead;
                var purchaseDetail = PostPurchaseMaster_Detail.PurchaseDetails;
                var DirName = purchaseHead.PurchaseNo;
                var key = purchaseHead.Type == 10 ? "PI" : "PO";

                var dt = DateTime.Now;
                // var PurchaseNo = dt.ToString("yyMMdd");
                // var NoData = _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(key + PurchaseNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                // var NoCount = NoData.Count() + 1;
                // if (NoCount != 1) {
                //     var LastPurchaseNo = NoData.FirstOrDefault().PurchaseNo;
                //     var NoLast = Int32.Parse(LastPurchaseNo.Substring(LastPurchaseNo.Length - 3, 3));
                //     if (NoCount <= NoLast) {
                //         NoCount = NoLast + 1;
                //     }
                // }
                // var PurchaseNumber = key + PurchaseNo + NoCount.ToString("000");
                var PurchaseNumber = purchaseHead.PurchaseNo;
                
                if (purchaseHead.SupplierId == 0) {
                    return Ok(MyFun.APIResponseError("請選擇供應商!"));
                }
                var checkPurchaseNo = _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(PurchaseNumber) && x.DeleteFlag == 0).Count();
                if (checkPurchaseNo != 0) {
                    return Ok(MyFun.APIResponseError("[採購單號]已存在! 請重新確認!"));
                }

                purchaseHead.PurchaseNo = PurchaseNumber;
                purchaseHead.CreateTime = dt;
                purchaseHead.CreateUser = 1;
                var PurchaseDetail = new List<PurchaseDetail>();
                foreach (var item in purchaseDetail)
                {
                    var BillP = _context.BillofPurchaseDetails.Find(item.DataId);
                    if (BillP != null)
                    {
                        item.DataNo = BillP.DataNo;
                        item.DataName = BillP.DataName;
                        item.Specification = BillP.Specification;
                        item.CreateTime = dt;
                        item.CreateUser = 1;
                    }
                    else
                    {
                        var MaterialBasicData = _context.MaterialBasics.Find(item.DataId);
                        item.DataNo = MaterialBasicData.MaterialNo;
                        item.DataName = MaterialBasicData.Name;
                        item.Specification = MaterialBasicData.Specification;
                        item.CreateTime = dt;
                        item.CreateUser = 1;
                    }
                    item.PurchaseType = purchaseHead.Type;
                    item.SupplierId = purchaseHead.SupplierId;
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

        /// <summary>
        /// 進貨單驗收
        /// </summary>
        /// <param name="BillofPurchaseCheckin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseCheckin>> PostPurchaseCheckIn(BillofPurchaseCheckin BillofPurchaseCheckin)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var BillofPurchaseDetails = _context.BillofPurchaseDetails.Find(BillofPurchaseCheckin.BillofPurchaseDetailId);
            if (BillofPurchaseDetails == null)
            {
                return Ok(MyFun.APIResponseError("進貨單資料有誤!"));
            }
            BillofPurchaseDetails.BillofPurchaseCheckins.Add(BillofPurchaseCheckin);
            if (BillofPurchaseDetails.BillofPurchaseCheckins.Sum(x => x.Quantity) > BillofPurchaseDetails.Quantity)
            {
                return Ok(MyFun.APIResponseError("驗收數量超過採購數量!"));
            }
            var dt = DateTime.Now;
            BillofPurchaseCheckin.CreateTime = dt;
            BillofPurchaseCheckin.CreateUser = 1;
            BillofPurchaseDetails.UpdateTime = dt;
            BillofPurchaseDetails.UpdateUser = 1;
            BillofPurchaseDetails.CheckStatus = 1;
            BillofPurchaseDetails.CheckCountIn = BillofPurchaseDetails.BillofPurchaseCheckins.Sum(x => x.Quantity);
            BillofPurchaseDetails.CheckPriceIn = BillofPurchaseDetails.CheckCountIn * BillofPurchaseDetails.OriginPrice;

            //更新採購單明細資訊
            var PurchaseDetail = _context.PurchaseDetails.Find(BillofPurchaseDetails.PurchaseDetailId);
            if (PurchaseDetail == null)
            {
                return Ok(MyFun.APIResponseError("採購單明細資料有誤!"));
            }
            var tempPurchaseCount = PurchaseDetail.PurchaseCount;
            PurchaseDetail.PurchaseCount += BillofPurchaseCheckin.Quantity;
            PurchaseDetail.UpdateTime = dt;
            PurchaseDetail.UpdateUser = 1;
            if (PurchaseDetail.Quantity < PurchaseDetail.PurchaseCount) {
                return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " ] *實際採購數量：" + PurchaseDetail.Quantity + "  *已交貨數量：" + tempPurchaseCount));
            }
            
            //入庫
            var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
            if (Material == null)
            {
                return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
            }
            Material.MaterialLogs.Add(new MaterialLog { Original = Material.Quantity, Quantity = BillofPurchaseCheckin.Quantity, Message = "進貨檢驗入庫" });
            Material.Quantity += BillofPurchaseCheckin.Quantity;

            //檢查進貨單明細是否都完成進貨
            var CheckBillofPurchaseHeadStatus = true;
            var BillofPurchaseDetailData = _context.BillofPurchaseDetails.Find(BillofPurchaseCheckin.BillofPurchaseDetailId);
            var BillofPurchaseHeadData = _context.BillofPurchaseHeads.Find(BillofPurchaseDetailData.BillofPurchaseId);
            foreach (var Detailitem in BillofPurchaseHeadData.BillofPurchaseDetails)
            {
                if (Detailitem.CheckStatus != 1)
                {
                    CheckBillofPurchaseHeadStatus = false;
                }
            }
            if (CheckBillofPurchaseHeadStatus)
            {
                BillofPurchaseHeadData.Status = 1;
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(BillofPurchaseCheckin));
        }

        /// <summary>
        /// 進貨單驗收
        /// </summary>
        /// <param name="BillofPurchaseReturn"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseReturn>> PostPurchaseCheckReturn(BillofPurchaseReturn BillofPurchaseReturn)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (BillofPurchaseReturn.WarehouseId == 0)
            {
                return Ok(MyFun.APIResponseError("請選擇驗退倉!"));
            }
            var BillofPurchaseDetails = _context.BillofPurchaseDetails.Find(BillofPurchaseReturn.BillofPurchaseDetailId);
            if (BillofPurchaseDetails == null)
            {
                return Ok(MyFun.APIResponseError("進貨單資料有誤!"));
            }
            BillofPurchaseDetails.BillofPurchaseReturns.Add(BillofPurchaseReturn);
            var ReturnCount = BillofPurchaseDetails.BillofPurchaseReturns.Sum(x => x.Quantity);
            if (ReturnCount > BillofPurchaseDetails.Quantity)
            {
                return Ok(MyFun.APIResponseError("驗退數量超過採購數量! [已驗退數量：" + (ReturnCount - BillofPurchaseReturn.Quantity) + "]"));
            }
            
            var dt = DateTime.Now;
            BillofPurchaseReturn.CreateTime = dt;
            BillofPurchaseReturn.CreateUser = 1;
            BillofPurchaseDetails.UpdateTime = dt;
            BillofPurchaseDetails.UpdateUser = 1;
            BillofPurchaseDetails.CheckCountOut = (int)BillofPurchaseDetails.BillofPurchaseReturns.Sum(x => x.Quantity);
            BillofPurchaseDetails.CheckPriceOut = BillofPurchaseDetails.CheckCountOut * BillofPurchaseDetails.OriginPrice;

            //更新採購單明細資訊
            var PurchaseDetail = _context.PurchaseDetails.Find(BillofPurchaseDetails.PurchaseDetailId);
            if (PurchaseDetail == null) 
            {
                return Ok(MyFun.APIResponseError("採購單明細資料有誤!"));
            }
            if (PurchaseDetail.PurchaseCount < (int)BillofPurchaseReturn.Quantity) 
            {
                return Ok(MyFun.APIResponseError("驗退數量超過驗收數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " 實際已驗收數量： " + PurchaseDetail.PurchaseCount + " ]"));
            }
            PurchaseDetail.PurchaseCount = PurchaseDetail.PurchaseCount - (int)BillofPurchaseReturn.Quantity;
            PurchaseDetail.UpdateTime = dt;
            PurchaseDetail.UpdateUser = 1;
            
            //出庫(驗退)
            var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
            if (Material == null)
            {
                return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
            }
            Material.MaterialLogs.Add(new MaterialLog { Original = Material.Quantity, Quantity = (int)BillofPurchaseReturn.Quantity, Message = "進貨驗退出庫" });
            Material.Quantity -= (int)BillofPurchaseReturn.Quantity;

            //檢查進貨單明細是否都完成進貨(此為驗退，訂單狀態需修改?)
            // var CheckBillofPurchaseHeadStatus = true;
            // var BillofPurchaseDetailData = _context.BillofPurchaseDetails.Find(BillofPurchaseReturn.BillofPurchaseDetailId);
            // var BillofPurchaseHeadData = _context.BillofPurchaseHeads.Find(BillofPurchaseDetailData.BillofPurchaseId);
            // foreach (var Detailitem in BillofPurchaseHeadData.BillofPurchaseDetails)
            // {
            //     if (Detailitem.CheckStatus != 1)
            //     {
            //         CheckBillofPurchaseHeadStatus = false;
            //     }
            // }
            // if (CheckBillofPurchaseHeadStatus)
            // {
            //     BillofPurchaseHeadData.Status = 1;
            // }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(BillofPurchaseReturn));
        }

        /// <summary>
        /// 進貨單可驗收的數量
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BillofPurchaseDetail>> CanCheckIn(int Id)
        {
            var BillofPurchaseDetail = await _context.BillofPurchaseDetails.FindAsync(Id);
            return Ok(MyFun.APIResponseOK(BillofPurchaseDetail));
        }
    }
}