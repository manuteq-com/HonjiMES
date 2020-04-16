﻿using System;
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
    /// 銷貨資料
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly HonjiContext _context;

        public SalesController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 銷貨列表
        /// </summary>
        /// <param name="status">0:未銷貨，1:已銷貨</param>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleHead>>> GetSales(int? status)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var SaleHeads = _context.SaleHeads.AsQueryable();
            if (status.HasValue)
            {
                SaleHeads = SaleHeads.Where(x => x.Status == status);
            }
            var data = await SaleHeads.OrderByDescending(x=>x.CreateTime).ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }

        /// <summary>
        /// 用ID取銷貨單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleHead>> GetSale(int id)
        {
            var SaleHeads = await _context.SaleHeads.FindAsync(id);

            if (SaleHeads == null)
            {
                return Ok(MyFun.APIResponseError("銷貨單ID錯誤"));
            }

            return Ok(MyFun.APIResponseOK(SaleHeads));
        }

        /// <summary>
        /// 用ID取銷貨單號
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<SaleHead> GetSaleNo(int id)
        {
            var SaleHeads = _context.SaleHeads.Find(id);
            if (SaleHeads == null)
            {
                return Ok(MyFun.APIResponseError("銷貨單ID錯誤"));
            }
            return Ok(MyFun.APIResponseOK(new { SaleHeads.Id, SaleHeads.SaleNo }));
        }
        // PUT: api/Sales/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, SaleHead saleHead)
        {
            if (id != saleHead.Id)
            {
                return BadRequest();
            }

            _context.Entry(saleHead).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sales
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(SaleHead saleHead)
        {
            _context.SaleHeads.Add(saleHead);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSale", new { id = saleHead.Id }, saleHead);
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SaleHead>> DeleteSale(int id)
        {
            var SaleHead = await _context.SaleHeads.FindAsync(id);
            if (SaleHead == null)
            {
                return NotFound();
            }

            _context.SaleHeads.Remove(SaleHead);
            await _context.SaveChangesAsync();

            return SaleHead;
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
