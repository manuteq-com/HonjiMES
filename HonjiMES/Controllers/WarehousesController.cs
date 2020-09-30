using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 倉庫
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public WarehousesController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        ///  倉庫列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var data = _context.Warehouses.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code);
            var Warehouses = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Warehouses));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouse(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(warehouse));
        }
        
        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseForBom>> GetWarehouseListByWiproductBasic(int id)
        {
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            var WarehouseData = await _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasic.Id == id && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();
            var data = new List<WarehouseForBom>();
            foreach (var item in Warehouses)
            {
                item.Name = item.Code + item.Name;
                decimal QuantityTemp = 0;
                var Data = WarehouseData.Where(x => x.WarehouseId == item.Id);
                if (Data.Count() != 0) {
                    item.Name += " (庫存 " + Data.First().Quantity + ")"; 
                    QuantityTemp = Data.First().Quantity;
                }
                data.Add(new WarehouseForBom {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Quantity = QuantityTemp,
                    HasWarehouse = Data.Count() != 0
                });
            }
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByBasic(int id)
        {
            var WarehouseData = await _context.Wiproducts.AsQueryable().Where(x => x.WiproductBasic.Id == id && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();

            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByWiproduct(int id)
        {
            var wiproducts = _context.Products.Find(id);
            var WarehouseData = await _context.Products.AsQueryable().Where(x => x.ProductNo == wiproducts.ProductNo && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();
            
            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseForBom>> GetWarehouseListByProductBasic(int id)
        {
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            var WarehouseData = await _context.Products.AsQueryable().Where(x => x.ProductBasic.Id == id && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();
            var data = new List<WarehouseForBom>();
            foreach (var item in Warehouses)
            {
                item.Name = item.Code + item.Name;
                decimal QuantityTemp = 0;
                var Data = WarehouseData.Where(x => x.WarehouseId == item.Id);
                if (Data.Count() != 0) {
                    item.Name += " (庫存 " + Data.First().Quantity + ")"; 
                    QuantityTemp = Data.First().Quantity;
                }
                data.Add(new WarehouseForBom {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Quantity = QuantityTemp,
                    HasWarehouse = Data.Count() != 0
                });
            }
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByProductBasic(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料
            var WarehouseData = await _context.Products.AsQueryable().Where(x => x.ProductBasic.Id == id && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();

            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByProduct(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料

            var products = _context.Products.Find(id);
            /////// typeA
            // var productsList = _context.Products.Where(x=>x.ProductNo==products.ProductNo);
            // foreach(var item in productsList) {
            //     item.UpdateTime=DateTime.Now;
            // }
            /////// typeB
            // Product products = _context.Products.Find(id);
            // IEnumerable<Product> productsList = _context.Products.Where(x=>x.ProductNo==products.ProductNo);
            // foreach(Product item in productsList) {
            //     item.UpdateTime=DateTime.Now;
            // }
            // _context.SaveChanges();

            var WarehouseData = await _context.Products.AsQueryable().Where(x => x.ProductNo == products.ProductNo && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();
            // var WarehouseData = await _context.Products.Where(x => x.ProductNo == products.ProductNo && x.DeleteFlag == 0).Include(x => x.Warehouse).Select(x =>new{x.Warehouse.Id,x.Warehouse.Name} ).OrderBy(x=> x.Name).ThenBy(x=>x.Id).ToListAsync();

            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseForBom>> GetWarehouseListByMaterialBasic(int id)
        {
            var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0).OrderBy(x => x.Code).ToListAsync();
            var WarehouseData = await _context.Materials.AsQueryable().Where(x => x.MaterialBasic.Id == id && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();
            var data = new List<WarehouseForBom>();
            foreach (var item in Warehouses)
            {
                item.Name = item.Code + item.Name;
                decimal QuantityTemp = 0;
                var Data = WarehouseData.Where(x => x.WarehouseId == item.Id);
                if (Data.Count() != 0) {
                    item.Name += " (庫存 " + Data.First().Quantity + ")"; 
                    QuantityTemp = Data.First().Quantity;
                }
                data.Add(new WarehouseForBom {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Quantity = QuantityTemp,
                    HasWarehouse = Data.Count() != 0
                });
            }
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByMaterialBasic(int id)
        {
            var materialBasics = _context.MaterialBasics.Find(id);
            var WarehouseData = await _context.Materials.AsQueryable().Where(x => x.MaterialNo == materialBasics.MaterialNo && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();

            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 用ID找倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Warehouse>> GetWarehouseByMaterial(int id)
        {
            var materials = _context.Materials.Find(id);
            var WarehouseData = await _context.Materials.AsQueryable().Where(x => x.MaterialNo == materials.MaterialNo && x.DeleteFlag == 0).Include(x => x.Warehouse).ToListAsync();

            if (WarehouseData == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(WarehouseData));
        }

        /// <summary>
        /// 新增倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        // PUT: api/Warehouses/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, Warehouse warehouse)
        {
            warehouse.Id = id;
            var Osupplier = _context.Warehouses.Find(id);
            var Csupplier = Osupplier;
            // if (!string.IsNullOrWhiteSpace(warehouse.Code))
            // {
            //     Csupplier.Code = warehouse.Code;
            // }
            if (!string.IsNullOrWhiteSpace(warehouse.Name))
            {
                Csupplier.Name = warehouse.Name;
            }
            //修改時檢查[代號][名稱]是否重複
            if (_context.Warehouses.AsQueryable().Where(x => x.Id != id && x.Name == Csupplier.Name && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("倉庫的名稱 [" + Csupplier.Name + "] 重複!", Csupplier));
            }

            var Msg = MyFun.MappingData(ref Osupplier, warehouse);
            Osupplier.UpdateTime = DateTime.Now;
            Osupplier.UpdateUser = MyFun.GetUserID(HttpContext);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(warehouse));
        }
        /// <summary>
        /// 新增倉庫
        /// </summary>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        // POST: api/Warehouses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse warehouse)
        {
            //新增時檢查[代號][名稱]是否重複
            if (_context.Warehouses.AsQueryable().Where(x => x.Code == warehouse.Code && x.Name == warehouse.Name && x.DeleteFlag == 0).Any())
            {
                return Ok(MyFun.APIResponseError("倉庫名稱已存在!", warehouse));
            }
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(warehouse));
        }
        /// <summary>
        /// 刪除倉庫
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Warehouse>> DeleteWarehouse(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            warehouse.DeleteFlag = 1;
            // _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(warehouse));
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouses.Any(e => e.Id == id);
        }
    }
}
