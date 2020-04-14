using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;

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
                if (!(Material.MaterialLogs.Any()))//同步資料用，建立原始庫存數
                {
                    Material.MaterialLogs.Add(new MaterialLog { Quantity = Material.Quantity, Message = "原始數量", CreateTime = dt, CreateUser = UserID });
                }
                Material.MaterialLogs.Add(new MaterialLog { Quantity = inventorychange.quantity, Original= Material.Quantity, Message = inventorychange.Message, Reason = inventorychange.Reason, CreateTime = dt, CreateUser = UserID });
                Material.Quantity += inventorychange.quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Material));
            }
            else if (inventorychange.mod == "product")
            {
                var Products = _context.Products.Find(inventorychange.id);
                if (!(Products.ProductLogs.Any()))//同步資料用，建立原始庫存數
                {
                    Products.ProductLogs.Add(new ProductLog { Quantity = Products.Quantity, Message = "原始數量", CreateTime = dt, CreateUser = UserID });
                }
                Products.ProductLogs.Add(new ProductLog { Quantity = inventorychange.quantity, Original = Products.Quantity, Message = inventorychange.Message, Reason = inventorychange.Reason, CreateTime = dt, CreateUser = UserID });
                Products.Quantity += inventorychange.quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Products));
            }
            return Ok(MyFun.APIResponseError("無對應的資料"));
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }
    }
}