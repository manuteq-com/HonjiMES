
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;


namespace HonjiMES.Models
{
    public class DBHelper
    {
        /// <summary>
        /// Excel對應資料表的欄位
        /// </summary>
        /// <returns></returns>
        internal static List<ExcelOrderModel> MappingExtelToModelData()
        {
            //var item = default(List<T>);
            #region Excel比對檔
            var ExcelOrderModellist = new List<ExcelOrderModel>();
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "CustomerNo", ExcelName = "訂單單號", TableName = "OrderHead", ExcelOrder = 1 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Serial", ExcelName = "序號", TableName = "OrderDetail", ExcelOrder = 2 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ProductId", ExcelName = "品號", TableName = "OrderDetail", Change = "Product", ExcelOrder = 3 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "品名", TableName = "OrderDetail", ExcelOrder = 4 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "規格", TableName = "OrderDetail", ExcelOrder = 5 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Quantity", ExcelName = "數量", TableName = "OrderDetail", ExcelOrder = 6 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Delivered", ExcelName = "已交", TableName = "OrderDetail", ExcelOrder = 7 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Unit", ExcelName = "單位", TableName = "OrderDetail", ExcelOrder = 8 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "OriginPrice", ExcelName = "原單價", TableName = "OrderDetail", ExcelOrder = 9 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Discount", ExcelName = "折扣率", TableName = "OrderDetail", ExcelOrder = 10 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "DiscountPrice", ExcelName = "折後單價", TableName = "OrderDetail", ExcelOrder = 11 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Price", ExcelName = "金額", TableName = "OrderDetail", ExcelOrder = 12 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "DueDate", ExcelName = "預交日", TableName = "OrderDetail", ExcelOrder = 13 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Remark", ExcelName = "備註", TableName = "OrderDetail", ExcelOrder = 14 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Reply", ExcelName = "回覆量", TableName = "OrderDetail", ExcelOrder = 15 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ReplyDate", ExcelName = "回覆交期", TableName = "OrderDetail", ExcelOrder = 16 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ReplyRemark", ExcelName = "回覆備註", TableName = "OrderDetail", ExcelOrder = 17 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "MachineNo", ExcelName = "機號", TableName = "OrderDetail", ExcelOrder = 18 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Drawing", ExcelName = "圖檔", TableName = "OrderDetail", ExcelOrder = 19 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Ink", ExcelName = "噴墨", TableName = "OrderDetail", ExcelOrder = 20 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Label", ExcelName = "標籤", TableName = "OrderDetail", ExcelOrder = 21 });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Package", ExcelName = "包裝數", TableName = "OrderDetail", ExcelOrder = 22 });
            #endregion

            //foreach (var excel in Allexcel)
            //{
            //    foreach (var excelworkBook in excel as List<object>)
            //    {
            //        foreach (var excelRow in excelworkBook as List<object>)
            //        {
            //            foreach (var excelCell in excelRow as List<object>)
            //            {

            //            }
            //        }
            //    }
            //}

