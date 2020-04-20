using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    /// <summary>
    /// 庫存俢改
    /// </summary>
    public class inventorychange
    {
        /// <summary>
        /// 主Key
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 要修改的倉庫類型
        /// </summary>
        public string mod { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 修改原因
        /// </summary>
        public string Reason { get; internal set; }
        /// <summary>
        /// 修改原因說明
        /// </summary>
        public string Message { get; internal set; }
    }
    /// <summary>
    /// 轉銷貨
    /// </summary>
    public class ToSales
    {
        /// <summary>
        /// 要轉的訂單項目
        /// </summary>
        public List<int> Orderlist { get; set; }
        /// <summary>
        /// 要合併的訂單ID
        /// </summary>
        public int? SaleID { get; set; }
        /// <summary>
        /// 訂單銷貨日期
        /// </summary>
        public DateTime? SaleDate { get; set; }
        /// <summary>
        /// 訂單銷貨備註
        /// </summary>
        public string Remarks { get; set; }

    }
    /// <summary>
    /// 銷貨單銷貨
    /// </summary>
    public class OrderSale
    {
        /// <summary>
        /// 銷貨單ID，整批銷貨
        /// </summary>
        public int? SaleID { get; set; }
        /// <summary>
        /// 銷貨單內容ID，分批銷貨
        /// </summary>
        public int? SaleDID { get; set; }
    }
}
