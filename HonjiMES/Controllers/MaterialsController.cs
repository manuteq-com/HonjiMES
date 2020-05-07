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
    public class MaterialsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public MaterialsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
        {
            var data = _context.Materials.Where(x => x.DeleteFlag == 0);
            var Materials = await data.ToListAsync();
            return Ok(MyFun.APIResponseOK(Materials));
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(material));
        }

        // PUT: api/Materials/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterial(int id, Material material)
        {
            //修改時檢查名稱重複
            if (!string.IsNullOrWhiteSpace(material.MaterialNo))
            {
                if (_context.Materials.Where(x => x.MaterialNo == material.MaterialNo && x.Id != id).Any())
                {
                    return BadRequest("元件品號：" + material.MaterialNo + "重複");    
                    //return Ok(MyFun.APIResponseError("元件品號：" + material.MaterialNo + "重複"));
                }
            }
            material.Id = id;
            var Oldmaterial = _context.Materials.Find(id);

            var Msg = MyFun.MappingData(ref Oldmaterial, material);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(Oldmaterial));
        }

        // POST: api/Materials
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Material>> PostMaterial(MaterialW material)
        {
            if (material.wid == null) {
                return Ok(MyFun.APIResponseError("請選擇 [存放庫別]!", material));
            }
            
            //優先確認Basic是否存在
            var MaterialBasicData = _context.MaterialBasics.Where(x => x.MaterialNo == material.MaterialNo && x.DeleteFlag == 0).FirstOrDefault();
            if (MaterialBasicData == null)
            {
                var MaterialBasics = new List<MaterialBasic>();
                _context.MaterialBasics.Add(new MaterialBasic
                {
                    MaterialNo = material.MaterialNo,
                    Name = material.Name,
                    Specification = material.Specification,
                    Property = material.Property
                });
                _context.SaveChanges();
                MaterialBasicData = _context.MaterialBasics.Where(x => x.MaterialNo == material.MaterialNo && x.DeleteFlag == 0).FirstOrDefault();
            }
            material.MaterialBasicId = MaterialBasicData.Id;

            string sRepeatMaterial = null;
            var nMateriallist = new List<Material>();
            foreach (var warehouseId in material.wid)
            {
                //新增時檢查主件品號是否重複
                if (_context.Materials.Where(x => x.MaterialNo == material.MaterialNo && x.WarehouseId == warehouseId).Any())
                {
                    sRepeatMaterial += "元件品號：[" + material.MaterialNo + "] 已經存在 [" + material.warehouseData[warehouseId - 1].Name + "] !<br/>";
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

        // DELETE: api/Materials/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Material>> DeleteMaterial(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return material;
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }
}
