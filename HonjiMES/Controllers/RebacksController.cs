using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Mvc;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 退料資料
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RebacksController : ControllerBase
    {
        private readonly HonjiContext _context;

        public RebacksController(HonjiContext context)
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
        public async Task<ActionResult<IEnumerable<Requisition>>> GetRebacks([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(_context.Requisitions.AsQueryable().Where(x => x.DeleteFlag == 0 && x.Type == 1).Include(x => x.WorkOrderHead), FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Rebacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requisition>> GetReback(int id)
        {
            var materialRequisition = await _context.Requisitions.FindAsync(id);

            if (materialRequisition == null)
            {
                return NotFound();
            }

            //return materialRequisition;
            return Ok(MyFun.APIResponseOK(materialRequisition));
        }

        // PUT: api/MaterialRebacks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReback(int id, Requisition Requisition)
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
                if (!RebackExists(id))
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

        // POST: api/MaterialRebacks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Requisition>> PostReback(Requisition requisition)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (requisition.MaterialBasicId == 0)
            {
                return Ok(MyFun.APIResponseError("沒有品號資料"));
            }
            // 此API無使用
            var ProductBasics = _context.ProductBasics.Find(requisition.MaterialBasicId);

            var key = "MR";
            var dt = DateTime.Now;
            var RequisitionNo = dt.ToString("yyMMdd");

            var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0 && x.Type == 1).OrderByDescending(x => x.CreateTime).ToListAsync();
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
        /// 用工單號取出可退的品號 一階
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<RequisitionDetailAll>>> GetRebacksDetailMaterialByWorkOrderNoId(int id)
        {
            var RequisitionDetailAllList = new List<RequisitionDetailAll>();
            _context.ChangeTracker.LazyLoadingEnabled = true;
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

                    var WorkOrderHeads = _context.WorkOrderHeads.Find(id);
                    var MaterialBasics = _context.MaterialBasics.Find(WorkOrderHeads.DataId);
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
                                Master = item.Master
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
        /// 新增退料單，同時退料
        /// </summary>
        /// <param name="PostRequisition"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostRebacksDetailAll(PostRequisition PostRequisition)
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
                    return Ok(MyFun.APIResponseError("請輸入要退料的倉別和數量"));
                }
            }
            var WorkOrderHead = _context.WorkOrderHeads.Find(PostRequisition.WorkOrderNo);
            if (WorkOrderHead == null)
            {
                return Ok(MyFun.APIResponseError("工單資訊錯誤!"));
            }
            else
            {
                _context.ChangeTracker.LazyLoadingEnabled = true;
                var MaterialBasics = _context.MaterialBasics.Find(WorkOrderHead.DataId);
                var key = "MR";
                var dt = DateTime.Now;
                var RequisitionNo = dt.ToString("yyMMdd");

                var NoData = await _context.Requisitions.AsQueryable().Where(x => x.RequisitionNo.Contains(key + RequisitionNo) && x.DeleteFlag == 0 && x.Type == 1).OrderByDescending(x => x.CreateTime).ToListAsync();
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
                var requisition = new Requisition();//表頭
                requisition.Type = 1;
                requisition.MaterialBasicId = WorkOrderHead.DataId;
                requisition.WorkOrderHeadId = WorkOrderHead.Id;
                requisition.RequisitionNo = key + RequisitionNo + NoCount.ToString("000");
                requisition.Name = "";
                requisition.MaterialNo = MaterialBasics.MaterialNo;
                requisition.MaterialNumber = MaterialBasics.MaterialNumber;
                requisition.Specification = MaterialBasics.Specification;
                requisition.Quantity = WorkOrderHead.Count;
                requisition.CreateTime = dt;
                requisition.CreateUser = PostRequisition.CreateUser;
                // BOM內容
                var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.MaterialBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
                foreach (var item in MyFun.GetBomList(BillOfMaterials, 0, requisition.Quantity))
                {
                    if (item.Lv == 1)
                    {
                        var Receive = PostRequisition.ReceiveList.Where(x => x.RQty.HasValue && x.WarehouseID.HasValue &&
                        ((item.MaterialBasicId.HasValue && x.MaterialBasicId == item.MaterialBasicId) || (item.ProductBasicId.HasValue && x.ProductBasicId == item.ProductBasicId))).FirstOrDefault();
                        if (Receive != null)
                        {
                            if (Receive.RQty > 0)
                            {
                                //新增子表
                                var nRequisitionDetail = new RequisitionDetail
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
                                    CreateUser = PostRequisition.CreateUser
                                };
                                nRequisitionDetail.Receives.Add(new Receive
                                {
                                    Quantity = -Receive.RQty ?? 0,
                                    WarehouseId = Receive.WarehouseID,
                                    CreateTime = dt,
                                    CreateUser = PostRequisition.CreateUser
                                });
                                requisition.RequisitionDetails.Add(nRequisitionDetail);
                                //新增LOG
                                if (Receive.MaterialBasicId.HasValue)
                                {
                                    var Material = _context.Materials.Where(x => x.WarehouseId == Receive.WarehouseID && x.MaterialBasicId == Receive.MaterialBasicId && x.DeleteFlag == 0).FirstOrDefault();
                                    if (Material == null)
                                    {
                                        return Ok(MyFun.APIResponseError("沒有庫存資料! 請確認[ " + item.MaterialNo + " ]的庫存資訊!"));
                                    }
                                    var Original = Material.Quantity;
                                    Material.Quantity = Original + Receive.RQty ?? 0;
                                    Material.UpdateTime = dt;
                                    Material.UpdateUser = PostRequisition.CreateUser;
                                    Material.MaterialLogs.Add(new MaterialLog
                                    {
                                        LinkOrder = requisition.RequisitionNo,
                                        Original = Original,
                                        Quantity = Receive.RQty ?? 0,
                                        Message = "退料入庫",
                                        CreateTime = dt,
                                        CreateUser = PostRequisition.CreateUser
                                    });
                                }
                                else if (Receive.ProductBasicId.HasValue)
                                {
                                    var Product = _context.Materials.Where(x => x.WarehouseId == Receive.WarehouseID && x.MaterialBasicId == Receive.ProductBasicId && x.DeleteFlag == 0).FirstOrDefault();
                                    if (Product == null)
                                    {
                                        return Ok(MyFun.APIResponseError("沒有庫存資料! 請確認[ " + item.ProductNo + " ]的庫存資訊!"));
                                    }
                                    var Original = Product.Quantity;
                                    Product.Quantity = Original + Receive.RQty ?? 0;
                                    Product.UpdateTime = dt;
                                    Product.UpdateUser = PostRequisition.CreateUser;
                                    Product.MaterialLogs.Add(new MaterialLog
                                    {
                                        LinkOrder = requisition.RequisitionNo,
                                        Original = Original,
                                        Quantity = Receive.RQty ?? 0,
                                        Message = "退料入庫",
                                        CreateTime = dt,
                                        CreateUser = PostRequisition.CreateUser
                                    });
                                }
                            }
                            
                        }
                    }
                }
                try
                {
                    _context.Requisitions.Add(requisition);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Ok(MyFun.APIResponseError(ex.Message));
                }
                _context.ChangeTracker.LazyLoadingEnabled = false;
                return Ok(MyFun.APIResponseOK(requisition, "退料單新增成功!"));
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
                        Name = item.FirstOrDefault()?.Warehouse.Name,
                        StockQty = item.Sum(y => y.Quantity)
                    });
                }
            }
            else if (Req.ProductBasicId.HasValue)
            {
                var ProductBasicId = Req.ProductBasicId;
                var GWarehouseList = _context.Materials.AsEnumerable().Where(y => y.DeleteFlag == 0 && y.MaterialBasicId == Req.ProductBasicId).GroupBy(x => x.WarehouseId).ToList();
                foreach (var item in GWarehouseList)
                {
                    WarehouseList.Add(new ReqWarehouse
                    {
                        ID = item.Key,
                        Name = item.FirstOrDefault()?.Warehouse.Name,
                        StockQty = item.Sum(y => y.Quantity)
                    });
                }
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return WarehouseList;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<RequisitionDetailAllShow>>> GetRebacksDetailMaterialByAllShow(int id)
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

        private bool RebackExists(int id)
        {
            return _context.Requisitions.Any(e => e.Id == id);
        }
    }
}
