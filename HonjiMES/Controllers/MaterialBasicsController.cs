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
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialBasicsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MaterialBasicsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/MaterialBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasics()
        {
            var materialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).ToListAsync();
            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        // GET: api/MaterialBasics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialBasic>> GetMaterialBasic(int id)
        {
            var materialBasic = await _context.MaterialBasics.FindAsync(id);

            if (materialBasic == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        // PUT: api/MaterialBasics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterialBasic(int id, MaterialBasic materialBasic)
        {
            materialBasic.Id = id;
            var OmaterialBasic = _context.MaterialBasics.Find(id);
            var Cproduct = OmaterialBasic;
            if (!string.IsNullOrWhiteSpace(materialBasic.MaterialNo))
            {
                Cproduct.MaterialNo = materialBasic.MaterialNo;
            }

            var Msg = MyFun.MappingData(ref OmaterialBasic, materialBasic);
            OmaterialBasic.UpdateTime = DateTime.Now;
            OmaterialBasic.UpdateUser = 1;

            //更新完basic後，同步更新底下資料
            var Materials = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == OmaterialBasic.Id && x.DeleteFlag == 0);
            foreach (var item in Materials)
            {
                item.MaterialNo = OmaterialBasic.MaterialNo;
                item.Name = OmaterialBasic.Name;
                item.Specification = OmaterialBasic.Specification;
                item.Property = OmaterialBasic.Property;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialBasicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(OmaterialBasic));
        }

        // POST: api/MaterialBasics
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MaterialBasic>> PostMaterialBasic(MaterialBasic materialBasic)
        {
            _context.MaterialBasics.Add(materialBasic);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        // POST: api/Materials
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Material>> PostMaterial(MaterialW material)
        {
            if (material.wid == null)
            {
                return Ok(MyFun.APIResponseError("請選擇 [存放庫別]!", material));
            }

            //優先確認Basic是否存在
            var MaterialBasicData = _context.MaterialBasics.AsQueryable().Where(x => x.MaterialNo == material.MaterialNo && x.DeleteFlag == 0).FirstOrDefault();
            if (MaterialBasicData == null)
            {
                var MaterialBasics = new List<MaterialBasic>();
                _context.MaterialBasics.Add(new MaterialBasic
                {
                    MaterialNo = material.MaterialNo,
                    Name = material.Name,
                    Specification = material.Specification,
                    Property = material.Property,
                    CreateUser = 1
                });
                _context.SaveChanges();
                MaterialBasicData = _context.MaterialBasics.AsQueryable().Where(x => x.MaterialNo == material.MaterialNo && x.DeleteFlag == 0).FirstOrDefault();
            } else {
                return Ok(MyFun.APIResponseError("[元件品號] 已存在!"));
            }
            material.MaterialBasicId = MaterialBasicData.Id;

            string sRepeatMaterial = null;
            var nMateriallist = new List<Material>();
            foreach (var warehouseId in material.wid)
            {
                //新增時檢查主件品號是否重複
                if (_context.Materials.AsQueryable().Where(x => x.MaterialNo == material.MaterialNo && x.WarehouseId == warehouseId && x.DeleteFlag == 0).Any())
                {
                    sRepeatMaterial += "元件品號 [" + material.MaterialNo + "] 已經存在 [" + material.warehouseData[warehouseId - 1].Name + "] !<br/>";
                }
                else
                {
                    nMateriallist.Add(new Material
                    {
                        MaterialNo = material.MaterialNo,
                        Name = material.Name,
                        Quantity = material.Quantity,
                        Specification = material.Specification,
                        Property = material.Property,
                        Composition = 1,
                        BaseQuantity = 2,
                        WarehouseId = warehouseId,
                        MaterialBasicId = material.MaterialBasicId,
                        CreateUser = 1
                    });
                }
            }

            if (sRepeatMaterial == null)
            {
                _context.AddRange(nMateriallist);
                await _context.SaveChangesAsync();
                return Ok(MyFun.APIResponseOK(material));
            }
            else
            {
                // return BadRequest("主件品號：" + material.MaterialNo + "重複");
                return Ok(MyFun.APIResponseError(sRepeatMaterial, material));
            }

            // //新增時檢查元件品號是否重複
            // if (_context.Materials.Where(x => x.MaterialNo == material.MaterialNo).Any())
            // {
            //     return BadRequest("元件品號：" + material.MaterialNo + "重複");
            //     //return NotFound("元件品號：" + material.MaterialNo + "重複");
            //     //return Ok(MyFun.APIResponseError("元件品號：" + material.MaterialNo + "重複"));
            // }
            // _context.Materials.Add(material);
            // await _context.SaveChangesAsync();

            // return Ok(MyFun.APIResponseOK(material));
        }

        // DELETE: api/MaterialBasics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaterialBasic>> DeleteMaterialBasic(int id)
        {
            var materialBasic = await _context.MaterialBasics.FindAsync(id);
            if (materialBasic == null)
            {
                return NotFound();
            }

            _context.MaterialBasics.Remove(materialBasic);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        private bool MaterialBasicExists(int id)
        {
            return _context.MaterialBasics.Any(e => e.Id == id);
        }
    }
}
