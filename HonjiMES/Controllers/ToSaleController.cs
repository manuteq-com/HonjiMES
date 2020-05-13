using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;

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
                    var dt = DateTime.Now;
                    var nlist = new List<SaleDetailNew>();
                    //var OrderDetails = _context.OrderDetails.Where(x => ToSales.Orderlist.Contains(x.Id)).ToList();
                    foreach (var PDetailitem in ToSales.Orderlist)
                    {
                        var Detailitem = _context.OrderDetails.Find(PDetailitem.Id);//取目前的資料來使用
                        if (Detailitem.Quantity >= Detailitem.SaleCount + PDetailitem.Quantity)//原有數量+目前數量不超過未銷貨完的可以開
                        {
                            //var Qty = PDetailitem.Quantity - Detailitem.SaleCount;//剩下可開的銷貨數量
                            var Qty = PDetailitem.Quantity;
                            Detailitem.SaleCount += Qty;
                            Detailitem.Product.QuantityAdv += Qty;
                            nlist.Add(new SaleDetailNew
                            {
                                OrderId = Detailitem.OrderId,
                                OrderDetailId = Detailitem.Id,
                                Quantity = Qty,
                                OriginPrice = Detailitem.OriginPrice,
                                Price = Detailitem.Price,
                                ProductId = Detailitem.ProductId,
                                ProductNo = Detailitem.Product.ProductNo,
                                Name = Detailitem.Product.Name,
                                Specification = Detailitem.Product.Specification,
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
                        var SaleNo = dt.ToString("yyyyMMdd");
                        var NoCount = _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.StartsWith("S" + SaleNo)).Count() + 1;
                        //S  20200415  001
                        var nsale = new SaleHead
                        {
                            SaleNo = "S" + SaleNo + NoCount.ToString("000"),
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
        public async Task<ActionResult<SaleHead>> OrderSale(OrderSale OrderSale)
        {
            var dt = DateTime.Now;
            var SaleDetailNewList = new List<SaleDetailNew>();
            if (OrderSale.SaleID.HasValue)
            {
                var SaleHead = _context.SaleHeads.Find(OrderSale.SaleID);
                SaleDetailNewList.AddRange(SaleHead.SaleDetailNews);
            }
            else if (OrderSale.SaleDID.HasValue)
            {
                var SaleDetail = _context.SaleDetailNews.Find(OrderSale.SaleDID);
                SaleDetailNewList.Add(SaleDetail);
            }
            else
            {
                return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
            }
            var oversale = new List<string>();
            foreach (var item in SaleDetailNewList.Where(x => x.Status == 0))
            {
                if (item.OrderDetail.Quantity >= item.OrderDetail.SaleCount)
                {
                    //銷貨扣庫
                    item.Product.ProductLogs.Add(new ProductLog { Original = item.Product.Quantity, Quantity = -item.Quantity, Reason = item.Sale.SaleNo + "銷貨", CreateTime = dt, CreateUser = 1 });
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
            if (oversale.Any())
            {
                var ErrMsg = string.Join(';', oversale) + "\r\n" + "庫存不足，銷貨失敗";
                return Ok(MyFun.APIResponseError(ErrMsg));
            }
            else
            {
                await _context.SaveChangesAsync();
            }
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
        public async Task<ActionResult<SaleHead>> ReOrderSale(ReOrderSale ReOrderSale)
        {
            var dt = DateTime.Now;
            var SaleDetail = _context.SaleDetailNews.Find(ReOrderSale.SaleDID);
            if (SaleDetail == null)
            {
                return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
            }
            if (SaleDetail.Status == 1)//抓已銷貨的
            {
                var Warehouses = _context.Warehouses.Find(ReOrderSale.WarehouseId);
                if (Warehouses.Recheck.HasValue && Warehouses.Recheck == 0)//不用檢查直接存回庫存
                {
                    var ProductsData = _context.Products.AsQueryable().Where(x => x.WarehouseId == ReOrderSale.WarehouseId && x.DeleteFlag == 0 && x.ProductNo == SaleDetail.Product.ProductNo).FirstOrDefault();
                    ProductsData.ProductLogs.Add(new ProductLog { Original = ProductsData.Quantity, Quantity = ReOrderSale.Quantity, Reason = ReOrderSale.Reason, Message = SaleDetail.Sale.SaleNo + "銷貨直接退庫", CreateTime = dt, CreateUser = 1 });
                    ProductsData.Quantity += ReOrderSale.Quantity;
                    ProductsData.UpdateTime = dt;
                    ProductsData.UpdateUser = 1;
                    // SaleDetail.Product.ProductLogs.Add(new ProductLog { Original = SaleDetail.Product.Quantity, Quantity = ReOrderSale.Quantity, Reason = ReOrderSale.Reason, Message = SaleDetail.Sale.SaleNo + "銷貨直接退庫", CreateTime = dt, CreateUser = 1 });
                    // SaleDetail.Product.Quantity += ReOrderSale.Quantity;
                    // SaleDetail.Product.UpdateTime = dt;
                    // SaleDetail.Product.UpdateUser = 1;
                }
                else
                {
                    SaleDetail.ReturnSales.Add(new ReturnSale { WarehouseId = ReOrderSale.WarehouseId, Reason = ReOrderSale.Reason, Quantity = ReOrderSale.Quantity, CreateTime = dt, CreateUser = 1 });
                }

                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK("OK"));
            }
            return Ok(MyFun.APIResponseError("無已銷貨資料"));
        }
    }
}