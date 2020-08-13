using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Filter;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HonjiMES.Controllers
{
    /// <summary>
    /// 庫存API
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController : Controller
    {
        private readonly HonjiContext _context;

        public InventoryController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 取得庫存調整單號
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<AdjustData>> GetAdjustNo()
        {
            var key = "AJ";
            var dt = DateTime.Now.ToString("yyMMdd");

            var MaterialNoData = await _context.MaterialLogs.AsQueryable().Where(x => x.AdjustNo.Contains(key + dt) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var MaterialNoCount = MaterialNoData.Count() + 1;
            var ProductNoData = await _context.ProductLogs.AsQueryable().Where(x => x.AdjustNo.Contains(key + dt) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var ProductNoCount = ProductNoData.Count() + 1;
            var WiproductNoData = await _context.WiproductLogs.AsQueryable().Where(x => x.AdjustNo.Contains(key + dt) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var WiproductNoCount = WiproductNoData.Count() + 1;

            if (MaterialNoCount != 1)
            {
                foreach (var item in MaterialNoData)
                {
                    if (MyFun.CheckNoFormat(item.AdjustNo, key, dt, 3))
                    {
                        var No = Int32.Parse(item.AdjustNo.Substring(item.AdjustNo.Length - 3, 3));
                        if (MaterialNoCount <= No)
                        {
                            MaterialNoCount = No + 1;
                        }
                    }
                }
            }
            if (ProductNoCount != 1)
            {
                foreach (var item in ProductNoData)
                {
                    if (MyFun.CheckNoFormat(item.AdjustNo, key, dt, 3))
                    {
                        var No = Int32.Parse(item.AdjustNo.Substring(item.AdjustNo.Length - 3, 3));
                        if (ProductNoCount <= No)
                        {
                            ProductNoCount = No + 1;
                        }
                    }
                }
            }
            if (WiproductNoCount != 1)
            {
                foreach (var item in WiproductNoData)
                {
                    if (MyFun.CheckNoFormat(item.AdjustNo, key, dt, 3))
                    {
                        var No = Int32.Parse(item.AdjustNo.Substring(item.AdjustNo.Length - 3, 3));
                        if (WiproductNoCount <= No)
                        {
                            WiproductNoCount = No + 1;
                        }
                    }
                }
            }

            // var AdjustNoName = "AJ";
            // var NoDataProduct = await _context.ProductLogs.AsQueryable().Where(x => x.DeleteFlag == 0 && x.AdjustNo.Contains(AdjustNoName) 
            //     && x.AdjustNo.Length == (AdjustNoName.Length + 6)).OrderByDescending(x => x.CreateTime).ToListAsync();
            // var NoCountProduct = NoDataProduct.Count() + 1;
            // var NoDataMaterial = await _context.MaterialLogs.AsQueryable().Where(x => x.DeleteFlag == 0 && x.AdjustNo.Contains(AdjustNoName) 
            //     && x.AdjustNo.Length == (AdjustNoName.Length + 6)).OrderByDescending(x => x.CreateTime).ToListAsync();
            // var NoCountMaterial = NoDataMaterial.Count() + 1;

            // if (NoCountProduct != 1) {
            //     var LastAdjustNo = NoDataProduct.FirstOrDefault().AdjustNo;
            //     var LastLength = LastAdjustNo.Length - (LastAdjustNo.Length - 6);
            //     var NoLast = Int32.Parse(LastAdjustNo.Substring(LastAdjustNo.Length - LastLength, LastLength));
            //     if (NoCountProduct <= NoLast) {
            //         NoCountProduct = NoLast + 1;
            //     }
            // }
            // if (NoCountMaterial != 1) {
            //     var LastAdjustNo = NoDataMaterial.FirstOrDefault().AdjustNo;
            //     var LastLength = LastAdjustNo.Length - (LastAdjustNo.Length - 6);
            //     var NoLast = Int32.Parse(LastAdjustNo.Substring(LastAdjustNo.Length - LastLength, LastLength));
            //     if (NoCountMaterial <= NoLast) {
            //         NoCountMaterial = NoLast + 1;
            //     }
            // }

            var NoCount = MaterialNoCount;
            if (NoCount < ProductNoCount)
            {
                NoCount = ProductNoCount;
            }
            if (NoCount < WiproductNoCount)
            {
                NoCount = WiproductNoCount;
            }

            var AdjustData = new AdjustData
            {
                AdjustNo = key + dt + NoCount.ToString("000")
            };
            return Ok(MyFun.APIResponseOK(AdjustData));
        }

        // GET: api/MaterialBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasicData>>> GetMaterialBasics()
        {
            var materialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.MaterialNo).ToListAsync();
            var BasicData = new List<BasicData>();
            foreach (var item in materialBasic)
            {
                var tempData = new BasicData
                {
                    DataType = 1,
                    DataId = item.Id,
                    DataNo = item.MaterialNo,
                    Name = item.Name,
                    Specification = item.Specification,
                    Property = item.Property,
                    Price = item.Price
                };
                BasicData.Add(tempData);
            }
            return Ok(MyFun.APIResponseOK(BasicData));
        }

        // GET: api/ProductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasicData>>> GetProductBasics()
        {
            var ProductBasic = await _context.ProductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.ProductNo).ToListAsync();
            var BasicData = new List<BasicData>();
            foreach (var item in ProductBasic)
            {
                var tempData = new BasicData
                {
                    DataType = 2,
                    DataId = item.Id,
                    DataNo = item.ProductNo,
                    Name = item.Name,
                    Specification = item.Specification,
                    Property = item.Property,
                    Price = item.Price
                };
                BasicData.Add(tempData);
            }
            return Ok(MyFun.APIResponseOK(BasicData));
        }

        // GET: api/MaterialBasics&ProductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasicData>>> GetBasicsData()
        {
            var MaterialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.Materials).OrderBy(x => x.MaterialNo).ToListAsync();
            var ProductBasic = await _context.ProductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.Products).OrderBy(x => x.ProductNo).ToListAsync();
            var WiproductBasic = await _context.WiproductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.Wiproducts).OrderBy(x => x.WiproductNo).ToListAsync();
            var AdjustData = new List<BasicData>();
            var TempId = 1;

            foreach (var item in ProductBasic)
            {
                var tempData = new BasicData
                {
                    TempId = TempId++,
                    DataType = 2,
                    DataId = item.Id,
                    DataNo = item.ProductNo,
                    Name = item.Name,
                    Specification = item.Specification,
                    Property = item.Property,
                    Price = item.Price,
                    Quantity = item.Products.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
                };
                AdjustData.Add(tempData);
            }
            foreach (var item in WiproductBasic)
            {
                var tempData = new BasicData
                {
                    TempId = TempId++,
                    DataType = 3,
                    DataId = item.Id,
                    DataNo = item.WiproductNo,
                    Name = item.Name,
                    Specification = item.Specification,
                    Property = item.Property,
                    Price = item.Price,
                    Quantity = item.Wiproducts.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
                };
                AdjustData.Add(tempData);
            }
            foreach (var item in MaterialBasic)
            {
                var tempData = new BasicData
                {
                    TempId = TempId++,
                    DataType = 1,
                    DataId = item.Id,
                    DataNo = item.MaterialNo,
                    Name = item.Name,
                    Specification = item.Specification,
                    Property = item.Property,
                    Price = item.Price,
                    Quantity = item.Materials.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
                };
                AdjustData.Add(tempData);
            }

            return Ok(MyFun.APIResponseOK(AdjustData));
        }

        /// <summary>
        /// 修改庫存
        /// </summary>
        /// <param name="inventorychange">修改內容</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<APIResponse>> inventorychange(inventorychange inventorychange)
        {
            var UserID = MyFun.GetUserID(HttpContext);
            var dt = DateTime.Now;
            if (inventorychange.mod == "material")
            {
                var Material = _context.Materials.Find(inventorychange.id);
                // if (!(Material.MaterialLogs.Any()))//同步資料用，建立原始庫存數
                // {
                //     Material.MaterialLogs.Add(new MaterialLog { 
                //         Quantity = Material.Quantity, 
                //         Message = "原始數量", 
                //         CreateTime = dt, 
                //         CreateUser = UserID 
                //     });
                // }
                if (inventorychange.MaterialLog.AdjustNo.Length == 0)
                {
                    inventorychange.MaterialLog.AdjustNo = null;
                }

                inventorychange.MaterialLog.Original = Material.Quantity;
                inventorychange.MaterialLog.CreateTime = dt;
                inventorychange.MaterialLog.CreateUser = UserID;
                inventorychange.MaterialLog.Message = "原料庫存調整";
                Material.MaterialLogs.Add(inventorychange.MaterialLog);
                Material.Quantity += inventorychange.MaterialLog.Quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Material));
            }
            else if (inventorychange.mod == "product")
            {
                var Products = _context.Products.Find(inventorychange.id);
                // if (!(Products.ProductLogs.Any()))//同步資料用，建立原始庫存數
                // {
                //     Products.ProductLogs.Add(new ProductLog { 
                //         Quantity = Products.Quantity, 
                //         Message = "原始數量", 
                //         CreateTime = dt, 
                //         CreateUser = UserID 
                //     });
                // }
                if (inventorychange.ProductLog.AdjustNo.Length == 0)
                {
                    inventorychange.ProductLog.AdjustNo = null;
                }

                inventorychange.ProductLog.Original = Products.Quantity;
                inventorychange.ProductLog.CreateTime = dt;
                inventorychange.ProductLog.CreateUser = UserID;
                inventorychange.ProductLog.Message = "成品庫存調整";
                Products.ProductLogs.Add(inventorychange.ProductLog);
                Products.Quantity += inventorychange.ProductLog.Quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Products));
            }
            else if (inventorychange.mod == "wiproduct")
            {
                var Wiproducts = _context.Wiproducts.Find(inventorychange.id);
                // if (!(Wiproducts.WiproductLogs.Any()))//同步資料用，建立原始庫存數
                // {
                //     Wiproducts.WiproductLogs.Add(new WiproductLog { 
                //         Quantity = Wiproducts.Quantity, 
                //         Message = "原始數量", 
                //         CreateTime = dt, 
                //         CreateUser = UserID 
                //     });
                // }
                if (inventorychange.WiproductLog.AdjustNo.Length == 0)
                {
                    inventorychange.WiproductLog.AdjustNo = null;
                }

                inventorychange.WiproductLog.Original = Wiproducts.Quantity;
                inventorychange.WiproductLog.CreateTime = dt;
                inventorychange.WiproductLog.CreateUser = UserID;
                inventorychange.WiproductLog.Message = "半成品庫存調整";
                Wiproducts.WiproductLogs.Add(inventorychange.WiproductLog);
                Wiproducts.Quantity += inventorychange.WiproductLog.Quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Wiproducts));
            }
            return Ok(MyFun.APIResponseError("無對應的資料"));
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }

        /// <summary>
        /// 修改庫存
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<APIResponse>> inventoryListChange(AdjustData AdjustData)
        {
            var UserID = MyFun.GetUserID(HttpContext);
            var dt = DateTime.Now;
            if (AdjustData.AdjustDetailData.Count() == 0)
            {
                return Ok(MyFun.APIResponseError("無庫存調整項目!"));
            }
            else
            {
                var tempId = 0;
                var tempOriginalQuantity = 0;
                var AdjustDetails = new List<AdjustDetail>();

                //// 建立主檔
                var AdjustHead = new AdjustHead
                {
                    AdjustNo = AdjustData.AdjustNo,
                    LinkOrder = AdjustData.LinkOrder,
                    Remarks = AdjustData.Remarks,
                    CreateTime = dt,
                    CreateUser = UserID
                };

                foreach (var item in AdjustData.AdjustDetailData)
                {
                    //// 產生Log
                    if (item.DataType == 1)//material
                    {
                        var MaterialBasic = _context.MaterialBasics.Find(item.DataId);
                        var Material = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Material != null)
                        {
                            Material.MaterialLogs.Add(new MaterialLog
                            {
                                AdjustNo = AdjustData.AdjustNo,
                                LinkOrder = AdjustData.LinkOrder,
                                MaterialId = Material.Id,
                                Original = Material.Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remark,
                                Message = "庫存調整單",
                                CreateTime = dt,
                                CreateUser = UserID
                            });
                            tempId = Material.Id;
                            tempOriginalQuantity = Material.Quantity;
                            Material.Quantity += item.Quantity;
                        }
                        else
                        {
                            return Ok(MyFun.APIResponseError("查無 [" + MaterialBasic.MaterialNo + "] 的倉別資訊!"));
                        }
                    }
                    else if (item.DataType == 2)//product
                    {
                        var ProductBasic = _context.ProductBasics.Find(item.DataId);
                        var Product = _context.Products.AsQueryable().Where(x => x.ProductBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Product != null)
                        {
                            Product.ProductLogs.Add(new ProductLog
                            {
                                AdjustNo = AdjustData.AdjustNo,
                                LinkOrder = AdjustData.LinkOrder,
                                ProductId = Product.Id,
                                Original = Product.Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remark,
                                Message = "庫存調整單",
                                CreateTime = dt,
                                CreateUser = UserID
                            });
                            tempId = Product.Id;
                            tempOriginalQuantity = Product.Quantity;
                            Product.Quantity += item.Quantity;
                        }
                        else
                        {
                            return Ok(MyFun.APIResponseError("查無 [" + ProductBasic.ProductNo + "] 的倉別資訊!"));
                        }
                    }
                    else if (item.DataType == 3)//wiproduct
                    {
                        var WiproductBasic = _context.WiproductBasics.Find(item.DataId);
                        var Wiproduct = _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasicId == item.DataId && x.WarehouseId == item.WarehouseId && x.DeleteFlag == 0).FirstOrDefault();
                        if (Wiproduct != null)
                        {
                            Wiproduct.WiproductLogs.Add(new WiproductLog
                            {
                                AdjustNo = AdjustData.AdjustNo,
                                LinkOrder = AdjustData.LinkOrder,
                                WiproductId = Wiproduct.Id,
                                Original = Wiproduct.Quantity,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                PriceAll = item.PriceAll,
                                Unit = item.Unit,
                                UnitCount = item.UnitCount,
                                UnitPrice = item.UnitPrice,
                                UnitPriceAll = item.UnitPriceAll,
                                WorkPrice = item.WorkPrice,
                                Reason = item.Remark,
                                Message = "庫存調整單",
                                CreateTime = dt,
                                CreateUser = UserID
                            });
                            tempId = Wiproduct.Id;
                            tempOriginalQuantity = Wiproduct.Quantity;
                            Wiproduct.Quantity += item.Quantity;
                        }
                        else
                        {
                            return Ok(MyFun.APIResponseError("查無 [" + WiproductBasic.WiproductNo + "] 的倉別資訊!"));
                        }
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("資訊錯誤!"));
                    }

                    //// 建立明細
                    AdjustHead.AdjustDetails.Add(new AdjustDetail
                    {
                        ItemType = item.DataType,
                        ItemId = tempId,
                        Original = tempOriginalQuantity,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        PriceAll = item.PriceAll,
                        Unit = item.Unit,
                        UnitCount = item.UnitCount,
                        UnitPrice = item.UnitPrice,
                        UnitPriceAll = item.UnitPriceAll,
                        WorkPrice = item.WorkPrice,
                        Reason = item.Remark,
                        Message = "",
                        CreateTime = dt,
                        CreateUser = UserID
                    });
                }
                _context.AdjustHeads.Add(AdjustHead);

                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK("OK"));
            }
        }


    }
}