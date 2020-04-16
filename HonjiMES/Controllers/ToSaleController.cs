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
                        nlist.Add(new SaleDetailNew
                        {
                            OrderId = Detailitem.OrderId,
                            OrderDetailId = Detailitem.Id,
                            Quantity = Detailitem.Quantity,
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
                        var NoCount = _context.SaleHeads.Where(x => x.SaleNo.StartsWith(SaleNo)).Count() + 1;
                        //S  20200415  001
                        var nsale = new SaleHead
                        {
                            SaleNo = "s" + SaleNo + NoCount.ToString("000"),
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
    }
}