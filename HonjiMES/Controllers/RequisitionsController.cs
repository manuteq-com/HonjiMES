using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 領料管理
    /// </summary>
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
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(_context.Requisitions.AsQueryable().Where(x => x.DeleteFlag == 0).Include(x => x.WorkOrderHead), FromQuery);
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

            var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
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
            requisition.CreateUser = 1;
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
                        CreateUser = 1
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
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (string.IsNullOrEmpty(WorkOrderHead.WorkOrderNo))
            {
                return Ok(MyFun.APIResponseError("沒有工單資訊!"));
            }
            var WorkOrderHeads = await _context.WorkOrderHeads.Where(x => x.WorkOrderNo == WorkOrderHead.WorkOrderNo && x.DeleteFlag == 0).ToListAsync();
            if (WorkOrderHeads.Count() == 1)
            {
                var WorkOrderHeadData = WorkOrderHeads.FirstOrDefault();
                var ProductBasics = _context.ProductBasics.Find(WorkOrderHeadData.DataId);

                var key = "PK";
                var dt = DateTime.Now;
                var RequisitionNo = dt.ToString("yyMMdd");

                var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
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
                requisition.ProductBasicId = WorkOrderHeadData.DataId;
                requisition.WorkOrderHeadId = WorkOrderHeadData.Id;
                requisition.RequisitionNo = key + RequisitionNo + NoCount.ToString("000");
                requisition.Name = "";
                requisition.ProductNo = ProductBasics.ProductNo;
                requisition.ProductNumber = ProductBasics.ProductNumber;
                requisition.Specification = ProductBasics.Specification;
                requisition.CreateTime = dt;
                requisition.CreateUser = 1;
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
                            CreateUser = 1
                        });
                    }

                }
                if (requisition.RequisitionDetails.Count() == 0)
                {
                    return Ok(MyFun.APIResponseError("新增失敗! [ " + ProductBasics.ProductNo + " ] 查無組成資訊!"));
                }
                else
                {
                    _context.Requisitions.Add(requisition);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.LazyLoadingEnabled = false;
                    return Ok(MyFun.APIResponseOK("OK", "領料單新增成功!"));
                }
            }
            else
            {
                return Ok(MyFun.APIResponseError("工單資訊錯誤!"));
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
                                Quantity = Receive.RQty,
                                CreateTime = dt,
                                CreateUser = 1
                            });
                            var Original = Material.Quantity;
                            Material.Quantity = Original - Receive.RQty;
                            Material.UpdateTime = dt;
                            Material.UpdateUser = 1;
                            Material.MaterialLogs.Add(new MaterialLog
                            {
                                Original = Original,
                                Quantity = -Receive.RQty,
                                Message = "領料出庫",
                                CreateTime = dt,
                                CreateUser = 1
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
                                Quantity = Receive.RQty,
                                CreateTime = dt,
                                CreateUser = 1
                            });
                            var Original = Product.Quantity;
                            Product.Quantity = Original - Receive.RQty;
                            Product.UpdateTime = dt;
                            Product.UpdateUser = 1;
                            Product.ProductLogs.Add(new ProductLog
                            {
                                Original = Original,
                                Quantity = -Receive.RQty,
                                Message = "領料出庫",
                                CreateTime = dt,
                                CreateUser = 1
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

        private bool RequisitionExists(int id)
        {
            return _context.Requisitions.Any(e => e.Id == id);
        }
    }
}
