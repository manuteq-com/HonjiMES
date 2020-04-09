﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HonjiMES.Models
{
    public partial class HonjiContext : DbContext
    {
        public virtual DbSet<BillofPurchase> BillofPurchases { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<MaterialLog> MaterialLogs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderHead> OrderHeads { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductLog> ProductLogs { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<System> Systems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<WebSession> WebSessions { get; set; }

        public HonjiContext(DbContextOptions<HonjiContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillofPurchase>(entity =>
            {
                entity.HasIndex(e => e.BillofPurchaseNo)
                    .HasName("billof_purchase_no");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BillofPurchaseNo)
                    .HasComment("進貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.Date)
                    .HasComment("進貨日期")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MaterialNo).HasComment("品號");

                entity.Property(e => e.Name)
                    .HasComment("品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("驗收/退數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Address)
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Bank)
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Branch)
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Email)
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UniformNo)
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BaseQuantity).HasComment("底數");

                entity.Property(e => e.Composition).HasComment("組成用量");

                entity.Property(e => e.MaterialNo)
                    .HasComment("元件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasComment("元件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("庫存量");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");
            });

            modelBuilder.Entity<MaterialLog>(entity =>
            {
                entity.HasIndex(e => e.MaterialId)
                    .HasName("fk_material_log_material1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.MaterialId).HasComment("原料ID");

                entity.Property(e => e.Message)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialLogs)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_log_material1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.ProjectNo)
                    .HasName("project_no");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.CustomerOrderNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Finish)
                    .HasComment("結案否")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MachineId)
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderDeliveryDate).HasComment("訂單交期");

                entity.Property(e => e.Price).HasComment("折後單價");

                entity.Property(e => e.ProductNo).HasComment("主件品號");

                entity.Property(e => e.ProjectNo)
                    .HasComment("專案號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Status)
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("fk_order_order_detail");

                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.Delivered).HasComment("已交");

                entity.Property(e => e.Discount).HasComment("折扣率");

                entity.Property(e => e.DiscountPrice).HasComment("折後單價");

                entity.Property(e => e.Drawing)
                    .HasComment("圖檔")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DueDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("預交日");

                entity.Property(e => e.Ink)
                    .HasComment("噴墨")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Label)
                    .HasComment("標籤")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MachineNo).HasComment("機號");

                entity.Property(e => e.OrderId).HasComment("訂單id");

                entity.Property(e => e.OriginPrice).HasComment("原單價");

                entity.Property(e => e.Package).HasComment("包裝數");

                entity.Property(e => e.Price).HasComment("折後價格");

                entity.Property(e => e.ProductId).HasComment("產品id");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Reply).HasComment("回覆量");

                entity.Property(e => e.ReplyDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("回復交期")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ReplyRemark)
                    .HasComment("回覆備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Serial).HasComment("序號");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_order_detail");
            });

            modelBuilder.Entity<OrderHead>(entity =>
            {
                entity.Property(e => e.Id).HasComment("訂單id");

                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.Customer).HasComment("客戶id");

                entity.Property(e => e.CustomerNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FinishDate)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasComment("完成日期");

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("訂單日期")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.OrderNo)
                    .HasComment("訂單單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderType)
                    .HasComment("訂單類型")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StartDate)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasComment("開始日期");

                entity.Property(e => e.Status).HasComment("訂單狀態");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CopyAdd).HasComment("複製新增");

                entity.Property(e => e.Del).HasComment("刪除");

                entity.Property(e => e.Edit).HasComment("修改");

                entity.Property(e => e.Export).HasComment("匯出");

                entity.Property(e => e.Import).HasComment("匯入");

                entity.Property(e => e.New).HasComment("新增");

                entity.Property(e => e.Search).HasComment("搜尋");

                entity.Property(e => e.Sort).HasComment("排序");
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.Property(e => e.Id).HasComment("id");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remark)
                    .HasComment("備註欄位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.MaterialId).HasComment("元件品號");

                entity.Property(e => e.MaterialRequire).HasComment("原料需求量");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price).HasComment("原價格");

                entity.Property(e => e.ProductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ProductNumber)
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Property)
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("庫存量");

                entity.Property(e => e.QuantityLimit).HasComment("庫存極限");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SubInventory)
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<ProductLog>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasName("fk_product_log_product1_idx");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasComment("使用者id");

                entity.Property(e => e.Message)
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Original).HasComment("原始數量");

                entity.Property(e => e.ProductId).HasComment("成品ID");

                entity.Property(e => e.Quantity).HasComment("增減數量");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductLogs)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_log_product1");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasIndex(e => e.PurchaseNo)
                    .HasName("purchase_no");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.MaterialNo).HasComment("元件品號");

                entity.Property(e => e.Name)
                    .HasComment("元件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price).HasComment("價格");

                entity.Property(e => e.PurchaseDate).HasComment("採購日期");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier).HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasIndex(e => e.SaleNo)
                    .HasName("sale_no");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.Customer)
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CustomerNo)
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.ProductNo).HasComment("主件品號");

                entity.Property(e => e.ProjectNo).HasComment("專案號");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleDate).HasComment("銷貨日期");

                entity.Property(e => e.SaleNo)
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Name)
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.ProductNo)
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleNo)
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Address)
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Bank)
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Branch)
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Code)
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Email)
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasComment("供應商")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UniformNo)
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<System>(entity =>
            {
                entity.HasComment("系統參數");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Name)
                    .HasComment("功能名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Value)
                    .HasComment("值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.Department)
                    .HasComment("部門")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Password)
                    .HasComment("密碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Permission)
                    .HasComment("身分別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Realname)
                    .HasComment("姓名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Username)
                    .HasComment("帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("user_logs_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.LoginTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("登入時間");

                entity.Property(e => e.LogoutTime).HasComment("登出時間");

                entity.Property(e => e.UserId).HasComment("使用者ID");
            });

            modelBuilder.Entity<WebSession>(entity =>
            {
                entity.HasIndex(e => e.Timestamp)
                    .HasName("web_sessions_timestamp");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.IpAddress)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}