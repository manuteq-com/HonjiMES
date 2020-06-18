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
    /// 銷貨作業
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToSaleController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ToSaleController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        
        /// <summary>
        /// 銷貨單號
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToSales>>> GetSaleNumber()
        {
            var key = "S";
            var dt = DateTime.Now;
            var SaleNo = dt.ToString("yyMMdd");

            var NoData = await _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(key + SaleNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var SaleHeadData = new ToSales{
                CreateTime = dt,
                SaleNo = key + SaleNo + NoCount.ToString("000"),
                SaleDate = null
            };
            return Ok(MyFun.APIResponseOK(SaleHeadData));
        }

        /// <summary>
        /// 銷貨單號
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetSaleNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null) {
                var key = "S";
                var SaleNo = CreateNoData.CreateTime.ToString("yyMMdd");
                
                var NoData = await _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(key + SaleNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1) {
                    var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                    var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                    if (NoCount <= NoLast) {
                        NoCount = NoLast + 1;
                    }
                }
                CreateNoData.CreateNumber = key + SaleNo + NoCount.ToString("000");
                return Ok(MyFun.APIResponseOK(CreateNoData));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 取得銷退單號
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ReturnSale>> GetSaleReturnNo()
        {
            var ReturnNoName = "SR";
            var NoData = await _context.ReturnSales.AsQueryable().Where(x => x.DeleteFlag == 0 && x.ReturnNo.Contains(ReturnNoName)).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastReturnNo = NoData.FirstOrDefault().ReturnNo;
                var LastLength = LastReturnNo.Length - ReturnNoName.Length;
                var NoLast = Int32.Parse(LastReturnNo.Substring(LastReturnNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var ReturnData = new ReturnSale{
                ReturnNo = ReturnNoName + NoCount.ToString("000000")
            };
            return Ok(MyFun.APIResponseOK(ReturnData));
        }

        /// <summary>
        /// 訂單轉銷貨
        /// </summary>
        /// <param name="ToSales"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaleHead>> OrderToSale(ToSales ToSales)
        {
            if (ToSales.Orderlist.Any())
            {
                try
                {
                    _context.ChangeTracker.LazyLoadingEnabled = true;
                    var OrderHeadId = 0;
                    var dt = DateTime.Now;
                    var nlist = new List<SaleDetailNew>();
                    //var OrderDetails = _context.OrderDetails.Where(x => ToSales.Orderlist.Contains(x.Id)).ToList();
                    foreach (var PDetailitem in ToSales.Orderlist)
                    {
                        OrderHeadId = PDetailitem.OrderId; 
                        var Detailitem = _context.OrderDetails.Find(PDetailitem.Id);//取目前的資料來使用
                        if (Detailitem.Quantity >= Detailitem.SaleCount + PDetailitem.Quantity)//原有數量+目前數量不超過未銷貨完的可以開
                        {
                            //var Qty = PDetailitem.Quantity - Detailitem.SaleCount;//剩下可開的銷貨數量
                            var Qty = PDetailitem.Quantity;
                            Detailitem.SaleCount += Qty;
                            // Detailitem.Product.QuantityAdv += Qty;//暫時停用，等待更新做法
                            nlist.Add(new SaleDetailNew
                            {
                                OrderId = Detailitem.OrderId,
                                OrderDetailId = Detailitem.Id,
                                Quantity = Qty,
                                OriginPrice = Detailitem.OriginPrice,
                                Price = Detailitem.Price,
                                ProductBasicId = Detailitem.ProductBasicId,
                                ProductNo = Detailitem.ProductBasic.ProductNo,
                                Name = Detailitem.ProductBasic.Name,
                                Specification = Detailitem.ProductBasic.Specification,
                                CreateTime = dt,
                                CreateUser = 1,
                                DeleteFlag = 0
                            });
                        }
                        else
                        {
                            return Ok(MyFun.APIResponseError("序號" + Detailitem.Serial + ":超過可銷貨數量"));
                        }
                    }

                    //檢查訂單明細是否都完成銷貨
                    var CheckOrderHeadStatus = true;
                    var OrderHeadData = _context.OrderHeads.Find(OrderHeadId);
                    foreach (var Detailitem in OrderHeadData.OrderDetails)
                    {
                        if (Detailitem.SaleCount != Detailitem.Quantity) {
                            CheckOrderHeadStatus = false;
                        }
                    }
                    if (CheckOrderHeadStatus) {
                        OrderHeadData.Status = 1;//完成銷貨(尚未結案)
                    }

                    if (ToSales.SaleID.HasValue)//有訂單ID，合併銷貨單
                    {
                        var SaleHead = _context.SaleHeads.Find(ToSales.SaleID);
                        foreach (var Detailitem in nlist)
                        {
                            SaleHead.SaleDetailNews.Add(Detailitem);
                        }
                        SaleHead.PriceAll = SaleHead.SaleDetailNews.Sum(x => x.Quantity * x.OriginPrice);
                        await _context.SaveChangesAsync();
                    }
                    else if (ToSales.SaleDate.HasValue)//有銷貨日期，新增銷貨單
                    {
                        var SaleNo = dt.ToString("yyMMdd");
                        // var NoData = _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.StartsWith("S" + SaleNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                        // var NoCount = NoData.Count() + 1;
                        // if (NoCount != 1) {
                        //     var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                        //     var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                        //     if (NoCount <= NoLast) {
                        //         NoCount = NoLast + 1;
                        //     }
                        // }
                        //S  20200415  001
                        var checkSaleNo = _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(ToSales.SaleNo) && x.DeleteFlag == 0).Count();
                        if (checkSaleNo != 0) {
                            return Ok(MyFun.APIResponseError("[銷貨單號]已存在! 請刷新單號!"));
                        }
                        var nsale = new SaleHead
                        {
                            // SaleNo = "S" + SaleNo + NoCount.ToString("000"),
                            SaleNo = ToSales.SaleNo,
                            Status = 0,
                            SaleDate = ToSales.SaleDate,
                            PriceAll = nlist.Sum(x => x.Quantity * x.OriginPrice),
                            Remarks = ToSales.Remarks,
                            DeleteFlag = 0,
                            CreateTime = dt,
                            CreateUser = 1,
                            SaleDetailNews = nlist
                        };
                        _context.SaleHeads.Add(nsale);
                        await _context.SaveChangesAsync();
                        _context.ChangeTracker.LazyLoadingEnabled = false;
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
                    }

                    return Ok(MyFun.APIResponseOK("OK"));
                }
                catch (Exception ex)
                {
                    return Ok(MyFun.APIResponseError(ex.Message + ";" + ex.InnerException.Message));
                }
            }
            else
            {
                return Ok(MyFun.APIResponseError("無訂單資料"));
            }
        }
        /// <summary>
        /// 銷貨單銷貨
        /// </summary>
        /// <param name="OrderSale"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaleHead>> OrderSale(ToOrderSale OrderSale)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var dt = DateTime.Now;
            var SaleDetailNewList = new List<SaleDetailNew>();
            var saleId = 0;
            if (OrderSale.SaleID.HasValue)
            {
                var SaleHead = _context.SaleHeads.Find(OrderSale.SaleID);
                SaleDetailNewList.AddRange(SaleHead.SaleDetailNews);
            }
            else if (OrderSale.SaleDID.HasValue)
            {
                var SaleDetail = _context.SaleDetailNews.Find(OrderSale.SaleDID);
                // var ProductId = _context.Products.AsQueryable().Where(x => x.ProductBasicId == SaleDetail.ProductBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault()?.Id;
                // SaleDetail.ProductId = ProductId;
                             
                var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == SaleDetail.ProductBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                SaleDetail.Product = Product;
                // foreach (var item in ProductData)
                // {
                //     var ProductsId =_context.Products.AsQueryable().Where(x => x.ProductBasicId == item.ProductBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault()?.Id;
                //     item.p
                // }
                SaleDetailNewList.Add(SaleDetail);
            }
            else
            {
                return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
            }
            var oversale = new List<string>();
            foreach (var item in SaleDetailNewList.Where(x => x.Status == 0))
            {
                saleId = item.SaleId;

                if (item.OrderDetail.Quantity >= item.OrderDetail.SaleCount)
                {
                    //銷貨扣庫
                    item.Product.ProductLogs.Add(new ProductLog { 
                        LinkOrder = item.Sale.SaleNo,
                        Original = item.Product.Quantity,
                        Quantity = -item.Quantity,
                        Price = item.Price,
                        PriceAll = item.Quantity * item.Price,
                        Message = "銷貨",
                        CreateUser = 1
                    });
                    item.Product.Quantity -= item.Quantity;
                    item.Product.QuantityAdv -= item.Quantity;
                    item.Status = 1;//1已銷貨
                    item.UpdateTime = dt;
                    item.UpdateUser = 1;
                    item.Product.UpdateTime = dt;
                    item.Product.UpdateUser = 1;
                    if (item.Product.Quantity < 0)
                    {
                        oversale.Add(item.Product.ProductNo);
                    }
                }
            }

            // 檢查該銷貨單各個明細狀態，更新銷貨單(head)狀態。
            var checkStatus = true;
            var SaleHeadData = _context.SaleHeads.Find(saleId);
            foreach (var SaleDetail in SaleHeadData.SaleDetailNews) {
                if (SaleDetail.Status == 0) {
                    checkStatus = false;
                }
            }
            if (checkStatus == true) {
                SaleHeadData.Status = 1;
            }

            if (oversale.Any())
            {
                var ErrMsg = string.Join(';', oversale) + "\r\n" + "庫存不足，銷貨失敗";
                return Ok(MyFun.APIResponseError(ErrMsg));
            }
            else
            {
                await _context.SaveChangesAsync();
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;

            //foreach (var GDetailitem in SaleHead.SaleDetailNews.GroupBy(x => x.ProductId))
            //{
            //    var sQuantity = GDetailitem.Sum(x => x.Quantity);//品項的總銷貨數
            //    var Products = _context.Products.Find(GDetailitem.Key);//目前庫存數
            //    if (sQuantity > Products.Quantity)//超過庫存
            //    {
            //        oversale += Products.ProductNo + ":" + Products.Quantity + "=>" + sQuantity;
            //    }
            //    else
            //    {
            //        foreach (var item in GDetailitem)
            //        {
            //            //檢查訂單
            //            if (item.OrderDetail.SaleCount == 0)//都沒銷過貨
            //            {
            //                //if (item.Quantity)
            //                //{

            //                //}
            //            }

            //        }
            //    }
            //}
            //if (string.IsNullOrWhiteSpace(oversale))
            //{
            //    return Ok(MyFun.APIResponseError(oversale));
            //}
            return Ok(MyFun.APIResponseOK("OK"));
        }
        [HttpPost]
        public async Task<ActionResult<ReturnSale>> ReOrderSale(ReturnSale ReturnSale)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (ReturnSale.WarehouseId == 0)
            {
                return Ok(MyFun.APIResponseError("請選擇銷退倉!"));
            }
            var SaleDetail = _context.SaleDetailNews.Find(ReturnSale.SaleDetailNewId);
            if (SaleDetail == null)
            {
                return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
            }
            SaleDetail.ReturnSales.Add(ReturnSale);
            var ReturnCount = SaleDetail.ReturnSales.Sum(x => x.Quantity);
            if (ReturnCount > SaleDetail.Quantity)
            {
                return Ok(MyFun.APIResponseError("銷退數量超過銷貨數量! [已銷退數量：" + (ReturnCount - ReturnSale.Quantity) + "]"));
            }
            
            var dt = DateTime.Now;
            if (SaleDetail.Status == 1)//抓已銷貨的
            {
                var Warehouses = _context.Warehouses.Find(ReturnSale.WarehouseId);
                if (Warehouses.Recheck.HasValue && Warehouses.Recheck == 0)//不用檢查直接存回庫存
                {
                    var ProductsData = _context.Products.AsQueryable().Where(x => x.WarehouseId == ReturnSale.WarehouseId && x.DeleteFlag == 0 && x.ProductBasicId == SaleDetail.ProductBasicId).FirstOrDefault();
                    ProductsData.ProductLogs.Add(new ProductLog { 
                        LinkOrder = ReturnSale.ReturnNo,
                        Original = ProductsData.Quantity,
                        Quantity = ReturnSale.Quantity,
                        Price = ProductsData.Price,
                        PriceAll = ProductsData.Price * ReturnSale.Quantity,
                        Reason = ReturnSale.Reason,
                        Message = "銷退",
                        CreateUser = 1
                    });
                    ProductsData.Quantity += ReturnSale.Quantity;
                    ProductsData.UpdateTime = dt;
                    ProductsData.UpdateUser = 1;
                    // SaleDetail.Product.ProductLogs.Add(new ProductLog { Original = SaleDetail.Product.Quantity, Quantity = ReturnSale.Quantity, Reason = ReturnSale.Reason, Message = SaleDetail.Sale.SaleNo + "銷貨直接退庫", CreateTime = dt, CreateUser = 1 });
                    // SaleDetail.Product.Quantity += ReturnSale.Quantity;
                    // SaleDetail.Product.UpdateTime = dt;
                    // SaleDetail.Product.UpdateUser = 1;
                }
                else
                {
                    SaleDetail.ReturnSales.Add(new ReturnSale { WarehouseId = ReturnSale.WarehouseId, Reason = ReturnSale.Reason, Quantity = ReturnSale.Quantity, CreateTime = dt, CreateUser = 1 });
                }

                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            return Ok(MyFun.APIResponseError("無已銷貨資料"));
        }
    }
}