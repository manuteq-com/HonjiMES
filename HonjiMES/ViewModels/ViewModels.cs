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
        public List<OrderDetail> Orderlist { get; set; }
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
    /// <summary>
    /// 銷貨單退貨
    /// </summary>
    public class ReOrderSale : OrderSale
    {
        /// <summary>
        /// 退貨原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 退貨數量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 倉庫
        /// </summary>
        public int WarehouseId { get; set; }
    }

    /// <summary>
    /// 採購轉進貨單
    /// </summary>
    public class ToPurchase
    {
        /// <summary>
        /// 要轉的進貨單項目
        /// </summary>
        public List<BillofPurchaseDetail> BillofPurchaseDetail { get; set; }
        /// <summary>
        /// 要合併的進貨單ID
        /// </summary>
        public int? PurchaseID { get; set; }
        /// <summary>
        /// 進貨日期
        /// </summary>
        public DateTime? PurchaseDate { get; set; }
        /// <summary>
        /// 進貨單備註
        /// </summary>
        public string Remarks { get; set; }

    }
    public class ProductW : Product
    {
        public List<int> wid { get; set; }
        public List<Warehouse> warehouseData { get; set; }
    }
    public class MaterialW : Material
    {
        public List<int> wid { get; set; }
        public List<Warehouse> warehouseData { get; set; }
    }
}
    public class BOM
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string F { get; set; }
        public string G { get; set; }
        public string H { get; set; }
        public string I { get; set; }
        public string J { get; set; }
        public string K { get; set; }
    }
}