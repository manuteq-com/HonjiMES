using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class ExcelOrderModel
    {
        /// <summary>
        /// 資料模型的名稱
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// Excel表頭的名稱
        /// </summary>
        public string ExcelName { get; set; }
        /// <summary>
        /// 所屬的資料表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 資料轉換
        /// </summary>
        public string Change { get; set; }
        /// <summary>
        /// Excel資料順序
        /// </summary>
        public int ExcelOrder { get; set; }
    }
    /// <summary>
    /// 從匯入的Excel產生Product
    /// </summary>
    public class ProductByExcel
    {
        /// <summary>
        /// 訂單號
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 客戶單號
        /// </summary>
        public string CustomerNo { get; set; }
        /// <summary>
        /// 少的Product
        /// </summary>
        public string Products { get; set; }
    }
}
