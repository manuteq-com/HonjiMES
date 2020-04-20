using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;

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
                    var OrderDetails = _context.OrderDetails.Where(x => ToSales.Orderlist.Contains(x.Id)).ToList();
                    foreach (var Detailitem in OrderDetails)
                    {
                        if (Detailitem.Quantity > Detailitem.SaleCount)//未銷貨完的可以開
                        {
                            var Qty = Detailitem.Quantity - Detailitem.SaleCount;//剩下可開的銷貨數量
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
                        var NoCount = _context.SaleHeads.Where(x => x.SaleNo.StartsWith("S" + SaleNo)).Count() + 1;
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
        /// <param name="id">銷貨單ID</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<ActionResult<SaleHead>> OrderSale(int id)
        {
            var oversale = "";
            var SaleHead = _context.SaleHeads.Find(id);
            if (SaleHead == null)
            {
                return Ok(MyFun.APIResponseError("銷貨資訊錯誤"));
            }

            foreach (var GDetailitem in SaleHead.SaleDetailNews.GroupBy(x => x.ProductId))
            {
                var sQuantity = GDetailitem.Sum(x => x.Quantity);//品項的總銷貨數
                var Products = _context.Products.Find(GDetailitem.Key);//目前庫存數
                if (sQuantity > Products.Quantity)//超過庫存
                {
                    oversale += Products.ProductNo + ":" + Products.Quantity + "=>" + sQuantity;
                }
                else
                {
                    foreach (var item in GDetailitem)
                    {
                        //檢查訂單
                        if (item.OrderDetail.SaleCount == 0)//都沒銷過貨
                        {
                            //if (item.Quantity)
                            //{

                            //}
                        }

                    }
                }
            }
            if (string.IsNullOrWhiteSpace(oversale))
            {
                return Ok(MyFun.APIResponseError(oversale));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }
    }
}