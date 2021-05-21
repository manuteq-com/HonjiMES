using DevExtreme.AspNet.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;

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

        public MaterialLog MaterialLog { get; set; }
        public WiproductLog WiproductLog { get; set; }
        public ProductLog ProductLog { get; set; }
    }

    public class UserPasswordSet
    {
        public string Password { get; set; }
        public string CheckPassword { get; set; }
    }

    /// <summary>
    /// 原料主檔資訊
    /// </summary>
    public class MaterialBasicData : MaterialBasic
    {
        public decimal TotalCount { get; set; }
    }

    /// <summary>
    /// 成品主檔資訊
    /// </summary>
    public class ProductBasicData : ProductBasic
    {
        public decimal TotalCount { get; set; }
    }

    /// <summary>
    /// 半成品主檔資訊
    /// </summary>
    public class WiproductBasicData : WiproductBasic
    {
        public decimal TotalCount { get; set; }
    }

    /// <summary>
    /// 銷貨單資訊
    /// </summary>
    public class SaleDetailNewData : SaleDetailNew
    {
        public decimal TotalCount { get; set; }
        public string CustomerNo { get; set; }
        public string OrderNo { get; set; }
        public int Serial { get; set; }
        public string MachineNo { get; set; }
        public string SaleNo { get; set; }
        public DateTime? SaleDate { get; set; }
        public int WarehouseId { get; set; }
    }

    /// <summary>
    /// 銷貨單銷退資訊
    /// </summary>
    public class SaleDetailNewReturnData : SaleDetailNewData
    {
        public string ReturnWarehouse { get; set; }
        public int ReturnQuantity { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnRemarks { get; set; }
        public DateTime ReturnCreateTime { get; set; }
        public string ReturnCreateUser { get; set; }
    }

    /// <summary>
    /// 銷貨單銷退資訊
    /// </summary>
    public class BillofPurchaseReturnData : BillofPurchaseReturn
    {
        public string BillofPurchaseNo { get; set; }
        public string PurchaseNo { get; set; }
        public string SupplierName { get; set; }
        public string ProductNo { get; set; }
        public string Specification { get; set; }
        public string ReturnWarehouse { get; set; }
        public decimal ReturnQuantity { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnRemarks { get; set; }
        public DateTime ReturnCreateTime { get; set; }
        public string ReturnCreateUser { get; set; }
    }

    /// <summary>
    /// 庫存調整單
    /// </summary>
    public class AdjustData
    {
        public string AdjustNo { get; set; }
        public string LinkOrder { get; set; }
        public string Remarks { get; set; }
        public List<MaterialLog> MaterialLog { get; set; }
        public List<ProductLog> ProductLog { get; set; }
        public List<AdjustDetailData> AdjustDetailData { get; set; }
    }

    /// <summary>
    /// 庫存調整單
    /// </summary>
    public class AdjustDetailData
    {
        public int TempId { get; set; }
        public int? DataType { get; set; }
        public int DataId { get; set; }
        public string DataNo { get; set; }
        public string DataName { get; set; }
        public int WarehouseId { get; set; }
        public decimal Original { get; set; }
        public decimal AftQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAll { get; set; }
        public string Unit { get; set; }
        public decimal UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceAll { get; set; }
        public decimal WorkPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 庫存入庫單
    /// </summary>
    public class StockDetailData
    {
        public int TempId { get; set; }
        public int? DataType { get; set; }
        public int DataId { get; set; }
        public string DataNo { get; set; }
        public string DataName { get; set; }
        public int WarehouseId { get; set; }
        public decimal Original { get; set; }
        public decimal AftQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAll { get; set; }
        public string Unit { get; set; }
        public decimal UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceAll { get; set; }
        public decimal WorkPrice { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 庫存調整單Basic資料(包含原料、成品)
    /// </summary>
    public class BasicData
    {
        public int TempId { get; set; }
        public int? DataType { get; set; }
        public int DataId { get; set; }
        public string DataNo { get; set; }
        public string Name { get; set; }
        public string Specification { get; set; }
        public string Property { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int? WarehouseId { get; set; }
        public int? WarehouseIdA { get; set; }
        public int? WarehouseIdB { get; set; }
    }

    /// <summary>
    /// 建立單號
    /// </summary>
    public class CreateNumberInfo
    {
        public int Type { get; set; }
        public string CreateNumber { get; set; }
        public DateTime CreateTime { get; set; }
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
        public DateTime CreateTime { get; set; }
        public string SaleNo { get; set; }
    }

    /// <summary>
    /// 訂單明細
    /// </summary>
    public class OrderDetailData : OrderDetail
    {
        public decimal TotalCount { get; set; }
        public string OrderNo { get; set; }

    }


    public class ToSalesOrderDetail : OrderDetail
    {
        public DateTime SaleDate { get; set; }
        public int SaleQuantity { get; set; }
    }
    public class ToWorksOrderDetail : OrderDetail
    {
        public List<OrderDetailIdListInfo> OrderDetailIdList { get; set; }
    }
    public class OrderDetailIdListInfo
    {
        public int OrderDetailId { get; set; }
        public decimal Count { get; set; }
    }
    public class OrderDetailInfo : OrderDetail
    {
        public string OrderNo { get; set; }
        public string OrderType { get; set; }
        public int Customer { get; set; }
    }
    /// <summary>
    /// 過濾
    /// </summary>
    public class SearchValue
    {
        public string OrderNo { get; set; }
        public string MachineNo { get; set; }
        public string ProductNo { get; set; }
        public string PurchaseNo { get; set; }
        public string SupplierCode { get; set; }
        public string MaterialNo { get; set; }
        public string WorkOrderNo { get; set; }
        /// <summary>
        /// 查詢種類
        /// </summary>
        /// <value></value>
        public int? Type { get; set; }
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
    /// 銷貨單銷貨
    /// </summary>
    public class ToOrderSale : OrderSale
    {
        public int Quantity { get; set; }
        /// <summary>
        /// 倉庫
        /// </summary>
        public int WarehouseId { get; set; }
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

    public class Orderlist
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string MachineNo { get; set; }
        public int ProductBasicId { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Specification { get; set; }
        public int SaleCount { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public int OriginPrice { get; set; }
        public int Price { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReplyDate { get; set; }
        public string Remark { get; set; }
        public string ReplyRemark { get; set; }
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
    public class WiproductW : Wiproduct
    {
        public int? SupplierId { get; set; }
        public List<int> wid { get; set; }
        public List<Warehouse> warehouseData { get; set; }
    }
    public class MaterialW : Material
    {
        public int? MaterialType { get; set; }
        public int? SupplierId { get; set; }
        public decimal? Weight { get; set; }
        public string Remarks { get; set; }
        public List<int> wid { get; set; }
        public List<Warehouse> warehouseData { get; set; }
    }

    /// <summary>
    /// 進貨單驗收內容
    /// </summary>
    public class BillofPurchaseCheckData
    {
        public string ReturnNo { get; set; }
        public int BillofPurchaseDetailId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAll { get; set; }
        public string Unit { get; set; }
        public decimal? UnitCount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPriceAll { get; set; }
        public decimal? WorkPrice { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public int? Responsibility { get; set; }
        public DateTime? ReturnTime { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateUser { get; set; }
    }

    /// <summary>
    /// 回傳From的查詢條件
    /// </summary>
    public class FromQuery
    {
        public int skip { get; set; }
        public int take { get; set; }
        public int? requireTotalCount { get; set; }
        public int? requireGroupCount { get; set; }
        public string sort { get; set; }
        public string filter { get; set; }
        public int? totalSummary { get; set; }
        public string group { get; set; }
        public int? groupSummary { get; set; }
    }
    public class FromQueryResult
    {
        public object data { get; set; }
        public int totalCount { get; set; }
        public decimal? summary { get; set; }
        public decimal? groupCount { get; set; }
    }
    public class QueryList
    {
        public string key { get; set; }
        public string where { get; set; }
        public string val { get; set; }
    }

    public class BomList
    {
        public int Id { get; set; }
        public int? Pid { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public sbyte? Outsource { get; set; }
        public int Group { get; set; }
        public sbyte? Type { get; set; }
        public string Remarks { get; set; }
        public int Master { get; set; }
        public int? MaterialBasicId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialSpecification { get; internal set; }
        public decimal MaterialPrice { get; set; }
        public int? ProductBasicId { get; set; }
        public string ProductName { get; set; }
        public string ProductNo { get; set; }
        public string ProductNumber { get; internal set; }
        public string ProductSpecification { get; internal set; }
        public decimal ProductPrice { get; set; }
        public int Lv { get; set; }
        public decimal ReceiveQty { get; internal set; }
        public bool? Ismaterial { get; internal set; }
    }
    public class PostBom
    {
        public string Name { get; set; }
        public int? BasicType { get; set; }
        public int? BasicId { get; set; }
        public decimal Quantity { get; set; }
    }
    public class DataDetailSourceLoadOptions : DataSourceLoadOptions
    {
        public IList Detailfilter { get; set; }
    }
    /// <summary>
    /// 對應EXCL
    /// </summary>
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
    public class RequisitionDetailR : RequisitionDetail
    {
        /// <summary>
        /// 已領數量
        /// </summary>
        public decimal ReceiveQty { get; set; }
        /// <summary>
        /// 庫存數量
        /// </summary>
        public decimal StockQty { get; set; }
    }
    public class RequisitionDetailLog
    {
        /// <summary>
        /// LogId
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 領退料單號
        /// </summary>
        public string RequisitionNo { get; set; }
        /// <summary>
        /// 已領數量
        /// </summary>
        public decimal ReceiveQty { get; set; }
        /// <summary>
        /// 已退數量
        /// </summary>
        public decimal RbackQty { get; set; }
        /// <summary>
        /// 品項NO
        /// </summary>
        public string NameNo { get; set; }
        /// <summary>
        /// 品項類型
        /// </summary>
        public string NameType { get; set; }
        /// <summary>
        /// 庫存數量
        /// </summary>
        public decimal StockQty { get; set; }
        /// <summary>
        /// 品項的倉庫名稱
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        public int? ReceiveUser { get; set; }
        public int CreateUser { get; set; }
    }
    public class RequisitionDetailAll : RequisitionDetail
    {
        /// <summary>
        /// 已領數量
        /// </summary>
        public decimal ReceiveQty { get; set; }
        /// <summary>
        /// 已退數量
        /// </summary>
        public decimal RbackQty { get; set; }
        /// <summary>
        /// 品項NO
        /// </summary>
        public string NameNo { get; set; }
        /// <summary>
        /// 品項類型
        /// </summary>
        public string NameType { get; set; }
        /// <summary>
        /// 庫存數量
        /// </summary>
        public decimal StockQty { get; set; }
        /// <summary>
        /// 主要用料
        /// </summary>
        public int Master { get; set; }
        /// <summary>
        /// 品項的倉庫資料
        /// </summary>
        public List<ReqWarehouse> WarehouseList { get; set; }
    }
    public class RequisitionDetailAllShow : RequisitionDetail
    {
        /// <summary>
        /// 已領數量
        /// </summary>
        public decimal ReceiveQty { get; set; }
        /// <summary>
        /// 已退數量
        /// </summary>
        public decimal RbackQty { get; set; }
        /// <summary>
        /// 品項NO
        /// </summary>
        public string NameNo { get; set; }
        /// <summary>
        /// 品項類型
        /// </summary>
        public string NameType { get; set; }

        /// <summary>
        /// 品項的倉庫資料
        /// </summary>
        public string WarehouseName { get; set; }
    }
    public class ReqWarehouse
    {
        /// <summary>
        /// 倉庫ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 倉庫名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 庫存數量
        /// </summary>
        public decimal StockQty { get; set; }
    }
    public class GetReceive
    {
        public int? ProductBasicId { get; set; }
        public int? MaterialBasicId { get; set; }
        public decimal? RQty { get; set; }
        public int? WarehouseID { get; set; }
    }

    public class PostRequisition
    {
        public int WorkOrderNo { get; set; }
        public int? ReceiveUser { get; set; }
        public int CreateUser { get; set; }
        public List<GetReceive> ReceiveList { get; set; }
    }

    /// <summary>
    /// 登入用
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 目錄清單
    /// </summary>
    public class MenuViewModel
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 圖示
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 路徑
        /// </summary>
        public string[] routerLink { get; set; }
        /// <summary>
        /// 子層
        /// </summary>
        public MenuViewModel[] items { get; set; }
        public bool Query { get; set; }
        public bool Creat { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
    /// <summary>
    /// 使用登入權限
    /// </summary>
    public class UserRolesToken
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Realname { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 可以用的選單
        /// </summary>
        public MenuViewModel[] Menu { get; set; }
        public DateTime Timeout { get; internal set; }
    }
    public class MenuListViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 可查詢資料
        /// </summary>
        public bool? Query { get; set; }
        /// <summary>
        /// 可新增資料
        /// </summary>
        public bool? Creat { get; set; }
        /// <summary>
        /// 可修改資料
        /// </summary>
        public bool? Edit { get; set; }
        /// <summary>
        /// 可刪除資料
        /// </summary>
        public bool? Delete { get; set; }
    }
    public class PostUserViewModel
    {
        /// <summary>
        /// 使用者資料
        /// </summary>
        public User user { get; set; }
        /// <summary>
        /// 目錄權限
        /// </summary>
        public List<MenuListViewModel> MenuList { get; set; }
    }

    public class MbomData
    {
        public int MaterialBasicId { get; set; }
        public int BomId { get; set; }
        public List<MBillOfMaterial> MBillOfMaterialList { get; set; }
    }

    public class SurfaceData : PurchaseDetail
    {
        public List<WorkOrderHead> WorkOrderHead { get; set; }
        public string[] nWorkOrderNo { get; set; }
    }

    public class OrderData
    {
        public OrderHead OrderHead { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<WorkOrderHead> WorkOrderHead { get; set; }
    }
    public class WorkOrderHeadInfo : WorkOrderHead
    {
        public string StockCount { get; set; }
        public decimal OrderCount { get; set; }
        public decimal? Total { get; set; }
    }
    public class WorkOrderHeadInfoBySurfacetreat : WorkOrderHead
    {
        public int? PurchaseDetailId { get; set; }
        public string StockCount { get; set; }        
        public decimal? Total { get; set; }
    }

    public class OrderToWorkCheckData
    {
        public ToWorksOrderDetail OrderDetail { get; set; }
        public List<WorkOrderHead> WorkOrderHead { get; set; }
    }
    public class WorkOrderDetailData : WorkOrderDetail
    {
        /// <summary>
        /// 工序的種類
        /// </summary>
        public int? ProcessType { get; set; }
        /// <summary>
        /// 實際總工時
        /// </summary>
        public decimal? ActualTotalTime { get; set; }
        /// <summary>
        /// 預計總工時
        /// </summary>
        public decimal? ExpectedlTotalTime { get; set; }
    }
    public class WorkOrderData
    {
        public WorkOrderHead WorkOrderHead { get; set; }
        public List<WorkOrderDetail> WorkOrderDetail { get; set; }
        public List<WorkOrderDetailData> WorkOrderDetailData { get; set; }
        public string mod { get; set; }
    }
    public class WorkOrderData2
    {
        public WorkOrderHead WorkOrderHead { get; set; }
        public List<WorkOrderDetailData> WorkOrderDetail { get; set; }
    }
    public class ResourceProcessData
    {
        public WorkOrderHead WorkOrderHead { get; set; }
        public WorkOrderDetail WorkOrderDetail { get; set; }
    }

    public class MbomModelData
    {
        public MbomModelHead MbomModelHead { get; set; }
        public List<MbomModelDetail> MbomModelDetail { get; set; }
    }

    public class WorkOrderReportData
    {
        public int WorkOrderID { get; set; }
        public int WorkOrderSerial { get; set; }
        public int ReCount { get; set; }
        public int NgCount { get; set; }
        public decimal RePrice { get; set; }
        public string Message { get; set; }
        public string ProducingMachine { get; set; }
        public int? PurchaseId { get; set; }
        public string PurchaseNo { get; set; }
        public int? SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public string CodeNo { get; set; }
        public int CreateUser { get; set; }
        public int Type { get; set; }

        public int ReportType { get; set; }
        public int CkCount { get; set; }
        public int OkCount { get; set; }
        public int NcCount { get; set; }
        public int MCount { get; set; }
        public string DrawNo { get; set; }
        public int CheckResult { get; set; }
    }
    public class WorkOrderReportDataAll
    {
        /// <summary>
        /// 完工數量
        /// </summary>
        public int? ReCount { get; set; }
        /// <summary>
        /// 回報數量
        /// </summary>
        public int? ReportCount { get; set; }
        /// <summary>
        /// 回報NG數量
        /// </summary>
        public int? ReportNgCount { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string Message { get; set; }
        public string CodeNo { get; set; }
        public string ProducingMachine { get; set; }
        public int CreateUser { get; set; }
    }

    public class WorkOrderReportLogData : WorkOrderReportLog
    {
        public string WorkOrderNo { get; set; }
        public string DataNo { get; set; }
        public int QCReportType { get; set; }
        public int QCReCount { get; set; }
        public int QCCkCount { get; set; }
        public int QCOkCount { get; set; }
        public int QCNgCount { get; set; }
        public int QCNcCount { get; set; }
        public string QCMessage { get; set; }
    }
    public class WorkOrderLog
    {
        public string WorkOrderNo { get; set; }
        public int ReportType { get; set; }
        public int SerialNumber { get; set; }
        public string Process { get; set; }
        public int? ProcessTime { get; set; }
        public int? ReCount { get; set; }
        public string Remarks { get; set; }
        public int StatusO { get; set; }
        public int StatusN { get; set; }
        public DateTime? DueStartTime { get; set; }
        public DateTime? DueEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class WorkOrderDetailForResourceal
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public int SerialNumber { get; set; }
        public string WorkOrderNo { get; set; }
        public string MaterialBasicName { get; set; }
        public int WorkOrderHeadStatus { get; set; }
        public int WorkOrderDetailStatus { get; set; }
        public int Count { get; set; }
        public int? ReCount { get; set; }
        public string Remarks { get; set; }
        public DateTime? DueStartTime { get; set; }
        public DateTime? DueEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public DateTime CreateTime { get; set; }
    }


    /// <summary>
    /// 製成進度
    /// </summary>
    public class ProcessesStatus
    {
        /// <summary>
        /// 製成進度的欄位
        /// </summary>
        public List<ColumnOption> ColumnOptionlist { get; set; }
        /// <summary>
        /// 製成進度的資料
        /// </summary>
        public List<ProcessesData> ProcessesDataList { get; set; }
    }
    /// <summary>
    /// 要動態產生的欄位
    /// </summary>
    public class ColumnOption
    {
        /// <summary>
        /// table 欄位
        /// </summary>
        public string key { get; set; }
        /// <summary>
        ///  table 欄位顯示的名稱
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool show { get; set; }
        /// <summary>
        /// 欄位固定 左：nzLeft  右：nzRight
        /// </summary>
        public string Columnlock { get; set; }
    }
    public class ProcessesData
    {
        public int Key { get; set; }
        public string WorkOrderNo { get; set; }
        public string BasicDataName { get; set; }
        public string BasicDataNo { get; set; }
        public string MachineNo { get; set; }
        public string Remark { get; set; }
        public decimal OrderCount { get; set; }
        public int Count { get; set; }
        public int Status { get; set; }
        public string DueEndTime { get; set; }
        /// <summary>
        ///  製成0~19 共20組
        /// </summary>
        public TempString Temp0 { get; set; }
        public TempString Temp1 { get; set; }
        public TempString Temp2 { get; set; }
        public TempString Temp3 { get; set; }
        public TempString Temp4 { get; set; }
        public TempString Temp5 { get; set; }
        public TempString Temp6 { get; set; }
        public TempString Temp7 { get; set; }
        public TempString Temp8 { get; set; }
        public TempString Temp9 { get; set; }
        public TempString Temp10 { get; set; }
        public TempString Temp11 { get; set; }
        public TempString Temp12 { get; set; }
        public TempString Temp13 { get; set; }
        public TempString Temp14 { get; set; }
        public TempString Temp15 { get; set; }
        public TempString Temp16 { get; set; }
        public TempString Temp17 { get; set; }
        public TempString Temp18 { get; set; }
        public TempString Temp19 { get; set; }
    }

    /// <summary>
    /// 製成進度
    /// </summary>
    public class AdjustInfo
    {
        /// <summary>
        /// 製成進度的欄位
        /// </summary>
        public List<ColumnOption> ColumnOptionlist { get; set; }
        /// <summary>
        /// 製成進度的資料
        /// </summary>
        public List<AdjustInfoData> AdjustInfoData { get; set; }
    }
    public class AdjustInfoData
    {
        public int Key { get; set; }
        public string LinkOrder { get; set; }
        public string BasicDataName { get; set; }
        public string BasicDataNo { get; set; }
        public decimal Original { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAll { get; set; }
        public string Unit { get; set; }
        public decimal UnitCount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceAll { get; set; }
        public decimal WorkPrice { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
        public int WarehouseId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///  製成0~19 共20組
        /// </summary>
        public TempString Temp0 { get; set; }
        public TempString Temp1 { get; set; }
        public TempString Temp2 { get; set; }
        public TempString Temp3 { get; set; }
        public TempString Temp4 { get; set; }
        public TempString Temp5 { get; set; }
        public TempString Temp6 { get; set; }
        public TempString Temp7 { get; set; }
        public TempString Temp8 { get; set; }
        public TempString Temp9 { get; set; }
        public TempString Temp10 { get; set; }
        public TempString Temp11 { get; set; }
        public TempString Temp12 { get; set; }
        public TempString Temp13 { get; set; }
        public TempString Temp14 { get; set; }
        public TempString Temp15 { get; set; }
        public TempString Temp16 { get; set; }
        public TempString Temp17 { get; set; }
        public TempString Temp18 { get; set; }
        public TempString Temp19 { get; set; }
    }

    public class TempString
    {
        public string value0 { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public int value3 { get; set; }
        public int value4 { get; set; }
        public int? value5 { get; set; }
    }
    public class BillOfMaterialVerLv : BillOfMaterialVer
    {
        public int ShowLV { get; set; }
        public int ShowPLV { get; set; }
    }
    public class BillOfMaterialData : BillOfMaterial
    {
        public string DataNo { get; set; }
    }
    public class RequisitionsDetailInfo
    {
        public int RequisitionId { get; set; }
        public int WarehouseId { get; set; }
    }
    public class BomListVM : BomList
    {
        /// <summary>
        /// 顯示層級
        /// </summary>
        public string LvS { get; set; }
        /// <summary>
        /// 品項和元件
        /// </summary>
        public string LvName { get; set; }

        /// <summary>
        /// 物件類型
        /// </summary>
        public string BomType { get; set; }

        /// <summary>
        /// 主件
        /// </summary>
        public int Master { get; set; }
    }
    public class PostSupplierMaterial
    {
        public int MaterialBasicId { get; set; }
    }

    public class AdjustType
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }

    public class Suppliers
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PuschaseList : PurchaseDetail
    {
        public int TempId { get; set; }
        public string PurchaseNo { get; set; }
        public string WarehouseName { get; set; }
    }
    public class WorkSchedulerVM
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public bool AllDay { get; set; }
    }

    /// <summary>
    /// 資源分配資訊
    /// </summary>
    public class ResoureAllocation
    {
        public string ProducingMachine { get; set; }
        public string New { get; set; }
        public string Assign { get; set; }
        public string Start { get; set; }
        public string Ready { get; set; }
        public string ToNew { get; set; }
        public string Finish { get; set; }
        public int? AllCount { get; set; }
        public decimal? AllTime { get; set; }

    }


    public class ProcessReportVM
    {
        public int SerialNumber { get; set; }
        public string ProcessName { get; set; }
        public string ProducingMachine { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int? ReCount { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string Img { get; set; }
    }

    public class PurchaseHeadPut : PurchaseHead
    {
        public int? StatusVal { get; set; }
    }

    public class WarehouseForBom : Warehouse
    {
        public bool HasWarehouse { get; set; }
        public decimal Quantity { get; set; }
    }
    public class StockHeadInfo : StockHead
    {
        public string DataNo { get; set; }
        public decimal Original { get; set; }
        public decimal Quantity { get; set; }
    }

    public class DealPriceList
    {
        public int Id { get; set; }
        public int Customer { get; set; }
        public int ProductId { get; set; }
        public decimal Original { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceAll { get; set; }
        public string LinkOrder { get; set; }
        public DateTime? SaleDate { get; set; }
        public string OrderNo { get; set; }
        public string CustomerNo { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class PurchaseTotal
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public int New { get; set; }
        public int PurchaseFinished { get; set; }
        public int Undone { get; set; }
    }

    public class InventoryAdjust : AllStockLog
    {
        public decimal Increase { get; set; }
        public decimal Decrease { get; set; }
        public string No { get; set; }
    }

    public class PurchaseDetailVM
    {
        public int Id { get; set; }
        public PurchaseDetail PurchaseDetails { get; set; }
        public MaterialBasic MaterialBasics { get; set; }
    }

    public class SaleDetailNewVM
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public decimal? OriginPrice { get; set; }
    }

    public class machine
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public string DataNo { get; set; }
        public string No { get; set; }
        public int SerialNumber { get; set; }
        public string ProcessName { get; set; }
        public decimal RemainingTime { get; set; }
        public decimal DelayTime { get; set; }
        public int ProcessTotal { get; set; }
        public decimal TotalTime { get; set; }
        public List<machineOrder> machineOrderList { get; set; }
    }

    public class machineOrder
    {
        public int Id { get; set; }
        public string WorkOrderNo { get; set; }
        public int DetailSerialNumber { get; set; }
    }


    public class ProcessesList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WorkOrderNo { get; set; }
        public int SerialNumber { get; set; }

    }



}