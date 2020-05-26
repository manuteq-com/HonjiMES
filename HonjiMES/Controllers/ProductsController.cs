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
            var data = _context.Products.AsQueryable().Where(x => x.DeleteFlag == 0);
            var Products = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Products));
            //return Ok(new { data = Products, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsById(int? id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Products.AsQueryable().Where(x => x.DeleteFlag == 0);
            if (id.HasValue)
            {
                data = data.Where(x => x.ProductBasicId == id);
            }
            var Products = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Products));
            //return Ok(new { data = Products, success = true, timestamp = DateTime.Now, message = "" });
        }

        /// <summary>
        /// 查詢產品列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasic>>> GetProductBasics()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.ProductBasics.AsQueryable().Where(x => x.DeleteFlag == 0);
            var ProductBasics = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(ProductBasics));
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
            product.Id = id;
            var Oproduct = _context.Products.Find(id);
            var Cproduct = Oproduct;
            if (!string.IsNullOrWhiteSpace(product.ProductNo))
            {
                Cproduct.ProductNo = product.ProductNo;
            }
            if (product.WarehouseId != 0)
            {
                Cproduct.WarehouseId = product.WarehouseId;
            }
            //修改時檢查[品號][倉庫]是否重複
            if (_context.Products.AsQueryable().Where(x => x.Id != id && x.ProductNo == Cproduct.ProductNo  && x.WarehouseId == Cproduct.WarehouseId && x.DeleteFlag == 0).Any())
            {
                var warehouse = _context.Warehouses.Find(Cproduct.WarehouseId);
                return Ok(MyFun.APIResponseError("成品的品號 [" + Cproduct.ProductNo + "] 與存放褲別 [" + warehouse.Name + "] 重複!", Cproduct));
            }
            
            var Msg = MyFun.MappingData(ref Oproduct, product);
            Oproduct.UpdateTime = DateTime.Now;
            Oproduct.UpdateUser = 1;

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
            return Ok(MyFun.APIResponseOK(Oproduct));
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
            if (product.wid == null) {
                return Ok(MyFun.APIResponseError("請選擇 [存放庫別]!", product));
            }

            //優先確認Basic是否存在
            var ProductsBasicData = _context.ProductBasics.AsQueryable().Where(x => x.ProductNo == product.ProductNo && x.DeleteFlag == 0).FirstOrDefault();
            if (ProductsBasicData == null)
            {
                return Ok(MyFun.APIResponseError("[主件品號] 不存在，請確認資訊是否正確。"));
            } else {
                product.ProductBasicId = ProductsBasicData.Id;
            }

            string sRepeatProduct = null;
            var nProductlist = new List<Product>();
            foreach (var warehouseId in product.wid)
            {
                //新增時檢查主件品號是否重複
                if (_context.Products.AsQueryable().Where(x => x.ProductNo == product.ProductNo && x.WarehouseId == warehouseId && x.DeleteFlag == 0).Any())
                {
                    sRepeatProduct += "主件品號 [" + product.ProductNo + "] 已經存在 [" + product.warehouseData[warehouseId - 1].Name + "] !<br/>";
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
                        MaterialRequire = 1,
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
            product.DeleteFlag = 1;
            // _context.Products.Remove(product);
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
