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
    /// 調整紀錄
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdjustsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public AdjustsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/Adjusts
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdjustHead>>> GetAdjustLists(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.AdjustHeads.Where(x => x.DeleteFlag == 0);
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 用父ID查調整單明細
        /// </summary>
        /// <param name="Pid">父ID</param>
        /// <returns></returns>
        public async Task<ActionResult<AdjustDetailData>> GetAdjustDetailByPId(int Pid)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.AdjustDetails.AsQueryable().Where(x => x.AdjustHeadId == Pid).ToListAsync();
            var AdjustDetailData = new List<AdjustDetailData>();
            var tempId = 1;
            foreach (var item in data)
            {
                var BasicData = new BasicData();
                // if (item.ItemType == 1)
                // {
                    var tempInfo = _context.Materials.Find(item.ItemId);
                    BasicData.DataNo = tempInfo.MaterialNo;
                    BasicData.Name = tempInfo.Name;
                    BasicData.WarehouseId = tempInfo.WarehouseId;
                // }
                // else if (item.ItemType == 2)
                // {
                //     var tempInfo = _context.Products.Find(item.ItemId);
                //     BasicData.DataNo = tempInfo.ProductNo;
                //     BasicData.Name = tempInfo.Name;
                //     BasicData.WarehouseId = tempInfo.WarehouseId;
                // }
                // else if (item.ItemType == 3)
                // {
                //     var tempInfo = _context.Wiproducts.Find(item.ItemId);
                //     BasicData.DataNo = tempInfo.WiproductNo;
                //     BasicData.Name = tempInfo.Name;
                //     BasicData.WarehouseId = tempInfo.WarehouseId;
                // }
                AdjustDetailData.Add(new AdjustDetailData
                {
                    TempId = tempId,
                    DataType = item.ItemType,
                    DataId = item.ItemId,
                    DataNo = BasicData.DataNo,
                    DataName = BasicData.Name,
                    WarehouseId = BasicData?.WarehouseId ?? 0,
                    Original = item.Original,
                    AftQuantity = item.Original + item.Quantity,
                    Quantity = item.Quantity,
                    Price = item?.Price ?? 0,
                    PriceAll = item?.PriceAll ?? 0,
                    Unit = item.Unit,
                    UnitCount = item?.UnitCount ?? 0,
                    UnitPrice = item?.UnitPrice ?? 0,
                    UnitPriceAll = item?.UnitPriceAll ?? 0,
                    WorkPrice = item?.WorkPrice ?? 0,
                    Remark = item.Reason,
                });
                tempId++;
            }
            return Ok(MyFun.APIResponseOK(AdjustDetailData));
        }

        // GET: api/Adjusts
        /// <summary>
        /// 調整單種類列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllStockLog>>> GetAdjustType()
        {
            var data = await _context.AllStockLogs.OrderBy(x => x.Message).ToListAsync();

            var AdjustTypes = new List<AdjustType>();
            var index = 0;
            AdjustTypes.Add(new AdjustType
            {
                Message = "全部資料"
            });
            foreach (var item in data.GroupBy(x => x.Message).ToList())
            {
                AdjustTypes.Add(new AdjustType
                {
                    Id = index,
                    Message = item.Key
                });
                index++;
            }

            return Ok(MyFun.APIResponseOK(AdjustTypes));
        }

        // GET: api/Adjusts
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllStockLog>>> GetAdjustLog(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.AllStockLogs.Where(x => x.DeleteFlag == 0);
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Adjusts/5
        /// <summary>
        /// 查詢調整紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetAdjustLog(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var allStockLog = await _context.AllStockLogs.FindAsync(id);

            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }

        // GET: api/Adjusts/5
        /// <summary>
        /// 查詢調整紀錄 By ProductBasicID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetAdjustLogByProductBasicID1(int id)
        {
            var allStockLog = await _context.AllStockLogs.Where(x => x.NameType == 2 && x.DataId == id).ToListAsync();
            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }

        /// <summary>
        /// 查詢調整紀錄 By MaterialBasicID
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<AdjustInfo>> GetAdjustLogByMaterialBasicID(int id)
        {
            var ptype = typeof(AdjustInfoData);
            var AdjustInfo = new AdjustInfo();
            var ColumnOptionlist = new List<ColumnOption>();
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            foreach (var item in Warehouses)
            {
                ColumnOptionlist.Add(new ColumnOption
                {
                    key = "Temp" + item.Id.ToString(),
                    title = item.Code,
                    show = true,
                    Columnlock = ""
                });
            }

            _context.ChangeTracker.LazyLoadingEnabled = true;
            var AdjustInfoDataList = new List<AdjustInfoData>();
            try
            {
                var MaterialBasics = await _context.MaterialBasics.FindAsync(id);
                var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
                var AllStockLog = new List<AllStockLog>();
                AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 1 && x.DataNo == MaterialBasics.MaterialNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

                foreach (var item in AllStockLog)
                {
                    var nAdjustInfoData = new AdjustInfoData
                    {
                        Key = item.Id,
                        LinkOrder = item.LinkOrder,
                        BasicDataName = item.DataName,
                        BasicDataNo = item.DataNo,
                        Original = item.Original,
                        Quantity = item.Quantity,
                        PriceAll = item?.PriceAll ?? 0,
                        Unit = item.Unit,
                        UnitCount = item?.UnitCount ?? 0,
                        UnitPrice = item?.UnitPrice ?? 0,
                        UnitPriceAll = item?.UnitPriceAll ?? 0,
                        WorkPrice = item?.WorkPrice ?? 0,
                        Reason = item.Reason,
                        Message = item.Message,
                        WarehouseId = item?.WarehouseId ?? 0,
                        CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
                        CreateTime = item.CreateTime,
                    };
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
                        {
                            var nTempString = new TempString
                            {
                                value0 = item.Quantity.ToString(),
                            };
                            typeitem.SetValue(nAdjustInfoData, nTempString);
                            // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
                            // {
                            //     Columnitem.show = true;
                            // }
                        }
                    }
                    AdjustInfoDataList.Add(nAdjustInfoData);
                }
                AdjustInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
                AdjustInfo.AdjustInfoData = AdjustInfoDataList;
            }
            catch (System.Exception ex)
            {
                throw;
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(AdjustInfo));
        }

        /// <summary>
        /// 查詢調整紀錄 By WiproductBasicID
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<AdjustInfo>> GetAdjustLogByWiproductBasicID(int id)
        {
            var ptype = typeof(AdjustInfoData);
            var AdjustInfo = new AdjustInfo();
            var ColumnOptionlist = new List<ColumnOption>();
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            foreach (var item in Warehouses)
            {
                ColumnOptionlist.Add(new ColumnOption
                {
                    key = "Temp" + item.Id.ToString(),
                    title = item.Code,
                    show = true,
                    Columnlock = ""
                });
            }

            _context.ChangeTracker.LazyLoadingEnabled = true;
            var AdjustInfoDataList = new List<AdjustInfoData>();
            try
            {
                var WiproductBasics = await _context.WiproductBasics.FindAsync(id);
                var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
                var AllStockLog = new List<AllStockLog>();
                AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 3 && x.DataNo == WiproductBasics.WiproductNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

                foreach (var item in AllStockLog)
                {
                    var nAdjustInfoData = new AdjustInfoData
                    {
                        Key = item.Id,
                        LinkOrder = item.LinkOrder,
                        BasicDataName = item.DataName,
                        BasicDataNo = item.DataNo,
                        Original = item.Original,
                        Quantity = item.Quantity,
                        PriceAll = item?.PriceAll ?? 0,
                        Unit = item.Unit,
                        UnitCount = item?.UnitCount ?? 0,
                        UnitPrice = item?.UnitPrice ?? 0,
                        UnitPriceAll = item?.UnitPriceAll ?? 0,
                        WorkPrice = item?.WorkPrice ?? 0,
                        Reason = item.Reason,
                        Message = item.Message,
                        WarehouseId = item?.WarehouseId ?? 0,
                        CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
                        CreateTime = item.CreateTime,
                    };
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
                        {
                            var nTempString = new TempString
                            {
                                value0 = item.Quantity.ToString(),
                            };
                            typeitem.SetValue(nAdjustInfoData, nTempString);
                            // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
                            // {
                            //     Columnitem.show = true;
                            // }
                        }
                    }
                    AdjustInfoDataList.Add(nAdjustInfoData);
                }
                AdjustInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
                AdjustInfo.AdjustInfoData = AdjustInfoDataList;
            }
            catch (System.Exception ex)
            {
                throw;
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(AdjustInfo));
        }

        /// <summary>
        /// 查詢調整紀錄 By ProductBasicID
        /// </summary>
        /// <returns></returns>
        // GET: api/Processes
        [HttpGet("{id}")]
        public async Task<ActionResult<AdjustInfo>> GetAdjustLogByProductBasicID(int id)
        {
            var ptype = typeof(AdjustInfoData);
            var AdjustInfo = new AdjustInfo();
            var ColumnOptionlist = new List<ColumnOption>();
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            foreach (var item in Warehouses)
            {
                ColumnOptionlist.Add(new ColumnOption
                {
                    key = "Temp" + item.Id.ToString(),
                    title = item.Code,
                    show = true,
                    Columnlock = ""
                });
            }

            _context.ChangeTracker.LazyLoadingEnabled = true;
            var AdjustInfoDataList = new List<AdjustInfoData>();
            try
            {
                var ProductBasics = await _context.ProductBasics.FindAsync(id);
                var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
                var AllStockLog = new List<AllStockLog>();
                AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 2 && x.DataNo == ProductBasics.ProductNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

                foreach (var item in AllStockLog)
                {
                    var nAdjustInfoData = new AdjustInfoData
                    {
                        Key = item.Id,
                        LinkOrder = item.LinkOrder,
                        BasicDataName = item.DataName,
                        BasicDataNo = item.DataNo,
                        Original = item.Original,
                        Quantity = item.Quantity,
                        PriceAll = item?.PriceAll ?? 0,
                        Unit = item.Unit,
                        UnitCount = item?.UnitCount ?? 0,
                        UnitPrice = item?.UnitPrice ?? 0,
                        UnitPriceAll = item?.UnitPriceAll ?? 0,
                        WorkPrice = item?.WorkPrice ?? 0,
                        Reason = item.Reason,
                        Message = item.Message,
                        WarehouseId = item?.WarehouseId ?? 0,
                        CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
                        CreateTime = item.CreateTime,
                    };
                    foreach (var typeitem in ptype.GetProperties())
                    {
                        if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
                        {
                            var nTempString = new TempString
                            {
                                value0 = item.Quantity.ToString(),
                            };
                            typeitem.SetValue(nAdjustInfoData, nTempString);
                            // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
                            // {
                            //     Columnitem.show = true;
                            // }
                        }
                    }
                    AdjustInfoDataList.Add(nAdjustInfoData);
                }
                AdjustInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
                AdjustInfo.AdjustInfoData = AdjustInfoDataList;
            }
            catch (System.Exception ex)
            {
                throw;
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(AdjustInfo));
        }

        // PUT: api/Adjusts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 修改調整紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <param name="materiallog"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdjustLog(int id, MaterialLog materiallog)
        {
            materiallog.Id = id;
            var Omateriallog = _context.MaterialLogs.Find(id);
            var Cmateriallog = Omateriallog;
            if (!string.IsNullOrWhiteSpace(materiallog.AdjustNo))
            {
                Cmateriallog.AdjustNo = materiallog.AdjustNo;
            }
            //修改時檢查[單號]是否重複
            if (_context.MaterialLogs.AsQueryable().Where(x => x.Id != id && (x.AdjustNo == Cmateriallog.AdjustNo) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("調整紀錄的 [調整紀錄號] 重複!", Cmateriallog));
            }

            var Msg = MyFun.MappingData(ref Omateriallog, materiallog);
            Omateriallog.UpdateTime = DateTime.Now;
            Omateriallog.UpdateUser = MyFun.GetUserID(HttpContext);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdjustLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(Omateriallog));
        }

        // POST: api/Adjusts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 新增調整紀錄
        /// </summary>
        /// <param name="materiallog"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<MaterialLog>> PostAdjustLog(MaterialLog materiallog)
        {
            //新增時檢查[單號]是否重複
            if (_context.MaterialLogs.AsQueryable().Where(x => (x.AdjustNo == materiallog.AdjustNo) && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("調整紀錄的 [調整紀錄號]  已存在!", materiallog));
            }
            _context.MaterialLogs.Add(materiallog);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materiallog));
        }

        // DELETE: api/Adjusts/5
        /// <summary>
        /// 刪除供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterialLog>> DeleteAdjustLog(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var materiallog = await _context.MaterialLogs.FindAsync(id);
            if (materiallog == null)
            {
                return NotFound();
            }
            materiallog.DeleteFlag = 1;
            // _context.MaterialLogs.Remove(materiallog);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materiallog));
        }

        private bool AdjustLogExists(int id)
        {
            return _context.MaterialLogs.Any(e => e.Id == id);
        }


        // GET: api/Inventorys
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllStockLog>>> GetInventoryLog(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.AllStockLogs.Where(x => x.DeleteFlag == 0 && x.Message == "[庫存調整單]");
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Inventorys/5
        /// <summary>
        /// 查詢調整紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetInventoryLog(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var allStockLog = await _context.AllStockLogs.FindAsync(id);

            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }
    }
}
