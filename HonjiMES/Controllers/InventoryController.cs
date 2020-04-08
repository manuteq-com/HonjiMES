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
            if (inventorychange.mod == "material")
            {
                var Material = _context.Materials.Find(inventorychange.id);
                Material.Quantity += inventorychange.quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Material));
            }
            else if (inventorychange.mod == "product")
            {
                var Products = _context.Products.Find(inventorychange.id);
                Products.Quantity += inventorychange.quantity;
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(Products));
            }
            return Ok(MyFun.APIResponseError(null, "無對應的資料"));
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }
    }
}