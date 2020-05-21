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
            var materialBasic = await _context.MaterialBasics.AsQueryable().ToListAsync();
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
            if (id != materialBasic.Id)
            {
                return BadRequest();
            }

            _context.Entry(materialBasic).State = EntityState.Modified;

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

            return Ok(MyFun.APIResponseOK(materialBasic));
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
