using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Filter;
namespace HonjiMES.Controllers
{
    /// <summary>
    /// 採購API
    /// </summary>
    [JWTAuthorize]
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
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var purchaseHead = PostPurchaseMaster_Detail.PurchaseHead;
                var purchaseDetail = PostPurchaseMaster_Detail.PurchaseDetails;
                var DirName = purchaseHead.PurchaseNo;
                var key = purchaseHead.Type == 10 ? "PI" : "PO";
                var dt = DateTime.Now;

                purchaseHead.PurchaseNo = purchaseHead.PurchaseNo;
                purchaseHead.CreateTime = dt;
                purchaseHead. CreateUser = MyFun.GetUserID(HttpContext);
                var PurchaseDetail = new List<PurchaseDetail>();
                foreach (var item in purchaseDetail)
                {
                    if (item.DataType == 1)
                    {
                        var MaterialBasic = _context.MaterialBasics.Find(item.DataId);
                        item.DataNo = MaterialBasic.MaterialNo;
                        item.DataName = MaterialBasic.Name;
                        item.Specification = MaterialBasic.Specification;
                    }
                    else if (item.DataType == 2)
                    {
                        var ProductBasic = _context.ProductBasics.Find(item.DataId);
                        item.DataNo = ProductBasic.ProductNo;
                        item.DataName = ProductBasic.Name;
                        item.Specification = ProductBasic.Specification;
                    }
                    else if (item.DataType == 3)
                    {
                        var WiproductBasic = _context.WiproductBasics.Find(item.DataId);
                        item.DataNo = WiproductBasic.WiproductNo;
                        item.DataName = WiproductBasic.Name;
                        item.Specification = WiproductBasic.Specification;
                    }
                    item.CreateTime = dt;
                    item.CreateUser = MyFun.GetUserID(HttpContext);
                    item.PurchaseType = purchaseHead.Type;
                    item.SupplierId = purchaseHead.SupplierId;
                    PurchaseDetail.Add(item);
                }

