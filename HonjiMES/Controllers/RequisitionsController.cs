﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 領料管理
    /// </summary>
    //[JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RequisitionsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public RequisitionsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 取領料單資訊
        /// </summary>
        /// <returns></returns>
        // GET: api/MaterialRequisitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requisition>>> GetRequisitions([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(_context.Requisitions.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Type == 0).Include(x => x.WorkOrderHead), FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Requisitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requisition>> GetRequisition(int id)
        {
            var materialRequisition = await _context.Requisitions.FindAsync(id);

            if (materialRequisition == null)
            {
                return NotFound();
            }

            //return materialRequisition;
            return Ok(MyFun.APIResponseOK(materialRequisition));
        }

        // PUT: api/MaterialRequisitions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequisition(int id, Requisition Requisition)
        {
            if (id != Requisition.Id)
            {
                return BadRequest();
            }

            _context.Entry(Requisition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequisitionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(Requisition));
        }

        // POST: api/MaterialRequisitions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Requisition>> PostRequisition(Requisition requisition)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (requisition.MaterialBasicId == 0)
            {
                return Ok(MyFun.APIResponseError("沒有品號資料"));
            }
            // 此API無使用
            var ProductBasics = _context.ProductBasics.Find(requisition.MaterialBasicId);

            var key = "MS";
            var dt = DateTime.Now;
            var RequisitionNo = dt.ToString("yyMMdd");

            var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0 && x.Type == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastRequisitionNo = NoData.FirstOrDefault().RequisitionNo;
                var NoLast = Int32.Parse(LastRequisitionNo.Substring(LastRequisitionNo.Length - 3, 3));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            requisition.RequisitionNo = key + RequisitionNo + NoCount.ToString("000");
            // requisition.Name = ProductBasics.Name;
            requisition.Name = requisition?.Name ?? "";
            requisition.MaterialNo = ProductBasics.ProductNo;
            requisition.MaterialNumber = ProductBasics.ProductNumber;
            requisition.Specification = ProductBasics.Specification;
            requisition.CreateTime = dt;
            requisition.CreateUser = MyFun.GetUserID(HttpContext);
            requisition.Type = 0;
            // BOM內容
            var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.MaterialBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
            foreach (var item in MyFun.GetBomList(BillOfMaterials, 0, requisition.Quantity))
            {
                var sameCheck = false;
                if (!string.IsNullOrWhiteSpace(item.ProductNo))
                {   //判斷是否為成品
                    foreach (var item2 in requisition.RequisitionDetails)
                    {
                        if (item2.ProductBasicId == item.ProductBasicId &&
                            item2.ProductNo == item.ProductNo &&
                            item2.MaterialBasicId == item.MaterialBasicId &&
                            item2.MaterialNo == item.MaterialNo)
                        {
                            sameCheck = true;
                            item2.Quantity += item.ReceiveQty;
                        }
                    }
                }
                else
                {    //判斷是否為原料
                    foreach (var item2 in requisition.RequisitionDetails)
                    {
                        if (item2.MaterialBasicId == item.MaterialBasicId &&
                            item2.MaterialNo == item.MaterialNo)
                        {
                            sameCheck = true;
                            item2.Quantity += item.ReceiveQty;
                        }
                    }
                }

                if (!sameCheck)
                {
                    requisition.RequisitionDetails.Add(new RequisitionDetail
                    {
                        // Lv = item.Lv,
                        // Name = item.Name,
                        //原料
                        MaterialBasicId = item.MaterialBasicId,
                        MaterialNo = item.MaterialNo,
                        MaterialName = item.MaterialName,
                        MaterialSpecification = item.MaterialSpecification,
                        //成品，半成品，組件
                        ProductBasicId = item.ProductBasicId,
                        ProductName = item.Name,
                        ProductNo = item.ProductNo,
                        ProductNumber = item.ProductNumber,
                        ProductSpecification = item.ProductSpecification,
                        Ismaterial = item.Ismaterial,
                        Quantity = item.ReceiveQty,
                        CreateTime = dt,
                        CreateUser = MyFun.GetUserID(HttpContext)
                    });
                }

            }
            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(null));
        }

        /// <summary>
        /// 依照工單號建立領料單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Requisition>> PostRequisitionByWorkOrderNo(WorkOrderHead WorkOrderHead)
        {
            return Ok(await PostRequisitionByWorkOrderNoFun(WorkOrderHead, null));
        }
        /// <summary>
        /// 依照工單號建立領料單 主程式
        /// </summary>
        /// <param name="WorkOrderHead"></param>
        /// <param name="PostRequisition"></param>
        /// <returns></returns>
        private async Task<APIResponse> PostRequisitionByWorkOrderNoFun(WorkOrderHead WorkOrderHead, PostRequisition PostRequisition)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (string.IsNullOrEmpty(WorkOrderHead.WorkOrderNo))
            {
                return MyFun.APIResponseError("沒有工單資訊!");
            }
            var WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.WorkOrderNo == WorkOrderHead.WorkOrderNo && x.DeleteFlag == 0).ToListAsync();
            if (WorkOrderHeads.Count() == 1)
            {
                var WorkOrderHeadData = WorkOrderHeads.FirstOrDefault();
                var MaterialBasics = _context.MaterialBasics.Find(WorkOrderHeadData.DataId);

                var key = "MS";
                var dt = DateTime.Now;
                var RequisitionNo = dt.ToString("yyMMdd");

                var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0 && x.Type == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastRequisitionNo = NoData.FirstOrDefault().RequisitionNo;
                    var NoLast = Int32.Parse(LastRequisitionNo.Substring(LastRequisitionNo.Length - 3, 3));
                    if (NoCount <= NoLast)
                    {
                        NoCount = NoLast + 1;
                    }
                }
                var requisition = new Requisition();
                requisition.Type = 0;
                requisition.MaterialBasicId = WorkOrderHeadData.DataId;
                requisition.WorkOrderHeadId = WorkOrderHeadData.Id;
                requisition.RequisitionNo = key + RequisitionNo + NoCount.ToString("000");
                requisition.Name = "";
                requisition.MaterialNo = MaterialBasics.MaterialNo;
                requisition.MaterialNumber = MaterialBasics.MaterialNumber;
                requisition.Specification = MaterialBasics.Specification;
                requisition.Quantity = WorkOrderHeadData.Count;
                requisition.CreateTime = dt;
                requisition.ReceiveUser = PostRequisition?.ReceiveUser ?? null;
                requisition.CreateUser = PostRequisition.CreateUser;
                // requisition.CreateUser = MyFun.GetUserID(HttpContext);
                // BOM內容
                var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.MaterialBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
                foreach (var item in MyFun.GetBomList(BillOfMaterials, 0, requisition.Quantity))
                {
                    var sameCheck = false;
                    if (!string.IsNullOrWhiteSpace(item.ProductNo))
                    {   //判斷是否為成品
                        foreach (var item2 in requisition.RequisitionDetails)
                        {
                            if (item2.ProductBasicId == item.ProductBasicId &&
                                item2.ProductNo == item.ProductNo &&
                                item2.MaterialBasicId == item.MaterialBasicId &&
                                item2.MaterialNo == item.MaterialNo)
                            {
                                sameCheck = true;
                                item2.Quantity += item.ReceiveQty;
                            }
                        }
                    }
                    else
                    {    //判斷是否為原料
                        foreach (var item2 in requisition.RequisitionDetails)
                        {
                            if (item2.MaterialBasicId == item.MaterialBasicId &&
                                item2.MaterialNo == item.MaterialNo)
                            {
                                sameCheck = true;
                                item2.Quantity += item.ReceiveQty;
                            }
                        }
                    }

                    if (!sameCheck)
                    {
                        requisition.RequisitionDetails.Add(new RequisitionDetail
                        {
                            Lv = item.Lv,
                            // Name = item.Name,
                            //原料
                            MaterialBasicId = item.MaterialBasicId,
                            MaterialNo = item.MaterialNo,
                            MaterialName = item.MaterialName,
                            MaterialSpecification = item.MaterialSpecification,
                            //成品，半成品，組件
                            ProductBasicId = item.ProductBasicId,
                            ProductName = item.Name,
                            ProductNo = item.ProductNo,
                            ProductNumber = item.ProductNumber,
                            ProductSpecification = item.ProductSpecification,
                            Ismaterial = item.Ismaterial,
                            Quantity = item.ReceiveQty,
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext)
                        });
                    }

                }
                if (requisition.RequisitionDetails.Count() == 0)
                {
                    return MyFun.APIResponseError("新增失敗! [ " + MaterialBasics.MaterialNo + " ] 查無組成資訊!");
                }
                else
                {
                    _context.Requisitions.Add(requisition);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                    return MyFun.APIResponseOK(requisition, "領料單新增成功!");
                }
            }
            else
            {
                return MyFun.APIResponseError("工單資訊錯誤!");
            }
        }

        // DELETE: api/MaterialRequisitions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Requisition>> DeleteRequisition(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var Requisitions = await _context.Requisitions.FindAsync(id);
            if (Requisitions == null)
            {
                return NotFound();
            }
            else
            {
                var RequisitionDetails = await _context.RequisitionDetails.AsQueryable().Where(x => x.RequisitionId == id).ToListAsync();
                foreach (var item in RequisitionDetails)
                {
                    item.DeleteFlag = 1;
                    // _context.RequisitionDetails.Remove(item);
                }
                Requisitions.DeleteFlag = 1;
                // _context.Requisitions.Remove(Requisitions);
                await _context.SaveChangesAsync();
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(Requisitions));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailR>>> GetRequisitionsDetailMaterial(int id)
        {
            var RequisitionDetails = await _context.RequisitionDetails
            .Where(x => x.RequisitionId == id && x.DeleteFlag == 0 && x.MaterialBasicId.HasValue)
            .Select(x => new RequisitionDetailR
            {
                Id = x.Id,
                Name = x.Name,
                ProductNo = x.ProductNo,
                MaterialNo = x.MaterialNo,
                Quantity = x.Quantity,
                ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity),
                StockQty = x.MaterialBasic.Materials.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
            }).ToListAsync();
            return Ok(MyFun.APIResponseOK(RequisitionDetails));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAllShow>>> GetRequisitionsDetailMaterialByAllShow(int id)
        {
            try
            {
                var RequisitionDetails = await _context.RequisitionDetails
                .Where(x => x.RequisitionId == id && x.DeleteFlag == 0 && x.Lv == 1 && x.Receives.Any())
                .Select(x => new RequisitionDetailAllShow
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductBasicId = x.ProductBasicId,
                    ProductNo = x.ProductNo,
                    MaterialBasicId = x.MaterialBasicId,
                    MaterialNo = x.MaterialNo,
                    Quantity = x.Quantity,
                    ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity > 0).Sum(x => x.Quantity),
                    RbackQty = Math.Abs(x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity < 0).Sum(x => x.Quantity)),
                    NameNo = x.ProductBasicId.HasValue ? x.ProductNo : x.MaterialBasicId.HasValue ? x.MaterialNo : "",
                    NameType = x.ProductBasicId.HasValue ? "成品" : x.MaterialBasicId.HasValue ? "元件" : "",
                    //  WarehouseList=GetWarehouse(x)
                }).ToListAsync();

                return Ok(MyFun.APIResponseOK(RequisitionDetails));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 取出所有一階的組成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailMaterialByAll(RequisitionsDetailInfo RequisitionsDetailInfo)
        {
            try
            {
                var RequisitionDetails = await _context.RequisitionDetails
                .Where(x => x.RequisitionId == RequisitionsDetailInfo.RequisitionId && x.DeleteFlag == 0 && x.Lv == 1)
                .Select(x => new RequisitionDetailAll
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductBasicId = x.ProductBasicId,
                    ProductNo = x.ProductNo,
                    MaterialBasicId = x.MaterialBasicId,
                    MaterialNo = x.MaterialNo,
                    Quantity = x.Quantity,
                    ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity),
                    NameNo = x.ProductBasicId.HasValue ? x.ProductNo : x.MaterialBasicId.HasValue ? x.MaterialNo : "",
                    NameType = x.ProductBasicId.HasValue ? "成品" : x.MaterialBasicId.HasValue ? "元件" : "",
                    //  WarehouseList=GetWarehouse(x)
                }).ToListAsync();
                foreach (var item in RequisitionDetails.ToList())
                {
                    item.WarehouseList = GetWarehouse(item);
                }
                return Ok(MyFun.APIResponseOK(RequisitionDetails));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 取出品項所在的所有倉
        /// </summary>
        /// <param name="Req"></param>
        /// <returns></returns>
        private List<ReqWarehouse> GetWarehouse(RequisitionDetail Req)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var WarehouseList = new List<ReqWarehouse>();
            if (Req.MaterialBasicId.HasValue)
            {
                var MaterialBasicId = Req.MaterialBasicId;
                var GWarehouseList = _context.Materials.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.MaterialBasicId == MaterialBasicId).GroupBy(x => x.WarehouseId).ToList();
                foreach (var item in GWarehouseList)
                {
                    WarehouseList.Add(new ReqWarehouse
                    {
                        ID = item.Key,
                        Name = item.FirstOrDefault()?.Warehouse.Code + item.FirstOrDefault()?.Warehouse.Name,
                        StockQty = item.Sum(y => y.Quantity)
                    });
                }
            }
            else if (Req.ProductBasicId.HasValue)
            {
                var ProductBasicId = Req.ProductBasicId;
                // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                var GWarehouseList = _context.Materials.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.MaterialBasicId == Req.ProductBasicId).GroupBy(x => x.WarehouseId).ToList();
                foreach (var item in GWarehouseList)
                {
                    WarehouseList.Add(new ReqWarehouse
                    {
                        ID = item.Key,
                        Name = item.FirstOrDefault()?.Warehouse.Code + item.FirstOrDefault()?.Warehouse.Name,
                        StockQty = item.Sum(y => y.Quantity)
                    });
                }
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return WarehouseList;
        }

        /// <summary>
        /// 取出品項所在的所有倉ByPostBom
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetWarehouseByPostBom(PostBom PostBom)
        {
            try
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WarehouseList = new List<ReqWarehouse>();
                // if (PostBom.BasicType == 1)
                // {
                    var MaterialBasicId = PostBom.BasicId;
                    var GWarehouseList = _context.Materials.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.MaterialBasicId == MaterialBasicId).GroupBy(x => x.WarehouseId).ToList();
                    foreach (var item in GWarehouseList)
                    {
                        WarehouseList.Add(new ReqWarehouse
                        {
                            ID = item.Key,
                            Name = item.FirstOrDefault()?.Warehouse.Code + item.FirstOrDefault()?.Warehouse.Name,
                            StockQty = item.Sum(y => y.Quantity)
                        });
                    }
                // }
                // else if (PostBom.BasicType == 2)
                // {
                //     var ProductBasicId = PostBom.BasicId;
                //     var GWarehouseList = _context.Products.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.ProductBasicId == ProductBasicId).GroupBy(x => x.WarehouseId).ToList();
                //     foreach (var item in GWarehouseList)
                //     {
                //         WarehouseList.Add(new ReqWarehouse
                //         {
                //             ID = item.Key,
                //             Name = item.FirstOrDefault()?.Warehouse.Code + item.FirstOrDefault()?.Warehouse.Name,
                //             StockQty = item.Sum(y => y.Quantity)
                //         });
                //     }
                // }
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(WarehouseList));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 依照倉別過濾資訊
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<RequisitionDetailR>>> GetRequisitionsDetailMaterialByWarehouse(RequisitionsDetailInfo RequisitionsDetailInfo)
        {
            var RequisitionDetails = await _context.RequisitionDetails
            .Where(x => x.RequisitionId == RequisitionsDetailInfo.RequisitionId && x.DeleteFlag == 0 && x.MaterialBasicId.HasValue && x.Lv == 1)
            .Select(x => new RequisitionDetailR
            {
                Id = x.Id,
                Name = x.Name,
                ProductNo = x.ProductNo,
                MaterialNo = x.MaterialNo,
                Quantity = x.Quantity,
                ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity),
                StockQty = x.MaterialBasic.Materials.Where(y => y.DeleteFlag == 0 && y.WarehouseId == RequisitionsDetailInfo.WarehouseId).Sum(x => x.Quantity)
            }).ToListAsync();
            return Ok(MyFun.APIResponseOK(RequisitionDetails));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailR>>> GetRequisitionsDetailProduct(int id)
        {
            var RequisitionDetails = await _context.RequisitionDetails
            .Where(x => x.RequisitionId == id && x.DeleteFlag == 0 && !string.IsNullOrWhiteSpace(x.ProductNo))
            .Select(x => new RequisitionDetailR
            {
                Id = x.Id,
                Name = x.Name,
                ProductNo = x.ProductNo,
                MaterialNo = x.MaterialNo,
                Quantity = x.Quantity,
                ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity),
                StockQty = x.ProductBasic.Materials.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
            }).ToListAsync();
            return Ok(MyFun.APIResponseOK(RequisitionDetails));
        }

        /// <summary>
        /// 依照倉別過濾資訊
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<RequisitionDetailR>>> GetRequisitionsDetailProductByWarehouse(RequisitionsDetailInfo RequisitionsDetailInfo)
        {
            var RequisitionDetails = await _context.RequisitionDetails
            .Where(x => x.RequisitionId == RequisitionsDetailInfo.RequisitionId && x.DeleteFlag == 0 && !string.IsNullOrWhiteSpace(x.ProductNo) && x.Lv == 1)
            .Select(x => new RequisitionDetailR
            {
                Id = x.Id,
                Name = x.Name,
                ProductNo = x.ProductNo,
                MaterialNo = x.MaterialNo,
                Quantity = x.Quantity,
                ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity),
                StockQty = x.ProductBasic.Materials.Where(y => y.DeleteFlag == 0 && y.WarehouseId == RequisitionsDetailInfo.WarehouseId).Sum(x => x.Quantity)
            }).ToListAsync();
            return Ok(MyFun.APIResponseOK(RequisitionDetails));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequisitionsDetail(int id, [FromBody] GetReceive Receive)
        {
            if (Receive.WarehouseID == 0)
            {
                return Ok(MyFun.APIResponseError("請選擇倉別資訊!"));
            }
            var hasTake = false;
            var RequisitionDetail = _context.RequisitionDetails.Find(id);
            if (RequisitionDetail.Ismaterial != null)
            { //判斷是否為物料
                if (RequisitionDetail.Ismaterial ?? false)
                {
                    var Material = _context.Materials.Where(x => x.WarehouseId == Receive.WarehouseID && x.MaterialBasicId == RequisitionDetail.MaterialBasicId && x.DeleteFlag == 0).FirstOrDefault();
                    if (Material == null)
                    {
                        return Ok(MyFun.APIResponseError("沒有庫存資料! 請確認[ " + RequisitionDetail.MaterialNo + " ]的庫存資訊!"));
                    }
                    if (Receive.RQty > 0)
                    {
                        if (Receive.RQty > Material.Quantity)
                        {
                            return Ok(MyFun.APIResponseError("領用數量超過庫存數量! 原料[ " + RequisitionDetail.MaterialNo + " ]的庫存不足!"));
                        }
                        else
                        {
                            hasTake = true;
                            var dt = DateTime.Now;
                            RequisitionDetail.Receives.Add(new Receive
                            {
                                Quantity = Receive.RQty ?? 0,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            var Original = Material.Quantity;
                            Material.Quantity = Original - Receive.RQty ?? 0;
                            Material.UpdateTime = dt;
                            Material.UpdateUser = MyFun.GetUserID(HttpContext);
                            Material.MaterialLogs.Add(new MaterialLog
                            {
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "[工單領料]出庫",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                        }
                    }
                }
                else
                {
                    // 此API無使用
                    var Product = _context.Products.Where(x => x.WarehouseId == Receive.WarehouseID && x.ProductBasicId == RequisitionDetail.ProductBasicId && x.DeleteFlag == 0).FirstOrDefault();
                    if (Product == null)
                    {
                        return Ok(MyFun.APIResponseError("沒有庫存資料! 請確認[ " + RequisitionDetail.ProductNo + " ]的庫存資訊!"));
                    }
                    if (Receive.RQty > 0)
                    {
                        if (Receive.RQty > Product.Quantity)
                        {
                            return Ok(MyFun.APIResponseError("領用數量超過庫存數量! 成品[ " + RequisitionDetail.ProductNo + " ]的庫存不足!"));
                        }
                        else
                        {
                            hasTake = true;
                            var dt = DateTime.Now;
                            RequisitionDetail.Receives.Add(new Receive
                            {
                                Quantity = Receive.RQty ?? 0,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            var Original = Product.Quantity;
                            Product.Quantity = Original - Receive.RQty ?? 0;
                            Product.UpdateTime = dt;
                            Product.UpdateUser = MyFun.GetUserID(HttpContext);
                            Product.ProductLogs.Add(new ProductLog
                            {
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "[工單領料]出庫",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            if (hasTake)
            {
                return Ok(MyFun.APIResponseOK(RequisitionDetail, "完成領料!"));
            }
            else
            {
                return Ok(MyFun.APIResponseOK(RequisitionDetail));
            }
        }

        /// <summary>
        /// 同時領料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Receive"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequisitionsDetailAll(int id, [FromBody] GetReceive Receive)
        {
            return Ok(await PutRequisitionsDetailAllFun(id, Receive, "", MyFun.GetUserID(HttpContext)));
        }
        private async Task<APIResponse> PutRequisitionsDetailAllFun(int id, GetReceive Receive, string RequisitionNo, int CreateUser)
        {
            if (Receive.WarehouseID == 0)
            {
                return MyFun.APIResponseError("請選擇倉別資訊!");
            }
            var hasTake = false;
            var RequisitionDetail = _context.RequisitionDetails.Find(id);
            if (RequisitionDetail.Ismaterial != null)
            { //判斷是否為物料
                if (RequisitionDetail.Ismaterial ?? false)
                {
                    var Material = _context.Materials.Where(x => x.WarehouseId == Receive.WarehouseID && x.MaterialBasicId == RequisitionDetail.MaterialBasicId && x.DeleteFlag == 0).FirstOrDefault();
                    if (Material == null)
                    {
                        MyFun.APIResponseError("沒有庫存資料! 請確認[ " + RequisitionDetail.MaterialNo + " ]的庫存資訊!");
                    }
                    if (Receive.RQty > 0)
                    {
                        if (Receive.RQty > Material.Quantity)
                        {
                            MyFun.APIResponseError("領用數量超過庫存數量! 原料[ " + RequisitionDetail.MaterialNo + " ]的庫存不足!");
                        }
                        else
                        {
                            hasTake = true;
                            var dt = DateTime.Now;
                            RequisitionDetail.Receives.Add(new Receive
                            {
                                Quantity = Receive.RQty ?? 0,
                                WarehouseId = Receive.WarehouseID,
                                CreateTime = dt,
                                CreateUser = CreateUser
                            });
                            var Original = Material.Quantity;
                            Material.Quantity = Original - Receive.RQty ?? 0;
                            Material.UpdateTime = dt;
                            Material.UpdateUser = CreateUser;
                            Material.MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = RequisitionNo,
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "[工單領料]出庫",
                                CreateTime = dt,
                                CreateUser = CreateUser
                            });
                        }
                    }
                }
                else
                {
                    // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                    var Product = _context.Materials.Where(x => x.WarehouseId == Receive.WarehouseID && x.MaterialBasicId == RequisitionDetail.ProductBasicId && x.DeleteFlag == 0).FirstOrDefault();
                    if (Product == null)
                    {
                        return MyFun.APIResponseError("沒有庫存資料! 請確認[ " + RequisitionDetail.ProductNo + " ]的庫存資訊!");
                    }
                    if (Receive.RQty > 0)
                    {
                        if (Receive.RQty > Product.Quantity)
                        {
                            return MyFun.APIResponseError("領用數量超過庫存數量! 成品[ " + RequisitionDetail.ProductNo + " ]的庫存不足!");
                        }
                        else
                        {
                            hasTake = true;
                            var dt = DateTime.Now;
                            RequisitionDetail.Receives.Add(new Receive
                            {
                                Quantity = Receive.RQty ?? 0,
                                WarehouseId = Receive.WarehouseID,
                                CreateTime = dt,
                                CreateUser = CreateUser
                            });
                            var Original = Product.Quantity;
                            Product.Quantity = Original - Receive.RQty ?? 0;
                            Product.UpdateTime = dt;
                            Product.UpdateUser = CreateUser;
                            Product.MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = RequisitionNo,
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "[工單領料]出庫",
                                CreateTime = dt,
                                CreateUser = CreateUser
                            });
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            if (hasTake)
            {
                return MyFun.APIResponseOK(RequisitionDetail, "完成領料!");
            }
            else
            {
                return MyFun.APIResponseOK(RequisitionDetail);
            }
        }

        private async Task<APIResponse> NewRequisitionsDetail(GetReceive Receive, string RequisitionNo, int CreateUser)
        {
            // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
            var MaterialBasics = _context.MaterialBasics.Find(Receive.MaterialBasicId);
            var ProductBasics = _context.MaterialBasics.Find(Receive.ProductBasicId);
            var Requisitions = _context.Requisitions.Where(x => x.RequisitionNo == RequisitionNo).Include(x => x.RequisitionDetails).FirstOrDefault();
            Requisitions.RequisitionDetails.Add(new RequisitionDetail
            {
                Lv = 1,
                Ismaterial = Receive.MaterialBasicId.HasValue ? true : false,
                MaterialBasicId = MaterialBasics?.Id ?? null,
                MaterialName = MaterialBasics?.Name ?? null,
                MaterialNo = MaterialBasics?.MaterialNo ?? null,
                MaterialSpecification = MaterialBasics?.Specification ?? null, 
                ProductBasicId = ProductBasics?.Id ?? null,
                ProductNo = ProductBasics?.MaterialNo ?? null,
                ProductNumber = ProductBasics?.MaterialNumber ?? null,
                ProductName = ProductBasics?.Name ?? null,
                ProductSpecification = ProductBasics?.Specification ?? null,
                Quantity = Requisitions.Quantity,
                CreateUser = CreateUser
            });
            await _context.SaveChangesAsync();

            var RequisitionDetailsResult = _context.RequisitionDetails.Where(x => x.RequisitionId == Requisitions.Id && x.MaterialBasicId == Receive.MaterialBasicId && x.ProductBasicId == Receive.ProductBasicId).ToList();
            if (RequisitionDetailsResult.Count() != 1) {

            }
            return MyFun.APIResponseOK(RequisitionDetailsResult.FirstOrDefault().Id);
        }

        /// <summary>
        /// 用工單號取出可領的品號 一階
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailMaterialByWorkOrderNoId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var RequisitionDetailAllList = new List<RequisitionDetailAll>();
            try
            {
                // 有開過單的要抓數量
                var GRequisitionDetailAllList = _context.RequisitionDetails
                   .Where(x => x.Requisition.WorkOrderHeadId == id && x.DeleteFlag == 0 && x.Lv == 1)
                   .Select(x => new RequisitionDetailAll
                   {
                       Id = x.Id,
                       Name = x.Name,
                       ProductBasicId = x.ProductBasicId,
                       ProductNo = x.ProductNo,
                       MaterialBasicId = x.MaterialBasicId,
                       MaterialNo = x.MaterialNo,
                       Quantity = x.Quantity,
                       ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity > 0).Sum(x => x.Quantity),
                       RbackQty = x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity < 0).Sum(x => x.Quantity),
                       NameNo = x.ProductBasicId.HasValue ? x.ProductNo : x.MaterialBasicId.HasValue ? x.MaterialNo : "",
                       NameType = x.ProductBasicId.HasValue ? "成品" : x.MaterialBasicId.HasValue ? "元件" : "",
                       // WarehouseList = GetWarehouse(x)
                   }).ToList().GroupBy(x => x.NameNo);
                foreach (var item in GRequisitionDetailAllList)
                {
                    RequisitionDetailAllList.Add(new RequisitionDetailAll
                    {
                        Id = item.FirstOrDefault().Id,
                        Name = item.FirstOrDefault().Name,
                        ProductBasicId = item.FirstOrDefault().ProductBasicId,
                        ProductNo = item.FirstOrDefault().ProductNo,
                        MaterialBasicId = item.FirstOrDefault().MaterialBasicId,
                        MaterialNo = item.FirstOrDefault().MaterialNo,
                        Quantity = item.FirstOrDefault().Quantity,
                        ReceiveQty = item.Sum(x => x.ReceiveQty),
                        RbackQty = Math.Abs(item.Sum(x => x.RbackQty)),
                        NameNo = item.FirstOrDefault().NameNo,
                        NameType = item.FirstOrDefault().NameType,
                    });
                }
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var WorkOrderHeads = _context.WorkOrderHeads.Find(id);
                // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                // var ProductBasics = _context.MaterialBasics.Find(WorkOrderHeads.DataId);
                // BOM內容
                var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == WorkOrderHeads.DataId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
                var BomDataList = MyFun.GetBomList(BillOfMaterials, 0, WorkOrderHeads.Count);
                if (!RequisitionDetailAllList.Any())//沒開過領料單的要從頭抓
                {
                    foreach (var item in BomDataList)
                    {
                        if (item.Lv == 1)// 只取一階
                        {
                            RequisitionDetailAllList.Add(new RequisitionDetailAll
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Lv = item.Lv,
                                // Name = item.Name,
                                //原料
                                MaterialBasicId = item.MaterialBasicId,
                                MaterialNo = item.MaterialNo,
                                MaterialName = item.MaterialName,
                                MaterialSpecification = item.MaterialSpecification,
                                //成品，半成品，組件
                                ProductBasicId = item.ProductBasicId,
                                ProductName = item.Name,
                                ProductNo = item.ProductNo,
                                ProductNumber = item.ProductNumber,
                                ProductSpecification = item.ProductSpecification,
                                Ismaterial = item.Ismaterial,
                                Quantity = item.ReceiveQty,
                                NameNo = item.ProductBasicId.HasValue ? item.ProductNo : item.MaterialBasicId.HasValue ? item.MaterialNo : "",
                                NameType = item.ProductBasicId.HasValue ? "成品" : item.MaterialBasicId.HasValue ? "元件" : "",
                                Master = item.Master
                            });
                        }
                    }
                } else { // 已經開過的，更新master
                    foreach (var item in BomDataList)
                    {
                        if (item.Lv == 1)// 只取一階
                        {
                            foreach (var item2 in RequisitionDetailAllList)
                            {
                                if (item.MaterialBasicId == item2.MaterialBasicId && item.ProductBasicId == item2.ProductBasicId)
                                    item2.Master = item.Master;
                            }
                        }
                    }
                }
                _context.ChangeTracker.LazyLoadingEnabled = false;
                    
                foreach (var item in RequisitionDetailAllList)
                {
                    item.WarehouseList = GetWarehouse(item);
                }
                return Ok(MyFun.APIResponseOK(RequisitionDetailAllList));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 用品號取出可領的品號 一階
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailMaterialByProductBasicId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var RequisitionDetailAllList = new List<RequisitionDetailAll>();
            try
            {
                // 有開過單的要抓數量
                var GRequisitionDetailAllList = _context.RequisitionDetails
                   .Where(x => x.Requisition.MaterialBasicId == id && x.DeleteFlag == 0 && x.Lv == 1)
                   .Select(x => new RequisitionDetailAll
                   {
                       Id = x.Id,
                       Name = x.Name,
                       ProductBasicId = x.ProductBasicId,
                       ProductNo = x.ProductNo,
                       MaterialBasicId = x.MaterialBasicId,
                       MaterialNo = x.MaterialNo,
                       Quantity = x.Quantity,
                       ReceiveQty = x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity > 0).Sum(x => x.Quantity),
                       RbackQty = x.Receives.Where(y => y.DeleteFlag == 0 && y.Quantity < 0).Sum(x => x.Quantity),
                       NameNo = x.ProductBasicId.HasValue ? x.ProductNo : x.MaterialBasicId.HasValue ? x.MaterialNo : "",
                       NameType = x.ProductBasicId.HasValue ? "成品" : x.MaterialBasicId.HasValue ? "元件" : "",
                       // WarehouseList = GetWarehouse(x)
                   }).ToList().GroupBy(x => x.NameNo);
                foreach (var item in GRequisitionDetailAllList)
                {
                    RequisitionDetailAllList.Add(new RequisitionDetailAll
                    {
                        Id = item.FirstOrDefault().Id,
                        Name = item.FirstOrDefault().Name,
                        ProductBasicId = item.FirstOrDefault().ProductBasicId,
                        ProductNo = item.FirstOrDefault().ProductNo,
                        MaterialBasicId = item.FirstOrDefault().MaterialBasicId,
                        MaterialNo = item.FirstOrDefault().MaterialNo,
                        Quantity = item.Sum(x => x.Quantity),
                        ReceiveQty = item.Sum(x => x.ReceiveQty),
                        RbackQty = Math.Abs(item.Sum(x => x.RbackQty)),
                        NameNo = item.FirstOrDefault().NameNo,
                        NameType = item.FirstOrDefault().NameType,
                    });
                }
                _context.ChangeTracker.LazyLoadingEnabled = true;
                // 2020/10/27 品號合併(ProductBasicId使用MaterialBasic資料表)
                // var ProductBasics = _context.MaterialBasics.Find(WorkOrderHeads.DataId);
                // BOM內容
                var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == id && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
                var BomDataList = MyFun.GetBomList(BillOfMaterials, 0, 1);
                if (!RequisitionDetailAllList.Any())//沒開過領料單的要從頭抓
                {
                    foreach (var item in BomDataList)
                    {
                        if (item.Lv == 1)// 只取一階
                        {
                            RequisitionDetailAllList.Add(new RequisitionDetailAll
                            {
                                Id = item.Id,
                                Name = item.Name,
                                Lv = item.Lv,
                                // Name = item.Name,
                                //原料
                                MaterialBasicId = item.MaterialBasicId,
                                MaterialNo = item.MaterialNo,
                                MaterialName = item.MaterialName,
                                MaterialSpecification = item.MaterialSpecification,
                                //成品，半成品，組件
                                ProductBasicId = item.ProductBasicId,
                                ProductName = item.Name,
                                ProductNo = item.ProductNo,
                                ProductNumber = item.ProductNumber,
                                ProductSpecification = item.ProductSpecification,
                                Ismaterial = item.Ismaterial,
                                Quantity = item.ReceiveQty,
                                NameNo = item.ProductBasicId.HasValue ? item.ProductNo : item.MaterialBasicId.HasValue ? item.MaterialNo : "",
                                NameType = item.ProductBasicId.HasValue ? "成品" : item.MaterialBasicId.HasValue ? "元件" : "",
                                Master = item.Master
                            });
                        }
                    }
                }
                else
                { // 已經開過的，更新master
                    foreach (var item in BomDataList)
                    {
                        if (item.Lv == 1)// 只取一階
                        {
                            foreach (var item2 in RequisitionDetailAllList)
                            {
                                if (item.MaterialBasicId == item2.MaterialBasicId && item.ProductBasicId == item2.ProductBasicId)
                                    item2.Master = item.Master;
                            }
                        }
                    }
                }
                _context.ChangeTracker.LazyLoadingEnabled = false;

                foreach (var item in RequisitionDetailAllList)
                {
                    item.WarehouseList = GetWarehouse(item);
                }
                return Ok(MyFun.APIResponseOK(RequisitionDetailAllList));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 用工單號尋找領退料明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailByWorkOrderNoId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var RequisitionDetailAllList = new List<RequisitionDetailAll>();
            try
            {
                var ReceivesLog = await _context.Receives
                    .Where(x => x.RequisitionDetail.Requisition.WorkOrderHeadId == id && x.DeleteFlag == 0)
                    .Select(x => new RequisitionDetailLog
                    {
                        Id = x.Id,
                        RequisitionNo = x.RequisitionDetail.Requisition.RequisitionNo,
                        ReceiveQty = x.Quantity > 0 ? x.Quantity : 0,
                        RbackQty = x.Quantity < 0 ? Math.Abs(x.Quantity) : 0,
                        NameNo = x.RequisitionDetail.ProductBasicId.HasValue ? x.RequisitionDetail.ProductNo : x.RequisitionDetail.MaterialBasicId.HasValue ? x.RequisitionDetail.MaterialNo : "",
                        NameType = x.RequisitionDetail.ProductBasicId.HasValue ? "成品" : x.RequisitionDetail.MaterialBasicId.HasValue ? "元件" : "",
                        WarehouseName = x.WarehouseId.HasValue ? (x.Warehouse.Code + x.Warehouse.Name) : "",
                        CreateTime = x.CreateTime,
                        CreateUser = x.RequisitionDetail.Requisition.CreateUser,
                        ReceiveUser = x.RequisitionDetail.Requisition.ReceiveUser
                    }).ToListAsync();

                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(ReceivesLog));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 用品號尋找領退料明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailByProductBasicId(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var RequisitionDetailAllList = new List<RequisitionDetailAll>();
            try
            {
                var ReceivesLog = await _context.Receives
                    .Where(x => x.RequisitionDetail.Requisition.MaterialBasicId == id && x.DeleteFlag == 0)
                    .Select(x => new RequisitionDetailLog
                    {
                        Id = x.Id,
                        RequisitionNo = x.RequisitionDetail.Requisition.RequisitionNo,
                        ReceiveQty = x.Quantity > 0 ? x.Quantity : 0,
                        RbackQty = x.Quantity < 0 ? Math.Abs(x.Quantity) : 0,
                        NameNo = x.RequisitionDetail.ProductBasicId.HasValue ? x.RequisitionDetail.ProductNo : x.RequisitionDetail.MaterialBasicId.HasValue ? x.RequisitionDetail.MaterialNo : "",
                        NameType = x.RequisitionDetail.ProductBasicId.HasValue ? "成品" : x.RequisitionDetail.MaterialBasicId.HasValue ? "元件" : "",
                        WarehouseName = x.WarehouseId.HasValue ? (x.Warehouse.Code + x.Warehouse.Name) : "",
                        CreateTime = x.CreateTime,
                        CreateUser = x.RequisitionDetail.Requisition.CreateUser,
                        ReceiveUser = x.RequisitionDetail.Requisition.ReceiveUser
                    }).ToListAsync();

                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(ReceivesLog));
            }
            catch (System.Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
                throw;
            }
        }

        /// <summary>
        /// 新增領料單，同時領料
        /// </summary>
        /// <param name="PostRequisition"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostRequisitionsDetailAll(PostRequisition PostRequisition)
        {
            if (PostRequisition.WorkOrderNo == 0)
            {
                return Ok(MyFun.APIResponseError("沒有工單資訊!"));
            }
            if (!PostRequisition.ReceiveList.Any())
            {
                return Ok(MyFun.APIResponseError("沒有領料資料"));
            }
            else
            {
                if (PostRequisition.ReceiveList.Where(x => (x.RQty.HasValue && !x.WarehouseID.HasValue) || !x.RQty.HasValue && x.WarehouseID.HasValue).Any())
                {
                    return Ok(MyFun.APIResponseError("數量錯誤或倉庫錯誤"));
                }
                else if (!PostRequisition.ReceiveList.Where(x => x.RQty.HasValue && x.WarehouseID.HasValue).Any())
                {
                    return Ok(MyFun.APIResponseError("請輸入要領料的倉別和數量"));
                }
            }
            if (!PostRequisition.ReceiveList.Any())
            {
                return Ok(MyFun.APIResponseError("沒有領料資料"));
            }
            else
            {
                if (PostRequisition.ReceiveList.Where(x => (x.RQty.HasValue && !x.WarehouseID.HasValue) || !x.RQty.HasValue && x.WarehouseID.HasValue).Any())
                {
                    return Ok(MyFun.APIResponseError("數量錯誤或倉庫錯誤"));
                }
            }
            var WorkOrderHead = _context.WorkOrderHeads.Where(x => x.Id == PostRequisition.WorkOrderNo).Include(x => x.WorkOrderDetails).First();
            var head = PostRequisitionByWorkOrderNoFun(WorkOrderHead, PostRequisition);
            if (head.Result.success)
            {
                var Requisition = (Requisition)head.Result.data;
                foreach (var item in PostRequisition.ReceiveList)
                {
                    if (item.RQty.HasValue && item.RQty > 0 && item.WarehouseID.HasValue)
                    {
                        var id = 0;
                        if (item.ProductBasicId.HasValue)
                        {
                            id = Requisition.RequisitionDetails.Where(x => x.ProductBasicId == item.ProductBasicId).FirstOrDefault()?.Id ?? 0;
                        }
                        if (item.MaterialBasicId.HasValue)
                        {
                            id = Requisition.RequisitionDetails.Where(x => x.MaterialBasicId == item.MaterialBasicId).FirstOrDefault()?.Id ?? 0;
                        }
                        if (id != 0)
                        {
                            var Detail = PutRequisitionsDetailAllFun(id, item, Requisition.RequisitionNo, PostRequisition.CreateUser);
                            if (!Detail.Result.success)
                            {
                                return Ok(await Detail);
                            }
                        }
                        else
                        {
                            var NewDetail = NewRequisitionsDetail(item, Requisition.RequisitionNo, PostRequisition.CreateUser);
                            if (!NewDetail.Result.success)
                            {
                                return Ok(await NewDetail);
                            }
                            var NewDetailData = (int)NewDetail.Result.data;
                            var Detail = PutRequisitionsDetailAllFun(NewDetailData, item, Requisition.RequisitionNo, PostRequisition.CreateUser);
                            if (!Detail.Result.success)
                            {
                                return Ok(await Detail);
                            }
                        }

                        //// 2020/10/19 領料完成自動回報(報工) 工單第一站工序(AMA)。
                        //// 2021/11/09 領料完成自動回報(報工) 工單的領料工序(BBZ)，生管說上面這位理解錯誤。
                        var dt = DateTime.Now;
                        var FirstWorkOrderDetails = WorkOrderHead.WorkOrderDetails.Where(x => x.ProcessNo == "BBZ" && x.DeleteFlag == 0).FirstOrDefault();
                        if (FirstWorkOrderDetails != null)
                        {
                            if (FirstWorkOrderDetails.ActualStartTime == null)
                            {
                                FirstWorkOrderDetails.ActualStartTime = dt;
                            }
                            FirstWorkOrderDetails.ActualEndTime = dt;
                            FirstWorkOrderDetails.Status = 3; // 完工

                            // 如果[工單Head]狀態為[已派工]，則改為[以開工]
                            if (FirstWorkOrderDetails.WorkOrderHead.Status == 1)
                            {
                                FirstWorkOrderDetails.WorkOrderHead.Status = 2;
                            }

                            FirstWorkOrderDetails.WorkOrderReportLogs.Add(new WorkOrderReportLog
                            {
                                // WorkOrderDetailId = FirstWorkOrderDetails.Id,
                                ReportType = 2, // 完工回報
                                                // PurchaseId = FirstWorkOrderDetails.PurchaseId,
                                                // PurchaseNo = FirstWorkOrderDetails.,
                                                // SupplierId = FirstWorkOrderDetails.SupplierId,
                                                // DrawNo = FirstWorkOrderDetails.DrawNo,
                                                // CodeNo = WorkOrderReportData.CodeNo,
                                                // Manpower = FirstWorkOrderDetails.Manpower,
                                                // ProducingMachineId = FirstWorkOrderDetails.,
                                                // ProducingMachine = WorkOrderReportData.ProducingMachine,
                                                // ReCount = WorkOrderReportData.ReCount,
                                                // RePrice = WorkOrderReportData.RePrice,
                                                // NgCount = WorkOrderReportData.NgCount,
                                                // Message = WorkOrderReportData.Message,
                                StatusO = 0,
                                StatusN = 3,
                                DueStartTime = FirstWorkOrderDetails.DueStartTime,
                                DueEndTime = FirstWorkOrderDetails.DueEndTime,
                                ActualStartTime = dt,
                                ActualEndTime = dt,
                                CreateTime = dt,
                                CreateUser = PostRequisition.CreateUser,
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(await head);
            }
            else
            {
                return Ok(await head);
            }
        }

        private bool RequisitionExists(int id)
        {
            return _context.Requisitions.Any(e => e.Id == id);
        }
    }
}