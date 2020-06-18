using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 庫存API
    /// </summary>
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
        /// 取得原料庫存調整單號
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<MaterialLog>> GetMaterialAdjustNo()
        {
            var AdjustNoName = "AJM";
            var NoData = await _context.MaterialLogs.AsQueryable().Where(x => x.DeleteFlag == 0 && x.AdjustNo.Contains(AdjustNoName)).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastAdjustNo = NoData.FirstOrDefault().AdjustNo;
                var LastLength = LastAdjustNo.Length - AdjustNoName.Length;
                var NoLast = Int32.Parse(LastAdjustNo.Substring(LastAdjustNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var AdjustData = new MaterialLog{
                AdjustNo = AdjustNoName + NoCount.ToString("000000")
            };
            return Ok(MyFun.APIResponseOK(AdjustData));
        }

        /// <summary>
        /// 取得成品庫存調整單號
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ProductLog>> GetProductAdjustNo()
        {
            var AdjustNoName = "AJP";
            var NoData = await _context.ProductLogs.AsQueryable().Where(x => x.DeleteFlag == 0 && x.AdjustNo.Contains(AdjustNoName)).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1) {
                var LastAdjustNo = NoData.FirstOrDefault().AdjustNo;
                var LastLength = LastAdjustNo.Length - AdjustNoName.Length;
                var NoLast = Int32.Parse(LastAdjustNo.Substring(LastAdjustNo.Length - LastLength, LastLength));
                if (NoCount <= NoLast) {
                    NoCount = NoLast + 1;
                }
            }
            var AdjustData = new ProductLog{
                AdjustNo = AdjustNoName + NoCount.ToString("000000")
            };
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
            var UserID = 1;
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
                inventorychange.ProductLog.Original = Products.Quantity;
                inventorychange.ProductLog.CreateTime = dt;
                inventorychange.ProductLog.CreateUser = UserID;
                inventorychange.ProductLog.Message = "成品庫存調整";
                Products.ProductLogs.Add(inventorychange.ProductLog);
                Products.Quantity += inventorychange.ProductLog.Quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Products));
            }
            return Ok(MyFun.APIResponseError("無對應的資料"));
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }
    }
}