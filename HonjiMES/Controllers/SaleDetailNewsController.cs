﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 銷貨單明細
    /// </summary>
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SaleDetailNewsController : ControllerBase
    {
        private readonly HonjiContext _context;

        public SaleDetailNewsController(HonjiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查詢所有銷貨明細
        /// </summary>
        /// <returns></returns>
        // GET: api/SaleDetailNews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailNew>>> GetSaleDetailNews()
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var SaleDetailNews = await _context.SaleDetailNews.ToListAsync();
            return Ok(MyFun.APIResponseOK(SaleDetailNews));
        }
        /// <summary>
        /// 用ID查詢銷貨明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/SaleDetailNews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetailNew>> GetSaleDetailNew(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var saleDetailNew = await _context.SaleDetailNews.FindAsync(id);

            if (saleDetailNew == null)
            {
                return NotFound();
            }

            return Ok(MyFun.APIResponseOK(saleDetailNew));
 
        }
        /// <summary>
        /// 查詢銷貨單明細
        /// </summary>
        /// <param name="SaleId">銷貨單ID 非必填</param>
        /// <returns>訂單明細</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailNew>>> GetSaleDetailsBySaleId(int? SaleId)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var SaleDetailNews = _context.SaleDetailNews.Include(x => x.Order).Include(x => x.OrderDetail).AsQueryable();
            if (SaleId.HasValue)
            {
                SaleDetailNews = SaleDetailNews.Where(x => x.SaleId == SaleId);
            }
            var data = await SaleDetailNews.ToListAsync();
            return Ok(MyFun.APIResponseOK(data));
        }
        /// <summary>
        /// 修改銷貨明細
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saleDetailNew"></param>
        /// <returns></returns>
        // PUT: api/SaleDetailNews/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleDetailNew(int id, SaleDetailNew saleDetailNew)
        {

            saleDetailNew.Id = id;
            var OsaleDetailNew = _context.SaleDetailNews.Find(id);

            var Msg = MyFun.MappingData(ref OsaleDetailNew, saleDetailNew);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleDetailNewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(OsaleDetailNew));
        }
        /// <summary>
        /// 修改銷貨明細數量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saleDetailNew"></param>
        /// <returns></returns>
        // PUT: api/SaleDetailNews/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleDetailNewQty(int id, SaleDetailNew saleDetailNew)
        {
            var ErrMsg = "";
            var dQty = 0;
            saleDetailNew.Id = id;
            var OsaleDetailNew = _context.SaleDetailNews.Find(id);
            dQty = saleDetailNew.Quantity - OsaleDetailNew.Quantity; //差異數量
            var ProductId = OsaleDetailNew.ProductId; //產品ID
            var OrderDetailId = OsaleDetailNew.OrderDetailId; //訂單明細ID

            //修改銷貨數量
            OsaleDetailNew.Quantity += dQty;
            if (dQty <= 0)//減少數量直接計算
            {
                OsaleDetailNew.OrderDetail.SaleCount += dQty;  //修改訂單已銷貨數量
            }
            else
            {
                //檢查多次轉銷貨的數量
                var SaleDetailNewslist = _context.SaleDetailNews.Where(x => x.DeleteFlag == 0 && x.OrderDetailId == OrderDetailId && x.ProductId == ProductId);
                var AllQty = SaleDetailNewslist.Sum(x => x.Quantity);//轉銷貨的總數量
                if (AllQty + dQty <= OsaleDetailNew.OrderDetail.Quantity)//不超過總採購數
                {
                    OsaleDetailNew.OrderDetail.SaleCount += dQty;  //修改訂單已銷貨數量
                }
                else
                {
                    ErrMsg += "銷貨數超過採購數 \r\n" + (OsaleDetailNew.OrderDetail.Quantity + dQty) + ">" + OsaleDetailNew.OrderDetail.Quantity + " \r\n";
                    ErrMsg += "請檢查銷貨單號：" + string.Join(',', SaleDetailNewslist.Select(x => x.Sale.SaleNo).Distinct());
                }
            }
            OsaleDetailNew.Product.QuantityAdv += dQty;
            try
            {
                if (string.IsNullOrWhiteSpace(ErrMsg))
                {
                    _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Ok(MyFun.APIResponseError(ErrMsg));
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleDetailNewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MyFun.APIResponseOK(OsaleDetailNew));
        }
        /// <summary>
        /// 新增銷貨明細
        /// </summary>
        /// <param name="saleDetailNew"></param>
        /// <returns></returns>
        // POST: api/SaleDetailNews
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<SaleDetailNew>> PostSaleDetailNew(SaleDetailNew saleDetailNew)
        {
            _context.SaleDetailNews.Add(saleDetailNew);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(saleDetailNew));
        }
        /// <summary>
        /// 刪除銷貨明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/SaleDetailNews/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SaleDetailNew>> DeleteSaleDetailNew(int id)
        {
            var saleDetailNew = await _context.SaleDetailNews.FindAsync(id);
            if (saleDetailNew == null)
            {
                return NotFound();
            }
            _context.SaleDetailNews.Remove(saleDetailNew);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(saleDetailNew));
        }

        private bool SaleDetailNewExists(int id)
        {
            return _context.SaleDetailNews.Any(e => e.Id == id);
        }
    }
}