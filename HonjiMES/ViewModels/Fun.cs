﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        internal static APIResponse APIResponseError( string message,Object data=null)
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
            var Olddata_Type = Olddata.GetType();
            var Newdata_Type = Olddata.GetType();
            foreach (var New_Props in Newdata_Type.GetProperties())
            {
                var New_Props_Name = New_Props.Name;
                var New_Props_Value = New_Props.GetValue(Newdata);
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
            foreach (var item in Mappinglist.OrderBy(x=>x.ExcelOrder).ToList())
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

                var Product = _context.Products.Find(Detailitem.ProductId);
                if (Product != null)
                {
                    ws.GetRow(i).CreateCell(2).SetCellValue(Product.ProductNo);//品號
                    ws.GetRow(i).CreateCell(3).SetCellValue(Product.Name);//品名
                    ws.GetRow(i).CreateCell(4).SetCellValue(Product.Specification);//規格
                }
                ws.GetRow(i).CreateCell(5).SetCellValue(Detailitem.Quantity);//數量
                ws.GetRow(i).CreateCell(6).SetCellValue(Detailitem.Delivered ?? 0);//已交
                ws.GetRow(i).CreateCell(7).SetCellValue(Detailitem.Unit);//單位
                ws.GetRow(i).CreateCell(8).SetCellValue(Detailitem.OriginPrice);//原單價
                ws.GetRow(i).CreateCell(9).SetCellValue(Detailitem.Discount ?? 0);//折扣率
                ws.GetRow(i).CreateCell(10).SetCellValue(Detailitem.DiscountPrice ?? 0);//折後單價
                ws.GetRow(i).CreateCell(11).SetCellValue(Detailitem.Price);//金額
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
        /// Excel檔案暫存檔處理
        /// </summary>
        /// <param name="model">存檔類型</param>
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
        /// <summary>
        /// Excel檔案暫存檔處理
        /// </summary>
        /// <param name="dir">存檔路徑</param>
        /// <param name="dirName">資料夾名稱</param>
        /// <param name="OrderNo">訂單號</param>
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
    }
}