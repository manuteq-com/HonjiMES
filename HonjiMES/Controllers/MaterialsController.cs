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
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
        {
            var Materials = await _context.Materials.ToListAsync();
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
        public async Task<ActionResult<Material>> PostMaterial(Material material)
        {
            //新增時檢查元件品號是否重複
            if (_context.Materials.Where(x => x.MaterialNo == material.MaterialNo).Any())
            {
                return BadRequest("元件品號：" + material.MaterialNo + "重複");
                //return NotFound("元件品號：" + material.MaterialNo + "重複");
                //return Ok(MyFun.APIResponseError("元件品號：" + material.MaterialNo + "重複"));
            }
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

             return Ok(MyFun.APIResponseOK(material));
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
