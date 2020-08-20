using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;
using DevExtreme.AspNet.Mvc;

namespace HonjiMES.Controllers
{
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductBasicsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public ProductBasicsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/ProductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasicData>>> GetProductBasics(
            [FromQuery] DataSourceLoadOptions FromQuery)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var productBasic = _context.ProductBasics.Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.UpdateTime).Include(x => x.Products).Select(x => new ProductBasicData
            {
                TotalCount = x.Products.Where(y => y.DeleteFlag == 0).Sum(y => y.Quantity),
                Id = x.Id,
                ProductNo = x.ProductNo,
                ProductNumber = x.ProductNumber,
                Name = x.Name,
                Specification = x.Specification,
                Property = x.Property,
                Price = x.Price,
                SubInventory = x.SubInventory,
                Remarks = x.Remarks,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                DeleteFlag = x.DeleteFlag,
                Products = x.Products
            });
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(productBasic, FromQuery);
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/ProductBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBasic>>> GetProductBasicsAsc()
        {
            var productBasic = await _context.ProductBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.ProductNo).ToListAsync();
            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // GET: api/ProductBasics
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductBasic>> GetProductBasic(int id)
        {
            var productBasic = await _context.ProductBasics.FindAsync(id);

            if (productBasic == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(productBasic));
        }

        // PUT: api/ProductBasics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductBasic(int id, ProductBasic productBasic)
        {
            productBasic.Id = id;
            var OproductBasic = _context.ProductBasics.Find(id);
            var Cproduct = OproductBasic;
            if (!string.IsNullOrWhiteSpace(productBasic.ProductNo))
            {
                Cproduct.ProductNo = productBasic.ProductNo;
            }

            var Msg = MyFun.MappingData(ref OproductBasic, productBasic);
            OproductBasic.UpdateTime = DateTime.Now;
            OproductBasic.UpdateUser = MyFun.GetUserID(HttpContext);

            //更新完basic後，同步更新底下資料
            var Products = _context.Products.AsQueryable().Where(x => x.ProductBasicId == OproductBasic.Id && x.DeleteFlag == 0);
            foreach (var item in Products)
            {
                item.ProductNo = OproductBasic.ProductNo;
                item.ProductNumber = OproductBasic.ProductNumber;
                item.Name = OproductBasic.Name;
                item.Specification = OproductBasic.Specification;
                item.Property = OproductBasic.Property;
                item.Price = OproductBasic.Price;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductBasicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(OproductBasic));
            //return Ok(new { success = true, timestamp = DateTime.Now, message = "" });
        }

        // POST: api/ProductBasics
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProductBasic>> PostProductBasic(ProductBasic productBasic)
        {
            _context.ProductBasics.Add(productBasic);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(productBasic));
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
            if (product.wid == null)
            {
                return Ok(MyFun.APIResponseError("請選擇 [存放庫別]!", product));
            }

            //優先確認Basic是否存在
            var ProductsBasicData = _context.ProductBasics.AsQueryable().Where(x => x.ProductNo == product.ProductNo && x.DeleteFlag == 0).FirstOrDefault();
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
                    SubInventory = product.SubInventory,
                    CreateUser = MyFun.GetUserID(HttpContext)
                });
                _context.SaveChanges();
                ProductsBasicData = _context.ProductBasics.AsQueryable().Where(x => x.ProductNo == product.ProductNo && x.DeleteFlag == 0).FirstOrDefault();
            }
            else
            {
                return Ok(MyFun.APIResponseError("[主件品號] 已存在!"));
            }
            product.ProductBasicId = ProductsBasicData.Id;

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
                        CreateUser = MyFun.GetUserID(HttpContext)
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

        // DELETE: api/ProductBasics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductBasic>> DeleteProductBasic(int id)
        {
            var productBasic = await _context.ProductBasics.FindAsync(id);
            if (productBasic == null)
            {
                return NotFound();
            }
            productBasic.DeleteFlag = 1;
            // _context.ProductBasics.Remove(productBasic);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(productBasic));
        }

        [HttpGet]
        public async Task<ActionResult<ProductBasic>> CheckProductNumber(string DataNo)
        {
            var productBasicNo = await _context.ProductBasics.Where(x => x.ProductNo == DataNo && x.DeleteFlag == 0).AnyAsync();
            if (productBasicNo)
            {
                return Ok(MyFun.APIResponseError("[成品品號]已存在!"));
            }
            return Ok(MyFun.APIResponseOK(""));
        }

        private bool ProductBasicExists(int id)
        {
            return _context.ProductBasics.Any(e => e.Id == id);
        }
    }
}
