
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
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
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "CustomerNo", ExcelName = "訂單單號", TableName = "OrderHead" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Serial", ExcelName = "序號", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ProductId", ExcelName = "品號", TableName = "OrderDetail", Change = "Product" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "品名", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "規格", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Quantity", ExcelName = "數量", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "已交", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Unit", ExcelName = "單位", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "OriginPrice", ExcelName = "原單價", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "折扣率", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "折後單價", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Price", ExcelName = "金額", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "OrderDate", ExcelName = "預交日", TableName = "OrderHead" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "Remark", ExcelName = "備註", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "回覆量", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ReplyDate", ExcelName = "回覆交期", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "ReplyRemark", ExcelName = "回覆備註", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "MachineNo", ExcelName = "機號", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "圖檔", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "噴墨", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "標籤", TableName = "OrderDetail" });
            ExcelOrderModellist.Add(new ExcelOrderModel { ModelName = "", ExcelName = "包裝數", TableName = "OrderDetail" });
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

        internal static string GrtCellval(IFormulaEvaluator formulaEvaluator, ICell cell)
        {
            string cellval = string.Empty;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:  // 數值格式
                            if (DateUtil.IsCellDateFormatted(cell))
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
            catch (Exception)
            {

            }
            return cellval;
        }
    }
}
