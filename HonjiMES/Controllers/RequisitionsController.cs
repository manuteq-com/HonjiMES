using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 領料管理
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RequisitionsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public RequisitionsController(HonjiContext context)
        {
            _context = context;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }
        /// <summary>
        /// 取領料單資訊
        /// </summary>
        /// <returns></returns>
        // GET: api/MaterialRequisitions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Requisition>>> GetRequisitions([FromQuery] DataSourceLoadOptions FromQuery)
        {
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(_context.Requisitions.AsQueryable(), FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        // GET: api/Requisitions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Requisition>> GetRequisition(int id)
        {
            var materialRequisition = await _context.Requisitions.FindAsync(id);

            if (materialRequisition == null)
            {
                return NotFound();
            }

            //return materialRequisition;
            return Ok(MyFun.APIResponseOK(materialRequisition));
        }

        // PUT: api/MaterialRequisitions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequisition(int id, Requisition Requisition)
        {
            if (id != Requisition.Id)
            {
                return BadRequest();
            }

            _context.Entry(Requisition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequisitionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(MyFun.APIResponseOK(Requisition));
        }

        // POST: api/MaterialRequisitions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Requisition>> PostRequisition(Requisition requisition)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            if (requisition.ProductBasicId == 0)
            {
                return Ok(MyFun.APIResponseError("沒有品號資料"));
            }
            var dt = DateTime.Now;
            var ProductBasics = _context.ProductBasics.Find(requisition.ProductBasicId);
            requisition.RequisitionNo = dt.ToString("yyyyMMddHHmmss");
            requisition.Name = ProductBasics.Name;
            requisition.ProductNo = ProductBasics.ProductNo;
            requisition.ProductNumber = ProductBasics.ProductNumber;
            requisition.Specification = ProductBasics.Specification;
            requisition.CreateTime = dt;
            requisition.CreateUser = 1;
            // BOM內容
            var BillOfMaterials = await _context.BillOfMaterials.Where(x => x.ProductBasicId == requisition.ProductBasicId && x.DeleteFlag == 0 && !x.Pid.HasValue).ToListAsync();
            foreach (var item in MyFun.GetBomList(BillOfMaterials, 0, requisition.Quantity))
            {
                requisition.RequisitionDetails.Add(new RequisitionDetail
                {
                    Lv = item.Lv,
                    Name = item.Name,
                    //原料
                    MaterialBasicId = item.MaterialBasicId.Value,
                    MaterialNo = item.MaterialNo,
                    MaterialName = item.MaterialName,
                    MaterialSpecification = item.MateriaSpecification,
                    //成品，半成品，組件
                    ProductBasicId = item.ProductBasicId,
                    ProductName = item.Name,
                    ProductNo = item.ProductNo,
                    ProductNumber = item.ProductNumber,
                    ProductSpecification = item.ProductSpecification,

                    Quantity = item.ReceiveQty,
                    CreateTime = dt,
                    CreateUser = 1
                });
            }
            _context.Requisitions.Add(requisition);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(requisition));
        }

        // DELETE: api/MaterialRequisitions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Requisition>> DeleteRequisition(int id)
        {
            var Requisitions = await _context.Requisitions.FindAsync(id);
            if (Requisitions == null)
            {
                return NotFound();
            }
            _context.Requisitions.Remove(Requisitions);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(Requisitions));
        }

        private bool RequisitionExists(int id)
        {
            return _context.Requisitions.Any(e => e.Id == id);
        }
    }
}
