using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
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
        internal static APIResponse APIResponseError(Object data, string message)
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
    }
}