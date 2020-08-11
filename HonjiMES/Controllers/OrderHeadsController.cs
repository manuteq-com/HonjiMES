﻿using DevExtreme.AspNet.Mvc;
using HonjiMES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Filter;

namespace HonjiMES.Controllers
{

    /// <summary>
    /// 訂單主檔
    /// </summary>
    [JWTAuthorize]
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderHeadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly HonjiContext _context;
        public OrderHeadsController(HonjiContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _IWebHostEnvironment = environment;
            _context.ChangeTracker.LazyLoadingEnabled = false;//加快查詢用，不抓關連的資料
        }

        /// <summary>
        /// 訂單單號
        /// </summary>
        /// <returns></returns>
        // GET: api/OrderHeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetOrderNumber()
        {
            var key = "";
            var dt = DateTime.Now;
            var OrderNo = dt.ToString("yyMMdd");

            var NoData = await _context.OrderHeads.AsQueryable().Where(x => x.OrderNo.Contains(key + OrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
            var NoCount = NoData.Count() + 1;
            if (NoCount != 1)
            {
                var LastOrderNo = NoData.FirstOrDefault().OrderNo;
                var NoLast = Int32.Parse(LastOrderNo.Substring(LastOrderNo.Length - 3, 3));
                if (NoCount <= NoLast)
                {
                    NoCount = NoLast + 1;
                }
            }
            var OrderHeadData = new OrderHead
            {
                CreateTime = dt,
                OrderNo = key + OrderNo + NoCount.ToString("000"),
                OrderDate = dt,
                ReplyDate = dt
            };
            return Ok(MyFun.APIResponseOK(OrderHeadData));
        }

        /// <summary>
        /// 訂單單號
        /// </summary>
        /// <returns></returns>
        // POST: api/OrderHeads
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CreateNumberInfo>>> GetOrderNumberByInfo(CreateNumberInfo CreateNoData)
        {
            if (CreateNoData != null)
            {
                var key = "";
                var OrderNo = CreateNoData.CreateTime.ToString("yyMMdd");

                var NoData = await _context.OrderHeads.AsQueryable().Where(x => x.OrderNo.Contains(key + OrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime).ToListAsync();
                var NoCount = NoData.Count() + 1;
                if (NoCount != 1)
                {
                    var LastOrderNo = NoData.FirstOrDefault().OrderNo;
                    var NoLast = Int32.Parse(LastOrderNo.Substring(LastOrderNo.Length - 3, 3));
                    if (NoCount <= NoLast)
                    {
                        NoCount = NoLast + 1;
                    }
                }
                CreateNoData.CreateNumber = key + OrderNo + NoCount.ToString("000");
                return Ok(MyFun.APIResponseOK(CreateNoData));
            }
            return Ok(MyFun.APIResponseOK("OK"));
        }

        /// <summary>
        /// 查詢訂單主檔
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetOrderHeads(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            //_context.ChangeTracker.LazyLoadingEnabled = true;//加快查詢用，不抓關連的資料

            var data = _context.OrderHeads.Where(x => x.DeleteFlag == 0);
            var qSearchValue = MyFun.JsonToData<SearchValue>(detailfilter);
            if (!string.IsNullOrWhiteSpace(qSearchValue.MachineNo))
            {
                data = data.Where(x => x.OrderDetails.Where(y => y.MachineNo.Contains(qSearchValue.MachineNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            if (!string.IsNullOrWhiteSpace(qSearchValue.ProductNo))
            {
                data = data.Where(x => x.OrderDetails.Where(y => y.ProductBasic.ProductNo.Contains(qSearchValue.ProductNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }

            //var OrderHeads = await data.OrderByDescending(x => x.CreateTime).ToListAsync();
            // object[] parameters = new object[] { };
            // var query = "select id,create_date,order_no from order_head";
            // var FromSqlRawdata = await _context.OrderHeads.FromSqlRaw(query, parameters).Select(x => x).ToListAsync();
            // var id = 1;
            // FormattableString myCommand = $"select * from order_head where id={id}";
            // var ndata = await _context.Database.ExecuteSqlInterpolatedAsync(myCommand);
            // var conn = _context.Database.GetDbConnection();
            // await conn.OpenAsync();
            // var command = conn.CreateCommand();
            // command.CommandText = query;
            // var reader = await command.ExecuteReaderAsync();
            // var nOrderHead = new List<OrderHead>();
            // while (await reader.ReadAsync())
            // {
            //     nOrderHead.Add(DBHelper.DataReaderMapping<OrderHead>(reader));
            // }
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 使用ID查詢訂單主檔
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHead>> GetOrderHead(int id)
        {
            var orderHead = await _context.OrderHeads.FindAsync(id);

            if (orderHead == null)
            {
                return NotFound();
            }
            return Ok(MyFun.APIResponseOK(orderHead));
        }

        // PUT: api/OrderHeads/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.

        /// <summary>
        /// 修改訂單主檔
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderHead"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderHead(int id, OrderHead orderHead)
        {
            orderHead.Id = id;
            var OldorderHead = _context.OrderHeads.Find(id);
            var Msg = MyFun.MappingData(ref OldorderHead, orderHead);

            OldorderHead.UpdateTime = DateTime.Now;
            OldorderHead.UpdateUser = MyFun.GetUserID(HttpContext);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHeadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(MyFun.APIResponseOK(orderHead));
        }

        // POST: api/OrderHeads
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.

        /// <summary>
        /// 新增訂單主檔
        /// </summary>
        /// <param name="orderHead"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostOrderHead(OrderHead orderHead)
        {
            _context.OrderHeads.Add(orderHead);
            orderHead.CreateTime = DateTime.Now;
            orderHead.CreateUser = MyFun.GetUserID(HttpContext);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(orderHead));
        }

        // DELETE: api/OrderHeads/5
        /// <summary>
        /// 刪除訂單內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderHead>> DeleteOrderHead(int id)
        {
            var orderHead = await _context.OrderHeads.FindAsync(id);
            if (orderHead == null)
            {
                return NotFound();
            }
            orderHead.DeleteFlag = 1;
            // _context.OrderHeads.Remove(orderHead);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(orderHead));
        }
        /// <summary>
        /// 新增訂單主檔同時新明細
        /// </summary>
        /// <param name="PostOrderMaster_Detail"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostOrderMaster_Detail(PostOrderMaster_Detail PostOrderMaster_Detail)
        {
            try
            {
                var checkNo = 0;
                var errorList = "";
                if (PostOrderMaster_Detail.OrderDetail.Count() != 0)
                {
                    var ListOrderHeads = ListOrderHead(PostOrderMaster_Detail);
                    var ListOrderHeadsCheck = false;
                    foreach (var OrderHeadData in ListOrderHeads)
                    {
                        var checkCustomer = _context.OrderHeads.AsQueryable().Where(x => x.CustomerNo == OrderHeadData.CustomerNo && x.DeleteFlag == 0).Count();
                        if (checkCustomer != 0)
                        {
                            errorList += " [ " + OrderHeadData.CustomerNo + " ] ";
                            // return Ok(MyFun.APIResponseError("[客戶單號： " + OrderHeadData.CustomerNo + " ] 重複建立!"));
                        }
                        else
                        {
                            ListOrderHeadsCheck = true;
                            var OrderDetail = new List<OrderDetail>();
                            foreach (var item in PostOrderMaster_Detail.OrderDetail)
                            {
                                if (item.CustomerNo.Contains(OrderHeadData.CustomerNo))
                                {
                                    OrderDetail.Add(item);
                                }
                            }
                            var dt = DateTime.Now;
                            // var OrderNo = dt.ToString("yyMMdd");
                            var OrderNo = PostOrderMaster_Detail.OrderHead.CreateTime.ToString("yyMMdd");
                            var NoData = _context.OrderHeads.AsQueryable().Where(x => x.OrderNo.StartsWith(OrderNo) && x.DeleteFlag == 0).OrderByDescending(x => x.CreateTime);
                            var NoCount = NoData.Count() + 1;
                            if (NoCount != 1)
                            {
                                var LastOrderNo = NoData.FirstOrDefault().OrderNo;
                                var NoLast = Int32.Parse(LastOrderNo.Substring(LastOrderNo.Length - 3, 3));
                                if (NoCount <= NoLast)
                                {
                                    NoCount = NoLast + 1;
                                }
                                if (checkNo != 0 && checkNo >= NoCount)
                                {
                                    NoCount = checkNo + 1;
                                }
                                checkNo = NoCount;
                            }

                            var orderHead = new OrderHead
                            {
                                OrderNo = PostOrderMaster_Detail.OrderHead.OrderNo,
                                OrderDate = PostOrderMaster_Detail.OrderHead.OrderDate,
                                ReplyDate = PostOrderMaster_Detail.OrderHead.ReplyDate,
                                Customer = PostOrderMaster_Detail.OrderHead.Customer
                            };
                            var DirName = orderHead.OrderNo;
                            orderHead.CustomerNo = OrderHeadData.CustomerNo;//替換客戶單號
                            orderHead.OrderNo = OrderNo + NoCount.ToString("000");
                            orderHead.CreateTime = dt;
                            orderHead.CreateUser = MyFun.GetUserID(HttpContext);
                            var OrderDetails = new List<OrderDetail>();
                            foreach (var item in OrderDetail)
                            {
                                item.CreateTime = dt;
                                item.CreateUser = MyFun.GetUserID(HttpContext);
                                OrderDetails.Add(item);
                            }
                            orderHead.OrderDetails = OrderDetails.OrderBy(x => x.Serial).ToList();
                            _context.OrderHeads.Add(orderHead);
                            await _context.SaveChangesAsync();
                            MemoryStream excelDatas = MyFun.DataToExcel(_context, orderHead);
                            //Excel存檔
                            var webRootPath = _IWebHostEnvironment.WebRootPath;
                            var Dir = $"{webRootPath}";
                            var bsave = MyFun.ProcessSaveExcelAsync(Dir, DirName, orderHead.OrderNo, excelDatas.ToArray());
                        }
                    }
                    if (ListOrderHeadsCheck)
                    {
                        return Ok(MyFun.APIResponseOK("OK", errorList));
                    }
                    else
                    {
                        return Ok(MyFun.APIResponseError("新建失敗! 請確認[客戶單號]是否重複!"));
                    }
                }
                else
                {
                    return Ok(MyFun.APIResponseError("請確認[訂單明細]!"));
                }


                //return Ok(new { success = true, timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "", data = true});
                //return CreatedAtAction("GetOrderHead", new { id = PostOrderMaster_Detail.OrderHead.Id }, PostOrderMaster_Detail.OrderHead);
            }
            catch (Exception ex)
            {
                return Ok(MyFun.APIResponseError(ex.Message));
            }
        }
        /// <summary>
        /// 匯入訂單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public ActionResult<OrderHead> PostOrdeByExcel()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            string sLostProduct = null;
            var DirName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var OrderHeadlist = new List<OrderHead>();//所有檔案
            var myFile = Request.Form.Files;
            if (myFile.Any())
            {
                var MappingList = DBHelper.MappingExtelToModelData();
                foreach (var item in myFile)
                {
                    try
                    {
                        //暫存Excel檔案
                        var webRootPath = _IWebHostEnvironment.WebRootPath;
                        var Dir = $"{webRootPath}";
                        var bsave = MyFun.ProcessSaveTempExcelAsync(Dir, DirName, item);
                        if (string.IsNullOrWhiteSpace(bsave.Result))
                        {
                            var Files = MyFun.ProcessGetTempExcelAsync(Dir, DirName);
                            foreach (var Fileitem in Files)
                            {
                                OrderHeadlist.AddRange(DBHelper.GetExcelData(Fileitem, _context, ref sLostProduct));
                            }
                        }
                        else
                        {
                            return Ok(MyFun.APIResponseError("Excel存檔失敗：" + bsave.Result));
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok(MyFun.APIResponseError(ex.Message));
                    }
                }
            }
            foreach (var Headitem in OrderHeadlist.ToList())
            {
                var len = ("000-000000000").Length;
                Headitem.OrderNo = DirName;
                if (!string.IsNullOrWhiteSpace(Headitem.CustomerNo) && Headitem.CustomerNo.Length > len)
                    Headitem.CustomerNo = Headitem.CustomerNo.Substring(0, len);
                foreach (var Detailitem in Headitem.OrderDetails.ToList())
                {
                    if (Detailitem.Serial < 1)
                    {
                        Headitem.OrderDetails.Remove(Detailitem);
                    }
                }
                Headitem.OrderDate = Headitem.OrderDetails.OrderBy(x => x.DueDate).FirstOrDefault()?.DueDate ?? DateTime.Now;
                Headitem.ReplyDate = Headitem.OrderDetails.OrderBy(x => x.ReplyDate).FirstOrDefault()?.ReplyDate ?? DateTime.Now;
            }
            return Ok(MyFun.APIResponseOK(OrderHeadlist.FirstOrDefault(), sLostProduct));
        }

        /// <summary>
        /// 由Excel自動產生產品
        /// </summary>
        /// <param name="ProductByExcel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostCreatProductByExcelAsync(ProductByExcel ProductByExcel)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;

            var dt = DateTime.Now;
            var nProductBasicslist = new List<ProductBasic>();
            foreach (var Productitem in ProductByExcel.Products.Split("<br/>"))
            {
                var Productitemlist = Productitem.Split(" ; ");
                if (Productitemlist.Length == 3)
                {
                    //再檢查一次
                    if (!_context.ProductBasics.AsQueryable().Where(x => x.ProductNo == Productitemlist[0] && x.DeleteFlag == 0).Any())
                    {
                        var nProductBasics = new ProductBasic
                        {
                            ProductNo = Productitemlist[0].Trim(),
                            ProductNumber = Productitemlist[0].Trim(),
                            Name = Productitemlist[1].Trim(),
                            Specification = Productitemlist[2].Trim(),
                            Property = "",
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext),
                        };

                        // 暫時停用，自動新增Product資料。
                        // var  nProduct = new Product
                        // {
                        //     ProductNo = Productitemlist[0].Trim(),
                        //     ProductNumber = Productitemlist[0].Trim(),
                        //     Name = Productitemlist[1].Trim(),
                        //     Quantity = 0,
                        //     Specification = Productitemlist[2].Trim(),
                        //     Property = "",
                        //     MaterialId = 0,
                        //     MaterialRequire = 1,
                        //     CreateTime = dt,
                        //      CreateUser = MyFun.GetUserID(HttpContext),
                        //     WarehouseId  = 2
                        // };
                        // nProduct.ProductLogs.Add(new ProductLog{

                        // })
                        // nProductBasics.Products.Add(nProduct);
                        nProductBasicslist.Add(nProductBasics);
                    }
                }
            }
            try
            {
                _context.AddRange(nProductBasicslist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(MyFun.APIResponseError(ex.InnerException.Message));

            }

            //取暫存Excel檔案
            var webRootPath = _IWebHostEnvironment.WebRootPath;
            var Dir = $"{webRootPath}";
            var Files = MyFun.ProcessGetTempExcelAsync(Dir, ProductByExcel.OrderNo);
            var OrderHeadlist = new List<OrderHead>();
            foreach (var Fileitem in Files)
            {
                string sLostProduct = "";
                OrderHeadlist.AddRange(DBHelper.GetExcelData(Fileitem, _context, ref sLostProduct));
            }
            foreach (var Headitem in OrderHeadlist.ToList())
            {
                var len = ("000-000000000").Length;
                Headitem.OrderNo = ProductByExcel.OrderNo;
                if (!string.IsNullOrWhiteSpace(Headitem.CustomerNo) && Headitem.CustomerNo.Length > len)
                    Headitem.CustomerNo = Headitem.CustomerNo.Substring(0, len);
                foreach (var Detailitem in Headitem.OrderDetails.ToList())
                {
                    if (Detailitem.Serial < 1)
                    {
                        Headitem.OrderDetails.Remove(Detailitem);
                    }
                    //var oProduct = _context.Products.Find(Detailitem.ProductId); 補金額的功能先停掉，客戶要求金額不同只顯示不修改
                    //if (oProduct != null)
                    //    if (oProduct.Price == 0)
                    //    {
                    //        oProduct.Price = Detailitem.OriginPrice;
                    //        await _context.SaveChangesAsync();
                    //    }
                }
                Headitem.OrderDate = Headitem.OrderDetails.OrderBy(x => x.DueDate).FirstOrDefault()?.DueDate ?? DateTime.Now;
            }
            return Ok(MyFun.APIResponseOK(OrderHeadlist.FirstOrDefault()));

        }
        [HttpPut("{id}")]
        //public async Task<ActionResult<OrderHead>> Test1Async([FromBody] JsonPatchDocument<OrderHead> OrderHead)
        //public ActionResult<OrderHead> Test1([FromBody] OrderHead OrderHead)
        public async Task<ActionResult<OrderHead>> Test1Async(int id, dynamic val)
        {
            var OldOrderHead = _context.OrderHeads.Find(id);
            Stream req = Request.Body;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            using (StreamReader stream = new StreamReader(req))
            {
                string body = await stream.ReadToEndAsync();
                Newtonsoft.Json.JsonConvert.PopulateObject(body, OldOrderHead);
                // body = "param=somevalue&param2=someothervalue"
            }
            return Ok(MyFun.APIResponseOK(OldOrderHead));
        }

        [HttpPatch]
        public ActionResult<OrderHead> Test2(OrderHead OrderHead)
        {
            return Ok(MyFun.APIResponseOK(OrderHead));
        }
        private bool OrderHeadExists(int id)
        {
            return _context.OrderHeads.Any(e => e.Id == id);
        }

        internal List<OrderHead> ListOrderHead(PostOrderMaster_Detail Data)
        {
            //從明細中找出有多少[客戶單號]。
            var list = new List<OrderHead>();
            var len = ("000-000000000").Length;
            foreach (var item in Data.OrderDetail)
            {
                if (!string.IsNullOrWhiteSpace(item.CustomerNo) && item.CustomerNo.Length > len)
                {
                    var tempCustomerNoVal = item.CustomerNo.Substring(0, len);
                    if (!list.Any() || !list.Exists(x => x.CustomerNo == tempCustomerNoVal))
                    {
                        var tempOrderHead = new OrderHead
                        {
                            CustomerNo = tempCustomerNoVal
                        };
                        list.Add(tempOrderHead);
                    }
                }
            }
            return list;
        }
    }
}
