﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HonjiMES.Models
{
    public partial class HonjiContext : DbContext
    {
        public virtual DbSet<AdjustDetail> AdjustDetails { get; set; }
        public virtual DbSet<AdjustHead> AdjustHeads { get; set; }
        public virtual DbSet<AllStockLog> AllStockLogs { get; set; }
        public virtual DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public virtual DbSet<BillOfMaterialVer> BillOfMaterialVers { get; set; }
        public virtual DbSet<BillofPurchase> BillofPurchases { get; set; }
        public virtual DbSet<BillofPurchaseCheckin> BillofPurchaseCheckins { get; set; }
        public virtual DbSet<BillofPurchaseDetail> BillofPurchaseDetails { get; set; }
        public virtual DbSet<BillofPurchaseHead> BillofPurchaseHeads { get; set; }
        public virtual DbSet<BillofPurchaseReturn> BillofPurchaseReturns { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<MBillOfMaterial> MBillOfMaterials { get; set; }
        public virtual DbSet<MachineInformation> MachineInformations { get; set; }
        public virtual DbSet<MachineLog> MachineLogs { get; set; }
        public virtual DbSet<MachineMaintenance> MachineMaintenances { get; set; }
        public virtual DbSet<MachineWorkdate> MachineWorkdates { get; set; }
        public virtual DbSet<MaintenanceDetail> MaintenanceDetails { get; set; }
        public virtual DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<MaterialBasic> MaterialBasics { get; set; }
        public virtual DbSet<MaterialLog> MaterialLogs { get; set; }
        public virtual DbSet<MbomModelDetail> MbomModelDetails { get; set; }
        public virtual DbSet<MbomModelHead> MbomModelHeads { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderDetailAndWorkOrderHead> OrderDetailAndWorkOrderHeads { get; set; }
        public virtual DbSet<OrderHead> OrderHeads { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductBasic> ProductBasics { get; set; }
        public virtual DbSet<ProductLog> ProductLogs { get; set; }
        public virtual DbSet<ProductSn> ProductSns { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<PurchaseHead> PurchaseHeads { get; set; }
        public virtual DbSet<Receive> Receives { get; set; }
        public virtual DbSet<Requisition> Requisitions { get; set; }
        public virtual DbSet<RequisitionDetail> RequisitionDetails { get; set; }
        public virtual DbSet<ReturnSale> ReturnSales { get; set; }
        public virtual DbSet<SaleDetailNew> SaleDetailNews { get; set; }
        public virtual DbSet<SaleHead> SaleHeads { get; set; }
        public virtual DbSet<SaleLog> SaleLogs { get; set; }
        public virtual DbSet<StaffManagement> StaffManagements { get; set; }
        public virtual DbSet<StockDetail> StockDetails { get; set; }
        public virtual DbSet<StockHead> StockHeads { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierOfMaterial> SupplierOfMaterials { get; set; }
        public virtual DbSet<System> Systems { get; set; }
        public virtual DbSet<ToolManagement> ToolManagements { get; set; }
        public virtual DbSet<Toolset> Toolsets { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<WebSession> WebSessions { get; set; }
        public virtual DbSet<Wiproduct> Wiproducts { get; set; }
        public virtual DbSet<WiproductBasic> WiproductBasics { get; set; }
        public virtual DbSet<WiproductLog> WiproductLogs { get; set; }
        public virtual DbSet<WorkOrderDetail> WorkOrderDetails { get; set; }
        public virtual DbSet<WorkOrderHead> WorkOrderHeads { get; set; }
        public virtual DbSet<WorkOrderQcLog> WorkOrderQcLogs { get; set; }
        public virtual DbSet<WorkOrderReportLog> WorkOrderReportLogs { get; set; }

        public HonjiContext(DbContextOptions<HonjiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdjustDetail>(entity =>
            {
                entity.HasIndex(e => e.AdjustHeadId)
                    .HasName("adjust_head_id");

                entity.Property(e => e.AdjustHeadId).HasComment("調整單ID");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.ItemId).HasComment("料號ID");

                entity.Property(e => e.ItemType).HasComment("料號種類(1原料2成品3半成品)");

                entity.Property(e => e.Message)
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.AdjustHead)
                    .WithMany(p => p.AdjustDetails)
                    .HasForeignKey(d => d.AdjustHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("adjust_detail_ibfk_1");
            });

            modelBuilder.Entity<AdjustHead>(entity =>
            {
                entity.Property(e => e.AdjustNo)
                    .HasComment("調整單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.LinkOrder)
                    .HasComment("關聯單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<AllStockLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("all_stock_log");

                entity.Property(e => e.AdjustNo)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.DataName)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataNo)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.LinkOrder)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Message)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NameLog)
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Reason)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<BillOfMaterial>(entity =>
            {
                entity.HasComment("產品組成表");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("fk_bill_of_material_material_basic1_idx");

                entity.HasIndex(e => e.Pid)
                    .HasName("fk_bill_of_material_bill_of_material1_idx");

                entity.HasIndex(e => e.ProductBasicId)
                    .HasName("fk_bill_of_material_product_basic1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Group)
                    .HasDefaultValueSql("'1'")
                    .HasComment("組成表群組");

                entity.Property(e => e.Lv)
                    .HasDefaultValueSql("'1'")
                    .HasComment("層數");

                entity.Property(e => e.Master).HasComment("主件");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Outsource).HasComment("外包註記");

                entity.Property(e => e.Pid).HasComment("父ID");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Type).HasComment("BOM的類型");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.BillOfMaterialMaterialBasics)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .HasConstraintName("fk_bill_of_material_material_basic1");

                entity.HasOne(d => d.P)
                    .WithMany(p => p.InverseP)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_bill_of_material_bill_of_material1");

                entity.HasOne(d => d.ProductBasic)
                    .WithMany(p => p.BillOfMaterialProductBasics)
                    .HasForeignKey(d => d.ProductBasicId)
                    .HasConstraintName("fk_bill_of_material_product_basic1");
            });

            modelBuilder.Entity<BillOfMaterialVer>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Bomid })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Bomid).HasComment("bomID");

                entity.Property(e => e.Bompid).HasComment("父bomID	");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Group)
                    .HasDefaultValueSql("'1'")
                    .HasComment("組成表群組");

                entity.Property(e => e.Lv)
                    .HasDefaultValueSql("'1'")
                    .HasComment("層數");

                entity.Property(e => e.MaterialName)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialNo)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Outsource).HasComment("外包註記");

                entity.Property(e => e.ProductName)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProductNo)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Type).HasComment("BOM的類型");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Version)
                    .HasDefaultValueSql("'1.00'")
                    .HasComment("版本");
            });

            modelBuilder.Entity<BillofPurchase>(entity =>
            {
                entity.HasComment("進貨單");

                entity.HasIndex(e => e.BillofPurchaseNo)
                    .HasName("billof_purchase_no");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BillofPurchaseNo)
                    .HasComment("進貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("新增者id");

                entity.Property(e => e.Date)
                    .HasComment("進貨日期")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.MaterialNo).HasComment("品號");

                entity.Property(e => e.Name)
                    .HasComment("品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("驗收/退數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status)
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");
            });

            modelBuilder.Entity<BillofPurchaseCheckin>(entity =>
            {
                entity.HasComment("進貨檢驗");

                entity.HasIndex(e => e.BillofPurchaseDetailId)
                    .HasName("fk_billof_purchase_checkin_billof_purchase_detail1_idx");

                entity.Property(e => e.CheckinType).HasComment("驗收類型");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("驗收數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額 ");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.BillofPurchaseDetail)
                    .WithMany(p => p.BillofPurchaseCheckins)
                    .HasForeignKey(d => d.BillofPurchaseDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billof_purchase_checkin_billof_purchase_detail1");
            });

            modelBuilder.Entity<BillofPurchaseDetail>(entity =>
            {
                entity.HasComment("進貨單明細");

                entity.HasIndex(e => e.BillofPurchaseId)
                    .HasName("fk_billof_purchase_detail_billof_purchase_head_idx");

                entity.HasIndex(e => e.PurchaseDetailId)
                    .HasName("fk_billof_purchase_detail_purchase_detail_idx");

                entity.HasIndex(e => e.PurchaseId)
                    .HasName("fk_billof_purchase_detail_purchase_head_idx");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("fk_billof_purchase_detail_supplier_idx");

                entity.Property(e => e.BillofPurchaseId).HasComment("進貨單號");

                entity.Property(e => e.BillofPurchaseType).HasComment("進貨種類");

                entity.Property(e => e.CheckCountIn).HasComment("驗收數量");

                entity.Property(e => e.CheckCountOut).HasComment("驗退數量");

                entity.Property(e => e.CheckPriceIn).HasComment("驗收金額");

                entity.Property(e => e.CheckPriceOut).HasComment("驗退金額");

                entity.Property(e => e.CheckStatus).HasComment("驗收狀態");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DataId).HasComment("進貨內容ID");

                entity.Property(e => e.DataName)
                    .HasComment("進貨內容名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataNo)
                    .HasComment("進貨內容編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataType).HasComment("料號種類(1原料 2成品 3 半成品)");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Delivered).HasComment("實際交貨數");

                entity.Property(e => e.OrderId).HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice).HasComment("原單價	");

                entity.Property(e => e.Price).HasComment("價格");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.PurchaseCount).HasComment("已開採購數量");

                entity.Property(e => e.PurchaseId).HasComment("採購單id");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位單價");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WarehouseId).HasComment("倉別id");

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.BillofPurchase)
                    .WithMany(p => p.BillofPurchaseDetails)
                    .HasForeignKey(d => d.BillofPurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billof_purchase_detail_billof_purchase_head1");

                entity.HasOne(d => d.PurchaseDetail)
                    .WithMany(p => p.BillofPurchaseDetails)
                    .HasForeignKey(d => d.PurchaseDetailId)
                    .HasConstraintName("fk_billof_purchase_detail_purchase_detail1");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.BillofPurchaseDetails)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("fk_billof_purchase_detail_purchase_head1");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.BillofPurchaseDetails)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billof_purchase_detail_supplier1");
            });

            modelBuilder.Entity<BillofPurchaseHead>(entity =>
            {
                entity.HasComment("進貨單");

                entity.Property(e => e.BillofPurchaseDate).HasComment("進貨日期");

                entity.Property(e => e.BillofPurchaseNo)
                    .HasComment("進貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("進貨狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<BillofPurchaseReturn>(entity =>
            {
                entity.HasIndex(e => e.BillofPurchaseDetailId)
                    .HasName("billof_purchase_detail_id");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("warehouse_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BillofPurchaseDetailId).HasComment("進貨單明細id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記	");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Reason)
                    .HasComment("原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Responsibility).HasComment("歸責(0自己1廠商)");

                entity.Property(e => e.ReturnNo)
                    .HasComment("驗退單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ReturnTime).HasComment("寄回日");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WarehouseId).HasComment("倉庫id");

                entity.HasOne(d => d.BillofPurchaseDetail)
                    .WithMany(p => p.BillofPurchaseReturns)
                    .HasForeignKey(d => d.BillofPurchaseDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("billof_purchase_return_ibfk_1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.BillofPurchaseReturns)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("billof_purchase_return_ibfk_2");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasComment("客戶");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Address)
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Bank)
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Branch)
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Email)
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Phone)
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UniformNo)
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<MBillOfMaterial>(entity =>
            {
                entity.HasComment("MBOM");

                entity.HasIndex(e => e.ProcessId)
                    .HasName("process_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Manpower).HasComment("所需人力");

                entity.Property(e => e.Name)
                    .HasComment("工序名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Pid).HasComment("父ID");

                entity.Property(e => e.ProcessCost).HasComment("成本");

                entity.Property(e => e.ProcessLeadTime).HasComment("前置時間");

                entity.Property(e => e.ProcessName)
                    .HasComment("工序名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessNo)
                    .HasComment("工序代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessTime).HasComment("標準工時");

                entity.Property(e => e.ProducingMachine)
                    .HasComment("機台")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SerialNumber).HasComment("工序順序");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.Type)
                    .HasDefaultValueSql("'0'")
                    .HasComment("種類")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Version).HasComment("版本");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.MBillOfMaterials)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("m_bill_of_material_ibfk_1");
            });

            modelBuilder.Entity<MachineInformation>(entity =>
            {
                entity.HasComment("機台資訊");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.ControlBrandId).HasComment("控制器種類ID");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.EnableState).HasDefaultValueSql("'0'");

                entity.Property(e => e.ModelId).HasComment("機台種類ID");

                entity.Property(e => e.Name)
                    .HasComment("機台名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.PlanSetting)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Port).HasComment("連線埠號");

                entity.Property(e => e.SpecialSetting)
                    .HasComment("特殊設定JSON")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.TransferSetting)
                    .HasComment("檔案傳送設定JSON")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Url)
                    .HasComment("連線地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UserId).HasComment("負責人ID");
            });

            modelBuilder.Entity<MachineLog>(entity =>
            {
                entity.HasComment("機台警示");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AlarmMessage)
                    .HasComment("警報訊息")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.AlarmNumber).HasComment("警報號碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.EndTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("結束時間");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<MachineMaintenance>(entity =>
            {
                entity.HasComment("機台保養與維護");

                entity.HasIndex(e => e.MachineId)
                    .HasName("machine_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.CycleTime).HasComment("週期(月)");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Item)
                    .HasComment("保養項目")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.NextTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("下次保養時間");

                entity.Property(e => e.RecentTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("近期保養時間");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.MachineMaintenances)
                    .HasForeignKey(d => d.MachineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("machine_maintenance_ibfk_1");
            });

            modelBuilder.Entity<MachineWorkdate>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Setting).HasComment("設定");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkDate).HasComment("日期");

                entity.Property(e => e.WorkTimeEnd)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("結束時間");

                entity.Property(e => e.WorkTimeStart)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("起始時間");
            });

            modelBuilder.Entity<MaintenanceDetail>(entity =>
            {
                entity.HasComment("保養明細");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Item)
                    .HasComment("保養項目")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaintenanceId).HasComment("機台保養ID");

                entity.Property(e => e.RecentTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("維護時間");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間");

                entity.Property(e => e.UserId).HasComment("操作人員");
            });

            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.HasComment("機台保養紀錄");

                entity.HasIndex(e => e.MachineId)
                    .HasName("machine_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Item)
                    .HasComment("保養項目")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.MachineName)
                    .HasComment("機台名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.RecentTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("維護時間");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間");

                entity.Property(e => e.UserId).HasComment("操作人員");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasComment("原料庫存");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("fk_material_material_basic1_idx");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_material_warehouse1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BaseQuantity).HasComment("底數");

                entity.Property(e => e.Composition).HasComment("組成用量");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.MaterialNo)
                    .HasComment("品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialNumber)
                    .HasComment("場內品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialRequire).HasComment("原料需求量	");

                entity.Property(e => e.Name)
                    .HasComment("品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("庫存量");

                entity.Property(e => e.QuantityAdv).HasComment("預先扣庫數量");

                entity.Property(e => e.QuantityLimit).HasComment("庫存極限");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_material_basic1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_warehouse1");
            });

            modelBuilder.Entity<MaterialBasic>(entity =>
            {
                entity.HasComment("原料基本檔");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.ActualSpecification)
                    .HasComment("實際規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialNo)
                    .HasComment("品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialNumber)
                    .HasComment("廠內品號	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialType).HasComment("品號種類");

                entity.Property(e => e.Name)
                    .HasComment("品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SupplierId).HasComment("供應商");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Weight).HasComment("重量(公斤)");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.MaterialBasics)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("material_basic_ibfk_1");
            });

            modelBuilder.Entity<MaterialLog>(entity =>
            {
                entity.HasComment("原料LOG");

                entity.HasIndex(e => e.MaterialId)
                    .HasName("fk_material_log_material1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AdjustNo)
                    .HasComment("調整單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.LinkOrder)
                    .HasComment("關連訂單")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialId).HasComment("原料ID");

                entity.Property(e => e.Message)
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialLogs)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_log_material1");
            });

            modelBuilder.Entity<MbomModelDetail>(entity =>
            {
                entity.HasIndex(e => e.MbomModelHeadId)
                    .HasName("mbom_model_head_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Manpower).HasComment("所需人力");

                entity.Property(e => e.ProcessCost).HasComment("成本");

                entity.Property(e => e.ProcessLeadTime).HasComment("前置時間");

                entity.Property(e => e.ProcessName)
                    .HasComment("工序名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessNo)
                    .HasComment("工序代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessTime).HasComment("標準工時");

                entity.Property(e => e.ProducingMachine)
                    .HasComment("機台")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SerialNumber).HasComment("工序順序");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.Type)
                    .HasDefaultValueSql("'0'")
                    .HasComment("種類")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MbomModelHead)
                    .WithMany(p => p.MbomModelDetails)
                    .HasForeignKey(d => d.MbomModelHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("mbom_model_detail_ibfk_1");
            });

            modelBuilder.Entity<MbomModelHead>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.ModelCode)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ModelName)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ModelRemarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Icon)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Memo)
                    .HasComment("說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Order).HasComment("排序");

                entity.Property(e => e.Pid).HasComment("父ID");

                entity.Property(e => e.RouterLink)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Data)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasComment("訂單");

                entity.HasIndex(e => e.ProjectNo)
                    .HasName("project_no");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.CustomerOrderNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Finish)
                    .HasComment("結案否")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineId)
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.OrderDeliveryDate).HasComment("訂單交期");

                entity.Property(e => e.Price).HasComment("折後單價");

                entity.Property(e => e.ProductNo).HasComment("主件品號");

                entity.Property(e => e.ProjectNo)
                    .HasComment("專案號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Status)
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasComment("訂單明細");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("material_basic_id");

                entity.HasIndex(e => e.MaterialId)
                    .HasName("fk_order_detail_product1_idx");

                entity.HasIndex(e => e.OrderId)
                    .HasName("fk_order_order_detail");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立日期");

                entity.Property(e => e.CustomerNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Delivered).HasComment("實際交貨數");

                entity.Property(e => e.Discount).HasComment("折扣率");

                entity.Property(e => e.DiscountPrice).HasComment("折後單價");

                entity.Property(e => e.Drawing)
                    .HasComment("圖檔")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DueDate)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("預交日");

                entity.Property(e => e.Ink)
                    .HasComment("噴墨")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Label)
                    .HasComment("標籤")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineNo)
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MaterialBasicId).HasComment("品號基本資訊id");

                entity.Property(e => e.MaterialId).HasComment("品號id");

                entity.Property(e => e.OrderId).HasComment("訂單id");

                entity.Property(e => e.OriginPrice).HasComment("原單價");

                entity.Property(e => e.Package).HasComment("包裝數");

                entity.Property(e => e.Price).HasComment("折後價格");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Reply).HasComment("回覆量");

                entity.Property(e => e.ReplyDate)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("回覆交期");

                entity.Property(e => e.ReplyPrice).HasComment("回覆單價");

                entity.Property(e => e.ReplyRemark)
                    .HasComment("回覆備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SaleCount).HasComment("已銷貨數");

                entity.Property(e => e.SaledCount).HasComment("完成銷貨數量");

                entity.Property(e => e.Serial).HasComment("序號");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_detail_ibfk_1");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("fk_order_detail_product1");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_detail_order_head1");
            });

            modelBuilder.Entity<OrderDetailAndWorkOrderHead>(entity =>
            {
                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("order_detail_id");

                entity.HasIndex(e => e.WorkHeadId)
                    .HasName("work_head_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DataId).HasComment("料號Id");

                entity.Property(e => e.DataType).HasComment("料號種類(1原料 2成品 3 半成品)");

                entity.Property(e => e.OrdeCount).HasComment("訂單數量");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderDetailAndWorkOrderHeads)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_detail_and_work_order_head_ibfk_1");

                entity.HasOne(d => d.WorkHead)
                    .WithMany(p => p.OrderDetailAndWorkOrderHeads)
                    .HasForeignKey(d => d.WorkHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_detail_and_work_order_head_ibfk_2");
            });

            modelBuilder.Entity<OrderHead>(entity =>
            {
                entity.HasComment("訂單");

                entity.Property(e => e.Id).HasComment("訂單id");

                entity.Property(e => e.CheckFlag).HasComment("確認欄");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.Customer).HasComment("客戶id");

                entity.Property(e => e.CustomerNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.FinishDate).HasComment("完成日期");

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("訂單日期");

                entity.Property(e => e.OrderNo)
                    .HasComment("訂單單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.OrderType)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ReplyDate).HasComment("回覆交期");

                entity.Property(e => e.StartDate).HasComment("開始日期");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CopyAdd).HasComment("複製新增");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Del).HasComment("刪除");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Edit).HasComment("修改");

                entity.Property(e => e.Export).HasComment("匯出");

                entity.Property(e => e.Import).HasComment("匯入");

                entity.Property(e => e.New).HasComment("新增");

                entity.Property(e => e.Search).HasComment("搜尋");

                entity.Property(e => e.Sort).HasComment("排序");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.Property(e => e.Id).HasComment("id");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Cost).HasComment("成本");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.LeadTime).HasComment("前置時間	");

                entity.Property(e => e.Manpower).HasComment("所需人力	");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProducingMachine)
                    .HasComment("機台	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remark)
                    .HasComment("備註欄位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Type).HasComment("工序種類");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkTime).HasComment("標準工時	");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasComment("成品庫存");

                entity.HasIndex(e => e.ProductBasicId)
                    .HasName("fk_product_product_basic1_idx");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_product_warehouse1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.MaterialId).HasComment("元件品號");

                entity.Property(e => e.MaterialRequire).HasComment("原料需求量");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.ProductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProductNumber)
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("庫存量");

                entity.Property(e => e.QuantityAdv).HasComment("預先扣庫數量");

                entity.Property(e => e.QuantityLimit).HasComment("庫存極限");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.ProductBasic)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_product_basic1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_warehouse1");
            });

            modelBuilder.Entity<ProductBasic>(entity =>
            {
                entity.HasComment("成品基本檔");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.ProductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProductNumber)
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<ProductLog>(entity =>
            {
                entity.HasComment("成品LOG");

                entity.HasIndex(e => e.ProductId)
                    .HasName("fk_product_log_product1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AdjustNo)
                    .HasComment("調整單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.LinkOrder)
                    .HasComment("關連訂單")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Message)
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.ProductId).HasComment("成品ID");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductLogs)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_log_product1");
            });

            modelBuilder.Entity<ProductSn>(entity =>
            {
                entity.HasComment("品號");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CustomerId).HasComment("廠商");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.ProductId).HasComment("品號(內部)");

                entity.Property(e => e.ProductNumber)
                    .HasComment("品號(各廠商)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasComment("採購單");

                entity.HasIndex(e => e.PurchaseNo)
                    .HasName("purchase_no");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.MaterialNo).HasComment("元件品號");

                entity.Property(e => e.Name)
                    .HasComment("元件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("價格");

                entity.Property(e => e.PurchaseDate).HasComment("採購日期");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.HasComment("採購單明細");

                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("order_detail_id");

                entity.HasIndex(e => e.PurchaseId)
                    .HasName("fk_purchase_detail_purchase_head");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("warehouse_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DataId).HasComment("採購內容ID");

                entity.Property(e => e.DataName)
                    .HasComment("採購內容名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataNo)
                    .HasComment("採購內容編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataType).HasComment("料號種類(1原料 2成品 3 半成品)");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Delivered).HasComment("已交數量");

                entity.Property(e => e.DeliveryTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("預計交期");

                entity.Property(e => e.InNg).HasComment("廠內NG數量");

                entity.Property(e => e.NotOk).HasComment("不合格數量");

                entity.Property(e => e.Ok).HasComment("合格數量");

                entity.Property(e => e.OrderDetailId).HasComment("訂單明細id");

                entity.Property(e => e.OriginPrice).HasComment("原單價	");

                entity.Property(e => e.OutNg).HasComment("廠外NG數量");

                entity.Property(e => e.Price).HasComment("價格");

                entity.Property(e => e.PurchaseCount).HasComment("進貨單數量");

                entity.Property(e => e.PurchaseId).HasComment("採購單號");

                entity.Property(e => e.PurchaseType).HasComment("採購種類");

                entity.Property(e => e.PurchasedCount).HasComment("完成進貨量");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Repair).HasComment("可修數量");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.Undelivered).HasComment("未交數量");

                entity.Property(e => e.Unrepair).HasComment("不可修數量");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WarehouseId).HasComment("倉別id");

                entity.Property(e => e.WorkOrderLog)
                    .HasComment("工單紀錄")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.OrderDetailId)
                    .HasConstraintName("purchase_detail_ibfk_3");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_purchase_detail_purchase_head1");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_detail_ibfk_1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.WarehouseId)
                    .HasConstraintName("purchase_detail_ibfk_2");
            });

            modelBuilder.Entity<PurchaseHead>(entity =>
            {
                entity.HasComment("採購單");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.PurchaseDate).HasComment("採購日期");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("採購狀態");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.PurchaseHeads)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_head_ibfk_1");
            });

            modelBuilder.Entity<Receive>(entity =>
            {
                entity.HasComment("領料數量記錄表");

                entity.HasIndex(e => e.RequisitionDetailId)
                    .HasName("fk_receive_requisition_detail1_idx");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("warehouse_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Quantity).HasComment("領取數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WarehouseId).HasComment("使用倉別");

                entity.HasOne(d => d.RequisitionDetail)
                    .WithMany(p => p.Receives)
                    .HasForeignKey(d => d.RequisitionDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_receive_requisition_detail1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Receives)
                    .HasForeignKey(d => d.WarehouseId)
                    .HasConstraintName("receive_ibfk_1");
            });

            modelBuilder.Entity<Requisition>(entity =>
            {
                entity.HasComment("領料資料主檔");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("fk_requisition_product_basic1_idx");

                entity.HasIndex(e => e.WorkOrderHeadId)
                    .HasName("work_order_head_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.MaterialNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaterialNumber)
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("領料單名稱")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Quantity).HasComment("生產數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RequisitionNo)
                    .HasComment("領料單號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type).HasComment("領料單類型(0:領出單,1:退庫單)");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkOrderHeadId).HasComment("工單主檔ID");

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.Requisitions)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_requisition_product_basic1");

                entity.HasOne(d => d.WorkOrderHead)
                    .WithMany(p => p.Requisitions)
                    .HasForeignKey(d => d.WorkOrderHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("requisition_ibfk_1");
            });

            modelBuilder.Entity<RequisitionDetail>(entity =>
            {
                entity.HasComment("領料資料副檔");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("fk_requisition_detail_material_basic1_idx");

                entity.HasIndex(e => e.ProductBasicId)
                    .HasName("fk_requisition_detail_product_basic1_idx");

                entity.HasIndex(e => e.RequisitionId)
                    .HasName("fk_requisition_detail_requisition1_idx");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Ismaterial).HasComment("是否為原料");

                entity.Property(e => e.Lv).HasComment("階層");

                entity.Property(e => e.MaterialName)
                    .HasComment("元件品名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaterialNo)
                    .HasComment("元件品號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaterialSpecification)
                    .HasComment("元件規格")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("元件品名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProductName)
                    .HasComment("主件品名")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProductNumber)
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProductSpecification)
                    .HasComment("規格")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Quantity).HasComment("驗收數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.RequisitionDetailMaterialBasics)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .HasConstraintName("fk_requisition_detail_material_basic1");

                entity.HasOne(d => d.ProductBasic)
                    .WithMany(p => p.RequisitionDetailProductBasics)
                    .HasForeignKey(d => d.ProductBasicId)
                    .HasConstraintName("fk_requisition_detail_product_basic1");

                entity.HasOne(d => d.Requisition)
                    .WithMany(p => p.RequisitionDetails)
                    .HasForeignKey(d => d.RequisitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_requisition_detail_requisition1");
            });

            modelBuilder.Entity<ReturnSale>(entity =>
            {
                entity.HasComment("退貨記錄");

                entity.HasIndex(e => e.SaleDetailNewId)
                    .HasName("fk_return_sale_sale_detail_new1_idx");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_return_sale_warehouse1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Quantity).HasComment("退貨數量");

                entity.Property(e => e.Reason)
                    .HasComment("原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ReturnNo)
                    .HasComment("銷退單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WarehouseId).HasComment("退貨倉庫ID");

                entity.HasOne(d => d.SaleDetailNew)
                    .WithMany(p => p.ReturnSales)
                    .HasForeignKey(d => d.SaleDetailNewId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_return_sale_sale_detail_new1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.ReturnSales)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_return_sale_warehouse1");
            });

            modelBuilder.Entity<SaleDetailNew>(entity =>
            {
                entity.HasComment("銷貨明細");

                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("fk_sale_detail_new_product_basic1");

                entity.HasIndex(e => e.MaterialId)
                    .HasName("fk_sale_detail_new_product1_idx");

                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("fk_sale_detail_new_order_detail1_idx");

                entity.HasIndex(e => e.OrderId)
                    .HasName("fk_sale_detail_new_order_head1_idx");

                entity.HasIndex(e => e.SaleId)
                    .HasName("fk_sale_detail_new_sale_head1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.MaterialBasicId).HasComment("產品基本資訊id");

                entity.Property(e => e.MaterialId).HasComment("主件品號ID");

                entity.Property(e => e.MaterialNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.OrderDetailId).HasComment("訂單內容唯一碼");

                entity.Property(e => e.OrderId).HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice).HasComment("原單價	");

                entity.Property(e => e.Price).HasComment("價格");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SaleId).HasComment("銷貨單號");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("銷貨狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.SaleDetailNews)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_product_basic1");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.SaleDetailNews)
                    .HasForeignKey(d => d.MaterialId)
                    .HasConstraintName("fk_sale_detail_new_product1");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.SaleDetailNews)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_order_detail1");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.SaleDetailNews)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_order_head1");

                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleDetailNews)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_sale_head1");
            });

            modelBuilder.Entity<SaleHead>(entity =>
            {
                entity.HasComment("銷貨單");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SaleDate).HasComment("銷或日期");

                entity.Property(e => e.SaleNo)
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("銷貨狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SaleLog>(entity =>
            {
                entity.HasComment("銷貨LOG");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Message)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SaleId).HasComment("銷貨單id");

                entity.Property(e => e.Type).HasComment("log種類");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");
            });

            modelBuilder.Entity<StaffManagement>(entity =>
            {
                entity.HasComment("人員排班");

                entity.HasIndex(e => e.MachineId)
                    .HasName("machine_id");

                entity.HasIndex(e => e.ProcessId)
                    .HasName("process_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.HasIndex(e => e.WorkOrderId)
                    .HasName("work_order_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.EndTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("結束時間");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.ProcessId).HasComment("工序ID");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間");

                entity.Property(e => e.UserId).HasComment("人員ID");

                entity.Property(e => e.WorkOrderId).HasComment("工單ID");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.StaffManagements)
                    .HasForeignKey(d => d.MachineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_management_ibfk_1");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.StaffManagements)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_management_ibfk_4");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StaffManagements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_management_ibfk_2");

                entity.HasOne(d => d.WorkOrder)
                    .WithMany(p => p.StaffManagements)
                    .HasForeignKey(d => d.WorkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("staff_management_ibfk_3");
            });

            modelBuilder.Entity<StockDetail>(entity =>
            {
                entity.HasIndex(e => e.StockHeadId)
                    .HasName("stock_head_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DataNo)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ItemId).HasComment("料號ID");

                entity.Property(e => e.ItemType).HasComment("料號種類(1原料2成品3半成品)");

                entity.Property(e => e.Message)
                    .HasComment("補充說明")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasComment("修改原因")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StockHeadId).HasComment("調整單ID");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.StockHead)
                    .WithMany(p => p.StockDetails)
                    .HasForeignKey(d => d.StockHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stock_detail_ibfk_1");
            });

            modelBuilder.Entity<StockHead>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.LinkOrder)
                    .HasComment("關聯單號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.StockNo)
                    .HasComment("調整單號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasComment("供應商");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Address)
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Bank)
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Branch)
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ContactName)
                    .HasComment("聯絡人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Email)
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("供應商")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Phone)
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ShortName)
                    .HasComment("簡稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UniformNo)
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SupplierOfMaterial>(entity =>
            {
                entity.HasIndex(e => e.MaterialBasicId)
                    .HasName("material_basic_id");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Remarks)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.MaterialBasic)
                    .WithMany(p => p.SupplierOfMaterials)
                    .HasForeignKey(d => d.MaterialBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("supplier_of_material_ibfk_2");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierOfMaterials)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("supplier_of_material_ibfk_1");
            });

            modelBuilder.Entity<System>(entity =>
            {
                entity.HasComment("系統參數");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("新增者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Name)
                    .HasComment("功能名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");

                entity.Property(e => e.Value)
                    .HasComment("值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<ToolManagement>(entity =>
            {
                entity.HasComment("刀具清單");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ToolName)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ToolSerialno)
                    .HasComment("編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ToolSpecification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Type).HasComment("種類");

                entity.Property(e => e.UpdateTime).HasDefaultValueSql("current_timestamp()");
            });

            modelBuilder.Entity<Toolset>(entity =>
            {
                entity.HasComment("製程刀具表");

                entity.HasIndex(e => e.HolderId)
                    .HasName("fk_toolset_tool_management_holder_id");

                entity.HasIndex(e => e.ProcessId)
                    .HasName("fk_toolset_process_idx");

                entity.HasIndex(e => e.ToolId)
                    .HasName("fk_toolset_tool_management_tool_id");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.HolderId).HasComment("刀桿ID");

                entity.Property(e => e.ProcessId).HasComment("製程ID");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ToolId).HasComment("刀具ID");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Holder)
                    .WithMany(p => p.ToolsetHolders)
                    .HasForeignKey(d => d.HolderId)
                    .HasConstraintName("fk_toolset_tool_management_holder_id");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.Toolsets)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_toolset_process");

                entity.HasOne(d => d.Tool)
                    .WithMany(p => p.ToolsetTools)
                    .HasForeignKey(d => d.ToolId)
                    .HasConstraintName("fk_toolset_tool_management_tool_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("使用者");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Department).HasComment("部門");

                entity.Property(e => e.Password)
                    .HasComment("密碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Permission).HasComment("身分別");

                entity.Property(e => e.Realname)
                    .HasComment("姓名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Username)
                    .HasComment("帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.HasComment("使用者LOG");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_logs_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("新增者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.LoginTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("登入時間");

                entity.Property(e => e.LogoutTime).HasComment("登出時間");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");

                entity.Property(e => e.UserId).HasComment("使用者ID");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasIndex(e => e.MenuId)
                    .HasName("fk_user_roles_menu_idx");

                entity.HasIndex(e => e.UsersId)
                    .HasName("fk_user_roles_users1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Memo)
                    .HasComment("說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MenuId).HasComment("目錄ID");

                entity.Property(e => e.Roles)
                    .HasComment("權限")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UsersId).HasComment("使用都ID");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_roles_menu");

                entity.HasOne(d => d.Users)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UsersId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_roles_users1");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasComment("倉庫");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Address)
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Code)
                    .HasComment("內部代碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Contact)
                    .HasComment("連絡人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.Email)
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("倉庫名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Phone)
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Recheck).HasComment("是否要產品檢查,不檢查直接回存倉庫");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<WebSession>(entity =>
            {
                entity.HasIndex(e => e.Timestamp)
                    .HasName("web_sessions_timestamp");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("新增者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.IpAddress)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");
            });

            modelBuilder.Entity<Wiproduct>(entity =>
            {
                entity.HasComment("半成品庫存");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_wiproduct_warehouse1_idx");

                entity.HasIndex(e => e.WiproductBasicId)
                    .HasName("fk_wiproduct_wiproduct_basic1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.MaterialId).HasComment("元件品號");

                entity.Property(e => e.MaterialRequire).HasComment("原料需求量");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Quantity).HasComment("庫存量");

                entity.Property(e => e.QuantityAdv).HasComment("預先扣庫數量");

                entity.Property(e => e.QuantityLimit).HasComment("庫存極限");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WiproductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.WiproductNumber)
                    .HasComment("廠內半成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Wiproducts)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wiproduct_warehouse1");

                entity.HasOne(d => d.WiproductBasic)
                    .WithMany(p => p.Wiproducts)
                    .HasForeignKey(d => d.WiproductBasicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wiproduct_wiproduct_basic1");
            });

            modelBuilder.Entity<WiproductBasic>(entity =>
            {
                entity.HasComment("半成品基本檔");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SupplierId).HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WiproductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.WiproductNumber)
                    .HasComment("廠內半成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.WiproductBasics)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("wiproduct_basic_ibfk_1");
            });

            modelBuilder.Entity<WiproductLog>(entity =>
            {
                entity.HasComment("半成品LOG");

                entity.HasIndex(e => e.WiproductId)
                    .HasName("fk_wiproduct_log_wiproduct1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AdjustNo)
                    .HasComment("調整單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.DeleteFlag).HasComment("刪除註記");

                entity.Property(e => e.LinkOrder)
                    .HasComment("關連訂單")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Message)
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("總金額");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UnitCount).HasComment("單位數量");

                entity.Property(e => e.UnitPrice).HasComment("單位金額");

                entity.Property(e => e.UnitPriceAll).HasComment("單位總額");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新者id");

                entity.Property(e => e.WiproductId).HasComment("半成品ID");

                entity.Property(e => e.WorkPrice).HasComment("加工費用");

                entity.HasOne(d => d.Wiproduct)
                    .WithMany(p => p.WiproductLogs)
                    .HasForeignKey(d => d.WiproductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wiproduct_log_wiproduct1");
            });

            modelBuilder.Entity<WorkOrderDetail>(entity =>
            {
                entity.HasIndex(e => e.ProcessId)
                    .HasName("process_id");

                entity.HasIndex(e => e.PurchaseId)
                    .HasName("purchase_id");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.HasIndex(e => e.WorkOrderHeadId)
                    .HasName("work_order_head_id");

                entity.Property(e => e.ActualEndTime).HasComment("實際完工日");

                entity.Property(e => e.ActualStartTime).HasComment("實際開工日");

                entity.Property(e => e.CodeNo)
                    .HasComment("加工程式")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Count)
                    .HasDefaultValueSql("'1'")
                    .HasComment("需求量");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DueEndTime).HasComment("預計完工日");

                entity.Property(e => e.DueStartTime).HasComment("預計開工日");

                entity.Property(e => e.MCount).HasComment("可製造數量");

                entity.Property(e => e.MachineEndTime).HasComment("機台實際完工日");

                entity.Property(e => e.MachineStartTime).HasComment("機台實際開工日");

                entity.Property(e => e.Manpower).HasComment("所需人力	");

                entity.Property(e => e.NcCount).HasComment("NC未加工");

                entity.Property(e => e.NgCount).HasComment("NG數量");

                entity.Property(e => e.ProcessCost).HasComment("成本	");

                entity.Property(e => e.ProcessLeadTime).HasComment("前置時間	");

                entity.Property(e => e.ProcessName)
                    .HasComment("工序名稱	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessNo)
                    .HasComment("工序代號	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessTime).HasComment("標準工時	");

                entity.Property(e => e.ProducingMachine)
                    .HasComment("機台	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.PurchaseId).HasComment("採購單ID");

                entity.Property(e => e.ReCount).HasComment("實際完工數量");

                entity.Property(e => e.RePrice).HasComment("實際回報金額");

                entity.Property(e => e.Remarks)
                    .HasComment("備註	")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SerialNumber).HasComment("工序順序	");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.TotalTime).HasComment("總工時");

                entity.Property(e => e.Type).HasComment("種類");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkOrderHeadId).HasComment("工單ID");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.WorkOrderDetails)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("work_order_detail_ibfk_2");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.WorkOrderDetails)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("work_order_detail_ibfk_3");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.WorkOrderDetails)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("work_order_detail_ibfk_5");

                entity.HasOne(d => d.WorkOrderHead)
                    .WithMany(p => p.WorkOrderDetails)
                    .HasForeignKey(d => d.WorkOrderHeadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("work_order_detail_ibfk_4");
            });

            modelBuilder.Entity<WorkOrderHead>(entity =>
            {
                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("order_detail_id");

                entity.Property(e => e.ActualEndTime).HasComment("實際完工日");

                entity.Property(e => e.ActualStartTime).HasComment("實際開工日");

                entity.Property(e => e.Count).HasComment("數量");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DataId).HasComment("料號ID");

                entity.Property(e => e.DataName)
                    .HasComment("料號名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataNo)
                    .HasComment("料號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DataType).HasComment("料號種類(1原料 2成品 3 半成品)");

                entity.Property(e => e.DispatchTime).HasComment("派工時間");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DueEndTime).HasComment("預計完工日");

                entity.Property(e => e.DueStartTime).HasComment("預計開工日");

                entity.Property(e => e.MachineEndTime).HasComment("機台實際完工日");

                entity.Property(e => e.MachineNo)
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineStartTime).HasComment("機台實際開工日");

                entity.Property(e => e.OrderCount).HasComment("訂單數量");

                entity.Property(e => e.OrderDetailId).HasComment("訂單明細關聯");

                entity.Property(e => e.ReCount)
                    .HasDefaultValueSql("'0'")
                    .HasComment("實際完工數量");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.TotalTime).HasComment("總工時");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.WorkOrderNo)
                    .HasComment("工單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.WorkOrderHeads)
                    .HasForeignKey(d => d.OrderDetailId)
                    .HasConstraintName("work_order_head_ibfk_1");
            });

            modelBuilder.Entity<WorkOrderQcLog>(entity =>
            {
                entity.HasIndex(e => e.PurchaseHeadId)
                    .HasName("purchase_head_id");

                entity.HasIndex(e => e.SaleHeadId)
                    .HasName("sale_head_id");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.HasIndex(e => e.WorkOrderDetailId)
                    .HasName("work_order_detail_id");

                entity.HasIndex(e => e.WorkOrderHeadId)
                    .HasName("work_order_head_id");

                entity.Property(e => e.CheckResult).HasComment("檢驗結果(0合格 1不合格)");

                entity.Property(e => e.CkCount).HasComment("抽驗數量");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MCount).HasComment("可製造數量");

                entity.Property(e => e.Message)
                    .HasComment("回報說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NcCount).HasComment("NC未完工量");

                entity.Property(e => e.NgCount).HasComment("NG數量");

                entity.Property(e => e.OkCount).HasComment("OK數量");

                entity.Property(e => e.PurchaseHeadId).HasComment("採購單ID");

                entity.Property(e => e.ReCount).HasComment("回報數量");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ReportType).HasComment("回報種類");

                entity.Property(e => e.SaleHeadId).HasComment("採購單ID");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.WorkOrderDetailId).HasComment("工單明細ID");

                entity.Property(e => e.WorkOrderHeadId).HasComment("工單主檔Id");

                entity.HasOne(d => d.PurchaseHead)
                    .WithMany(p => p.WorkOrderQcLogs)
                    .HasForeignKey(d => d.PurchaseHeadId)
                    .HasConstraintName("work_order_qc_log_ibfk_1");

                entity.HasOne(d => d.SaleHead)
                    .WithMany(p => p.WorkOrderQcLogs)
                    .HasForeignKey(d => d.SaleHeadId)
                    .HasConstraintName("work_order_qc_log_ibfk_2");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.WorkOrderQcLogs)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("work_order_qc_log_ibfk_3");

                entity.HasOne(d => d.WorkOrderDetail)
                    .WithMany(p => p.WorkOrderQcLogs)
                    .HasForeignKey(d => d.WorkOrderDetailId)
                    .HasConstraintName("work_order_qc_log_ibfk_4");

                entity.HasOne(d => d.WorkOrderHead)
                    .WithMany(p => p.WorkOrderQcLogs)
                    .HasForeignKey(d => d.WorkOrderHeadId)
                    .HasConstraintName("work_order_qc_log_ibfk_5");
            });

            modelBuilder.Entity<WorkOrderReportLog>(entity =>
            {
                entity.HasIndex(e => e.PurchaseId)
                    .HasName("purchase_id");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("supplier_id");

                entity.HasIndex(e => e.WorkOrderDetailId)
                    .HasName("work_order_detail_id");

                entity.Property(e => e.ActualEndTime).HasComment("實際完工日");

                entity.Property(e => e.ActualStartTime).HasComment("實際開工日");

                entity.Property(e => e.CodeNo)
                    .HasComment("加工程式")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DrawNo)
                    .HasComment("圖號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.DueEndTime).HasComment("預計完工日");

                entity.Property(e => e.DueStartTime).HasComment("預計開工日");

                entity.Property(e => e.MCount).HasComment("可製造數量");

                entity.Property(e => e.MachineStartTime).HasComment("機台開工日");

                entity.Property(e => e.MachinelEndTime).HasComment("機台完工日");

                entity.Property(e => e.Manpower).HasComment("需求人力");

                entity.Property(e => e.Message)
                    .HasComment("回報說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NcCount).HasComment("NC未加工");

                entity.Property(e => e.NgCount).HasComment("NG數量");

                entity.Property(e => e.ProducingMachine)
                    .HasComment("加工機台")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProducingMachineId).HasComment("加工機台ID");

                entity.Property(e => e.PurchaseId).HasComment("採購單ID");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ReCount).HasComment("回報數量");

                entity.Property(e => e.RePrice).HasComment("實際回報金額");

                entity.Property(e => e.ReportType).HasComment("回報種類");

                entity.Property(e => e.StatusN).HasComment("更新狀態");

                entity.Property(e => e.StatusO).HasComment("上一個狀態");

                entity.Property(e => e.SupplierId).HasComment("供應商id");

                entity.Property(e => e.WorkOrderDetailId).HasComment("工單明細ID");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.WorkOrderReportLogs)
                    .HasForeignKey(d => d.PurchaseId)
                    .HasConstraintName("work_order_report_log_ibfk_2");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.WorkOrderReportLogs)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("work_order_report_log_ibfk_4");

                entity.HasOne(d => d.WorkOrderDetail)
                    .WithMany(p => p.WorkOrderReportLogs)
                    .HasForeignKey(d => d.WorkOrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("work_order_report_log_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
