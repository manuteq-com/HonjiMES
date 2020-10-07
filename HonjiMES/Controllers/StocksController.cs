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
    public class StocksController : ControllerBase
    {
        private readonly HonjiContext _context;

        public StocksController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/Stocks
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockHead>>> GetStockLists(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.StockHeads.Where(x => x.DeleteFlag == 0);
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 用父ID查調整單明細
        /// </summary>
        /// <param name="Pid">父ID</param>
        /// <returns></returns>
        public async Task<ActionResult<StockDetailData>> GetStockDetailByPId(int Pid)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = await _context.StockDetails.AsQueryable().Where(x => x.StockHeadId == Pid).ToListAsync();
            var StockDetailData = new List<StockDetailData>();
            var tempId = 1;
            foreach (var item in data)
            {
                var BasicData = new BasicData();
                if (item.ItemType == 1)
                {
                    var tempInfo = _context.Materials.Find(item.ItemId);
                    BasicData.DataNo = tempInfo.MaterialNo;
                    BasicData.Name = tempInfo.Name;
                    BasicData.WarehouseId = tempInfo.WarehouseId;
                }
                else if (item.ItemType == 2)
                {
                    var tempInfo = _context.Products.Find(item.ItemId);
                    BasicData.DataNo = tempInfo.ProductNo;
                    BasicData.Name = tempInfo.Name;
                    BasicData.WarehouseId = tempInfo.WarehouseId;
                }
                else if (item.ItemType == 3)
                {
                    var tempInfo = _context.Wiproducts.Find(item.ItemId);
                    BasicData.DataNo = tempInfo.WiproductNo;
                    BasicData.Name = tempInfo.Name;
                    BasicData.WarehouseId = tempInfo.WarehouseId;
                }
                StockDetailData.Add(new StockDetailData
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
            return Ok(MyFun.APIResponseOK(StockDetailData));
        }

        // GET: api/Stocks
        /// <summary>
        /// 調整紀錄列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllStockLog>>> GetStockLog(
                [FromQuery] DataSourceLoadOptions FromQuery)
        {
            var data = _context.AllStockLogs.Where(x => x.DeleteFlag == 0);
            // var MaterialLogs = await data.ToListAsync();
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Stocks/5
        /// <summary>
        /// 查詢調整紀錄
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetStockLog(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var allStockLog = await _context.AllStockLogs.FindAsync(id);

            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }

        // GET: api/Stocks/5
        /// <summary>
        /// 查詢調整紀錄 By ProductBasicID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AllStockLog>> GetStockLogByProductBasicID1(int id)
        {
            var allStockLog = await _context.AllStockLogs.Where(x => x.NameType == 2 && x.DataId == id).ToListAsync();
            if (allStockLog == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(allStockLog));
        }

        // /// <summary>
        // /// 查詢調整紀錄 By MaterialBasicID
        // /// </summary>
        // /// <returns></returns>
        // // GET: api/Processes
        // [HttpGet("{id}")]
        // public async Task<ActionResult<StockInfo>> GetStockLogByMaterialBasicID(int id)
        // {
        //     var ptype = typeof(StockInfoData);
        //     var StockInfo = new StockInfo();
        //     var ColumnOptionlist = new List<ColumnOption>();
        //     var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
        //     foreach (var item in Warehouses)
        //     {
        //         ColumnOptionlist.Add(new ColumnOption
        //         {
        //             key = "Temp" + item.Id.ToString(),
        //             title = item.Code,
        //             show = true,
        //             Columnlock = ""
        //         });
        //     }

        //     _context.ChangeTracker.LazyLoadingEnabled = true;
        //     var StockInfoDataList = new List<StockInfoData>();
        //     try
        //     {
        //         var MaterialBasics = await _context.MaterialBasics.FindAsync(id);
        //         var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
        //         var AllStockLog = new List<AllStockLog>();
        //         AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 1 && x.DataNo == MaterialBasics.MaterialNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

        //         foreach (var item in AllStockLog)
        //         {
        //             var nStockInfoData = new StockInfoData
        //             {
        //                 Key = item.Id,
        //                 LinkOrder = item.LinkOrder,
        //                 BasicDataName = item.DataName,
        //                 BasicDataNo = item.DataNo,
        //                 Original = item.Original,
        //                 Quantity = item.Quantity,
        //                 PriceAll = item?.PriceAll ?? 0,
        //                 Unit = item.Unit,
        //                 UnitCount = item?.UnitCount ?? 0,
        //                 UnitPrice = item?.UnitPrice ?? 0,
        //                 UnitPriceAll = item?.UnitPriceAll ?? 0,
        //                 WorkPrice = item?.WorkPrice ?? 0,
        //                 Reason = item.Reason,
        //                 Message = item.Message,
        //                 WarehouseId = item?.WarehouseId ?? 0,
        //                 CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
        //                 CreateTime = item.CreateTime,
        //             };
        //             foreach (var typeitem in ptype.GetProperties())
        //             {
        //                 if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
        //                 {
        //                     var nTempString = new TempString
        //                     {
        //                         value0 = item.Quantity.ToString(),
        //                     };
        //                     typeitem.SetValue(nStockInfoData, nTempString);
        //                     // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
        //                     // {
        //                     //     Columnitem.show = true;
        //                     // }
        //                 }
        //             }
        //             StockInfoDataList.Add(nStockInfoData);
        //         }
        //         StockInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
        //         StockInfo.StockInfoData = StockInfoDataList;
        //     }
        //     catch (System.Exception ex)
        //     {
        //         throw;
        //     }
        //     _context.ChangeTracker.LazyLoadingEnabled = false;
        //     return Ok(MyFun.APIResponseOK(StockInfo));
        // }

        // /// <summary>
        // /// 查詢調整紀錄 By WiproductBasicID
        // /// </summary>
        // /// <returns></returns>
        // // GET: api/Processes
        // [HttpGet("{id}")]
        // public async Task<ActionResult<StockInfo>> GetStockLogByWiproductBasicID(int id)
        // {
        //     var ptype = typeof(StockInfoData);
        //     var StockInfo = new StockInfo();
        //     var ColumnOptionlist = new List<ColumnOption>();
        //     var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
        //     foreach (var item in Warehouses)
        //     {
        //         ColumnOptionlist.Add(new ColumnOption
        //         {
        //             key = "Temp" + item.Id.ToString(),
        //             title = item.Code,
        //             show = true,
        //             Columnlock = ""
        //         });
        //     }

        //     _context.ChangeTracker.LazyLoadingEnabled = true;
        //     var StockInfoDataList = new List<StockInfoData>();
        //     try
        //     {
        //         var WiproductBasics = await _context.WiproductBasics.FindAsync(id);
        //         var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
        //         var AllStockLog = new List<AllStockLog>();
        //         AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 3 && x.DataNo == WiproductBasics.WiproductNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

        //         foreach (var item in AllStockLog)
        //         {
        //             var nStockInfoData = new StockInfoData
        //             {
        //                 Key = item.Id,
        //                 LinkOrder = item.LinkOrder,
        //                 BasicDataName = item.DataName,
        //                 BasicDataNo = item.DataNo,
        //                 Original = item.Original,
        //                 Quantity = item.Quantity,
        //                 PriceAll = item?.PriceAll ?? 0,
        //                 Unit = item.Unit,
        //                 UnitCount = item?.UnitCount ?? 0,
        //                 UnitPrice = item?.UnitPrice ?? 0,
        //                 UnitPriceAll = item?.UnitPriceAll ?? 0,
        //                 WorkPrice = item?.WorkPrice ?? 0,
        //                 Reason = item.Reason,
        //                 Message = item.Message,
        //                 WarehouseId = item?.WarehouseId ?? 0,
        //                 CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
        //                 CreateTime = item.CreateTime,
        //             };
        //             foreach (var typeitem in ptype.GetProperties())
        //             {
        //                 if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
        //                 {
        //                     var nTempString = new TempString
        //                     {
        //                         value0 = item.Quantity.ToString(),
        //                     };
        //                     typeitem.SetValue(nStockInfoData, nTempString);
        //                     // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
        //                     // {
        //                     //     Columnitem.show = true;
        //                     // }
        //                 }
        //             }
        //             StockInfoDataList.Add(nStockInfoData);
        //         }
        //         StockInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
        //         StockInfo.StockInfoData = StockInfoDataList;
        //     }
        //     catch (System.Exception ex)
        //     {
        //         throw;
        //     }
        //     _context.ChangeTracker.LazyLoadingEnabled = false;
        //     return Ok(MyFun.APIResponseOK(StockInfo));
        // }

        // /// <summary>
        // /// 查詢調整紀錄 By ProductBasicID
        // /// </summary>
        // /// <returns></returns>
        // // GET: api/Processes
        // [HttpGet("{id}")]
        // public async Task<ActionResult<StockInfo>> GetStockLogByProductBasicID(int id)
        // {
        //     var ptype = typeof(StockInfoData);
        //     var StockInfo = new StockInfo();
        //     var ColumnOptionlist = new List<ColumnOption>();
        //     var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
        //     foreach (var item in Warehouses)
        //     {
        //         ColumnOptionlist.Add(new ColumnOption
        //         {
        //             key = "Temp" + item.Id.ToString(),
        //             title = item.Code,
        //             show = true,
        //             Columnlock = ""
        //         });
        //     }

        //     _context.ChangeTracker.LazyLoadingEnabled = true;
        //     var StockInfoDataList = new List<StockInfoData>();
        //     try
        //     {
        //         var ProductBasics = await _context.ProductBasics.FindAsync(id);
        //         var Users = await _context.Users.Where(x => x.DeleteFlag == 0).ToListAsync();
        //         var AllStockLog = new List<AllStockLog>();
        //         AllStockLog = await _context.AllStockLogs.Where(x => x.NameType == 2 && x.DataNo == ProductBasics.ProductNo && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();

        //         foreach (var item in AllStockLog)
        //         {
        //             var nStockInfoData = new StockInfoData
        //             {
        //                 Key = item.Id,
        //                 LinkOrder = item.LinkOrder,
        //                 BasicDataName = item.DataName,
        //                 BasicDataNo = item.DataNo,
        //                 Original = item.Original,
        //                 Quantity = item.Quantity,
        //                 PriceAll = item?.PriceAll ?? 0,
        //                 Unit = item.Unit,
        //                 UnitCount = item?.UnitCount ?? 0,
        //                 UnitPrice = item?.UnitPrice ?? 0,
        //                 UnitPriceAll = item?.UnitPriceAll ?? 0,
        //                 WorkPrice = item?.WorkPrice ?? 0,
        //                 Reason = item.Reason,
        //                 Message = item.Message,
        //                 WarehouseId = item?.WarehouseId ?? 0,
        //                 CreateUser = Users.Where(x => x.Id == item.CreateUser).FirstOrDefault().Realname,
        //                 CreateTime = item.CreateTime,
        //             };
        //             foreach (var typeitem in ptype.GetProperties())
        //             {
        //                 if (typeitem.Name == "Temp" + item.WarehouseId.ToString())
        //                 {
        //                     var nTempString = new TempString
        //                     {
        //                         value0 = item.Quantity.ToString(),
        //                     };
        //                     typeitem.SetValue(nStockInfoData, nTempString);
        //                     // foreach (var Columnitem in ColumnOptionlist.Where(x => x.key == "Temp" + item.WarehouseId.ToString()))
        //                     // {
        //                     //     Columnitem.show = true;
        //                     // }
        //                 }
        //             }
        //             StockInfoDataList.Add(nStockInfoData);
        //         }
        //         StockInfo.ColumnOptionlist = ColumnOptionlist;//顯示項目
        //         StockInfo.StockInfoData = StockInfoDataList;
        //     }
        //     catch (System.Exception ex)
        //     {
        //         throw;
        //     }
        //     _context.ChangeTracker.LazyLoadingEnabled = false;
        //     return Ok(MyFun.APIResponseOK(StockInfo));
        // }

        // // POST: api/Stocks
        // // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // // more details see https://aka.ms/RazorPagesCRUD.
        // /// <summary>
        // /// 新增調整紀錄
        // /// </summary>
        // /// <param name="materiallog"></param>
        // /// <returns></returns>
        // [HttpPost]
        // public async Task<ActionResult<MaterialLog>> PostStockLog(MaterialLog materiallog)
        // {
        //     //新增時檢查[單號]是否重複
        //     if (_context.MaterialLogs.AsQueryable().Where(x => (x.StockNo == materiallog.StockNo) && x.DeleteFlag == 0).Any())
        //     {
        //         return Ok(MyFun.APIResponseError("調整紀錄的 [調整紀錄號]  已存在!", materiallog));
        //     }
        //     _context.MaterialLogs.Add(materiallog);
        //     await _context.SaveChangesAsync();
        //     return Ok(MyFun.APIResponseOK(materiallog));
        // }

        // DELETE: api/Stocks/5
        /// <summary>
        /// 刪除供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterialLog>> DeleteStockLog(int id)
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

        private bool StockLogExists(int id)
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
            var data = _context.AllStockLogs.Where(x => x.DeleteFlag == 0 && x.Message == "庫存調整單");
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

        
        /// <summary>
        /// 查詢入庫單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockHeadInfo>>> GetStockHeads(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料
            var data = _context.StockHeads.Where(x => x.DeleteFlag == 0).Include(x => x.StockDetails).OrderByDescending(x => x.CreateTime).Select(x => new StockHeadInfo
            {
                Id = x.Id,
                StockNo = x.StockNo,
                LinkOrder = x.LinkOrder,
                Type = x.Type,
                DataNo = x.StockDetails.FirstOrDefault().DataNo,
                Original = x.StockDetails.FirstOrDefault().Original,
                Quantity = x.StockDetails.FirstOrDefault().Quantity,
                DeleteFlag = x.DeleteFlag,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
            });
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            // if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            // {
            //     data = data.Where(x => x.WorkOrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            // }

            _context.ChangeTracker.LazyLoadingEnabled = false;
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
    }
}