                if (purchaseHead.Id != 0) {
                    var PurchaseHead = _context.PurchaseHeads.Find(purchaseHead.Id);
                    foreach (var Detailitem in PurchaseDetail)
                    {
                        PurchaseHead.PurchaseDetails.Add(Detailitem);
                    }
                    PurchaseHead.PriceAll = PurchaseHead.PurchaseDetails.Sum(x => x.Quantity * x.OriginPrice);
                    await _context.SaveChangesAsync();
                } else {
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
                    
                    if (purchaseHead.SupplierId == 0) {
                        return Ok(MyFun.APIResponseError("請選擇供應商!"));
                    }
                    var checkPurchaseNo = _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(purchaseHead.PurchaseNo) && x.DeleteFlag == 0).Count();
                    if (checkPurchaseNo != 0) {
                        return Ok(MyFun.APIResponseError("[採購單號]已存在! 請重新確認!"));
                    }

                    
                    purchaseHead.PurchaseDetails = PurchaseDetail.ToList();
                    purchaseHead.PriceAll = purchaseHead.PurchaseDetails.Sum(x => x.Quantity * x.OriginPrice);
                    _context.PurchaseHeads.Add(purchaseHead);
                    await _context.SaveChangesAsync();
                    //return Ok(new { success = true, timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "", data = true});
                    //return CreatedAtAction("GetOrderHead", new { id = PostOrderMaster_Detail.OrderHead.Id }, PostOrderMaster_Detail.OrderHead);
                }
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(purchaseHead.PurchaseNo));
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
        public async Task<ActionResult<BillofPurchaseCheckin>> PostPurchaseCheckIn(BillofPurchaseCheckData BillofPurchaseCheckin)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var BillofPurchaseDetails = _context.BillofPurchaseDetails.Find(BillofPurchaseCheckin.BillofPurchaseDetailId);
            if (BillofPurchaseDetails == null)
            {
                return Ok(MyFun.APIResponseError("進貨單資料有誤!"));
            }
            // BillofPurchaseDetails.BillofPurchaseCheckins.Add(BillofPurchaseCheckin);
            BillofPurchaseDetails.BillofPurchaseCheckins.Add(new BillofPurchaseCheckin(){
                BillofPurchaseDetailId = BillofPurchaseCheckin.BillofPurchaseDetailId,
                CheckinType = null,
                Quantity = BillofPurchaseCheckin.Quantity,
                Price = BillofPurchaseCheckin.Price,
                PriceAll = BillofPurchaseCheckin.PriceAll,
                Unit = BillofPurchaseCheckin.Unit,
                UnitCount = BillofPurchaseCheckin.UnitCount,
                UnitPrice = BillofPurchaseCheckin.UnitPrice,
                Remarks = BillofPurchaseCheckin.Remarks
            });
            if (BillofPurchaseDetails.BillofPurchaseCheckins.Sum(x => x.Quantity) > BillofPurchaseDetails.Quantity)
            {
                return Ok(MyFun.APIResponseError("驗收數量超過採購數量!"));
            }
            var dt = DateTime.Now;
            BillofPurchaseCheckin.CreateTime = dt;
            BillofPurchaseCheckin. CreateUser = MyFun.GetUserID(HttpContext);
            BillofPurchaseDetails.UpdateTime = dt;
            BillofPurchaseDetails.UpdateUser = MyFun.GetUserID(HttpContext);
            BillofPurchaseDetails.CheckStatus = 1;
            BillofPurchaseDetails.CheckCountIn = BillofPurchaseDetails.BillofPurchaseCheckins.Sum(x => x.Quantity);
            BillofPurchaseDetails.CheckPriceIn = BillofPurchaseDetails.CheckCountIn * BillofPurchaseDetails.Price;
            
            BillofPurchaseDetails.Quantity = BillofPurchaseCheckin.Quantity;
            BillofPurchaseDetails.Price = (int)BillofPurchaseCheckin.Price;
            BillofPurchaseDetails.PriceAll = BillofPurchaseCheckin.PriceAll;
            BillofPurchaseDetails.Unit = BillofPurchaseCheckin.Unit;
            BillofPurchaseDetails.UnitCount = BillofPurchaseCheckin.UnitCount;
            BillofPurchaseDetails.UnitPrice = BillofPurchaseCheckin.UnitPrice;
            BillofPurchaseDetails.UnitPriceAll = BillofPurchaseCheckin.UnitPriceAll;
            BillofPurchaseDetails.WorkPrice = BillofPurchaseCheckin.WorkPrice;
            BillofPurchaseDetails.Remarks = BillofPurchaseCheckin.Remarks;

            //更新採購單明細資訊
            var PurchaseDetail = _context.PurchaseDetails.Find(BillofPurchaseDetails.PurchaseDetailId);
            if (PurchaseDetail == null)
            {
                return Ok(MyFun.APIResponseError("採購單明細資料有誤!"));
            }
            var tempPurchaseCount = PurchaseDetail.PurchaseCount;
            PurchaseDetail.PurchaseCount += BillofPurchaseCheckin.Quantity;
            PurchaseDetail.UpdateTime = dt;
            PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
            if (PurchaseDetail.Quantity < PurchaseDetail.PurchaseCount) {
                return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " ] *實際採購數量：" + PurchaseDetail.Quantity + "  *已交貨數量：" + tempPurchaseCount));
            }
            
            //檢查該採購單是否完成
            var PurchaseHead = _context.PurchaseHeads.Find(PurchaseDetail.PurchaseId);
            if (PurchaseHead == null)
            {
                return Ok(MyFun.APIResponseError("採購單主檔資料有誤!"));
            }
            var CheckPurchaseHeadStatus = true;
            foreach (var item2 in PurchaseHead.PurchaseDetails)
            {
                if (item2.Quantity != item2.PurchaseCount) {
                    CheckPurchaseHeadStatus = false;
                }
            }
            if (CheckPurchaseHeadStatus)
            {
                PurchaseHead.Status = 2;
            }

            //入庫
            if (BillofPurchaseDetails.DataType == 1) {
                var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Material == null)
                {
                    return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
                }
                Material.MaterialLogs.Add(new MaterialLog { 
                    LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                    Original = Material.Quantity,
                    Quantity = BillofPurchaseCheckin.Quantity,
                    Price = BillofPurchaseCheckin.Price,
                    PriceAll = BillofPurchaseCheckin.PriceAll,
                    Unit = BillofPurchaseCheckin.Unit,
                    UnitCount = BillofPurchaseCheckin.UnitCount,
                    UnitPrice = BillofPurchaseCheckin.UnitPrice,
                    UnitPriceAll = BillofPurchaseCheckin.UnitPriceAll,
                    WorkPrice = BillofPurchaseCheckin.WorkPrice,
                    Reason = BillofPurchaseCheckin.Remarks,
                    Message = "進貨檢驗入庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Material.Quantity += BillofPurchaseCheckin.Quantity;
            } else if (BillofPurchaseDetails.DataType == 2) {
                var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Product == null)
                {
                    return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                }
                Product.ProductLogs.Add(new ProductLog { 
                    LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                    Original = Product.Quantity,
                    Quantity = BillofPurchaseCheckin.Quantity,
                    Price = BillofPurchaseCheckin.Price,
                    PriceAll = BillofPurchaseCheckin.PriceAll,
                    Unit = BillofPurchaseCheckin.Unit,
                    UnitCount = BillofPurchaseCheckin.UnitCount,
                    UnitPrice = BillofPurchaseCheckin.UnitPrice,
                    UnitPriceAll = BillofPurchaseCheckin.UnitPriceAll,
                    WorkPrice = BillofPurchaseCheckin.WorkPrice,
                    Reason = BillofPurchaseCheckin.Remarks,
                    Message = "進貨檢驗入庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Product.Quantity += BillofPurchaseCheckin.Quantity;
            } else if (BillofPurchaseDetails.DataType == 3) {
                var Wiproduct = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Wiproduct == null)
                {
                    return Ok(MyFun.APIResponseError("半成品庫存資料有誤"));
                }
                Wiproduct.WiproductLogs.Add(new WiproductLog { 
                    LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                    Original = Wiproduct.Quantity,
                    Quantity = BillofPurchaseCheckin.Quantity,
                    Price = BillofPurchaseCheckin.Price,
                    PriceAll = BillofPurchaseCheckin.PriceAll,
                    Unit = BillofPurchaseCheckin.Unit,
                    UnitCount = BillofPurchaseCheckin.UnitCount,
                    UnitPrice = BillofPurchaseCheckin.UnitPrice,
                    UnitPriceAll = BillofPurchaseCheckin.UnitPriceAll,
                    WorkPrice = BillofPurchaseCheckin.WorkPrice,
                    Reason = BillofPurchaseCheckin.Remarks,
                    Message = "進貨檢驗入庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Wiproduct.Quantity += BillofPurchaseCheckin.Quantity;
            }

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
        /// 進貨單批量驗收
        /// </summary>
        /// <param name="BillofPurchaseHead"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseCheckin>> PostPurchaseCheckInArray(BillofPurchaseHead BillofPurchaseHead)
        {
            var dt = DateTime.Now;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var BillofPurchaseHeads = _context.BillofPurchaseHeads.Find(BillofPurchaseHead.Id);
            if (BillofPurchaseHeads == null)
            {
                return Ok(MyFun.APIResponseError("進貨單資料有誤!"));
            }

            var NoUpdataCount = 0;
            foreach (var item in BillofPurchaseHeads.BillofPurchaseDetails)
            {
                if (item.CheckStatus == 0) {
                    item.BillofPurchaseCheckins.Add(new BillofPurchaseCheckin(){
                        BillofPurchaseDetailId = item.Id,
                        CheckinType = null,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        PriceAll = item.PriceAll,
                        Unit = item.Unit,
                        UnitCount = item.UnitCount,
                        UnitPrice = item.UnitPrice,
                        Remarks = item.Remarks,
                        CreateTime = dt,
                         CreateUser = MyFun.GetUserID(HttpContext)
                    });
                    if (item.BillofPurchaseCheckins.Sum(x => x.Quantity) > item.Quantity)
                    {
                        return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + item.DataNo + " ]"));
                    }
                    item.UpdateTime = dt;
                    item.UpdateUser = MyFun.GetUserID(HttpContext);
                    item.CheckStatus = 1;
                    item.CheckCountIn = item.BillofPurchaseCheckins.Sum(x => x.Quantity);
                    item.CheckPriceIn = item.CheckCountIn * item.Price;

                    //更新採購單明細資訊
                    var PurchaseDetail = _context.PurchaseDetails.Find(item.PurchaseDetailId);
                    if (PurchaseDetail == null)
                    {
                        return Ok(MyFun.APIResponseError("採購單明細資料有誤!"));
                    }
                    var tempPurchaseCount = PurchaseDetail.PurchaseCount;
                    PurchaseDetail.PurchaseCount += item.Quantity;
                    PurchaseDetail.UpdateTime = dt;
                    PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
                    if (PurchaseDetail.Quantity < PurchaseDetail.PurchaseCount) {
                        return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " ] *實際採購數量：" + PurchaseDetail.Quantity + "  *已交貨數量：" + tempPurchaseCount));
                    }

                    //檢查該採購單是否完成
                    var PurchaseHead = _context.PurchaseHeads.Find(PurchaseDetail.PurchaseId);
                    if (PurchaseHead == null)
                    {
                        return Ok(MyFun.APIResponseError("採購單主檔資料有誤!"));
                    }
                    var CheckPurchaseHeadStatus = true;
                    foreach (var item2 in PurchaseHead.PurchaseDetails)
                    {
                        if (item2.Quantity != item2.PurchaseCount) {
                            CheckPurchaseHeadStatus = false;
                        }
                    }
                    if (CheckPurchaseHeadStatus)
                    {
                        PurchaseHead.Status = 2;
                    }

                    //入庫
                    if (item.DataType == 1) {
                        var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Material == null)
                        {
                            return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
                        }
                        Material.MaterialLogs.Add(new MaterialLog { 
                            LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                            Original = Material.Quantity,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            PriceAll = item.PriceAll,
                            Unit = item.Unit,
                            UnitCount = item.UnitCount,
                            UnitPrice = item.UnitPrice,
                            UnitPriceAll = item.UnitPriceAll,
                            WorkPrice = item.WorkPrice,
                            Reason = item.Remarks,
                            Message = "批量驗收入庫",
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Material.Quantity += item.Quantity;
                    } else if (item.DataType == 2) {
                        var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Product == null)
                        {
                            return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                        }
                        Product.ProductLogs.Add(new ProductLog { 
                            LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                            Original = Product.Quantity,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            PriceAll = item.PriceAll,
                            Unit = item.Unit,
                            UnitCount = item.UnitCount,
                            UnitPrice = item.UnitPrice,
                            UnitPriceAll = item.UnitPriceAll,
                            WorkPrice = item.WorkPrice,
                            Reason = item.Remarks,
                            Message = "批量驗收入庫",
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Product.Quantity += item.Quantity;
                    } else if (item.DataType == 3) {
                        var Wiproduct = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Wiproduct == null)
                        {
                            return Ok(MyFun.APIResponseError("半成品庫存資料有誤"));
                        }
                        Wiproduct.WiproductLogs.Add(new WiproductLog { 
                            LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                            Original = Wiproduct.Quantity,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            PriceAll = item.PriceAll,
                            Unit = item.Unit,
                            UnitCount = item.UnitCount,
                            UnitPrice = item.UnitPrice,
                            UnitPriceAll = item.UnitPriceAll,
                            WorkPrice = item.WorkPrice,
                            Reason = item.Remarks,
                            Message = "批量驗收入庫",
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Wiproduct.Quantity += item.Quantity;
                    }
                 
                } else {
                    NoUpdataCount++;
                }
            }

            if (NoUpdataCount == BillofPurchaseHeads.BillofPurchaseDetails.Count()) {
                return Ok(MyFun.APIResponseOK("OK", "項目皆已驗收完畢!"));
            }
            
            //檢查進貨單明細是否都完成進貨
            var CheckBillofPurchaseHeadStatus = true;
            foreach (var item in BillofPurchaseHeads.BillofPurchaseDetails)
            {
                if (item.CheckStatus == 0) {
                    CheckBillofPurchaseHeadStatus = false;
                }
            }
            if (CheckBillofPurchaseHeadStatus)
            {
                BillofPurchaseHeads.Status = 1;
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK("OK", "驗收完成!"));
        }

        /// <summary>
        /// 進貨單驗收
        /// </summary>
        /// <param name="BillofPurchaseReturn"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BillofPurchaseReturn>> PostPurchaseCheckReturn(BillofPurchaseCheckData BillofPurchaseReturn)
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
            // BillofPurchaseDetails.BillofPurchaseReturns.Add(BillofPurchaseReturn);
            BillofPurchaseDetails.BillofPurchaseReturns.Add(new BillofPurchaseReturn(){
                ReturnNo = BillofPurchaseReturn.ReturnNo,
                BillofPurchaseDetailId = BillofPurchaseReturn.BillofPurchaseDetailId,
                WarehouseId = BillofPurchaseReturn.WarehouseId,
                Quantity = BillofPurchaseReturn.Quantity,
                Price = BillofPurchaseReturn.Price,
                PriceAll = BillofPurchaseReturn.PriceAll,
                Unit = BillofPurchaseReturn.Unit,
                UnitCount = BillofPurchaseReturn.UnitCount,
                UnitPrice = BillofPurchaseReturn.UnitPrice,
                Reason = BillofPurchaseReturn.Reason,
                Remarks = BillofPurchaseReturn.Remarks
            });
            var ReturnCount = BillofPurchaseDetails.BillofPurchaseReturns.Sum(x => x.Quantity);
            if (ReturnCount > BillofPurchaseDetails.Quantity)
            {
                return Ok(MyFun.APIResponseError("驗退數量超過採購數量! [已驗退數量：" + (ReturnCount - BillofPurchaseReturn.Quantity) + "]"));
            }
            
            var dt = DateTime.Now;
            BillofPurchaseReturn.CreateTime = dt;
            BillofPurchaseReturn. CreateUser = MyFun.GetUserID(HttpContext);
            BillofPurchaseDetails.UpdateTime = dt;
            BillofPurchaseDetails.UpdateUser = MyFun.GetUserID(HttpContext);
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
            PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
            
            //出庫(驗退)
            if (BillofPurchaseDetails.DataType == 1) {
                var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Material == null)
                {
                    return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
                }
                Material.MaterialLogs.Add(new MaterialLog { 
                    LinkOrder = BillofPurchaseReturn.ReturnNo,
                    Original = Material.Quantity,
                    Quantity = (int)BillofPurchaseReturn.Quantity,
                    Price = BillofPurchaseReturn.Price,
                    PriceAll = BillofPurchaseReturn.PriceAll,
                    Unit = BillofPurchaseReturn.Unit,
                    UnitCount = BillofPurchaseReturn.UnitCount,
                    UnitPrice = BillofPurchaseReturn.UnitPrice,
                    UnitPriceAll = BillofPurchaseReturn.UnitPriceAll,
                    WorkPrice = BillofPurchaseReturn.WorkPrice,
                    Reason = BillofPurchaseReturn.Reason,
                    Message = "進貨驗退出庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Material.Quantity -= (int)BillofPurchaseReturn.Quantity;
            } else if (BillofPurchaseDetails.DataType == 2) {
                var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Product == null)
                {
                    return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                }
                Product.ProductLogs.Add(new ProductLog { 
                    LinkOrder = BillofPurchaseReturn.ReturnNo,
                    Original = Product.Quantity,
                    Quantity = (int)BillofPurchaseReturn.Quantity,
                    Price = BillofPurchaseReturn.Price,
                    PriceAll = BillofPurchaseReturn.PriceAll,
                    Unit = BillofPurchaseReturn.Unit,
                    UnitCount = BillofPurchaseReturn.UnitCount,
                    UnitPrice = BillofPurchaseReturn.UnitPrice,
                    UnitPriceAll = BillofPurchaseReturn.UnitPriceAll,
                    WorkPrice = BillofPurchaseReturn.WorkPrice,
                    Reason = BillofPurchaseReturn.Reason,
                    Message = "進貨驗退出庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Product.Quantity -= (int)BillofPurchaseReturn.Quantity;
            } else if (BillofPurchaseDetails.DataType == 3) {
                var Wiproduct = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Wiproduct == null)
                {
                    return Ok(MyFun.APIResponseError("半成品庫存資料有誤"));
                }
                Wiproduct.WiproductLogs.Add(new WiproductLog { 
                    LinkOrder = BillofPurchaseReturn.ReturnNo,
                    Original = Wiproduct.Quantity,
                    Quantity = (int)BillofPurchaseReturn.Quantity,
                    Price = BillofPurchaseReturn.Price,
                    PriceAll = BillofPurchaseReturn.PriceAll,
                    Unit = BillofPurchaseReturn.Unit,
                    UnitCount = BillofPurchaseReturn.UnitCount,
                    UnitPrice = BillofPurchaseReturn.UnitPrice,
                    UnitPriceAll = BillofPurchaseReturn.UnitPriceAll,
                    WorkPrice = BillofPurchaseReturn.WorkPrice,
                    Reason = BillofPurchaseReturn.Reason,
                    Message = "進貨驗退出庫",
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                Wiproduct.Quantity -= (int)BillofPurchaseReturn.Quantity;
            }

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