            return ExcelOrderModellist;
        }
        internal static string MappingExtelToModel<T>(ref T Data, string Cellval, string ModelName)
        {
            var Bbreak = false;
            if (string.IsNullOrWhiteSpace(ModelName))
            {
                return "";
            }
            foreach (var Props in Data.GetType().GetProperties())
            {
                if (Props.Name.ToLower() == ModelName)
                {
                    if (Props.PropertyType == typeof(DateTime) || Props.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(Cellval, out DateTime val))
                            Props.SetValue(Data, val);
                        Bbreak = true;
                    }
                    else if (Props.PropertyType == typeof(int) || Props.PropertyType == typeof(int?))
                    {
                        if (int.TryParse(Cellval, out int val))
                            Props.SetValue(Data, val);
                        Bbreak = true;
                    }
                    else if (Props.PropertyType == typeof(string))
                    {
                        Props.SetValue(Data, Cellval);
                        Bbreak = true;
                    }
                    else if (Props.PropertyType == typeof(bool) || Props.PropertyType == typeof(bool?))
                    {
                        if (bool.TryParse(Cellval, out bool val))
                            Props.SetValue(Data, val);
                        Bbreak = true;
                    }
                    else if (Props.PropertyType == typeof(float) || Props.PropertyType == typeof(float?))
                    {
                        if (float.TryParse(Cellval, out float val))
                            Props.SetValue(Data, val);
                        Bbreak = true;
                    }
                    else if (Props.PropertyType == typeof(char) || Props.PropertyType == typeof(char?))
                    {
                        if (char.TryParse(Cellval, out char val))
                            Props.SetValue(Data, val);
                        Bbreak = true;
                    }

                }
                if (Bbreak)//跳出迴圈
                {
                    break;
                }
            }
            return "";
        }
        //用一個簡單的Reflection去把資料放入
        internal static T DataReaderMapping<T>(IDataReader reader) where T : new()
        {
            var datetype = typeof(T);
            T _person = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                //取得 DB 欄位名稱
                string propName = reader.GetName(i);
                //取得 DB 值
                var dbvalue = reader.GetValue(i);

                foreach (var Props in typeof(T).GetProperties())
                {
                    if (Props.GetCustomAttribute<ColumnAttribute>()?.Name.ToLower() == propName.ToLower())
                    {
                        Props.SetValue(_person, dbvalue);
                    }
                }
            }
            return _person;
        }
        //寫一個簡單的方式來過濾DataReader的資料
        private static object ReturnValue(object data)
        {
            string typename = data.GetType().Name.ToLower();
            if (typename.Contains("string"))
            {
                return data.ToString();
            }
            if (typename.Contains("int"))
            {
                return int.Parse(data.ToString());
            }
            if (typename.Contains("datetime"))
            {
                DateTime dateTimeValue = new DateTime();
                DateTime.TryParse(data.ToString(), out dateTimeValue);
                return dateTimeValue;
            }
            if (typename.Contains("decimal"))
            {
                return decimal.Parse(data.ToString());
            }
            return null;
        }
        /// <summary>
        /// 取EXCEL資料
        /// </summary>
        /// <param name="Fileitem"></param>
        /// <param name="_context"></param>
        /// <param name="sLostProduct"></param>
        /// <returns></returns>
        internal static List<OrderHead> GetExcelData(string Fileitem, HonjiContext _context, ref string sLostProduct)
        {
            var OrderHeadlist = new List<OrderHead>();//所有檔案
            using (FileStream item = File.OpenRead(Fileitem))
            {
                IWorkbook workBook;
                IFormulaEvaluator formulaEvaluator;
                var ms = new MemoryStream();
                item.CopyTo(ms);
                ms.Position = 0; // <-- Add this, to make it work
                var bytes = ms.ToArray();
                try
                {
                    if (Path.GetExtension(Fileitem).ToLower() == ".xls")
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
                                throw new Exception(ex.Message + " 請檢查檔案格式");
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
                        var CustomerName = _context.Customers.Where(x => Path.GetExtension(Fileitem).Contains(x.Name)).FirstOrDefault();
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
                                                if (string.IsNullOrWhiteSpace(Cellval))
                                                {
                                                    sLostProduct += DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(j)) + " ; "
                                                        + DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(j + 1)) + " ; "
                                                        + DBHelper.GrtCellval(formulaEvaluator, sheet.GetRow(i).GetCell(j + 2)) + "<br/>";
                                                }
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
                    throw new Exception(ex.Message + " 請檢查資料格式");
                }
            }
            return OrderHeadlist;
        }

        internal static string GrtCellval(IFormulaEvaluator formulaEvaluator, ICell cell)
        {
            string cellval = string.Empty;
            try
            {
                if (cell != null)
                {
                    if (cell.CellType != CellType.Blank)
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Numeric:  // 數值格式
                                if (HSSFDateUtil.IsCellDateFormatted(cell))
                                {   // 日期格式
                                    cellval = cell.DateCellValue.ToString();
                                }
                                else if (DateUtil.IsCellDateFormatted(cell))
                                {   // 日期格式
                                    cellval = cell.DateCellValue.ToString();
                                }
                                else
                                {   // 數值格式
                                    cellval = cell.NumericCellValue.ToString();
                                }
                                break;
                            case CellType.String:   // 字串格式
                                cellval = cell.StringCellValue;
                                break;
                            case CellType.Boolean:
                                // Boolean type
                                cellval = cell.BooleanCellValue.ToString();
                                break;
                            case CellType.Formula:  // 公式格式
                                var formulaValue = formulaEvaluator.Evaluate(cell);
                                if (formulaValue.CellType == CellType.String) cellval = formulaValue.StringValue.ToString();          // 執行公式後的值為字串型態
                                else if (formulaValue.CellType == CellType.Numeric) cellval = formulaValue.NumberValue.ToString();    // 執行公式後的值為數字型態
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return cellval;
        }
    }
}
