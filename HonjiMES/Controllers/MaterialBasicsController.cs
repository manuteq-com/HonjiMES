﻿using System;
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
    [JWTAuthorize]
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
        public async Task<ActionResult<IEnumerable<MaterialBasicData>>> GetMaterialBasics()
        {
             _context.ChangeTracker.LazyLoadingEnabled = true;
            var materialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderByDescending(x => x.UpdateTime).Select(x => new MaterialBasicData
            {
                TotalCount = x.Materials.Where(y => y.DeleteFlag == 0).Sum(y => y.Quantity),
                Id = x.Id,
                MaterialNo = x.MaterialNo,
                Name = x.Name,
                Specification = x.Specification,
                Property = x.Property,
                Price = x.Price,
                Unit = x.Unit,
                Supplier = x.Supplier,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                DeleteFlag = x.DeleteFlag
            }).ToListAsync();

            return Ok(MyFun.APIResponseOK(materialBasic));
        }

        // GET: api/MaterialBasics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialBasic>>> GetMaterialBasicsAsc()
        {
            var materialBasic = await _context.MaterialBasics.AsQueryable().Where(x => x.DeleteFlag == 0).OrderBy(x => x.MaterialNo).ToListAsync();
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
            OmaterialBasic.UpdateUser = MyFun.GetUserID(HttpContext);

            //更新完basic後，同步更新底下資料
            var Materials = _context.Materials.AsQueryable().Where(x => x.MaterialBasicId == OmaterialBasic.Id && x.DeleteFlag == 0);
            foreach (var item in Materials)
            {
                item.MaterialNo = OmaterialBasic.MaterialNo;
                item.Name = OmaterialBasic.Name;
                item.Specification = OmaterialBasic.Specification;
                item.Property = OmaterialBasic.Property;
                item.Price = OmaterialBasic.Price;
                item.Unit = OmaterialBasic.Unit;
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
                    Price = material.Price,
                    Unit = material.Unit,
                     CreateUser = MyFun.GetUserID(HttpContext)
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
                        Price = material.Price,
                        Unit = material.Unit,
                        Composition = 1,
                        BaseQuantity = 2,
                        WarehouseId = warehouseId,
                        MaterialBasicId = material.MaterialBasicId,
                         CreateUser = MyFun.GetUserID(HttpContext)
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
            materialBasic.DeleteFlag = 1;
            // _context.MaterialBasics.Remove(materialBasic);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(materialBasic));
        }
 
        [HttpGet]
        public async Task<ActionResult<MaterialBasic>> CheckMaterialNumber(string DataNo)
        {
            var materialBasicNo = await _context.MaterialBasics.Where(x => x.MaterialNo == DataNo && x.DeleteFlag == 0).AnyAsync();
            if (materialBasicNo) {
                return Ok(MyFun.APIResponseError("[原料品號]已存在!"));
            }
            return Ok(MyFun.APIResponseOK(""));
        }

        private bool MaterialBasicExists(int id)
        {
            return _context.MaterialBasics.Any(e => e.Id == id);
        }
    }
}