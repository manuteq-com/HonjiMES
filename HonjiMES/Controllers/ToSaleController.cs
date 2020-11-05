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
    /// 銷貨作業
    /// </summary>
    [JWTAuthorize]
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
            var key = "SN";
            var dt = DateTime.Now;
            var SaleNo = dt.ToString("yyMMdd");

            var NoData = await _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(key + SaleNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            var SaleHeadData = new ToSales
            {
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
            if (CreateNoData != null)
            {
                var key = "SN";
                var SaleNo = CreateNoData.CreateTime.ToString("yyMMdd");

                var NoData = await _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(key + SaleNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                    var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                    if (NoCount <= NoLast)
                    {
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
            var key = "SR";
            var NoData = await _context.ReturnSales.AsQueryable().Where(x => x.DeleteFlag == 0 && x.ReturnNo.Contains(key)).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastReturnNo = NoData.FirstOrDefault().ReturnNo;
                var LastLength = LastReturnNo.Length - key.Length;
                var NoLast = Int32.Parse(LastReturnNo.Substring(LastReturnNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            var ReturnData = new ReturnSale
            {
                ReturnNo = key + NoCount.ToString("000000")
            };
            return Ok(MyFun.APIResponseOK(ReturnData));
        }

        /// <summary>
        /// 新增銷貨單
        /// </summary>
        /// <param name="ToSalesOrderDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaleHead>> OrderToSaleBySelected(List<ToSalesOrderDetail> ToSalesOrderDetail)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            // var OrderNum = ToSalesOrderDetail.AsQueryable().GroupBy(x => x.OrderId).ToList();// 2020/9/2確定取消，不用依照訂單號區分銷貨單號。
            var OrderNum = ToSalesOrderDetail.AsQueryable().GroupBy(x => x.SaleDate).ToList();
            var sale = new List<SaleDetailNew>();
            var NoIndex = 0;
            foreach (var item in OrderNum)
            {
                var dt = DateTime.Now;
                var OrderHeadId = 0;
                var nlist = new List<SaleDetailNew>();
                foreach (var PDetailitem in item)
                {
                    var Detailitem = _context.OrderDetails.Find(PDetailitem.Id);//取目前的資料來使用
                    OrderHeadId = Detailitem.OrderId;
                    if (Detailitem.Quantity >= Detailitem.SaleCount + PDetailitem.SaleQuantity)//原有數量+目前數量不超過未銷貨完的可以開
                    {
                        var Qty = PDetailitem.SaleQuantity;
                        Detailitem.SaleCount += Qty;

                        nlist.Add(new SaleDetailNew
                        {
                            OrderId = Detailitem.OrderId,
                            OrderDetailId = Detailitem.Id,
                            Quantity = Qty,
                            OriginPrice = Detailitem.OriginPrice,
                            Price = Detailitem.Price,
                            MaterialBasicId = Detailitem.MaterialBasicId,
                            MaterialNo = Detailitem.MaterialBasic.MaterialNo,
                            Name = Detailitem.MaterialBasic.Name,
                            Specification = Detailitem.MaterialBasic.Specification,
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext),
                            DeleteFlag = 0
                        });
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("訂單 [ " + PDetailitem.Order.OrderNo + " ] " + Detailitem.MaterialBasic.MaterialNo + " 超過可銷貨數量!!"));
                    }
                }
                //檢查訂單明細是否都完成銷貨
                var CheckOrderHeadStatus = true;
                var OrderHeadData = _context.OrderHeads.Find(OrderHeadId);
                foreach (var Detailitem in OrderHeadData.OrderDetails)
                {
                    if (Detailitem.SaledCount != Detailitem.Quantity)
                    {
                        CheckOrderHeadStatus = false;
                    }
                }
                if (CheckOrderHeadStatus)
                {
                    OrderHeadData.Status = 1;//完成銷貨(尚未結案)
                }

                var key = "SN";
                var Num = DateTime.Now.ToString("yyMMdd");
                var NoData = await _context.SaleHeads.AsQueryable().Where(x => x.SaleNo.Contains(key + Num) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastSaleNo = NoData.FirstOrDefault().SaleNo;
                    var NoLast = Int32.Parse(LastSaleNo.Substring(LastSaleNo.Length - 3, 3));
                    if (NoCount <= NoLast)
                    {
                        NoCount = NoLast + 1;
                    }
                }
                var nsale = new SaleHead
                {
                    // SaleNo = "S" + SaleNo + NoCount.ToString("000"),
                    SaleNo = key + Num + (NoCount + NoIndex).ToString("000"),
                    Status = 0,
                    SaleDate = item.FirstOrDefault().SaleDate,
                    PriceAll = nlist.Sum(x => x.Quantity * x.OriginPrice),
                    Remarks = null,
                    DeleteFlag = 0,
                    CreateTime = dt,
                    CreateUser = MyFun.GetUserID(HttpContext),
                    SaleDetailNews = nlist
                };
                _context.SaleHeads.Add(nsale);
                NoIndex++;
            }
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 訂單轉銷貨
        /// </summary>
        /// <param name="ToSales"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaleHead>> OrderToSale(ToSales ToSales)
        {
            return Ok(await OrderToSaleFun(ToSales));
        }
        public async Task<APIResponse> OrderToSaleFun(ToSales ToSales)
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
                        var Detailitem = _context.OrderDetails.Find(PDetailitem.Id);//取目前的資料來使用
                        OrderHeadId = Detailitem.OrderId;
                        if (Detailitem.Quantity >= Detailitem.SaleCount + PDetailitem.Quantity)//原有數量+目前數量不超過未銷貨完的可以開
                        {
                            //var Qty = PDetailitem.Quantity - Detailitem.SaleCount;//剩下可開的銷貨數量
                            var Qty = PDetailitem.Quantity;
                            Detailitem.SaleCount += Qty;
                            // Detailitem.Material.QuantityAdv += Qty;//暫時停用，等待更新做法
                            nlist.Add(new SaleDetailNew
                            {
                                OrderId = Detailitem.OrderId,
                                OrderDetailId = Detailitem.Id,
                                Quantity = Qty,
                                OriginPrice = Detailitem.OriginPrice,
                                Price = Detailitem.Price,
                                MaterialBasicId = Detailitem.MaterialBasicId,
                                MaterialNo = Detailitem.MaterialBasic.MaterialNo,
                                Name = Detailitem.MaterialBasic.Name,
                                Specification = Detailitem.MaterialBasic.Specification,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                DeleteFlag = 0
                            });
                        }
                        else
                        {
                            return MyFun.APIResponseError("序號" + Detailitem.Serial + ":超過可銷貨數量");
                        }
                    }

                    //檢查訂單明細是否都完成銷貨
                    var CheckOrderHeadStatus = true;
                    var OrderHeadData = _context.OrderHeads.Find(OrderHeadId);
                    foreach (var Detailitem in OrderHeadData.OrderDetails)
                    {
                        if (Detailitem.SaledCount != Detailitem.Quantity)
                        {
                            CheckOrderHeadStatus = false;
                        }
                    }
                    if (CheckOrderHeadStatus)
                    {
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
                        if (checkSaleNo != 0)
                        {
                            return MyFun.APIResponseError("[銷貨單號]已存在! 請刷新單號!");
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
                            CreateUser = MyFun.GetUserID(HttpContext),
                            SaleDetailNews = nlist
                        };
                        _context.SaleHeads.Add(nsale);
                        await _context.SaveChangesAsync();
                        _context.ChangeTracker.LazyLoadingEnabled = false;
                    }
                    else
                    {
                        return MyFun.APIResponseError("銷貨資訊錯誤");
                    }

                    return MyFun.APIResponseOK("OK");
                }
                catch (Exception ex)
                {
                    return MyFun.APIResponseError(ex.Message + ";" + ex.InnerException.Message);
                }
            }
            else
            {
                return MyFun.APIResponseError("無訂單資料");
            }
        }
        /// <summary>
        /// 銷貨單銷貨
        /// </summary>
        /// <param name="ToSaleList"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaleHead>> SaleOrderToSale(List<SaleDetailNewData> ToSaleList)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var dt = DateTime.Now;
            var oversale = new List<string>();
            var Materials = _context.Materials.Where(x => x.DeleteFlag == 0);
            foreach (var item in ToSaleList)
            {
                var SaleDetailData = await _context.SaleDetailNews.FindAsync(item.Id);
                var MaterialData = Materials.Where(x => x.MaterialBasicId == SaleDetailData.MaterialBasicId && x.WarehouseId == item.WarehouseId).ToList();
                var SaleHeadId = SaleDetailData.SaleId;

                if (MaterialData.Count() == 1)
                {
                    var Material = MaterialData.FirstOrDefault();
                    // if (SaleDetailData.OrderDetail.Quantity >= SaleDetailData.OrderDetail.SaleCount)
                    {
                        // 銷貨扣庫
                        Material.MaterialLogs.Add(new MaterialLog
                        {
                            LinkOrder = SaleDetailData.Sale.SaleNo,
                            Original = Material.Quantity,
                            Quantity = -SaleDetailData.Quantity,
                            Price = SaleDetailData.Price,
                            PriceAll = SaleDetailData.Quantity * SaleDetailData.Price,
                            Message = "[銷貨]出庫",
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        SaleDetailData.Status = 1; // 1已銷貨
                        SaleDetailData.UpdateTime = dt;
                        SaleDetailData.UpdateUser = MyFun.GetUserID(HttpContext);
                        Material.Quantity -= SaleDetailData.Quantity;
                        Material.QuantityAdv -= SaleDetailData.Quantity;
                        Material.UpdateTime = dt;
                        Material.UpdateUser = MyFun.GetUserID(HttpContext);

                        if (Material.Quantity < 0)
                        {
                            oversale.Add(Material.MaterialNo);
                        }

                        //更新訂單完成銷貨量
                        SaleDetailData.OrderDetail.SaledCount = SaleDetailData.OrderDetail.SaledCount + SaleDetailData.Quantity;

                        // 檢查該銷貨單各個明細狀態，更新銷貨單(head)狀態。
                        var checkStatus = true;
                        var SaleHeadData = _context.SaleHeads.Find(SaleHeadId);
                        SaleHeadData.UpdateUser = MyFun.GetUserID(HttpContext);
                        foreach (var SaleDetail in SaleHeadData.SaleDetailNews)
                        {
                            if (SaleDetail.Status == 0)
                            {
                                checkStatus = false;
                            }
                        }
                        if (checkStatus == true)
                        {
                            SaleHeadData.Status = 2; // 完成銷貨
                        }
                        else
                        {
                            SaleHeadData.Status = 1; // 銷貨一半(未完成)
                        }
                    }
                }
                else
                {
                    return Ok(MyFun.APIResponseError("銷貨單號：" + SaleDetailData.Sale.SaleNo + "　[ 品號：" + SaleDetailData.MaterialBasic.MaterialNo + " ] 查無庫存資訊!"));
                }
            }

            if (oversale.Any())
            {
                var ErrMsg = string.Join('；', oversale) + "\r\n" + "庫存不足，銷貨失敗";
                return Ok(MyFun.APIResponseError(ErrMsg));
            }
            else
            {
                await _context.SaveChangesAsync();
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK("OK"));
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
                // var MaterialId = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == SaleDetail.MaterialBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault()?.Id;
                // SaleDetail.MaterialId = MaterialId;

                var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == SaleDetail.MaterialBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                SaleDetail.Material = Material;
                // foreach (var item in MaterialData)
                // {
                //     var MaterialsId =_context.Materials.AsQueryable().Where(x => x.MaterialBasicId == item.MaterialBasicId && x.WarehouseId == OrderSale.WarehouseId && x.DeleteFlag == 0).FirstOrDefault()?.Id;
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

                // if (item.OrderDetail.Quantity >= item.OrderDetail.SaleCount)
                {
                    //銷貨扣庫
                    item.Material.MaterialLogs.Add(new MaterialLog
                    {
                        LinkOrder = item.Sale.SaleNo,
                        Original = item.Material.Quantity,
                        Quantity = -item.Quantity,
                        Price = item.Price,
                        PriceAll = item.Quantity * item.Price,
                        Message = "[銷貨]出庫",
                        CreateUser = MyFun.GetUserID(HttpContext)
                    });
                    item.Material.Quantity -= item.Quantity;
                    item.Material.QuantityAdv -= item.Quantity;
                    item.Status = 1;//1已銷貨
                    item.UpdateTime = dt;
                    item.UpdateUser = MyFun.GetUserID(HttpContext);
                    item.Material.UpdateTime = dt;
                    item.Material.UpdateUser = MyFun.GetUserID(HttpContext);
                    if (item.Material.Quantity < 0)
                    {
                        oversale.Add(item.Material.MaterialNo);
                    }

                    //更新訂單完成銷貨量
                    var OrderDetail = _context.OrderDetails.Find(item.OrderDetailId);
                    OrderDetail.SaledCount = OrderDetail.SaledCount + item.Quantity;
                }
            }

            // 檢查該銷貨單各個明細狀態，更新銷貨單(head)狀態。
            var checkStatus = true;
            var SaleHeadData = _context.SaleHeads.Find(saleId);
            SaleHeadData.UpdateUser = MyFun.GetUserID(HttpContext);
            foreach (var SaleDetail in SaleHeadData.SaleDetailNews)
            {
                if (SaleDetail.Status == 0)
                {
                    checkStatus = false;
                }
            }
            if (checkStatus == true)
            {
                SaleHeadData.Status = 2; // 完成銷貨
            }
            else
            {
                SaleHeadData.Status = 1; // 銷貨一半(未完成)
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

            //foreach (var GDetailitem in SaleHead.SaleDetailNews.GroupBy(x => x.MaterialId))
            //{
            //    var sQuantity = GDetailitem.Sum(x => x.Quantity);//品項的總銷貨數
            //    var Materials = _context.Materials.Find(GDetailitem.Key);//目前庫存數
            //    if (sQuantity > Materials.Quantity)//超過庫存
            //    {
            //        oversale += Materials.MaterialNo + ":" + Materials.Quantity + "=>" + sQuantity;
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
            ReturnSale.CreateUser = MyFun.GetUserID(HttpContext);
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
                // if (Warehouses.Recheck.HasValue && Warehouses.Recheck == 0)//不用檢查直接存回庫存
                {
                    var MaterialBasic = _context.MaterialBasics.Where(x => x.Id == SaleDetail.MaterialBasicId && x.DeleteFlag == 0).FirstOrDefault();
                    var Materials = MaterialBasic.Materials.Where(x => x.WarehouseId == ReturnSale.WarehouseId).ToList();
                    // var MaterialsData = _context.Materials.AsQueryable().Where(x => x.WarehouseId == ReturnSale.WarehouseId && x.DeleteFlag == 0 && x.MaterialBasicId == SaleDetail.MaterialBasicId).FirstOrDefault();
                    if (Materials.Count() == 1) {
                        Materials.FirstOrDefault().MaterialLogs.Add(new MaterialLog
                        {
                            // LinkOrder = ReturnSale.ReturnNo,
                            LinkOrder = SaleDetail.Sale.SaleNo,
                            Original = Materials.FirstOrDefault().Quantity,
                            Quantity = ReturnSale.Quantity,
                            Price = Materials.FirstOrDefault().Price,
                            PriceAll = Materials.FirstOrDefault().Price * ReturnSale.Quantity,
                            Reason = ReturnSale.Reason,
                            Message = "[銷退]入庫",
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                        Materials.FirstOrDefault().Quantity += ReturnSale.Quantity;
                        Materials.FirstOrDefault().UpdateTime = dt;
                        Materials.FirstOrDefault().UpdateUser = MyFun.GetUserID(HttpContext);
                        // SaleDetail.Material.MaterialLogs.Add(new MaterialLog { Original = SaleDetail.Material.Quantity, Quantity = ReturnSale.Quantity, Reason = ReturnSale.Reason, Message = SaleDetail.Sale.SaleNo + "銷貨直接退庫", CreateTime = dt,  CreateUser = MyFun.GetUserID(HttpContext) });
                        // SaleDetail.Material.Quantity += ReturnSale.Quantity;
                        // SaleDetail.Material.UpdateTime = dt;
                        // SaleDetail.Material.UpdateUser = MyFun.GetUserID(HttpContext);
                    } else {
                        MaterialBasic.Materials.Add(new Material
                        {
                            MaterialNo = MaterialBasic.MaterialNo,
                            MaterialNumber = MaterialBasic.MaterialNumber,
                            Name = MaterialBasic.Name,
                            Quantity = ReturnSale.Quantity,
                            Specification = MaterialBasic.Specification,
                            Property = MaterialBasic.Property,
                            Price = MaterialBasic.Price,
                            MaterialRequire = 1,
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext),
                            WarehouseId = ReturnSale.WarehouseId,
                            MaterialLogs = {new MaterialLog
                            {
                                // LinkOrder = ReturnSale.ReturnNo,
                                LinkOrder = SaleDetail.Sale.SaleNo,
                                Original = 0,
                                Quantity = ReturnSale.Quantity,
                                Price = MaterialBasic.Price,
                                PriceAll = MaterialBasic.Price * ReturnSale.Quantity,
                                Reason = ReturnSale.Reason,
                                Message = "[銷退]入庫",
                                CreateUser = MyFun.GetUserID(HttpContext)
                            }}
                        });
                    }
                }
                // else
                // {
                //     SaleDetail.ReturnSales.Add(new ReturnSale { WarehouseId = ReturnSale.WarehouseId, Reason = ReturnSale.Reason, Quantity = ReturnSale.Quantity, CreateTime = dt, CreateUser = MyFun.GetUserID(HttpContext) });
                // }
                SaleDetail.Sale.UpdateUser = MyFun.GetUserID(HttpContext);

                await _context.SaveChangesAsync();
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK("OK"));
            }
            return Ok(MyFun.APIResponseError("無已銷貨資料"));
        }
    }
}