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
    /// 供應商原料
    /// </summary>
    // [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SupplierOfMaterialsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public SupplierOfMaterialsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/Suppliers
        /// <summary>
        /// 供應商提供之原料
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SupplierOfMaterial>>> GetSupplierOfMaterials(int id)
        {
            var data = _context.SupplierOfMaterials.Where(x => x.DeleteFlag == 0 && x.SupplierId == id).Include(x => x.MaterialBasic);
            var SupplierOfMaterials = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(SupplierOfMaterials));
        }

        // GET: api/Suppliers/5
        /// <summary>
        /// 查詢供應商提供之原料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierOfMaterial>> GetSupplierOfMaterial(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var supplierofmaterials = await _context.SupplierOfMaterials.FindAsync(id);
            var data = _context.SupplierOfMaterials.AsQueryable().Where(x => x.DeleteFlag == 0 && x.MaterialBasic.Id == id).Include(x => x.MaterialBasic);

            if (supplierofmaterials == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(supplierofmaterials));
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 修改供應商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PutSupplierMaterial"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BomList>>> PutSupplierofMaterial(int id, [FromBody] PostSupplierMaterial PutSupplierMaterial)
        {
            var Osupplierofmaterial = _context.SupplierOfMaterials.Find(id);
            Osupplierofmaterial.MaterialBasicId = PutSupplierMaterial.MaterialBasicId;
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(Osupplierofmaterial));
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// 新增供應商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PostSupplierMaterial"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<ActionResult<PostSupplierMaterial>> PostSupplierOfMaterial(int id, [FromBody] PostSupplierMaterial PostSupplierMaterial)
        {
            //新增時檢查[代號][名稱]是否重複
            var suppliers = _context.Suppliers.Find(id);
            if (suppliers == null)
            {
                return Ok(MyFun.APIResponseError("供應商不存在!"));
            }
            else
            {
                suppliers.SupplierOfMaterials.Add(new SupplierOfMaterial
                {
                    MaterialBasicId = PostSupplierMaterial.MaterialBasicId,

                    CreateUser = MyFun.GetUserID(HttpContext)
                });
            }
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(""));
        }

        // DELETE: api/Suppliers/5
        /// <summary>
        /// 刪除供應商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<SupplierOfMaterial>> DeleteSupplierOfMaterial(int id)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
            var supplierofmaterials = await _context.SupplierOfMaterials.FindAsync(id);
            if (supplierofmaterials == null)
            {
                return NotFound();
            }
            supplierofmaterials.DeleteFlag = 1;
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(supplierofmaterials));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
}
