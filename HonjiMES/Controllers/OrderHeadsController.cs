using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonjiMES.Models;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Hosting;

namespace HonjiMES.Controllers
{

    /// <summary>
    /// 訂單主檔
    /// </summary>
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
        }

        /// <summary>
        /// 查詢訂單主檔
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHead>>> GetOrderHeads()
        {
            var OrderHeads = await _context.OrderHeads.OrderByDescending(x => x.CreateDate).ToListAsync();
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
            return Ok(MyFun.APIResponseOK(OrderHeads));
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

            // _context.Entry(orderHead).State = EntityState.Modified;

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

            _context.OrderHeads.Remove(orderHead);
            await _context.SaveChangesAsync();
            return Ok(MyFun.APIResponseOK(orderHead));
        }
        [HttpPost]
        public async Task<ActionResult<OrderHead>> PostOrderMaster_Detail(PostOrderMaster_Detail PostOrderMaster_Detail)
        {
            try
            {
               var dt = DateTime.Now;
                var OrderNo = dt.ToString("yyyyMMdd");
                var NoCount = _context.OrderHeads.Where(x => x.OrderNo.StartsWith(OrderNo)).Count() + 1;
                var orderHead = PostOrderMaster_Detail.OrderHead;
                var OrderDetail = PostOrderMaster_Detail.OrderDetail;
                var DirName = orderHead.OrderNo;
                orderHead.OrderNo = OrderNo + NoCount.ToString("0000");
                orderHead.CreateDate = dt;
                orderHead.CreateUser = 1;
                var OrderDetails = new List<OrderDetail>();
                foreach (var item in OrderDetail)
                {
                    item.CreateDate = dt;
                    item.CreateUser = 1;
                    OrderDetails.Add(item);
                }
                orderHead.OrderDetails = OrderDetails.OrderBy(x=>x.Serial).ToList();
                _context.OrderHeads.Add(orderHead);
                await _context.SaveChangesAsync();
                MemoryStream excelDatas = MyFun.DataToExcel(_context,orderHead);
                //Excel存檔
                var webRootPath = _IWebHostEnvironment.WebRootPath;
                var Dir = $"{webRootPath}";
                var bsave = MyFun.ProcessSaveExcelAsync(Dir, DirName, orderHead.OrderNo, excelDatas.ToArray());
                return Ok(MyFun.APIResponseOK(orderHead.OrderNo));
                //return Ok(new { success = true, timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "", data = true});
                //return CreatedAtAction("GetOrderHead", new { id = PostOrderMaster_Detail.OrderHead.Id }, PostOrderMaster_Detail.OrderHead);
            }
            catch (Exception ex)
            {

                return Ok(MyFun.APIResponseError(null,ex.Message));
            }
        }
        /// <summary>
        /// 匯入訂單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<OrderHead>> PostOrdeByExcel()
        {
            var DirName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var OrderHeadlist = new List<OrderHead>();//所有檔案
            var myFile = Request.Form.Files;
            if (myFile.Any())
            {
                var MappingList = DBHelper.MappingExtelToModelData();
                foreach (var item in myFile)
                {
                    //
                    var ms = new MemoryStream();
                    IWorkbook workBook;
                    IFormulaEvaluator formulaEvaluator;
                    item.CopyTo(ms);
                    ms.Position = 0; // <-- Add this, to make it work
                    var bytes = ms.ToArray();
                    try
                    {
                        if (Path.GetExtension(item.FileName).ToLower() == ".xls")
                        {
                            try
                            {
                                workBook = new HSSFWorkbook(ms);//xls格式
                            }
                            catch
                            {
                                //讀取錯誤，使用HTML方式讀取
                                try
                                {
                                    var htmlms = new MemoryStream(ms.ToArray());
                                    StreamReader sr = new StreamReader(htmlms);
                                    string MyHtml = sr.ReadToEnd();
                                    workBook = MyFun.ExportHtmlTableToObj(MyHtml);
                                }
                                catch (Exception ex)
                                {
                                    return Ok(MyFun.APIResponseError(null, ex.Message + " 請檢查檔案格式"));
                                }
                            }
                            formulaEvaluator = new HSSFFormulaEvaluator(workBook); // Important!! 取公式值的時候會用到
                        }
                        else
                        {
                            workBook = new XSSFWorkbook(ms);//xlsx格式
                            formulaEvaluator = new XSSFFormulaEvaluator(workBook); // Important!! 取公式值的時候會用到
                        }
                        foreach (ISheet sheet in workBook)
                        {
                            var MappingExtelToModel = new List<ExcelOrderModel>();//所有檔案
                            var nOrderHead = new OrderHead();
                            #region 表頭資料處理
                            //處理客戶代號
                            var CustomerName = _context.Customers.Where(x => item.FileName.Contains(x.Name)).FirstOrDefault();
                            if (CustomerName != null)
                            {
                                nOrderHead.Customer = CustomerName.Id;
                            }
                            #endregion

                            OrderHeadlist.Add(nOrderHead);
                            for (var i = 0; i < sheet.LastRowNum; i++)//筆數
                            {
                                var nOrderDetail = new OrderDetail();
                                nOrderHead.OrderDetails.Add(nOrderDetail);
                                var CellNum = sheet.GetRow(i).LastCellNum;
                                for (var j = 0; j < CellNum; j++)
                                {
                                    //抓出表頭及順序
                                    if (!MappingExtelToModel.Any() || CellNum > MappingExtelToModel.Count())
                                    {
                                        var val = sheet.GetRow(i).GetCell(j);
                                        if (val != null)
                                        {
                                            var ExcelName = DBHelper.MappingExtelToModelData().Where(x => x.ExcelName == val.ToString()).FirstOrDefault();
                                            if (ExcelName != null)
                                            {
                                                MappingExtelToModel.Add(ExcelName);
                                            }
                                            else
                                            {
                                                MappingExtelToModel.Add(new ExcelOrderModel { });//使數量一樣
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var Mappingitem = MappingExtelToModel[j];
                                        var Cellval = DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(j));//ExcelCell內容

                                        if (Mappingitem.TableName == "OrderHead")
                                        {
                                            DBHelper.MappingExtelToModel<OrderHead>(ref nOrderHead, Cellval, Mappingitem.ModelName.ToLower());
                                            //foreach (var Props in nOrderHead.GetType().GetProperties())
                                            //{
                                            //    if (Props.Name.ToLower() == Mappingitem.ModelName.ToLower())
                                            //    {
                                            //        Props.SetValue(nOrderHead, Cellval);
                                            //    }
                                            //}
                                        }
                                        else if (Mappingitem.TableName == "OrderDetail")
                                        {
                                            switch (Mappingitem.Change)
                                            {
                                                case "Product":
                                                    Cellval = _context.Products.Where(x => x.ProductNo == Cellval).FirstOrDefault()?.Id.ToString() ?? null;
                                                    break;
                                                default:
                                                    break;
                                            }
                                            DBHelper.MappingExtelToModel<OrderDetail>(ref nOrderDetail, Cellval, Mappingitem.ModelName.ToLower());
                                            //foreach (var Props in nOrderDetail.GetType().GetProperties())
                                            //{
                                            //    if (Props.Name.ToLower() == Mappingitem.ModelName.ToLower())
                                            //    {
                                            //        Props.SetValue(nOrderDetail, Cellval);
                                            //    }
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok(MyFun.APIResponseError(null, ex.Message + " 請檢查資料格式"));
                    }
                    //暫存Excel檔案
                    var webRootPath = _IWebHostEnvironment.WebRootPath;
                    var Dir = $"{webRootPath}";
                    var bsave = MyFun.ProcessSaveTempExcelAsync(Dir, DirName, item);       
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
            }
            return Ok(MyFun.APIResponseOK(OrderHeadlist.FirstOrDefault()));
        }
        private bool OrderHeadExists(int id)
        {
            return _context.OrderHeads.Any(e => e.Id == id);
        }
    }
}
