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
            if (!string.IsNullOrWhiteSpace(qSearchValue.MaterialNo))
            {
                data = data.Where(x => x.SaleDetailNews.Where(y => y.MaterialNo.Contains(qSearchValue.MaterialNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }

            data = data.Include(x => x.SaleDetailNews);
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
            var orderlists = await _context.OrderDetails
                .Where(x => x.DeleteFlag == 0 && x.Order.DeleteFlag == 0 && x.Quantity > x.SaledCount && x.MaterialBasic.DeleteFlag == 0)
                .Include(x => x.Order).ToListAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            return Ok(MyFun.APIResponseOK(orderlists));
        }

        /// <summary>
        /// 銷貨單列表
        /// </summary>
        /// <param name="status">0:未銷貨，1:開始銷貨，2:完成銷貨</param>
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
        // /// <summary>
        // /// 修改銷貨單
        // /// </summary>
        // /// <param name="id"></param>
        // /// <param name="saleHead"></param>
        // /// <returns></returns>
        // // PUT: api/Sales/5
        // // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // // more details see https://aka.ms/RazorPagesCRUD.
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutSale(int id, SaleHead saleHead)
        // {
        //     saleHead.Id = id;
        //     var OsaleHead = _context.SaleHeads.Find(id);
        //     var Msg = MyFun.MappingData(ref OsaleHead, saleHead);
        //     OsaleHead.UpdateTime = DateTime.Now;
        //     OsaleHead.UpdateUser = MyFun.GetUserID(HttpContext);
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!SaleExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //     return Ok(MyFun.APIResponseOK(saleHead));
        // }
        // /// <summary>
        // /// 新增銷貨單
        // /// </summary>
        // /// <param name="saleHead"></param>
        // /// <returns></returns>
        // // POST: api/Sales
        // // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // // more details see https://aka.ms/RazorPagesCRUD.
        // [HttpPost]
        // public async Task<ActionResult<Sale>> PostSale(SaleHead saleHead)
        // {
        //     _context.SaleHeads.Add(saleHead);
        //     await _context.SaveChangesAsync();

        //     return Ok(MyFun.APIResponseOK(saleHead));
        // }
        /// <summary>
        /// 刪除銷貨單
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SaleHead>> DeleteSale(int id)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var SaleHead = await _context.SaleHeads.FindAsync(id);
            if (SaleHead == null)
            {
                return NotFound();
            }

            SaleHead.DeleteFlag = 1;
            foreach (var item in SaleHead.SaleDetailNews)
            {
                if (item.DeleteFlag == 0)
                {
                    item.DeleteFlag = 1;
                    item.OrderDetail.SaleCount -= item.Quantity;
                }
            }
            // _context.SaleHeads.Remove(SaleHead);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            return Ok(MyFun.APIResponseOK(SaleHead));
        }

        /// <summary>
        /// 取得銷貨單明細列表
        /// </summary>
        /// <returns></returns>
        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailNew>>> GetSaleOrderList()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;//停止關連，減少資料
            var SaleDetailNews = await _context.SaleDetailNews
                .Where(x => x.DeleteFlag == 0 && x.Sale.DeleteFlag == 0)
                // .Include(x => x.Order)
                // .Include(x => x.OrderDetail)
                // .Include(x => x.Sale)
                .OrderByDescending(x => x.Sale.SaleNo).ThenBy(x => x.OrderDetail.Serial)
                .Select(x => new SaleDetailNewData
                {
                    TotalCount = x.MaterialBasic.Materials.Where(y => y.DeleteFlag == 0 && y.Warehouse.Code == "301").Sum(y => y.Quantity),
                    Id = x.Id,
                    SaleId = x.SaleId,
                    OrderId = x.OrderId,
                    OrderDetailId = x.OrderDetailId,
                    MaterialBasicId = x.MaterialBasicId,
                    MaterialId = x.MaterialId,
                    SaleNo = x.Sale.SaleNo,
                    SaleDate = x.Sale.SaleDate,
                    CustomerNo = x.Order.CustomerNo,
                    OrderNo = x.Order.OrderNo,
                    Serial = x.OrderDetail.Serial,
                    MachineNo = x.OrderDetail.MachineNo,
                    MaterialNo = x.MaterialNo,
                    Status = x.Status,
                    Name = x.Name,
                    Specification = x.Specification,
                    Quantity = x.Quantity,
                    OriginPrice = x.OriginPrice,
                    Price = x.Price,
                    Remarks = x.Remarks,
                    CreateTime = x.CreateTime,
                    CreateUser = x.CreateUser,
                    UpdateTime = x.UpdateTime,
                    UpdateUser = x.UpdateUser,
                    DeleteFlag = x.DeleteFlag,
                }).ToListAsync();
            _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            return Ok(MyFun.APIResponseOK(SaleDetailNews));
        }

        /// <summary>
        /// 查詢銷貨單全部資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailNew>>> GetSaleData(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
           _context.ChangeTracker.LazyLoadingEnabled = false;//停止關連，減少資料
            var SaleDetailNews = _context.SaleDetailNews.Where(x => x.DeleteFlag == 0 )
            .Include(x => x.Order).Include(x => x.OrderDetail).Include(x => x.Sale).OrderByDescending(x => x.Sale.SaleNo).ThenBy(x => x.OrderDetail.Serial)
            .Select(x => new SaleDetailNewData
            {
                TotalCount = x.MaterialBasic.Materials.Where(y => y.DeleteFlag == 0 && y.Warehouse.Code == "301").Sum(y => y.Quantity),
                Id = x.Id,
                SaleId = x.SaleId,
                OrderId = x.OrderId,
                OrderDetailId = x.OrderDetailId,
                MaterialBasicId = x.MaterialBasicId,
                MaterialId = x.MaterialId,
                SaleNo = x.Sale.SaleNo,
                SaleDate = x.Sale.SaleDate,
                CustomerNo = x.Order.CustomerNo,
                OrderNo = x.Order.OrderNo,
                Serial = x.OrderDetail.Serial,
                MachineNo = x.OrderDetail.MachineNo,
                MaterialNo = x.MaterialNo,
                Status = x.Status,
                Name = x.Name,
                Specification = x.Specification,
                Quantity = x.Quantity,
                OriginPrice = x.OriginPrice,
                Price = x.Price,
                Remarks = x.Remarks,
                CreateTime = x.CreateTime,
                CreateUser = x.CreateUser,
                UpdateTime = x.UpdateTime,
                UpdateUser = x.UpdateUser,
                DeleteFlag = x.DeleteFlag,
            });
           // var data = SaleDetailNews.ToListAsync();
            var FromQueryResult =await  MyFun.ExFromQueryResultAsync(SaleDetailNews, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 銷貨退回紀錄報表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetailNew>>> GetSaleReturnRecord(
            [FromQuery] DataSourceLoadOptions FromQuery,
            [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var Users = _context.Users.Where(x => x.DeleteFlag == 0);
            // var Warehouses = _context.Warehouses.Where(x => x.DeleteFlag == 0);
            var data = _context.ReturnSales.Where(x => x.DeleteFlag == 0)
            .Include(x => x.SaleDetailNew)
            .Include(x => x.SaleDetailNew.Sale)
            .Include(x => x.SaleDetailNew.Order)
            .Include(x => x.SaleDetailNew.OrderDetail)
            .Include(x => x.SaleDetailNew.MaterialBasic)
            .Include(x => x.SaleDetailNew.Material)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.SaleDetailNew.Sale.SaleNo)
            .Select(x => new SaleDetailNewReturnData {
                Id = x.Id,
                ReturnWarehouse = x.Warehouse.Code + x.Warehouse.Name,
                ReturnQuantity = x.Quantity,
                ReturnReason = x.Reason,
                ReturnRemarks = x.Remarks,
                ReturnCreateTime = x.CreateTime,
                ReturnCreateUser = Users.Where(y => y.Id == x.CreateUser).FirstOrDefault().Realname,

                SaleNo = x.SaleDetailNew.Sale.SaleNo,
                SaleDate = x.SaleDetailNew.Sale.SaleDate,
                CustomerNo = x.SaleDetailNew.Order.CustomerNo,
                OrderNo = x.SaleDetailNew.Order.OrderNo,
                Serial = x.SaleDetailNew.OrderDetail.Serial,
                MachineNo = x.SaleDetailNew.OrderDetail.MachineNo,
                MaterialNo = x.SaleDetailNew.MaterialBasic.MaterialNo,
                Specification = x.SaleDetailNew.MaterialBasic.Specification
            });
            // data.LeftOuterJoin(_context.SaleLogs, x => new {x.CreateTime, x.CreateUser }, y => y.CreateTime, (o,s) => new SaleDetailNewData{
            //     SaleNo = o.SaleNo, o,s});
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

    }
}
