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
            if (NoCount != 1)
            {
                var LastReturnNo = NoData.FirstOrDefault().ReturnNo;
                var LastLength = LastReturnNo.Length - ReturnNoName.Length;
                var NoLast = Int32.Parse(LastReturnNo.Substring(LastReturnNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            var ReturnData = new BillofPurchaseReturn
            {
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
                // var key = CreateNoData.Type == 10 ? "PI" : CreateNoData.Type == 20 ? "PO" : "PS";
                var key = "PI"; // 採購單號開頭一致，只用種類區分。 2020/09/11
                var dt = DateTime.Now;

                purchaseHead.PurchaseNo = purchaseHead.PurchaseNo;
                purchaseHead.CreateTime = dt;
                purchaseHead.CreateUser = MyFun.GetUserID(HttpContext);
                var PurchaseDetail = new List<PurchaseDetail>();
                foreach (var item in purchaseDetail)
                {
                    var Warehouse201Check = 0;
                    decimal Warehouse201Stock = 0;
                    if (item.DataType == 1)
                    {
                        var MaterialBasic = _context.MaterialBasics.Find(item.DataId);
                        item.DataNo = MaterialBasic.MaterialNo;
                        item.DataName = MaterialBasic.Name;
                        item.Specification = MaterialBasic.Specification;
                        var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        if (Warehouse201.Count() != 0)
                        {
                            Warehouse201Check = Warehouse201.Count();
                            Warehouse201Stock = MaterialBasic.Materials.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).First().Quantity;
                        }
                    }
                    else if (item.DataType == 2)
                    {
                        var ProductBasic = _context.ProductBasics.Find(item.DataId);
                        item.DataNo = ProductBasic.ProductNo;
                        item.DataName = ProductBasic.Name;
                        item.Specification = ProductBasic.Specification;
                        var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        if (Warehouse201.Count() != 0)
                        {
                            Warehouse201Check = Warehouse201.Count();
                            Warehouse201Stock = ProductBasic.Products.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).First().Quantity;
                        }
                    }
                    else if (item.DataType == 3)
                    {
                        var WiproductBasic = _context.WiproductBasics.Find(item.DataId);
                        item.DataNo = WiproductBasic.WiproductNo;
                        item.DataName = WiproductBasic.Name;
                        item.Specification = WiproductBasic.Specification;
                        var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        if (Warehouse201.Count() != 0)
                        {
                            Warehouse201Check = Warehouse201.Count();
                            Warehouse201Stock = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == item.WarehouseIdA && x.DeleteFlag == 0).First().Quantity;
                        }
                    }
                    item.CreateTime = dt;
                    item.CreateUser = MyFun.GetUserID(HttpContext);
                    item.PurchaseType = purchaseHead.Type;
                    item.SupplierId = purchaseHead.SupplierId;
                    PurchaseDetail.Add(item);

                    // 如採購單種類為[表處]，需要進行轉倉的動作 2020/09/09
                    if (purchaseHead.Type == 30)
                    {
                        var result = Warehouse201Fun(item, purchaseHead.PurchaseNo, Warehouse201Check, Warehouse201Stock);
                        if (!result.Result.success)
                        {
                            return Ok(await result);
                        }
                    }
                }

                if (purchaseHead.Id != 0) // 合併採購單
                {
                    var PurchaseHead = _context.PurchaseHeads.Find(purchaseHead.Id);
                    foreach (var Detailitem in PurchaseDetail)
                    {
                        var samePurchaseDetail = PurchaseHead.PurchaseDetails.Where(x => x.DataType == Detailitem.DataType && x.DataId == Detailitem.DataId && x.DeleteFlag == 0).ToList();
                        if (samePurchaseDetail.Count() != 0)
                        {
                            samePurchaseDetail.FirstOrDefault().Quantity += Detailitem.Quantity;
                            samePurchaseDetail.FirstOrDefault().Price = samePurchaseDetail.FirstOrDefault().Quantity * samePurchaseDetail.FirstOrDefault().OriginPrice;
                            samePurchaseDetail.FirstOrDefault().UpdateUser = MyFun.GetUserID(HttpContext);
                        }
                        else
                        {
                            PurchaseHead.PurchaseDetails.Add(Detailitem);
                        }
                    }
                    PurchaseHead.PriceAll = PurchaseHead.PurchaseDetails.Where(x => x.DeleteFlag == 0).Sum(x => x.Quantity * x.OriginPrice);
                    await _context.SaveChangesAsync();
                }
                else // 新建採購單
                {
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

                    if (purchaseHead.SupplierId == 0)
                    {
                        return Ok(MyFun.APIResponseError("請選擇供應商!"));
                    }
                    var checkPurchaseNo = _context.PurchaseHeads.AsQueryable().Where(x => x.PurchaseNo.Contains(purchaseHead.PurchaseNo) && x.DeleteFlag == 0).Count();
                    if (checkPurchaseNo != 0)
                    {
                        return Ok(MyFun.APIResponseError("[採購單號]已存在! 請重新確認!"));
                    }

                    purchaseHead.PurchaseDetails = PurchaseDetail.ToList();
                    purchaseHead.PriceAll = purchaseHead.PurchaseDetails.Where(x => x.DeleteFlag == 0).Sum(x => x.Quantity * x.OriginPrice);
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

        private async Task<APIResponse> Warehouse201Fun(PurchaseDetailData itemData, string PurchaseNo, int Warehouse201Check, decimal Warehouse201Stock)
        {
            if (Warehouse201Check == 0)
            {
                return MyFun.APIResponseError("品號 [ " + itemData.DataNo + " ] 無庫存資訊(轉出)! 請重新確認!");
            }
            else
            {
                if (itemData.Quantity > Warehouse201Stock)
                {
                    return MyFun.APIResponseError("品號 [ " + itemData.DataNo + " ] 轉出倉別庫存不足( 庫存 " + Warehouse201Stock + " / 需求 " + itemData.Quantity + " )! 請重新確認!");
                }
                else
                {
                    var dt = DateTime.Now;
                    if (itemData.DataType == 1) // 原料
                    {
                        var MaterialBasic = _context.MaterialBasics.Find(itemData.DataId);
                        var Warehouse201 = MaterialBasic.Materials.Where(x => x.WarehouseId == itemData.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = MaterialBasic.Materials.Where(x => x.WarehouseId == itemData.WarehouseIdB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().MaterialLogs.Add(new MaterialLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -itemData.Quantity,
                            Message = "表處轉倉",
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Warehouse201.First().Quantity -= itemData.Quantity;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse201.First().MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = itemData.Quantity,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Warehouse202.First().Quantity += itemData.Quantity;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            MaterialBasic.Materials.Add(new Material
                            {
                                MaterialNo = MaterialBasic.MaterialNo,
                                Name = MaterialBasic.Name,
                                Quantity = itemData.Quantity,
                                Specification = MaterialBasic.Specification,
                                Property = MaterialBasic.Property,
                                Price = MaterialBasic.Price,
                                BaseQuantity = 2,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = itemData.WarehouseIdB,
                                MaterialLogs = {new MaterialLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = itemData.Quantity,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                    }
                    else if (itemData.DataType == 2) // 成品
                    {
                        var ProductBasic = _context.ProductBasics.Find(itemData.DataId);
                        var Warehouse201 = ProductBasic.Products.Where(x => x.WarehouseId == itemData.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = ProductBasic.Products.Where(x => x.WarehouseId == itemData.WarehouseIdB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().ProductLogs.Add(new ProductLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -itemData.Quantity,
                            Message = "表處轉倉",
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Warehouse201.First().Quantity -= itemData.Quantity;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse201.First().ProductLogs.Add(new ProductLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = itemData.Quantity,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Warehouse202.First().Quantity += itemData.Quantity;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            ProductBasic.Products.Add(new Product
                            {
                                ProductNo = ProductBasic.ProductNo,
                                ProductNumber = ProductBasic.ProductNumber,
                                Name = ProductBasic.Name,
                                Quantity = itemData.Quantity,
                                Specification = ProductBasic.Specification,
                                Property = ProductBasic.Property,
                                Price = ProductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = itemData.WarehouseIdB,
                                ProductLogs = {new ProductLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = itemData.Quantity,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                    }
                    else if (itemData.DataType == 3) // 半成品
                    {
                        var WiproductBasic = _context.WiproductBasics.Find(itemData.DataId);
                        var Warehouse201 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == itemData.WarehouseIdA && x.DeleteFlag == 0).ToList();
                        var Warehouse202 = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == itemData.WarehouseIdB && x.DeleteFlag == 0).ToList();

                        Warehouse201.First().WiproductLogs.Add(new WiproductLog
                        {
                            LinkOrder = PurchaseNo,
                            Original = Warehouse201.First().Quantity,
                            Quantity = -itemData.Quantity,
                            Message = "表處轉倉",
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Warehouse201.First().Quantity -= itemData.Quantity;

                        if (Warehouse202.Count() != 0)
                        {
                            Warehouse201.First().WiproductLogs.Add(new WiproductLog
                            {
                                LinkOrder = PurchaseNo,
                                Original = Warehouse202.First().Quantity,
                                Quantity = itemData.Quantity,
                                Message = "表處轉倉",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Warehouse202.First().Quantity += itemData.Quantity;
                        }
                        else // 如無倉別資訊，則自動建立
                        {
                            WiproductBasic.Wiproducts.Add(new Wiproduct
                            {
                                WiproductNo = WiproductBasic.WiproductNo,
                                WiproductNumber = WiproductBasic.WiproductNumber,
                                Name = WiproductBasic.Name,
                                Quantity = itemData.Quantity,
                                Specification = WiproductBasic.Specification,
                                Property = WiproductBasic.Property,
                                Price = WiproductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = itemData.WarehouseIdB,
                                WiproductLogs = {new WiproductLog
                                {
                                    LinkOrder = PurchaseNo,
                                    Original = 0,
                                    Quantity = itemData.Quantity,
                                    Message = "表處轉倉",
                                    CreateTime = dt,
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                    }
                    return MyFun.APIResponseOK(itemData);
                }
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
            BillofPurchaseDetails.BillofPurchaseCheckins.Add(new BillofPurchaseCheckin()
            {
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
            BillofPurchaseCheckin.CreateUser = MyFun.GetUserID(HttpContext);
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
            var tempPurchasedCount = PurchaseDetail.PurchasedCount;
            PurchaseDetail.PurchasedCount += BillofPurchaseCheckin.Quantity;
            PurchaseDetail.UpdateTime = dt;
            PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
            if (PurchaseDetail.Quantity < PurchaseDetail.PurchasedCount)
            {
                return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " ] *實際採購數量：" + PurchaseDetail.Quantity + "  *已交貨數量：" + tempPurchasedCount));
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
                if (item2.Quantity != item2.PurchasedCount)
                {
                    CheckPurchaseHeadStatus = false;
                }
            }
            if (CheckPurchaseHeadStatus)
            {
                PurchaseHead.Status = 1;
            }

            //入庫
            if (BillofPurchaseDetails.DataType == 1) // 原料
            {
                var MaterialBasic = _context.MaterialBasics.Where(x => x.Id == BillofPurchaseDetails.DataId && x.DeleteFlag == 0).FirstOrDefault();
                var Material = MaterialBasic.Materials.Where(x => x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).ToList();
                if (Material.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                {
                    // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                    MaterialBasic.Materials.Add(new Material
                    {
                        MaterialNo = MaterialBasic.MaterialNo,
                        Name = MaterialBasic.Name,
                        Quantity = BillofPurchaseCheckin.Quantity,
                        Specification = MaterialBasic.Specification,
                        Property = MaterialBasic.Property,
                        Price = MaterialBasic.Price,
                        BaseQuantity = 2,
                        CreateTime = dt,
                        CreateUser = MyFun.GetUserID(HttpContext),
                        WarehouseId = BillofPurchaseDetails?.WarehouseId ?? 0, // 注意! 可能會重複建立
                        MaterialLogs = {new MaterialLog
                        {
                            LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                            Original = 0,
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
                        }}
                    });
                }
                else
                {
                    Material.FirstOrDefault().MaterialLogs.Add(new MaterialLog
                    {
                        LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                        Original = Material.FirstOrDefault().Quantity,
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
                    Material.FirstOrDefault().Quantity += BillofPurchaseCheckin.Quantity;
                }
            }
            else if (BillofPurchaseDetails.DataType == 2) // 成品
            {
                var ProductBasic = _context.ProductBasics.Where(x => x.Id == BillofPurchaseDetails.DataId && x.DeleteFlag == 0).FirstOrDefault();
                var Product = ProductBasic.Products.Where(x => x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).ToList();
                if (Product.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                {
                    // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                    ProductBasic.Products.Add(new Product
                    {
                        ProductNo = ProductBasic.ProductNo,
                        ProductNumber = ProductBasic.ProductNumber,
                        Name = ProductBasic.Name,
                        Quantity = BillofPurchaseCheckin.Quantity,
                        Specification = ProductBasic.Specification,
                        Property = ProductBasic.Property,
                        Price = ProductBasic.Price,
                        MaterialRequire = 1,
                        CreateTime = dt,
                        CreateUser = MyFun.GetUserID(HttpContext),
                        WarehouseId = BillofPurchaseDetails?.WarehouseId ?? 0, // 注意! 可能會重複建立
                        ProductLogs = {new ProductLog
                        {
                            LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                            Original = 0,
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
                        }}
                    });
                }
                else
                {
                    Product.FirstOrDefault().ProductLogs.Add(new ProductLog
                    {
                        LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                        Original = Product.FirstOrDefault().Quantity,
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
                    Product.FirstOrDefault().Quantity += BillofPurchaseCheckin.Quantity;
                }
            }
            else if (BillofPurchaseDetails.DataType == 3) // 半成品
            {
                var WiproductBasic = _context.WiproductBasics.Where(x => x.Id == BillofPurchaseDetails.DataId && x.DeleteFlag == 0).FirstOrDefault();
                var Wiproduct = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == BillofPurchaseDetails.WarehouseId && x.DeleteFlag == 0).ToList();
                if (Wiproduct.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                {
                    // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                    WiproductBasic.Wiproducts.Add(new Wiproduct
                    {
                        WiproductNo = WiproductBasic.WiproductNo,
                        WiproductNumber = WiproductBasic.WiproductNumber,
                        Name = WiproductBasic.Name,
                        Quantity = BillofPurchaseCheckin.Quantity,
                        Specification = WiproductBasic.Specification,
                        Property = WiproductBasic.Property,
                        Price = WiproductBasic.Price,
                        MaterialRequire = 1,
                        CreateTime = dt,
                        CreateUser = MyFun.GetUserID(HttpContext),
                        WarehouseId = BillofPurchaseDetails?.WarehouseId ?? 0, // 注意! 可能會重複建立
                        WiproductLogs = {new WiproductLog
                        {
                            LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                            Original = 0,
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
                        }}
                    });
                }
                else
                {
                    Wiproduct.FirstOrDefault().WiproductLogs.Add(new WiproductLog
                    {
                        LinkOrder = BillofPurchaseDetails.BillofPurchase.BillofPurchaseNo,
                        Original = Wiproduct.FirstOrDefault().Quantity,
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
                    Wiproduct.FirstOrDefault().Quantity += BillofPurchaseCheckin.Quantity;
                }
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
                BillofPurchaseHeadData.Status = 1; // 完成進貨
            }
            else
            {
                BillofPurchaseHeadData.Status = 2; // 開始進貨中
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
                if (item.CheckStatus == 0)
                {
                    item.BillofPurchaseCheckins.Add(new BillofPurchaseCheckin()
                    {
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
                    var tempPurchasedCount = PurchaseDetail.PurchasedCount;
                    PurchaseDetail.PurchasedCount += item.Quantity;
                    PurchaseDetail.UpdateTime = dt;
                    PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);
                    if (PurchaseDetail.Quantity < PurchaseDetail.PurchasedCount)
                    {
                        return Ok(MyFun.APIResponseError("驗收數量超過採購數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " ] *實際採購數量：" + PurchaseDetail.Quantity + "  *已交貨數量：" + tempPurchasedCount));
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
                        if (item2.Quantity != item2.PurchasedCount)
                        {
                            CheckPurchaseHeadStatus = false;
                        }
                    }
                    if (CheckPurchaseHeadStatus)
                    {
                        PurchaseHead.Status = 1;
                    }

                    //入庫
                    if (item.DataType == 1) // 原料
                    {
                        var MaterialBasic = _context.MaterialBasics.Where(x => x.Id == item.DataId && x.DeleteFlag == 0).FirstOrDefault();
                        var Material = MaterialBasic.Materials.Where(x => x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).ToList();
                        if (Material.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                        {
                            // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                            MaterialBasic.Materials.Add(new Material
                            {
                                MaterialNo = MaterialBasic.MaterialNo,
                                Name = MaterialBasic.Name,
                                Quantity = item.Quantity,
                                Specification = MaterialBasic.Specification,
                                Property = MaterialBasic.Property,
                                Price = MaterialBasic.Price,
                                BaseQuantity = 2,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = item?.WarehouseId ?? 0, // 注意! 可能會重複建立
                                MaterialLogs = {new MaterialLog
                                {
                                    LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                    Original = 0,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    PriceAll = item.PriceAll,
                                    Unit = item.Unit,
                                    UnitCount = item.UnitCount,
                                    UnitPrice = item.UnitPrice,
                                    UnitPriceAll = item.UnitPriceAll,
                                    WorkPrice = item.WorkPrice,
                                    Reason = item.Remarks,
                                    Message = "進貨檢驗入庫",
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                        else
                        {
                            Material.FirstOrDefault().MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                Original = Material.FirstOrDefault().Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remarks,
                                Message = "進貨檢驗入庫",
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Material.FirstOrDefault().Quantity += item.Quantity;
                        }
                    }
                    else if (item.DataType == 2) // 成品
                    {
                        var ProductBasic = _context.ProductBasics.Where(x => x.Id == item.DataId && x.DeleteFlag == 0).FirstOrDefault();
                        var Product = ProductBasic.Products.Where(x => x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).ToList();
                        if (Product.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                        {
                            // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                            ProductBasic.Products.Add(new Product
                            {
                                ProductNo = ProductBasic.ProductNo,
                                ProductNumber = ProductBasic.ProductNumber,
                                Name = ProductBasic.Name,
                                Quantity = item.Quantity,
                                Specification = ProductBasic.Specification,
                                Property = ProductBasic.Property,
                                Price = ProductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = item?.WarehouseId ?? 0, // 注意! 可能會重複建立
                                ProductLogs = {new ProductLog
                                {
                                    LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                    Original = 0,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    PriceAll = item.PriceAll,
                                    Unit = item.Unit,
                                    UnitCount = item.UnitCount,
                                    UnitPrice = item.UnitPrice,
                                    UnitPriceAll = item.UnitPriceAll,
                                    WorkPrice = item.WorkPrice,
                                    Reason = item.Remarks,
                                    Message = "進貨檢驗入庫",
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                        else
                        {
                            Product.FirstOrDefault().ProductLogs.Add(new ProductLog
                            {
                                LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                Original = Product.FirstOrDefault().Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remarks,
                                Message = "進貨檢驗入庫",
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Product.FirstOrDefault().Quantity += item.Quantity;
                        }
                    }
                    else if (item.DataType == 3) // 半成品
                    {
                        var WiproductBasic = _context.WiproductBasics.Where(x => x.Id == item.DataId && x.DeleteFlag == 0).FirstOrDefault();
                        var Wiproduct = WiproductBasic.Wiproducts.Where(x => x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).ToList();
                        if (Wiproduct.Count() == 0) // 無倉別資料，則自動建立 2020/09/09
                        {
                            // return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                            WiproductBasic.Wiproducts.Add(new Wiproduct
                            {
                                WiproductNo = WiproductBasic.WiproductNo,
                                WiproductNumber = WiproductBasic.WiproductNumber,
                                Name = WiproductBasic.Name,
                                Quantity = item.Quantity,
                                Specification = WiproductBasic.Specification,
                                Property = WiproductBasic.Property,
                                Price = WiproductBasic.Price,
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = item?.WarehouseId ?? 0, // 注意! 可能會重複建立
                                WiproductLogs = {new WiproductLog
                                {
                                    LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                    Original = 0,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    PriceAll = item.PriceAll,
                                    Unit = item.Unit,
                                    UnitCount = item.UnitCount,
                                    UnitPrice = item.UnitPrice,
                                    UnitPriceAll = item.UnitPriceAll,
                                    WorkPrice = item.WorkPrice,
                                    Reason = item.Remarks,
                                    Message = "進貨檢驗入庫",
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                }}
                            });
                        }
                        else
                        {
                            Wiproduct.FirstOrDefault().WiproductLogs.Add(new WiproductLog
                            {
                                LinkOrder = item.BillofPurchase.BillofPurchaseNo,
                                Original = Wiproduct.FirstOrDefault().Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remarks,
                                Message = "進貨檢驗入庫",
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            Wiproduct.FirstOrDefault().Quantity += item.Quantity;
                        }
                    }

                }
                else
                {
                    NoUpdataCount++;
                }
            }

            if (NoUpdataCount == BillofPurchaseHeads.BillofPurchaseDetails.Count())
            {
                return Ok(MyFun.APIResponseOK("OK", "項目皆已驗收完畢!"));
            }

            //檢查進貨單明細是否都完成進貨
            var CheckBillofPurchaseHeadStatus = true;
            foreach (var item in BillofPurchaseHeads.BillofPurchaseDetails)
            {
                if (item.CheckStatus == 0)
                {
                    CheckBillofPurchaseHeadStatus = false;
                }
            }
            if (CheckBillofPurchaseHeadStatus)
            {
                BillofPurchaseHeads.Status = 1; // 完成進貨
            }
            else
            {
                BillofPurchaseHeads.Status = 2; // 開始進貨中
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
            BillofPurchaseDetails.BillofPurchaseReturns.Add(new BillofPurchaseReturn()
            {
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
            BillofPurchaseReturn.CreateUser = MyFun.GetUserID(HttpContext);
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
            if (PurchaseDetail.PurchasedCount < (int)BillofPurchaseReturn.Quantity)
            {
                return Ok(MyFun.APIResponseError("驗退數量超過驗收數量! [ " + PurchaseDetail.Purchase.PurchaseNo + " 實際已驗收數量： " + PurchaseDetail.PurchasedCount + " ]"));
            }
            PurchaseDetail.PurchasedCount = PurchaseDetail.PurchasedCount - (int)BillofPurchaseReturn.Quantity;
            PurchaseDetail.UpdateTime = dt;
            PurchaseDetail.UpdateUser = MyFun.GetUserID(HttpContext);

            //出庫(驗退)
            if (BillofPurchaseDetails.DataType == 1)
            {
                var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseReturn.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Material == null)
                {
                    return Ok(MyFun.APIResponseError("原料庫存資料有誤"));
                }
                Material.MaterialLogs.Add(new MaterialLog
                {
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
            }
            else if (BillofPurchaseDetails.DataType == 2)
            {
                var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseReturn.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Product == null)
                {
                    return Ok(MyFun.APIResponseError("成品庫存資料有誤"));
                }
                Product.ProductLogs.Add(new ProductLog
                {
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
            }
            else if (BillofPurchaseDetails.DataType == 3)
            {
                var Wiproduct = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == BillofPurchaseDetails.DataId && x.WarehouseId == BillofPurchaseReturn.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                if (Wiproduct == null)
                {
                    return Ok(MyFun.APIResponseError("半成品庫存資料有誤"));
                }
                Wiproduct.WiproductLogs.Add(new WiproductLog
                {
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

            //當進貨單驗退時，採購單狀態更新為[未結案]  2020/09/07   by minja
            var PurchaseHead = _context.PurchaseHeads.Find(BillofPurchaseDetails.PurchaseId);
            if (PurchaseHead == null)
            {
                return Ok(MyFun.APIResponseError("採購單主檔資料有誤!"));
            }
            PurchaseHead.Status = 2;
            

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