using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ScaffoldingSample.Models;
using HonjiMES.Models;

namespace HonjiMES.Contexts
{
    public partial class HonjiContext : DbContext
    {
        public virtual DbSet<BillofPurchase> BillofPurchase { get; set; }
        public virtual DbSet<BillofPurchaseDetail> BillofPurchaseDetail { get; set; }
        public virtual DbSet<BillofPurchaseHead> BillofPurchaseHead { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<MaterialLog> MaterialLog { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderHead> OrderHead { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Process> Process { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductLog> ProductLog { get; set; }
        public virtual DbSet<ProductSn> ProductSn { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public virtual DbSet<PurchaseHead> PurchaseHead { get; set; }
        public virtual DbSet<ReturnSale> ReturnSale { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<SaleDetail> SaleDetail { get; set; }
        public virtual DbSet<SaleDetailNew> SaleDetailNew { get; set; }
        public virtual DbSet<SaleHead> SaleHead { get; set; }
        public virtual DbSet<SaleLog> SaleLog { get; set; }
        public virtual DbSet<Saleold> Saleold { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<System> System { get; set; }
        public virtual DbSet<UserLogs> UserLogs { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<WebSessions> WebSessions { get; set; }

        public HonjiContext(DbContextOptions<HonjiContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=127.0.0.1;user id=root;password=!QAZ2wsx3edc;port=33061;persistsecurityinfo=True;database=honji;convert zero datetime=True", x => x.ServerVersion("8.0.19-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillofPurchase>(entity =>
            {
                entity.ToTable("billof_purchase");

                entity.HasIndex(e => e.BillofPurchaseNo)
                    .HasName("billof_purchase_no");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.BillofPurchaseNo)
                    .IsRequired()
                    .HasColumnName("billof_purchase_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("進貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("新增者id");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("date")
                    .HasColumnType("varchar(50)")
                    .HasComment("進貨日期")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MaterialNo)
                    .HasColumnName("material_no")
                    .HasComment("品號");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("單價");

                entity.Property(e => e.PurchaseNo)
                    .IsRequired()
                    .HasColumnName("purchase_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("驗收/退數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .IsRequired()
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("varchar(50)")
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier)
                    .HasColumnName("supplier")
                    .HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<BillofPurchaseDetail>(entity =>
            {
                entity.ToTable("billof_purchase_detail");

                entity.HasIndex(e => e.BillofPurchaseId)
                    .HasName("fk_billof_purchase_detail_billof_purchase_head_idx");

                entity.HasIndex(e => e.PurchaseDetailId)
                    .HasName("fk_billof_purchase_detail_purchase_detail1_idx");

                entity.HasIndex(e => e.PurchaseId)
                    .HasName("fk_billof_purchase_detail_purchase_head1_idx");

                entity.HasIndex(e => e.SupplierId)
                    .HasName("fk_billof_purchase_detail_supplier1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BillofPurchaseId)
                    .HasColumnName("billof_purchase_id")
                    .HasComment("進貨單號");

                entity.Property(e => e.BillofPurchaseType)
                    .HasColumnName("billof_purchase_type")
                    .HasComment("進貨種類");

                entity.Property(e => e.CheckCountIn)
                    .HasColumnName("check_count_in")
                    .HasComment("驗收數量");

                entity.Property(e => e.CheckCountOut)
                    .HasColumnName("check_count_out")
                    .HasComment("驗退數量");

                entity.Property(e => e.CheckPriceIn)
                    .HasColumnName("check_price_in")
                    .HasComment("驗收金額");

                entity.Property(e => e.CheckPriceOut)
                    .HasColumnName("check_price_out")
                    .HasComment("驗退金額");

                entity.Property(e => e.CheckStatus)
                    .HasColumnName("check_status")
                    .HasComment("驗收狀態");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DataId)
                    .HasColumnName("data_id")
                    .HasComment("進貨內容ID");

                entity.Property(e => e.DataName)
                    .IsRequired()
                    .HasColumnName("data_name")
                    .HasColumnType("varchar(50)")
                    .HasComment("進貨內容名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DataNo)
                    .IsRequired()
                    .HasColumnName("data_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("進貨內容編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Delivered)
                    .HasColumnName("delivered")
                    .HasComment("實際交貨數");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice)
                    .HasColumnName("originPrice")
                    .HasComment("原單價	");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("價格");

                entity.Property(e => e.PurchaseCount)
                    .HasColumnName("purchase_count")
                    .HasComment("已開採購數量");

                entity.Property(e => e.PurchaseDetailId).HasColumnName("purchase_detail_id");

                entity.Property(e => e.PurchaseId)
                    .HasColumnName("purchase_id")
                    .HasComment("採購單id");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasComment("供應商id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.HasOne(d => d.BillofPurchase)
                    .WithMany(p => p.BillofPurchaseDetail)
                    .HasForeignKey(d => d.BillofPurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billof_purchase_detail_billof_purchase_head");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.BillofPurchaseDetail)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_billof_purchase_detail_supplier");
            });

            modelBuilder.Entity<BillofPurchaseHead>(entity =>
            {
                entity.ToTable("billof_purchase_head");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BillofPurchaseDate)
                    .HasColumnName("billof_purchase_date")
                    .HasColumnType("timestamp")
                    .HasComment("進貨日期");

                entity.Property(e => e.BillofPurchaseNo)
                    .IsRequired()
                    .HasColumnName("billof_purchase_no")
                    .HasColumnType("varchar(100)")
                    .HasComment("進貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CheckTime)
                    .HasColumnName("check_time")
                    .HasColumnType("timestamp");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

                entity.Property(e => e.PriceAll)
                    .HasColumnName("price_all")
                    .HasComment("總金額");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(100)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("進貨狀態");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .HasColumnName("account")
                    .HasColumnType("varchar(50)")
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("varchar(50)")
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Bank)
                    .HasColumnName("bank")
                    .HasColumnType("varchar(50)")
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Branch)
                    .HasColumnName("branch")
                    .HasColumnType("varchar(50)")
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(50)")
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(50)")
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fax)
                    .IsRequired()
                    .HasColumnName("fax")
                    .HasColumnType("varchar(50)")
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("varchar(50)")
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UniformNo)
                    .HasColumnName("uniform_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("material");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_material_warehouse1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.BaseQuantity)
                    .HasColumnName("base_quantity")
                    .HasComment("底數");

                entity.Property(e => e.Composition)
                    .HasColumnName("composition")
                    .HasComment("組成用量");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.MaterialNo)
                    .IsRequired()
                    .HasColumnName("material_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("元件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("元件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Property)
                    .IsRequired()
                    .HasColumnName("property")
                    .HasColumnType("varchar(50)")
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("庫存量");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SubInventory)
                    .IsRequired()
                    .HasColumnName("sub_inventory")
                    .HasColumnType("varchar(50)")
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier)
                    .HasColumnName("supplier")
                    .HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_warehouse1");
            });

            modelBuilder.Entity<MaterialLog>(entity =>
            {
                entity.ToTable("material_log");

                entity.HasIndex(e => e.MaterialId)
                    .HasName("fk_material_log_material1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("使用者id");

                entity.Property(e => e.MaterialId)
                    .HasColumnName("material_id")
                    .HasComment("原料ID");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("varchar(500)")
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Original)
                    .HasColumnName("original")
                    .HasComment("原始數量");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType("varchar(50)")
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialLog)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_material_log_material1");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("message");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.HasIndex(e => e.ProjectNo)
                    .HasName("project_no");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("建立人員");

                entity.Property(e => e.CustomerOrderNo)
                    .HasColumnName("customer_order_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Finish)
                    .HasColumnName("finish")
                    .HasColumnType("varchar(50)")
                    .HasComment("結案否")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MachineId)
                    .IsRequired()
                    .HasColumnName("machine_id")
                    .HasColumnType("varchar(50)")
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderDeliveryDate)
                    .HasColumnName("order_delivery_date")
                    .HasColumnType("date")
                    .HasComment("訂單交期");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("折後單價");

                entity.Property(e => e.ProductNo)
                    .HasColumnName("product_no")
                    .HasComment("主件品號");

                entity.Property(e => e.ProjectNo)
                    .IsRequired()
                    .HasColumnName("project_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("專案號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("varchar(50)")
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_detail");

                entity.HasIndex(e => e.OrderId)
                    .HasName("fk_order_order_detail");

                entity.HasIndex(e => e.ProductId)
                    .HasName("fk_order_detail_product1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Delivered)
                    .HasColumnName("delivered")
                    .HasComment("實際交貨數");

                entity.Property(e => e.Discount)
                    .HasColumnName("discount")
                    .HasComment("折扣率");

                entity.Property(e => e.DiscountPrice)
                    .HasColumnName("discount_price")
                    .HasComment("折後單價");

                entity.Property(e => e.Drawing)
                    .HasColumnName("drawing")
                    .HasColumnType("varchar(50)")
                    .HasComment("圖檔")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DueDate)
                    .HasColumnName("due_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("預交日")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Ink)
                    .HasColumnName("ink")
                    .HasColumnType("varchar(50)")
                    .HasComment("噴墨")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Label)
                    .HasColumnName("label")
                    .HasColumnType("varchar(50)")
                    .HasComment("標籤")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.MachineNo)
                    .IsRequired()
                    .HasColumnName("machine_no")
                    .HasColumnType("varchar(100)")
                    .HasComment("機號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasComment("訂單id");

                entity.Property(e => e.OriginPrice)
                    .HasColumnName("originPrice")
                    .HasComment("原單價");

                entity.Property(e => e.Package)
                    .HasColumnName("package")
                    .HasComment("包裝數");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("折後價格");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasComment("產品id");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Reply)
                    .HasColumnName("reply")
                    .HasComment("回覆量");

                entity.Property(e => e.ReplyDate)
                    .HasColumnName("reply_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("回復交期")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ReplyRemark)
                    .HasColumnName("replyRemark")
                    .HasColumnType("varchar(50)")
                    .HasComment("回覆備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleCount)
                    .HasColumnName("sale_count")
                    .HasComment("已銷貨數");

                entity.Property(e => e.Serial)
                    .HasColumnName("serial")
                    .HasComment("序號");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasColumnName("unit")
                    .HasColumnType("varchar(50)")
                    .HasComment("單位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_detail_order_head");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_detail_product");
            });

            modelBuilder.Entity<OrderHead>(entity =>
            {
                entity.ToTable("order_head");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("訂單id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("使用者id");

                entity.Property(e => e.Customer)
                    .HasColumnName("customer")
                    .HasComment("客戶id");

                entity.Property(e => e.CustomerNo)
                    .IsRequired()
                    .HasColumnName("customer_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FinishDate)
                    .HasColumnName("finish_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasComment("完成日期");

                entity.Property(e => e.OrderDate)
                    .HasColumnName("order_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("訂單日期")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.OrderNo)
                    .IsRequired()
                    .HasColumnName("order_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("訂單單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderType)
                    .HasColumnName("order_type")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasComment("開始日期");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permission");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CopyAdd)
                    .HasColumnName("copy_add")
                    .HasComment("複製新增");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Del)
                    .HasColumnName("del")
                    .HasComment("刪除");

                entity.Property(e => e.Edit)
                    .HasColumnName("edit")
                    .HasComment("修改");

                entity.Property(e => e.Export)
                    .HasColumnName("export")
                    .HasComment("匯出");

                entity.Property(e => e.Import)
                    .HasColumnName("import")
                    .HasComment("匯入");

                entity.Property(e => e.New)
                    .HasColumnName("new")
                    .HasComment("新增");

                entity.Property(e => e.Search)
                    .HasColumnName("search")
                    .HasComment("搜尋");

                entity.Property(e => e.Sort)
                    .HasColumnName("sort")
                    .HasComment("排序");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.ToTable("process");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(50)")
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("建立人員");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remark)
                    .HasColumnName("remark")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註欄位")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_product_warehouse1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

                entity.Property(e => e.MaterialId)
                    .HasColumnName("material_id")
                    .HasComment("元件品號");

                entity.Property(e => e.MaterialRequire)
                    .HasColumnName("material_require")
                    .HasComment("原料需求量");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("原價格");

                entity.Property(e => e.ProductNo)
                    .IsRequired()
                    .HasColumnName("product_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ProductNumber)
                    .IsRequired()
                    .HasColumnName("product_number")
                    .HasColumnType("varchar(50)")
                    .HasComment("廠內成品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Property)
                    .IsRequired()
                    .HasColumnName("property")
                    .HasColumnType("varchar(50)")
                    .HasComment("屬性")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("實際庫存數");

                entity.Property(e => e.QuantityAdv)
                    .HasColumnName("quantity_adv")
                    .HasComment("預先扣庫數量");

                entity.Property(e => e.QuantityLimit)
                    .HasColumnName("quantity_limit")
                    .HasComment("庫存極限");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SubInventory)
                    .IsRequired()
                    .HasColumnName("sub_inventory")
                    .HasColumnType("varchar(50)")
                    .HasComment("存放庫別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_warehouse1");
            });

            modelBuilder.Entity<ProductLog>(entity =>
            {
                entity.ToTable("product_log");

                entity.HasIndex(e => e.ProductId)
                    .HasName("fk_product_log_product1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("建立日期");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("使用者id");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("varchar(500)")
                    .HasComment("補充說明")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Original)
                    .HasColumnName("original")
                    .HasComment("原始數量");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasComment("成品ID");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("增減數量");

                entity.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType("varchar(50)")
                    .HasComment("修改原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductLog)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_log_product1");
            });

            modelBuilder.Entity<ProductSn>(entity =>
            {
                entity.ToTable("product_sn");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasComment("廠商");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasComment("品號(內部)");

                entity.Property(e => e.ProductNumber)
                    .IsRequired()
                    .HasColumnName("product_number")
                    .HasColumnType("varchar(100)")
                    .HasComment("品號(各廠商)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("purchase");

                entity.HasIndex(e => e.PurchaseNo)
                    .HasName("purchase_no");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.MaterialNo)
                    .HasColumnName("material_no")
                    .HasComment("元件品號");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("元件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("價格");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnName("purchase_date")
                    .HasColumnType("date")
                    .HasComment("採購日期");

                entity.Property(e => e.PurchaseNo)
                    .IsRequired()
                    .HasColumnName("purchase_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .IsRequired()
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Supplier)
                    .HasColumnName("supplier")
                    .HasComment("供應商");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<PurchaseDetail>(entity =>
            {
                entity.ToTable("purchase_detail");

                entity.HasIndex(e => e.PurchaseId)
                    .HasName("fk_purchase_detail_purchase_head");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DataId)
                    .HasColumnName("data_id")
                    .HasComment("採購內容ID");

                entity.Property(e => e.DataName)
                    .IsRequired()
                    .HasColumnName("data_name")
                    .HasColumnType("varchar(50)")
                    .HasComment("採購內容名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DataNo)
                    .IsRequired()
                    .HasColumnName("data_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("採購內容編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.DeliveryTime)
                    .HasColumnName("delivery_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("預計交期");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice)
                    .HasColumnName("originPrice")
                    .HasComment("原單價	");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("價格");

                entity.Property(e => e.PurchaseCount)
                    .HasColumnName("purchase_count")
                    .HasComment("進貨量");

                entity.Property(e => e.PurchaseId)
                    .HasColumnName("purchase_id")
                    .HasComment("採購單號");

                entity.Property(e => e.PurchaseType)
                    .HasColumnName("purchase_type")
                    .HasComment("採購種類");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasComment("供應商id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseDetail)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_purchase_detail_purchase_head");
            });

            modelBuilder.Entity<PurchaseHead>(entity =>
            {
                entity.ToTable("purchase_head");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

                entity.Property(e => e.PriceAll)
                    .HasColumnName("price_all")
                    .HasComment("總金額");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnName("purchase_date")
                    .HasColumnType("timestamp")
                    .HasComment("採購日期");

                entity.Property(e => e.PurchaseNo)
                    .IsRequired()
                    .HasColumnName("purchase_no")
                    .HasColumnType("varchar(100)")
                    .HasComment("採購單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(100)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("採購狀態");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasComment("供應商id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<ReturnSale>(entity =>
            {
                entity.ToTable("return_sale");

                entity.HasComment("退貨記錄");

                entity.HasIndex(e => e.SaleDetailNewId)
                    .HasName("fk_return_sale_sale_detail_new1_idx");

                entity.HasIndex(e => e.WarehouseId)
                    .HasName("fk_return_sale_warehouse1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("退貨數量");

                entity.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasColumnType("varchar(50)")
                    .HasComment("原因")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(500)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleDetailNewId).HasColumnName("sale_detail_new_id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.WarehouseId)
                    .HasColumnName("warehouse_id")
                    .HasComment("退貨倉庫ID");

                entity.HasOne(d => d.SaleDetailNew)
                    .WithMany(p => p.ReturnSale)
                    .HasForeignKey(d => d.SaleDetailNewId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_return_sale_sale_detail_new1");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.ReturnSale)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_return_sale_warehouse1");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("sale");

                entity.HasIndex(e => e.SaleNo)
                    .HasName("sale_no");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Customer)
                    .IsRequired()
                    .HasColumnName("customer")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CustomerNo)
                    .IsRequired()
                    .HasColumnName("customer_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("單價");

                entity.Property(e => e.ProductNo)
                    .HasColumnName("product_no")
                    .HasComment("主件品號");

                entity.Property(e => e.ProjectNo)
                    .HasColumnName("project_no")
                    .HasComment("專案號");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("date")
                    .HasComment("銷貨日期");

                entity.Property(e => e.SaleNo)
                    .IsRequired()
                    .HasColumnName("sale_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .IsRequired()
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("varchar(50)")
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.ToTable("sale_detail");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice)
                    .HasColumnName("originPrice")
                    .HasComment("原單價	");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("價格");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasComment("主件品號ID");

                entity.Property(e => e.ProductNo)
                    .IsRequired()
                    .HasColumnName("product_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleId)
                    .HasColumnName("sale_id")
                    .HasComment("銷貨單號");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<SaleDetailNew>(entity =>
            {
                entity.ToTable("sale_detail_new");

                entity.HasComment("銷貨明細");

                entity.HasIndex(e => e.OrderDetailId)
                    .HasName("fk_sale_detail_new_order_detail1_idx");

                entity.HasIndex(e => e.OrderId)
                    .HasName("fk_sale_detail_new_order_head1_idx");

                entity.HasIndex(e => e.ProductId)
                    .HasName("fk_sale_detail_new_product1_idx");

                entity.HasIndex(e => e.SaleId)
                    .HasName("fk_sale_detail_new_sale_head1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.OrderDetailId)
                    .HasColumnName("order_detail_id")
                    .HasComment("訂單內容唯一碼");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasComment("訂單單號id");

                entity.Property(e => e.OriginPrice)
                    .HasColumnName("originPrice")
                    .HasComment("原單價	");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("價格");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasComment("主件品號ID");

                entity.Property(e => e.ProductNo)
                    .IsRequired()
                    .HasColumnName("product_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleId)
                    .HasColumnName("sale_id")
                    .HasComment("銷貨單號");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("銷貨狀態");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.SaleDetailNew)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_order_detail");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.SaleDetailNew)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_order_head");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SaleDetailNew)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_product");

                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleDetailNew)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sale_detail_new_sale_head");
            });

            modelBuilder.Entity<SaleHead>(entity =>
            {
                entity.ToTable("sale_head");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

                entity.Property(e => e.PriceAll)
                    .HasColumnName("price_all")
                    .HasComment("總金額");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(100)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("timestamp")
                    .HasComment("銷或日期");

                entity.Property(e => e.SaleNo)
                    .IsRequired()
                    .HasColumnName("sale_no")
                    .HasColumnType("varchar(100)")
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("銷貨狀態");

                entity.Property(e => e.Temp).HasColumnName("temp");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<SaleLog>(entity =>
            {
                entity.ToTable("sale_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleId)
                    .HasColumnName("sale_id")
                    .HasComment("銷貨單id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasComment("log種類");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<Saleold>(entity =>
            {
                entity.ToTable("saleold");

                entity.HasIndex(e => e.SaleNo)
                    .HasName("sale_no");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Customer)
                    .IsRequired()
                    .HasColumnName("customer")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CustomerNo)
                    .IsRequired()
                    .HasColumnName("customer_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("客戶單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("主件品名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasComment("單價");

                entity.Property(e => e.ProductNo)
                    .HasColumnName("product_no")
                    .HasComment("主件品號");

                entity.Property(e => e.ProjectNo)
                    .HasColumnName("project_no")
                    .HasComment("專案號");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasComment("數量");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("date")
                    .HasComment("銷貨日期");

                entity.Property(e => e.SaleNo)
                    .IsRequired()
                    .HasColumnName("sale_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("銷貨單號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Specification)
                    .IsRequired()
                    .HasColumnName("specification")
                    .HasColumnType("varchar(50)")
                    .HasComment("規格")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("varchar(50)")
                    .HasComment("狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("supplier");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasColumnName("account")
                    .HasColumnType("varchar(50)")
                    .HasComment("銀行帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("varchar(50)")
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Bank)
                    .IsRequired()
                    .HasColumnName("bank")
                    .HasColumnType("varchar(50)")
                    .HasComment("收款銀行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Branch)
                    .IsRequired()
                    .HasColumnName("branch")
                    .HasColumnType("varchar(50)")
                    .HasComment("分行")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(50)")
                    .HasComment("代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ContactName)
                    .HasColumnName("contact_name")
                    .HasColumnType("varchar(100)")
                    .HasComment("聯絡人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(50)")
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fax)
                    .IsRequired()
                    .HasColumnName("fax")
                    .HasColumnType("varchar(50)")
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("供應商")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("varchar(50)")
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UniformNo)
                    .IsRequired()
                    .HasColumnName("uniform_no")
                    .HasColumnType("varchar(50)")
                    .HasComment("統一編號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<System>(entity =>
            {
                entity.ToTable("system");

                entity.HasComment("系統參數");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("新增者id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("功能名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("varchar(50)")
                    .HasComment("值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<UserLogs>(entity =>
            {
                entity.ToTable("user_logs");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_logs_ibfk_1");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("新增者id");

                entity.Property(e => e.LoginTime)
                    .HasColumnName("login_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("登入時間");

                entity.Property(e => e.LogoutTime)
                    .HasColumnName("logout_time")
                    .HasColumnType("timestamp")
                    .HasComment("登出時間");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasComment("使用者ID");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasColumnName("department")
                    .HasColumnType("varchar(50)")
                    .HasComment("部門")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("tinytext")
                    .HasComment("密碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Permission)
                    .IsRequired()
                    .HasColumnName("permission")
                    .HasColumnType("varchar(50)")
                    .HasComment("身分別")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Realname)
                    .IsRequired()
                    .HasColumnName("realname")
                    .HasColumnType("varchar(50)")
                    .HasComment("姓名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(50)")
                    .HasComment("帳號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("warehouse");

                entity.HasComment("倉庫");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasComment("唯一碼");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("varchar(50)")
                    .HasComment("地址")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("varchar(50)")
                    .HasComment("內部代碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Contact)
                    .HasColumnName("contact")
                    .HasColumnType("varchar(50)")
                    .HasComment("連絡人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreateUser).HasColumnName("create_user");

                entity.Property(e => e.DeleteFlag)
                    .HasColumnName("delete_flag")
                    .HasComment("刪除註記");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(50)")
                    .HasComment("電子郵件")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasColumnType("varchar(50)")
                    .HasComment("傳真")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasComment("倉庫名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(50)")
                    .HasComment("電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Recheck)
                    .HasColumnName("recheck")
                    .HasComment("是否要產品檢查,不檢查直接回存倉庫");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasColumnType("varchar(50)")
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp");

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");
            });

            modelBuilder.Entity<WebSessions>(entity =>
            {
                entity.ToTable("web_sessions");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("web_sessions_timestamp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("新增時間");

                entity.Property(e => e.CreateUser)
                    .HasColumnName("create_user")
                    .HasComment("新增者id");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("blob");

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasColumnName("ip_address")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Timestamp).HasColumnName("timestamp");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser)
                    .HasColumnName("update_user")
                    .HasComment("更新者id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
