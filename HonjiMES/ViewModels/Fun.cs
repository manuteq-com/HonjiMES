﻿using DevExtreme.AspNet.Mvc;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class MyFun
    {
        internal static APIResponse APIResponseOK(Object data, string message = "")
        {
            var APIResponse = new APIResponse { data = data, success = true, message = message };
            return APIResponse;
        }
        internal static APIResponse APIResponseError(string message, Object data = null)
        {
            var APIResponse = new APIResponse { data = data, success = false, message = message };
            return APIResponse;
        }


        /// <summary>
        /// 要回寫資料庫的資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Olddata">舊資料</param>
        /// <param name="Newdata">新資料</param>
        /// <returns>回傳訊息</returns>
        internal static string MappingData<T>(ref T Olddata, T Newdata)
        {
            var CanGetYype = new List<Type> {
                typeof(string),
                typeof(int),
                typeof(int?),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(decimal),
                typeof(decimal?),
                typeof(float),
                typeof(float?),
                typeof(bool),
                typeof(bool?),
                typeof(sbyte),
                typeof(sbyte?),
            };

            var Olddata_Type = Olddata.GetType();
            var Newdata_Type = Olddata.GetType();
            foreach (var New_Props in Newdata_Type.GetProperties())
            {
                if (!CanGetYype.Contains(New_Props.PropertyType))
                {
                    continue;
                }
                var New_Props_Value = New_Props.GetValue(Newdata);
                if (New_Props_Value == null)
                {
                    continue;
                }
                var New_Props_Name = New_Props.Name;
                foreach (var Old_Props in Olddata_Type.GetProperties())
                {
                    var Old_Props_Name = Old_Props.Name;
                    var Old_Props_Value = Old_Props.GetValue(Olddata);

                    if (New_Props_Name == Old_Props_Name)
                    {
                        if (New_Props_Value != Old_Props_Value)
                        {
                            if (New_Props_Value != null)
                            {
                                if (New_Props.PropertyType == typeof(DateTime))
                                {
                                    if (((DateTime)New_Props_Value).Year != 1)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }
                                else if (New_Props.PropertyType == typeof(int))
                                {
                                    if (((int)New_Props_Value) != 0)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }
                                else if (New_Props.PropertyType == typeof(decimal))
                                {
                                    if (((decimal)New_Props_Value) != 0)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }
                                else if (New_Props.PropertyType == typeof(float))
                                {
                                    if (((float)New_Props_Value) != 0)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }
                                else
                                {
                                    if (true)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// 從 Token 取出使用者ID 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        internal static int GetUserID(HttpContext httpContext)
        {
            var UserID = 1;
            int.TryParse(httpContext.User.Claims.Where(x => x.Type == "UserID").FirstOrDefault()?.Value, out UserID);
            return UserID;
        }

        internal static T JsonToData<T>(string jsonstring) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonstring);
            }
            catch (Exception)
            {

                return new T();
            }
        }
        /// <summary>
        /// 前端資料轉資料庫模式
        /// </summary>
        /// <param name="menuList">前端</param>
        /// <returns></returns>
        internal static List<UserRole> ReturnRole(List<MenuListViewModel> menuList)
        {
            var UserRoleList = new List<UserRole>();
            foreach (var Menuitem in menuList)
            {
                var Roles = "";
                Roles += Menuitem.Creat.HasValue ? Menuitem.Creat.Value ? "1" : "0" : "0";
                Roles += Menuitem.Edit.HasValue ? Menuitem.Edit.Value ? "1" : "0" : "0";
                Roles += Menuitem.Delete.HasValue ? Menuitem.Delete.Value ? "1" : "0" : "0";
                UserRoleList.Add(new UserRole
                {
                    MenuId = Menuitem.Id,
                    Roles = Roles
                });
            }
            return UserRoleList;
        }

        /// <summary>
        /// 密碼加密
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal static string Encryption(string Password)
        {
            var key = "60246598";
            var message = Password;
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacSHA256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacSHA256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }

        internal static HSSFWorkbook ExportHtmlTableToObj(string htmlTable)
        {
            try
            {
                #region 第一步：將HtmlTable轉換為DataTable
                htmlTable = htmlTable.Replace("\"", "'");
                var trReg = new Regex(pattern: @"(?<=(<[t|T][r|R]))[\s\S]*?(?=(</[t|T][r|R]>))");
                var trMatchCollection = trReg.Matches(htmlTable);
                DataTable dt = new DataTable("data");
                for (int i = 0; i < trMatchCollection.Count; i++)
                {
                    var row = "<tr " + trMatchCollection[i].ToString().Trim() + "</tr>";
                    var tdReg = new Regex(pattern: @"(?<=(<[t|T][d|D|h|H]))[\s\S]*?(?=(</[t|T][d|D|h|H]>))");
                    var tdMatchCollection = tdReg.Matches(row);
                    if (i == 0)
                    {
                        foreach (var rd in tdMatchCollection)
                        {
                            var tdValue = RemoveHtml("<td " + rd.ToString().Trim() + "</td>");
                            DataColumn dc = new DataColumn(tdValue);
                            dt.Columns.Add(dc);
                        }
                    }
                    if (i > 0)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < tdMatchCollection.Count; j++)
                        {
                            var tdValue = RemoveHtml("<td " + tdMatchCollection[j].ToString().Trim() + "</td>");
                            dr[j] = tdValue;
                        }
                        dt.Rows.Add(dr);
                    }
                }
                #endregion
                return ExportDataSetToExcel(dt);
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        internal static HSSFWorkbook ExportDataSetToExcel(DataTable dt)
        {
            #region 表頭
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            var hssfSheet = hssfworkbook.CreateSheet();
            // 表頭
            var tagRow = hssfSheet.CreateRow(0);
            int colIndex;
            for (colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
            {
                tagRow.CreateCell(colIndex).SetCellValue(dt.Columns[colIndex].ColumnName);
            }
            #endregion
            #region 資料表
            // 資料表  
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                DataRow dr = dt.Rows[k];
                var row = hssfSheet.CreateRow(k + 1);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(dr[i].ToString());
                }
            }
            #endregion
            return (hssfworkbook);
        }
        internal static string RemoveHtml(string htmlstring)
        {
            //删除脚本    
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML    
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            return htmlstring;
        }

        /// <summary>
        /// 資料轉成Excel
        /// </summary>
        /// <param name="_context">讀資料用</param>
        /// <param name="orderHead">訂單資料</param>
        /// <returns></returns>
        internal static MemoryStream DataToExcel(HonjiContext _context, OrderHead orderHead)
        {
            var Mappinglist = DBHelper.MappingExtelToModelData();

            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet(orderHead.OrderNo);
            ws.CreateRow(0);//第一行為欄位名稱
            var i = 0;
            var j = 0;
            foreach (var item in Mappinglist.OrderBy(x => x.ExcelOrder).ToList())
            {
                ws.GetRow(0).CreateCell(j).SetCellValue(item.ExcelName);//建立表頭
                j++;
            }
            foreach (var Detailitem in orderHead.OrderDetails)
            {
                i++;
                ws.CreateRow(i);
                ws.GetRow(i).CreateCell(0).SetCellValue(orderHead.CustomerNo);//訂單單號
                ws.GetRow(i).CreateCell(1).SetCellValue(Detailitem.Serial);//序號

                var Product = _context.Materials.Find(Detailitem.MaterialId);
                if (Product != null)
                {
                    ws.GetRow(i).CreateCell(2).SetCellValue(Product.MaterialNo);//品號
                    ws.GetRow(i).CreateCell(3).SetCellValue(Product.Name);//品名
                    ws.GetRow(i).CreateCell(4).SetCellValue(Product.Specification);//規格
                }
                ws.GetRow(i).CreateCell(5).SetCellValue(Detailitem.Quantity);//數量
                ws.GetRow(i).CreateCell(6).SetCellValue(Detailitem.Delivered ?? 0);//已交
                ws.GetRow(i).CreateCell(7).SetCellValue(Detailitem.Unit);//單位
                ws.GetRow(i).CreateCell(8).SetCellValue((double)Detailitem.OriginPrice);//原單價
                ws.GetRow(i).CreateCell(9).SetCellValue((double)(Detailitem.Discount ?? 0));//折扣率
                ws.GetRow(i).CreateCell(10).SetCellValue((double)(Detailitem.DiscountPrice ?? 0));//折後單價
                ws.GetRow(i).CreateCell(11).SetCellValue((double)Detailitem.Price);//金額
                ws.GetRow(i).CreateCell(12).SetCellValue(Detailitem.DueDate);//預交日
                ws.GetRow(i).CreateCell(13).SetCellValue(Detailitem.Remark);//備註
                ws.GetRow(i).CreateCell(14).SetCellValue(Detailitem.Reply ?? 0);//回覆量
                ws.GetRow(i).CreateCell(15).SetCellValue(Detailitem.ReplyDate);//回覆交期
                ws.GetRow(i).CreateCell(16).SetCellValue(Detailitem.ReplyRemark);//回覆備註
                ws.GetRow(i).CreateCell(17).SetCellValue(Detailitem.MachineNo);//機號
                ws.GetRow(i).CreateCell(18).SetCellValue(Detailitem.Drawing);//圖檔
                ws.GetRow(i).CreateCell(19).SetCellValue(Detailitem.Ink);//噴墨
                ws.GetRow(i).CreateCell(20).SetCellValue(Detailitem.Label);//標籤
                ws.GetRow(i).CreateCell(21).SetCellValue(Detailitem.Package ?? 0);//包裝數
                ws.GetRow(i).CreateCell(22).SetCellValue("v");//
            }
            var excelDatas = new MemoryStream();
            wb.Write(excelDatas);
            return excelDatas;
        }
        /// <summary>
        /// 依路徑取得檔案
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        internal static string[] ProcessGetTempExcelAsync(string dir, string orderNo)
        {
            var sPath = $"{dir}\\TempFile\\" + orderNo;
            return Directory.GetFiles(sPath);
        }

        /// <summary>
        /// Excel檔案暫存檔處理
        /// </summary>
        /// <param name="dir">存檔路徑</param>
        /// <param name="dirName">資料夾名稱</param>
        /// <param name="item">檔案資料串流</param>
        /// <returns></returns>
        internal static async Task<string> ProcessSaveTempExcelAsync(string dir, string dirName, IFormFile item)
        {
            try
            {
                var sPath = $"{dir}\\TempFile\\{dirName}\\";
                // 要存放的位置
                if (!Directory.Exists(sPath))
                {
                    //新增資料夾
                    Directory.CreateDirectory(sPath);
                }
                var savePath = $"{sPath}{ Path.GetFileName(item.FileName)}";
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                return "";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        internal static List<QueryList> GetFilterToList(IList filter)
        {
            var QueryList = new List<QueryList>();
            var filterarray = JArray.Parse(JsonConvert.SerializeObject(filter));
            if (filterarray.Any())
            {
                if (!filterarray.FirstOrDefault().Any())
                {
                    if (filterarray.Count() == 3)
                    {
                        QueryList.Add(new QueryList { key = filterarray[0].ToString(), where = filterarray[1].ToString(), val = filterarray[2].ToString() });
                    }
                }
                else
                {
                    foreach (var item in filterarray)
                    {
                        if (item.Children().Any())
                        {
                            IList Iitem = (IList)item;
                            QueryList.AddRange(GetFilterToList(Iitem));
                        }
                    }
                }
            }
            return QueryList;
        }
        /// <summary>
        /// 用遞迴的方式取所有的BOM層
        /// </summary>
        /// <param name="billOfMaterials"></param>
        /// <param name="Lv"></param>
        /// <param name="Receive">生產數量</param>
        /// <returns></returns>
        internal static List<BomList> GetBomList(IEnumerable<BillOfMaterial> billOfMaterials, int Lv = 0, decimal Receive = 1)
        {
            var bomlist = new List<BomList>();
            if (Lv < 3)//目前限定3階
            {
                foreach (var item in billOfMaterials)
                {
                    bomlist.Add(new BomList
                    {
                        Lv = Lv + 1,
                        Id = item.Id,
                        Pid = Lv == 0 ? 0 : item.Pid ?? 0,
                        Name = item.Name,
                        MaterialName = item.MaterialBasic?.Name,
                        MaterialNo = item.MaterialBasic?.MaterialNo,
                        MaterialSpecification = item.MaterialBasic?.Specification,
                        MaterialPrice = item.MaterialBasic?.Price ?? 0,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Outsource = item.Outsource,
                        Group = item.Group,
                        Type = item.Type,
                        Remarks = item.Remarks,
                        ProductName = item.InverseP.FirstOrDefault()?.ProductBasic?.Name ?? "",
                        ProductNo = item.InverseP.FirstOrDefault()?.ProductBasic?.MaterialNo ?? "",
                        ProductNumber = item.InverseP.FirstOrDefault()?.ProductBasic?.MaterialNumber ?? "",
                        ProductSpecification = item.InverseP.FirstOrDefault()?.ProductBasic?.Specification ?? "",
                        ProductPrice = item.InverseP.FirstOrDefault()?.ProductBasic?.Price ?? 0,
                        MaterialBasicId = item.MaterialBasicId,
                        // ProductBasicId = item.ProductBasicId,
                        ProductBasicId = item.InverseP.FirstOrDefault()?.ProductBasic?.Id ?? null,//改放ProductBasic本身的ID
                        ReceiveQty = item.Quantity * Receive,
                        Ismaterial = !item.InverseP.Any(),
                        Master = item.Master
                    });
                    if (item.InverseP.Any())
                    {
                        bomlist.AddRange(MyFun.GetBomList(item.InverseP, Lv + 1, item.Quantity * Receive));
                    }
                }
            }
            return bomlist;
        }
        /// <summary>
        /// 用遞迴的方式刪除所有的BOM層
        /// </summary>
        /// <param name="billOfMaterials"></param>
        /// <returns></returns>
        internal static List<BillOfMaterial> DeleteBomList(IEnumerable<BillOfMaterial> billOfMaterials)
        {
            var BillOfMaterial = new List<BillOfMaterial>();
            foreach (var item in billOfMaterials)
            {
                if (item.InverseP.Any())
                {
                    BillOfMaterial.AddRange(MyFun.DeleteBomList(item.InverseP));
                }
                BillOfMaterial.Add(item);
            }
            return BillOfMaterial;
        }
        /// <summary>
        /// 確認單號格式
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="Len"></param>
        /// <returns></returns>
        internal static Boolean CheckNoFormat(string Number, string Type, string Date, int Len)
        {
            var result = false;
            var checkLength = (Type + Date).Length + Len;

            if (Type == "AJ")
            {
                if (Number.Contains(Type + Date) && Number.Length == checkLength)
                {
                    if (int.TryParse(Number.Substring(Number.Length - Len, Len), out int n))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
        internal static async Task<FromQueryResult> ExFromQueryResultAsync<T>(IQueryable<T> db, DataSourceLoadOptions fromQuery) where T : class
        {
            QueryCollection queries = new QueryCollection();
            // object DeleteFlag = 0;

            //dbQuery = dbQuery.Where(x => x.GetType().GetProperty("DeleteFlag").GetValue(x) == DeleteFlag);
            // var cDeleteFlag = typeof(T).GetProperty("DeleteFlag");
            // if (cDeleteFlag != null)
            // {
            //     queries.Add(new Query { Name = "DeleteFlag", Operator = Query.Operators.Equal, Value = 0 });
            // }
            var FromQueryResult = new FromQueryResult();
            if (fromQuery.Filter != null)
            {
                var FilterToList = GetFilterToList(fromQuery.Filter);
                foreach (var item in FilterToList)
                {
                    if (item.where == "contains")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.Contains, Value = item.val });
                    }
                    else if (item.where == "notcontains")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.NotContains, Value = item.val });
                    }
                    else if (item.where == "startswith")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.StartWith, Value = item.val });
                    }
                    else if (item.where == "endswith")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.EndWidth, Value = item.val });
                    }
                    else if (item.where == "=")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.Equal, Value = item.val });
                    }
                    else if (item.where == "<>")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.NotEqual, Value = item.val });
                    }
                    else if (item.where == ">")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.GreaterThan, Value = item.val });
                    }
                    else if (item.where == "<")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.LessThan, Value = item.val });
                    }
                    else if (item.where == ">=")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.GreaterThanOrEqual, Value = item.val });
                    }
                    else if (item.where == "<=")
                    {
                        queries.Add(new Query { Name = item.key, Operator = Query.Operators.LessThanOrEqual, Value = item.val });
                    }
                }
            }
            var dbQuery = queries.Count() > 0 ? db.Where(queries.AsExpression<T>()) : db;
            if (fromQuery.Sort != null)
            {
                var firstSort = true;
                foreach (var Sortitem in fromQuery.Sort)
                {
                    if (firstSort)
                    {
                        if (Sortitem.Desc)
                        {
                            dbQuery = dbQuery.OrderBy(Sortitem.Selector, true);
                        }
                        else
                        {
                            dbQuery = dbQuery.OrderBy(Sortitem.Selector);
                        }
                    }
                    else
                    {
                        if (Sortitem.Desc)
                        {
                            dbQuery = dbQuery.ThenBy(Sortitem.Selector, true);
                        }
                        else
                        {
                            dbQuery = dbQuery.ThenBy(Sortitem.Selector);
                        }
                    }
                    //else
                    //{
                    //    if (Sortitem.Desc)
                    //    {
                    //        dbQuery = dbQuery.OrderByDescending(x => x.GetType().GetProperty(Sortitem.Selector).GetValue(x));
                    //    }
                    //    else
                    //    {
                    //        dbQuery = dbQuery.ThenBy(x => x.GetType().GetProperty(item.Selector).GetValue(x));
                    //    }
                    //}
                    firstSort = false;
                }
            }
            FromQueryResult.totalCount = await dbQuery.CountAsync();
            if (fromQuery.Skip != 0)
            {
                dbQuery = dbQuery.Skip(fromQuery.Skip);
            }
            if (fromQuery.Take != 0)
            {
                dbQuery = dbQuery.Take(fromQuery.Take);
            }
            var sql = dbQuery.ToSql();//看SQL語法專用
            FromQueryResult.data = await dbQuery.ToListAsync();
            return FromQueryResult;
        }
        internal static FromQueryResult FromQueryResultAsync<T>(DbSet<T> db, DataSourceLoadOptions fromQuery) where T : class
        {
            var dbQuery = db.AsQueryable();
            dbQuery = dbQuery.Where(x => x.GetType().GetProperty("DeleteFlag").GetValue(x).ToString() == "0");
            var FromQueryResult = new FromQueryResult();
            if (fromQuery.Filter != null)
            {
                var FilterToList = GetFilterToList(fromQuery.Filter);
                foreach (var item in FilterToList)
                {
                    if (item.where == "contains")
                    {
                        dbQuery = dbQuery.Where(x => x.GetType().GetProperty(item.key).GetValue(x).ToString().Contains(item.val, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (item.where == "notcontains")
                    {
                        dbQuery = dbQuery.Where(x => !x.GetType().GetProperty(item.key).GetValue(x).ToString().Contains(item.val, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (item.where == "startswith")
                    {
                        dbQuery = dbQuery.Where(x => x.GetType().GetProperty(item.key).GetValue(x).ToString().StartsWith(item.val, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (item.where == "endswith")
                    {
                        dbQuery = dbQuery.Where(x => x.GetType().GetProperty(item.key).GetValue(x).ToString().EndsWith(item.val, StringComparison.OrdinalIgnoreCase));
                    }
                    else if (item.where == "=")
                    {
                        dbQuery = dbQuery.Where(x => x.GetType().GetProperty(item.key).GetValue(x).ToString().ToLower() == item.val.ToLower());
                    }
                    else if (item.where == "<>")
                    {
                        dbQuery = dbQuery.Where(x => x.GetType().GetProperty(item.key).GetValue(x).ToString().ToLower() != item.val.ToLower());
                    }
                }
            }
            if (fromQuery.Sort != null)
            {
                var firstSort = true;
                foreach (var Sortitem in fromQuery.Sort)
                {
                    if (firstSort)
                    {
                        if (Sortitem.Desc)
                        {
                            dbQuery = dbQuery.OrderByDescending(x => x.GetType().GetProperty(Sortitem.Selector).GetValue(x));
                        }
                        else
                        {
                            dbQuery = dbQuery.OrderBy(x => x.GetType().GetProperty(Sortitem.Selector).GetValue(x));
                        }
                    }
                    // else
                    // {
                    //     if (Sortitem.Desc)
                    //     {
                    //         dbQuery = dbQuery.OrderByDescending(x => x.GetType().GetProperty(Sortitem.Selector).GetValue(x));
                    //     }
                    //     else
                    //     {
                    //         dbQuery = dbQuery.OrderBy(x => x.GetType().GetProperty(item.Selector).GetValue(x));
                    //     }
                    // }
                    firstSort = false;
                }
            }
            FromQueryResult.totalCount = dbQuery.Count();
            if (fromQuery.Skip != 0)
            {
                dbQuery = dbQuery.Skip(fromQuery.Skip);
            }
            if (fromQuery.Take != 0)
            {
                dbQuery = dbQuery.Take(fromQuery.Take);
            }
            FromQueryResult.data = dbQuery.ToList();
            return FromQueryResult;
        }

        /// <summary>
        /// 把字串轉為查詢陣列
        /// </summary>
        /// <param name="filter">差詢字串</param>
        /// <returns></returns>
        internal static List<QueryList> GetFilterToList(string filter)
        {

            var QueryList = new List<QueryList>();
            var filterarray = JArray.Parse(filter);
            if (filterarray.Any())
            {
                if (!filterarray.FirstOrDefault().Any())
                {
                    QueryList.Add(new QueryList { key = filterarray[0].ToString(), where = filterarray[1].ToString(), val = filterarray[2].ToString() });
                }
                else
                {

                    foreach (var item in filterarray)
                    {
                        if (item.Count() == 3)
                        {
                            QueryList.Add(new QueryList { key = item[0].ToString(), where = item[1].ToString(), val = item[2].ToString() });
                        }
                    }
                }
                // if (filterarray.First().Count() > 0)
                // {
                //     foreach (var filteritem in filterarray)
                //     {
                //         if (filteritem.FirstOrDefault().Count() > 0)
                //         {

                //         }
                //         else if (filteritem.Count() == 3)
                //         {
                //             QueryList.Add(new QueryList { key = filteritem[0].ToString(), where = filteritem[1].ToString(), val = filteritem[2].ToString() });
                //         }
                //     }
                // }
                // else if (filterarray.Count() == 3)
                // {
                //     QueryList.Add(new QueryList { key = filterarray[0].ToString(), where = filterarray[1].ToString(), val = filterarray[2].ToString() });
                // }
            }
            return QueryList;
        }

        /// <summary>
        /// Excel檔案暫存檔處理
        /// </summary>
        /// <param name="dir">存檔路徑</param>
        /// <param name="dirName">資料夾名稱</param>
        /// <param name="OrderNo">訂單號</param>
        /// <param name="excelDatas">Excel內容</param>
        /// <returns></returns>
        internal static async Task<string> ProcessSaveExcelAsync(string dir, string dirName, string OrderNo, byte[] excelDatas)
        {
            try
            {
                var sTempPath = $"{dir}\\TempFile\\{dirName}\\";//暫存路徑
                var sUpdatePath = $"{dir}\\UpdateFile\\{OrderNo}\\";//存檔路徑
                var sSavePath = $"{dir}\\SaveFile\\{OrderNo}\\";//存檔路徑
                if (!Directory.Exists(sUpdatePath))
                {
                    //新增資料夾
                    Directory.CreateDirectory(sUpdatePath);
                }
                if (!Directory.Exists(sSavePath))
                {
                    //新增資料夾
                    Directory.CreateDirectory(sSavePath);
                }
                //Excel存檔
                var savePath = $"{sSavePath}{ OrderNo}.xlsx";
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    Stream ms = new MemoryStream(excelDatas);
                    await ms.CopyToAsync(stream);
                }
                //移動Excel暫存檔
                var fdir = new DirectoryInfo(sTempPath);
                var filelist = fdir.GetFiles();
                foreach (var item in filelist)
                {
                    File.Move(item.FullName, sUpdatePath + Path.GetFileName(item.FullName));
                }
                Directory.Delete(sTempPath);
                return "";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        internal static string ImgToBase64String(byte[] bQrCode)
        {
            return Convert.ToBase64String(bQrCode);
            // return $"data:image/png;base64,{Convert.ToBase64String(bQrCode)}";
        }
    }
}