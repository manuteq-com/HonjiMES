using DevExtreme.AspNet.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HonjiMES.Models
{
    /// <summary>
    /// 工單報表
    /// </summary>
    public class WorkOrderReport{
        public int No { get; set; }
        public DateTime Date { get; set; }
        public int Number { get; set; }
        public DateTime ADate { get; set; }
        public int ANumber { get; set; }
        public DateTime BDate { get; set; }
        public int BNumber { get; set; }
        public DateTime CDate { get; set; }
        public int CNumber { get; set; }
        public int NG { get; set; }
        /// <summary>
        /// 生產者
        /// </summary>
        public string person { get; set; }
        public int MaterialNumber { get; set; }
        public string ProcessNo { get; set; }

        public string Picture { get; set; }
    }

    /// <summary>
    /// 工單
    /// </summary>
    public class WorkOrderReportVM
    {
        public int No { get; set; }
        public string ProcessNo { get; set; }
        public string ProcessName { get; set; }
        public decimal ProcessTime { get; set; }
        public string MachineName { get; set; }
        public string DueStartTime { get; set; }
        public string DueEndTime { get; set; }
    }

    /// <summary>
    /// 採購單
    /// </summary>
    public class PurchaseOrderReportVM{
        public int No { get; set; }
        public string DataNo { get; set; }
        public string DataName { get; set; }
        public string Specification { get; set; }
        public int PurchaseQuantity { get; set; }
        public int PurchasedQuantity { get; set; }
        // public string Status { get; set; }
        public string Warehouse { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal PurchaseTotal { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
    
    /// <summary>
    /// 銷貨單
    /// </summary>
    public class SaleOrderReportVM{
        public string SaleNo { get; set; }
        public string MachineNo { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }
    }
}