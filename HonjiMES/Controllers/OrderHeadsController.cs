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
            var key = "ON";
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
                var key = "ON";
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
            if (!string.IsNullOrWhiteSpace(qSearchValue.MaterialNo))
            {
                data = data.Where(x => x.OrderDetails.Where(y => y.MaterialBasic.MaterialNo.Contains(qSearchValue.MaterialNo, StringComparison.InvariantCultureIgnoreCase)).Any());
            }
            data = data.Include(x => x.OrderDetails);
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
                            var key = "ON";
                            var OrderNo = PostOrderMaster_Detail.OrderHead.CreateTime.ToString("yyMMdd");
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
                                Customer = PostOrderMaster_Detail.OrderHead.Customer,
                                OrderType = PostOrderMaster_Detail.OrderHead.OrderType
                            };
                            var DirName = orderHead.OrderNo;
                            orderHead.CustomerNo = OrderHeadData.CustomerNo;//替換客戶單號
                            orderHead.OrderNo = key + OrderNo + NoCount.ToString("000");
                            orderHead.CreateTime = dt;
                            orderHead.CreateUser = MyFun.GetUserID(HttpContext);

                            // 訂單複製功能，必須清除關聯資料。
                            var OrderDetails = new List<OrderDetail>();
                            foreach (var item in OrderDetail)
                            {
                                OrderDetails.Add(new OrderDetail
                                {
                                    CustomerNo = item.CustomerNo,
                                    Serial = item.Serial,
                                    MaterialBasicId = item.MaterialBasicId,
                                    Quantity = item.Quantity,
                                    OriginPrice = item.OriginPrice,
                                    Discount = item.Discount,
                                    DiscountPrice = item.DiscountPrice,
                                    Price = item.Price,
                                    Unit = item.Unit,
                                    DueDate = item.DueDate,
                                    Remark = item.Remark,
                                    ReplyDate = item.ReplyDate,
                                    ReplyPrice = item.ReplyPrice,
                                    ReplyRemark = item.ReplyRemark,
                                    MachineNo = item.MachineNo,
                                    Package = item.Package,
                                    Reply = item.Reply,
                                    CreateTime = dt,
                                    CreateUser = MyFun.GetUserID(HttpContext)
                                });
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
                        return Ok(MyFun.APIResponseError("新建失敗! 請確認[客戶單號]是否有誤!"));
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
                // var MappingList = DBHelper.MappingExtelToModelData();
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
            var dt = DateTime.Now;
            foreach (var Headitem in OrderHeadlist.ToList())
            {
                var len = ("000-0000000000").Length; // 客戶單號格式
                Headitem.OrderNo = DirName;
                if (!string.IsNullOrWhiteSpace(Headitem.CustomerNo) && Headitem.CustomerNo.Length > len)
                    Headitem.CustomerNo = Headitem.CustomerNo.Substring(0, len);
                foreach (var Detailitem in Headitem.OrderDetails.ToList())
                {
                    if (Detailitem.Serial < 1)
                    {
                        Headitem.OrderDetails.Remove(Detailitem);
                    }
                    else
                    {

                        if (Detailitem.DueDate <= dt)
                        {
                            Detailitem.DueDate = dt;
                        }
                        if (Detailitem.ReplyDate <= dt)
                        {
                            Detailitem.ReplyDate = dt;
                        }
                    }
                }
                Headitem.OrderDate = Headitem.OrderDetails.OrderBy(x => x.DueDate).FirstOrDefault()?.DueDate ?? dt;
                Headitem.ReplyDate = Headitem.OrderDetails.OrderBy(x => x.ReplyDate).FirstOrDefault()?.ReplyDate ?? dt;
            }
            return Ok(MyFun.APIResponseOK(OrderHeadlist.FirstOrDefault(), sLostProduct));
        }

        /// <summary>
        /// 由Excel自動產生產品
        /// </summary>
        /// <param name="MaterialByExcel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostCreatMaterialByExcel(MaterialByExcel MaterialByExcel)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;

            var dt = DateTime.Now;
            var nMaterialBasicslist = new List<MaterialBasic>();
            foreach (var Materialitem in MaterialByExcel.Materials.Split("<br/>"))
            {
                var Materialitemlist = Materialitem.Split(" ; ");
                if (Materialitemlist.Length == 3)
                {
                    //再檢查一次
                    if (!_context.MaterialBasics.AsQueryable().Where(x => x.MaterialNo == Materialitemlist[0] && x.DeleteFlag == 0).Any())
                    {
                        var nMaterialBasics = new MaterialBasic
                        {
                            MaterialNo = Materialitemlist[0].Trim(),
                            MaterialNumber = Materialitemlist[0].Trim(),
                            MaterialType = 2, // Excel匯入，自動建立[成品]品號
                            Name = Materialitemlist[1].Trim(),
                            Specification = Materialitemlist[2].Trim(),
                            Property = "",
                            CreateTime = dt,
                            CreateUser = MyFun.GetUserID(HttpContext),
                        };

                        // 自動新增Material資料。(2020/08/20 確認自動新增)
                        var Warehouses = await _context.Warehouses.Where(x => x.DeleteFlag == 0 && x.Code == "301").FirstAsync();// 固定新增301成品倉
                        if (Warehouses != null)
                        {
                            var nMaterial = new Material
                            {
                                MaterialNo = Materialitemlist[0].Trim(),
                                MaterialNumber = Materialitemlist[0].Trim(),
                                Name = Materialitemlist[1].Trim(),
                                Quantity = 0,
                                Specification = Materialitemlist[2].Trim(),
                                Property = "",
                                MaterialRequire = 1,
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext),
                                WarehouseId = Warehouses.Id
                            };
                            nMaterial.MaterialLogs.Add(new MaterialLog
                            {
                                LinkOrder = MaterialByExcel.CustomerNo,
                                Reason = "Excel匯入新增",
                                Message = "[新增品號]",
                                CreateTime = dt,
                                CreateUser = MyFun.GetUserID(HttpContext)
                            });
                            nMaterialBasics.Materials.Add(nMaterial);
                        }
                        nMaterialBasicslist.Add(nMaterialBasics);
                    }
                }
            }
            try
            {
                _context.AddRange(nMaterialBasicslist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(MyFun.APIResponseError(ex.InnerException.Message));

            }

            //取暫存Excel檔案
            var webRootPath = _IWebHostEnvironment.WebRootPath;
            var Dir = $"{webRootPath}";
            var Files = MyFun.ProcessGetTempExcelAsync(Dir, MaterialByExcel.OrderNo);
            var OrderHeadlist = new List<OrderHead>();
            foreach (var Fileitem in Files)
            {
                string sLostMaterial = "";
                OrderHeadlist.AddRange(DBHelper.GetExcelData(Fileitem, _context, ref sLostMaterial));
            }
            foreach (var Headitem in OrderHeadlist.ToList())
            {
                var len = ("000-000000000").Length;
                Headitem.OrderNo = MaterialByExcel.OrderNo;
                if (!string.IsNullOrWhiteSpace(Headitem.CustomerNo) && Headitem.CustomerNo.Length > len)
                    Headitem.CustomerNo = Headitem.CustomerNo.Substring(0, len);
                foreach (var Detailitem in Headitem.OrderDetails.ToList())
                {
                    if (Detailitem.Serial < 1)
                    {
                        Headitem.OrderDetails.Remove(Detailitem);
                    }
                    //var oMaterial = _context.Materials.Find(Detailitem.MaterialId); 補金額的功能先停掉，客戶要求金額不同只顯示不修改
                    //if (oMaterial != null)
                    //    if (oMaterial.Price == 0)
                    //    {
                    //        oMaterial.Price = Detailitem.OriginPrice;
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
            var len = ("000-0000000000").Length;
            foreach (var item in Data.OrderDetail)
            {
                if (!string.IsNullOrWhiteSpace(item.CustomerNo) && item.CustomerNo.Length >= len)
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

        /// <summary>
        /// 訂單單號
        /// </summary>
        /// <returns></returns>
        // GET: api/OrderHeads
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<OrderHead>>> CheckData(int id)
        {
            var orderHead = _context.OrderHeads.Find(id);
            if (orderHead.CheckFlag == 0)
            {
                orderHead.CheckFlag = 1;
            }
            else
            {
                orderHead.CheckFlag = 0;
            }
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

        /// <summary>
        /// 查詢訂單全部資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderData(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            // _context.ChangeTracker.LazyLoadingEnabled = true;
            var data = _context.OrderDetails.Where(x => x.DeleteFlag == 0 && x.Order.Status == 0)
                .OrderByDescending(x => x.Order.OrderNo).ThenBy(x => x.Serial).Select(x => new OrderDetailInfo
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    CustomerNo = x.CustomerNo,
                    Serial = x.Serial,
                    MaterialBasicId = x.MaterialBasicId,
                    MaterialId = x.MaterialId,
                    Quantity = x.Quantity,
                    OriginPrice = x.OriginPrice,
                    Discount = x.Discount,
                    DiscountPrice = x.DiscountPrice,
                    Price = x.Price,
                    Delivered = x.Delivered,
                    Unit = x.Unit,
                    DueDate = x.DueDate,
                    Remark = x.Remark,
                    ReplyDate = x.ReplyDate,
                    ReplyRemark = x.ReplyRemark,
                    MachineNo = x.MachineNo,
                    Drawing = x.Drawing,
                    Ink = x.Ink,
                    Label = x.Label,
                    Package = x.Package,
                    Reply = x.Reply,
                    SaleCount = x.SaleCount,
                    SaledCount = x.SaledCount,
                    DeleteFlag = x.DeleteFlag,
                    CreateTime = x.CreateTime,
                    CreateUser = x.CreateUser,
                    UpdateTime = x.UpdateTime,
                    UpdateUser = x.UpdateUser,

                    OrderNo = x.Order.OrderNo,
                    OrderType = x.Order.OrderType,
                    Customer = x.Order.Customer
                });
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            // _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 查詢尚未銷貨的訂單全部資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDataWithNoSale(
                 [FromQuery] DataSourceLoadOptions FromQuery,
                 [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            // _context.ChangeTracker.LazyLoadingEnabled = true;
            var data = _context.OrderDetails.Where(x => x.DeleteFlag == 0 && x.Order.Status == 0 && x.SaleCount == 0)
                .OrderByDescending(x => x.Order.OrderNo).ThenBy(x => x.Serial).Select(x => new OrderDetailInfo
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    CustomerNo = x.CustomerNo,
                    Serial = x.Serial,
                    MaterialBasicId = x.MaterialBasicId,
                    MaterialId = x.MaterialId,
                    Quantity = x.Quantity,
                    OriginPrice = x.OriginPrice,
                    Discount = x.Discount,
                    DiscountPrice = x.DiscountPrice,
                    Price = x.Price,
                    Delivered = x.Delivered,
                    Unit = x.Unit,
                    DueDate = x.DueDate,
                    Remark = x.Remark,
                    ReplyDate = x.ReplyDate,
                    ReplyRemark = x.ReplyRemark,
                    MachineNo = x.MachineNo,
                    Drawing = x.Drawing,
                    Ink = x.Ink,
                    Label = x.Label,
                    Package = x.Package,
                    Reply = x.Reply,
                    SaleCount = x.SaleCount,
                    SaledCount = x.SaledCount,
                    DeleteFlag = x.DeleteFlag,
                    CreateTime = x.CreateTime,
                    CreateUser = x.CreateUser,
                    UpdateTime = x.UpdateTime,
                    UpdateUser = x.UpdateUser,

                    OrderNo = x.Order.OrderNo,
                    OrderType = x.Order.OrderType,
                    Customer = x.Order.Customer
                });
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            // _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }

        /// <summary>
        /// 交易單價紀錄報表_old
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductLog>>> GetDealPriceRecords_old(
            [FromQuery] DataSourceLoadOptions FromQuery,
            [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            var data = _context.ProductLogs.Where(x => x.DeleteFlag == 0 && x.Message == "銷貨");
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            var q = ((List<ProductLog>)FromQueryResult.data).Select(x => x.Id).ToList();
            FromQueryResult.data = data.Where(x => q.Contains(x.Id)).LeftOuterJoin(_context.SaleHeads.Include(y => y.SaleDetailNews).ThenInclude(z => z.Order), x => x.LinkOrder, y => y.SaleNo, (ProductLogs, SaleHeads) => new
            {
                ProductLogs.Id,
                ProductLogs,
                SaleHeads,
                SaleHeads.SaleDetailNews,
            }).ToList();
            _context.ChangeTracker.LazyLoadingEnabled = false;
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }


        /// <summary>
        /// 交易單價紀錄
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult<SaleDetailNew>> GetDealPriceRecord(int? id,
            [FromQuery] DataSourceLoadOptions FromQuery,
            [FromQuery(Name = "detailfilter")] string detailfilter)
        {
            var data = _context.SaleDetailNews.AsQueryable().Where(x => x.OrderDetailId == id).Include(x => x.Sale);
            var FromQueryResult = await MyFun.ExFromQueryResultAsync(data, FromQuery);
            return Ok(MyFun.APIResponseOK(FromQueryResult));
        }
    }
}
