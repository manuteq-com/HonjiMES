using System;
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
            if (requisition.ProductBasicId == 0)
            {
                return Ok(MyFun.APIResponseError("沒有品號資料"));
            }
            var ProductBasics = _context.ProductBasics.Find(requisition.ProductBasicId);

            var key = "PK";
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
            requisition.ProductNo = ProductBasics.ProductNo;
            requisition.ProductNumber = ProductBasics.ProductNumber;
            requisition.Specification = ProductBasics.Specification;
            requisition.CreateTime = dt;
            requisition.CreateUser = MyFun.GetUserID(HttpContext);
            requisition.Type = 0;
            // BOM內容
            var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.ProductBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
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
            return Ok(await PostRequisitionByWorkOrderNoFun(WorkOrderHead));
        }
        /// <summary>
        /// 依照工單號建立領料單 主程式
        /// </summary>
        /// <param name="WorkOrderHead"></param>
        /// <returns></returns>
        private async Task<APIResponse> PostRequisitionByWorkOrderNoFun(WorkOrderHead WorkOrderHead)
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
                var ProductBasics = _context.ProductBasics.Find(WorkOrderHeadData.DataId);

                var key = "PK";
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
                requisition.ProductBasicId = WorkOrderHeadData.DataId;
                requisition.WorkOrderHeadId = WorkOrderHeadData.Id;
                requisition.RequisitionNo = key + RequisitionNo + NoCount.ToString("000");
                requisition.Name = "";
                requisition.ProductNo = ProductBasics.ProductNo;
                requisition.ProductNumber = ProductBasics.ProductNumber;
                requisition.Specification = ProductBasics.Specification;
                requisition.Quantity = WorkOrderHeadData.Count;
                requisition.CreateTime = dt;
                requisition.CreateUser = MyFun.GetUserID(HttpContext);
                // BOM內容
                var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.ProductBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
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
                    return MyFun.APIResponseError("新增失敗! [ " + ProductBasics.ProductNo + " ] 查無組成資訊!");
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
                .Where(x => x.RequisitionId == id && x.DeleteFlag == 0 && x.Lv == 1)
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
                var GWarehouseList = _context.Products.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.ProductBasicId == Req.ProductBasicId).GroupBy(x => x.WarehouseId).ToList();
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
                StockQty = x.ProductBasic.Products.Where(y => y.DeleteFlag == 0).Sum(x => x.Quantity)
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
                StockQty = x.ProductBasic.Products.Where(y => y.DeleteFlag == 0 && y.WarehouseId == RequisitionsDetailInfo.WarehouseId).Sum(x => x.Quantity)
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
                                Message = "領料出庫",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                        }
                    }
                }
                else
                {
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
                                Message = "領料出庫",
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
            return Ok(await PutRequisitionsDetailAllFun(id, Receive, ""));
        }
        private async Task<APIResponse> PutRequisitionsDetailAllFun(int id, GetReceive Receive, string RequisitionNo)
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
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            var Original = Material.Quantity;
                            Material.Quantity = Original - Receive.RQty ?? 0;
                            Material.UpdateTime = dt;
                            Material.UpdateUser = MyFun.GetUserID(HttpContext);
                            Material.MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = RequisitionNo,
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "領料出庫",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                        }
                    }
                }
                else
                {
                    var Product = _context.Products.Where(x => x.WarehouseId == Receive.WarehouseID && x.ProductBasicId == RequisitionDetail.ProductBasicId && x.DeleteFlag == 0).FirstOrDefault();
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
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            var Original = Product.Quantity;
                            Product.Quantity = Original - Receive.RQty ?? 0;
                            Product.UpdateTime = dt;
                            Product.UpdateUser = MyFun.GetUserID(HttpContext);
                            Product.ProductLogs.Add(new ProductLog
                            {
                                LinkOrder = RequisitionNo,
                                Original = Original,
                                Quantity = -Receive.RQty ?? 0,
                                Message = "領料出庫",
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
                return MyFun.APIResponseOK(RequisitionDetail, "完成領料!");
            }
            else
            {
                return MyFun.APIResponseOK(RequisitionDetail);
            }
        }
        /// <summary>
        /// 用工單號取出可領的料號 一階
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRequisitionsDetailMaterialByWorkOrderNo(int id)
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
                if (!RequisitionDetailAllList.Any())//沒開過的要從頭抓
                {
                    _context.ChangeTracker.LazyLoadingEnabled = true;
                    var WorkOrderHeads = _context.WorkOrderHeads.Find(id);
                    var ProductBasics = _context.ProductBasics.Find(WorkOrderHeads.DataId);
                    // BOM內容
                    var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == WorkOrderHeads.DataId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();

                    foreach (var item in MyFun.GetBomList(BillOfMaterials, 0, WorkOrderHeads.Count))
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
                            });
                        }
                    }
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                }
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
            var WorkOrderHead = _context.WorkOrderHeads.Find(PostRequisition.WorkOrderNo);
            var head = PostRequisitionByWorkOrderNoFun(WorkOrderHead);
            if (head.Result.success)
            {
                var Requisition = (Requisition)head.Result.data;
                foreach (var item in PostRequisition.ReceiveList)
                {
                    if (item.RQty.HasValue && item.WarehouseID.HasValue)
                    {

                        var id = 0;
                        if (item.ProductBasicId.HasValue)
                        {
                            id = Requisition.RequisitionDetails.Where(x => x.ProductBasicId == item.ProductBasicId).FirstOrDefault().Id;
                        }
                        if (item.MaterialBasicId.HasValue)
                        {
                            id = Requisition.RequisitionDetails.Where(x => x.MaterialBasicId == item.MaterialBasicId).FirstOrDefault().Id;
                        }
                        var Detail = PutRequisitionsDetailAllFun(id, item, Requisition.RequisitionNo);
                        if (!Detail.Result.success)
                        {
                            return Ok(await Detail);
                        }
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