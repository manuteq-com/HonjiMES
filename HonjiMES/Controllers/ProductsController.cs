using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 產品列表
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProductsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Products.Where(x => x.DeleteFlag == 0);
            var Products = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Products));
            //return Ok(new { data = Products, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 使用ID查詢產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(product));
            //return Ok(new { data = product, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 修改產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            //修改時檢查名稱重複
            if (!string.IsNullOrWhiteSpace(product.ProductNo))
            {
                if (_context.Products.Where(x => x.ProductNo == product.ProductNo && x.Id != id).Any())
                {
                    //return Ok(MyFun.APIResponseError("主件品號：" + product.ProductNo + "重複"));
                    return BadRequest("主件品號：" + product.ProductNo + "重複");
                }
            }
            product.Id = id;
            var Oldproduct = _context.Products.Find(id);
            var Msg = MyFun.MappingData(ref Oldproduct, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(Oldproduct));
            //return Ok(new { success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 新增產品列表
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductW product)
        {
            //優先確認Basic是否存在
            var ProductsBasicData = _context.ProductBasics.Where(x => x.ProductNo == product.ProductNo && x.DeleteFlag == 0).FirstOrDefault();
            if (ProductsBasicData == null)
            {
                var ProductBasics = new List<ProductBasic>();
                _context.ProductBasics.Add(new ProductBasic
                {
                    ProductNo = product.ProductNo,
                    ProductNumber = product.ProductNumber,
                    Name = product.Name,
                    Specification = product.Specification,
                    Property = product.Property,
                    Price = product.Price,
                    SubInventory = product.SubInventory
                });
                _context.SaveChanges();
            }
            product.ProductBasicId = ProductsBasicData.Id;

            string sRepeatProduct = null;
            var nProductlist = new List<Product>();
            foreach (var warehouseId in product.wid)
            {
                //新增時檢查主件品號是否重複
                if (_context.Products.Where(x => x.ProductNo == product.ProductNo && x.WarehouseId == warehouseId).Any())
                {
                    sRepeatProduct += "主件品號：[" + product.ProductNo + "] 已經存在 [" + product.warehouseData[warehouseId - 1].Name + "] !<br/>";
                }
                else
                {
                    nProductlist.Add(new Product
                    {
                        ProductNo = product.ProductNo,
                        ProductNumber = product.ProductNumber,
                        Name = product.Name,
                        Quantity = product.Quantity,
                        QuantityLimit = product.QuantityLimit,
                        Specification = product.Specification,
                        Property = product.Property,
                        Price = product.Price,
                        MaterialId = product.MaterialId,
                        SubInventory = product.SubInventory,
                        WarehouseId = warehouseId,
                        ProductBasicId = product.ProductBasicId,
                        CreateUser = 1
                    });
                }
            }

            if (sRepeatProduct == null)
            {
                _context.AddRange(nProductlist);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(product));
            }
            else
            {
                // return BadRequest("主件品號：" + product.ProductNo + "重複");
                return Ok(MyFun.APIResponseError(sRepeatProduct, product));
            }
            //return Ok(new { data = CreatedAtAction("GetProduct", new { id = product.Id }, product), success = true });
        }

        /// <summary>
        /// 刪除產品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(product));
            //return Ok(new { data = product, success = true, timestamp = DateTime.Now, message = "" });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
