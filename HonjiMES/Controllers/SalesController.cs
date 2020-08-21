using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{
    /// <summary>
    /// 銷貨資料
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly HonjiContext _context;

        public SalesController(HonjiContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _IWebHostEnvironment = environment;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 銷貨單列表
        /// </summary>
        /// <param name="FromQuery"></param>
        /// <param name="detailfilter"></param>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleHead>>> GetSales(
                [FromQuery] DataSourceLoadOptions FromQuery,
                [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.SaleHeads.Where(x => x.DeleteFlag == 0);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            if (!string.IsNullOrWhiteSpace(qSearchValue.OrderNo))
            {
                data = data.Where(x => x.SaleDetailNews.Where(y => y.Order.OrderNo.Contains(qSearchValue.OrderNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            {
                data = data.Where(x => x.SaleDetailNews.Where(y => y.OrderDetail.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            if (!string.IsNullOrWhiteSpace(qSearchValue.ProductNo))
            {
                data = data.Where(x => x.SaleDetailNews.Where(y => y.ProductNo.Contains(qSearchValue.ProductNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }

            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 新增銷貨單之訂單列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderlist>>> GetOrderList()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;//停止關連，減少資料
            var orderlists = await _context.OrderDetails.Where(x => x.DeleteFlag == 0 && x.Quantity > x.SaleCount && x.ProductBasic.DeleteFlag == 0).Include(x => x.Order).Include(x => x.ProductBasic).Select(x => new Orderlist
            {
                Id = x.Id,
                OrderNo = x.Order.OrderNo,
                MachineNo = x.MachineNo,
                ProductBasicId = x.ProductBasic.Id,
                ProductNo = x.ProductBasic.ProductNo,
                ProductName = x.ProductBasic.Name,
                Specification = x.ProductBasic.Specification,
                SaleCount = x.SaleCount,
                Quantity = x.Quantity,
                Unit = x.Unit,
                OriginPrice = x.OriginPrice,
                Price = x.Price,
                DueDate = x.DueDate,
                ReplyDate = x.ReplyDate,
                Remark = x.Remark,
                ReplyRemark = x.ReplyRemark
            }).ToListAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            return Ok(MyFun.APIResponseOK(orderlists));
        }

        /// <summary>
        /// 銷貨單列表
        /// </summary>
        /// <param name="status">0:未銷貨，1:已銷貨</param>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleHead>>> GetSalesByStatus(int? status)
        {
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var SaleHeads = _context.SaleHeads.AsQueryable();
            if (status.HasValue)
            {
                SaleHeads = SaleHeads.Where(x => x.Status == status && x.DeleteFlag == 0);
            }
            var data = await SaleHeads.OrderByDescending(x => x.CreateTime).ToListAsync();
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
        /// <summary>
        /// 修改銷貨單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saleHead"></param>
        /// <returns></returns>
        // PUT: api/Sales/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, SaleHead saleHead)
        {
            saleHead.Id = id;
            var OsaleHead = _context.SaleHeads.Find(id);
            var Msg = MyFun.MappingData(ref OsaleHead, saleHead);
            OsaleHead.UpdateTime = DateTime.Now;
            OsaleHead.UpdateUser = MyFun.GetUserID(HttpContext);
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
            return Ok(MyFun.APIResponseOK(saleHead));
        }
        /// <summary>
        /// 新增銷貨單
        /// </summary>
        /// <param name="saleHead"></param>
        /// <returns></returns>
        // POST: api/Sales
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Sale>> PostSale(SaleHead saleHead)
        {
            _context.SaleHeads.Add(saleHead);
            await _context.SaveChangesAsync();

            return Ok(MyFun.APIResponseOK(saleHead));
        }
        /// <summary>
        /// 刪除銷貨單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SaleHead>> DeleteSale(int id)
        {
            var SaleHead = await _context.SaleHeads.FindAsync(id);
            if (SaleHead == null)
            {
                return NotFound();
            }
            SaleHead.DeleteFlag = 1;
            // _context.SaleHeads.Remove(SaleHead);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(SaleHead));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
