﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HonjiMES.CncModels
{
    public partial class HonjiCncContext : DbContext
    {
        public virtual DbSet<CncModel> CncModels { get; set; }
        public virtual DbSet<ControlBrand> ControlBrands { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<MachineCurrentStatus> MachineCurrentStatuses { get; set; }
        public virtual DbSet<MachineGroup> MachineGroups { get; set; }
        public virtual DbSet<MachineGroupMachineInformation> MachineGroupMachineInformations { get; set; }
        public virtual DbSet<MachineInformation> MachineInformations { get; set; }
        public virtual DbSet<MachineLog> MachineLogs { get; set; }
        public virtual DbSet<Maintain> Maintains { get; set; }
        public virtual DbSet<MaintainInfo> MaintainInfos { get; set; }
        public virtual DbSet<NcFileInformation> NcFileInformations { get; set; }
        public virtual DbSet<NcFileLog> NcFileLogs { get; set; }
        public virtual DbSet<OneDayDatum> OneDayData { get; set; }
        public virtual DbSet<OperateLog> OperateLogs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessRule> ProcessRules { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseInfo> PurchaseInfos { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<System> Systems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<WebSession> WebSessions { get; set; }
        public virtual DbSet<WorkTimeGroup> WorkTimeGroups { get; set; }
        public virtual DbSet<WorkTimeSpec> WorkTimeSpecs { get; set; }

        public HonjiCncContext(DbContextOptions<HonjiCncContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CncModel>(entity =>
            {
                entity.HasComment("機台種類");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Name)
                    .HasComment("機台種類名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<ControlBrand>(entity =>
            {
                entity.HasComment("控制器種類");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Name)
                    .HasComment("控制器名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.DepartmentName)
                    .HasComment("部門名稱")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<MachineCurrentStatus>(entity =>
            {
                entity.HasNoKey();

                entity.HasComment("機台目前狀況");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.Property(e => e.ActullySpindleFeedrate)
                    .HasDefaultValueSql("'0'")
                    .HasComment("實際進給率")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ActullySpindleSpeed)
                    .HasDefaultValueSql("'0'")
                    .HasComment("實際轉速")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CycleTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("循環時間")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.MachineAxis)
                    .HasComment("機械座標JSON")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NcComment)
                    .HasDefaultValueSql("'()'")
                    .HasComment("程式註解")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NcName)
                    .HasComment("檔案名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.PartsCount)
                    .HasDefaultValueSql("'0'")
                    .HasComment("加工部品數");

                entity.Property(e => e.RunTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("總運轉時間")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.SpindleToolNumber)
                    .HasDefaultValueSql("'0'")
                    .HasComment("主軸刀號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasComment("機台狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間");
            });

            modelBuilder.Entity<MachineGroup>(entity =>
            {
                entity.HasComment("機台群組");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Latitude).HasComment("緯度");

                entity.Property(e => e.Longitude).HasComment("經度");

                entity.Property(e => e.Name)
                    .HasComment("機台群組名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<MachineGroupMachineInformation>(entity =>
            {
                entity.HasComment("機台群組ID與機台ID組合");

                entity.HasIndex(e => e.MachineGroupId)
                    .HasName("machine_group__machine_information_ibfk_2");

                entity.HasIndex(e => e.MachineId)
                    .HasName("machine_group__machine_information_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.MachineGroupId).HasComment("機台群組ID");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.HasOne(d => d.MachineGroup)
                    .WithMany(p => p.MachineGroupMachineInformations)
                    .HasForeignKey(d => d.MachineGroupId)
                    .HasConstraintName("machine_group__machine_information_ibfk_2");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.MachineGroupMachineInformations)
                    .HasForeignKey(d => d.MachineId)
                    .HasConstraintName("machine_group__machine_information_ibfk_1");
            });

            modelBuilder.Entity<MachineInformation>(entity =>
            {
                entity.HasComment("機台資訊");

                entity.HasIndex(e => e.ControlBrandId)
                    .HasName("control_brand_id");

                entity.HasIndex(e => e.ModelId)
                    .HasName("model_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

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

                entity.HasOne(d => d.ControlBrand)
                    .WithMany(p => p.MachineInformations)
                    .HasForeignKey(d => d.ControlBrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("machine_informations_ibfk_2");

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.MachineInformations)
                    .HasForeignKey(d => d.ModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("machine_informations_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MachineInformations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("machine_informations_ibfk_3");
            });

            modelBuilder.Entity<MachineLog>(entity =>
            {
                entity.HasComment("機台狀態紀錄");

                entity.HasIndex(e => e.FileId)
                    .HasName("file_id");

                entity.HasIndex(e => e.MachineId)
                    .HasName("machine_logs_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.ActionTime).HasComment("加工時間");

                entity.Property(e => e.AlarmMessage)
                    .HasComment("警報訊息")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.AlarmNumber)
                    .HasComment("警報號碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CompletedNumber)
                    .HasDefaultValueSql("'0'")
                    .HasComment("加工部品數");

                entity.Property(e => e.DurationTime).HasComment("持續時間");

                entity.Property(e => e.EndTime).HasComment("結束時間");

                entity.Property(e => e.FileId).HasComment("檔案ID");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.MoveTime).HasComment("移動時間");

                entity.Property(e => e.StartTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("開始時間");

                entity.Property(e => e.Status).HasComment("機台狀態");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.MachineLogs)
                    .HasForeignKey(d => d.MachineId)
                    .HasConstraintName("machine_logs_ibfk_1");
            });

            modelBuilder.Entity<Maintain>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Cycle).HasComment("週期");

                entity.Property(e => e.Email1)
                    .HasComment("E-mail 1")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Email2)
                    .HasComment("E-mail 2")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Email3)
                    .HasComment("E-mail 3")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.MachineId).HasComment("機台名稱");

                entity.Property(e => e.MaintainFinishTime).HasComment("保養结束時間");

                entity.Property(e => e.MaintainItem).HasComment("保養項目");

                entity.Property(e => e.MaintainTime).HasComment("保養時間");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'1'")
                    .HasComment("狀態");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<MaintainInfo>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteFlag).HasComment("刪除標示");

                entity.Property(e => e.MaintainName)
                    .HasComment("保養名稱")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .HasDefaultValueSql("'DEFAULT'")
                    .HasComment("分類")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<NcFileInformation>(entity =>
            {
                entity.HasComment("加工程式資訊");

                entity.HasIndex(e => e.Source)
                    .HasName("nc_file_informations_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Comment)
                    .HasDefaultValueSql("'()'")
                    .HasComment("程式註解")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.FileCreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("檔案建立時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .HasComment("檔案名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Size)
                    .HasComment("檔案大小")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Source).HasComment("檔案來源");

                entity.Property(e => e.Tag)
                    .HasComment("標籤JSON")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.SourceNavigation)
                    .WithMany(p => p.NcFileInformations)
                    .HasForeignKey(d => d.Source)
                    .HasConstraintName("nc_file_informations_ibfk_1");
            });

            modelBuilder.Entity<NcFileLog>(entity =>
            {
                entity.HasComment("加工程式紀錄");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Action)
                    .HasComment("執行動作")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.FileId).HasComment("檔案ID");

                entity.Property(e => e.From).HasComment("從哪個地方");

                entity.Property(e => e.Message)
                    .HasComment("訊息")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.NcCheck)
                    .HasDefaultValueSql("'0'")
                    .HasComment("程式確認");

                entity.Property(e => e.Path)
                    .HasDefaultValueSql("''")
                    .HasComment("到的路徑")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.To).HasComment("到哪個地方");

                entity.Property(e => e.UserId).HasComment("使用者ID");
            });

            modelBuilder.Entity<OneDayDatum>(entity =>
            {
                entity.HasComment("加工程式紀錄");

                entity.HasIndex(e => e.MachineId)
                    .HasName("one_day_data_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AlarmCount)
                    .HasDefaultValueSql("'0'")
                    .HasComment("警報次數");

                entity.Property(e => e.AlarmTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("警報時間");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Date)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("日期");

                entity.Property(e => e.IdleTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("待機時間");

                entity.Property(e => e.MachineId).HasComment("機台ID");

                entity.Property(e => e.OfflineTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("關機時間");

                entity.Property(e => e.PartsCount)
                    .HasDefaultValueSql("'0'")
                    .HasComment("加工部品數");

                entity.Property(e => e.RunTime)
                    .HasDefaultValueSql("'0'")
                    .HasComment("運轉時間");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.OneDayData)
                    .HasForeignKey(d => d.MachineId)
                    .HasConstraintName("one_day_data_ibfk_1");
            });

            modelBuilder.Entity<OperateLog>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("操作時間");

                entity.Property(e => e.OpActionType).HasComment("1:insert 2:update 3:delete 4:disable/enable");

                entity.Property(e => e.OpTableName)
                    .HasComment("操作的資料表")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Operator).HasComment("操作人員");

                entity.Property(e => e.RelativeId).HasComment("資料列id");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.BusinessId)
                    .HasName("business_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.BusinessId)
                    .HasComment("業務號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Dimension)
                    .HasComment("進料尺寸")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Material)
                    .HasComment("材質")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Name)
                    .HasComment("名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.OrderDeliveryTime).HasComment("訂單交期");

                entity.Property(e => e.PartNo)
                    .HasComment("料號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.ProcessStatus)
                    .HasDefaultValueSql("'0'")
                    .HasComment("是否開工(1:YES)");

                entity.Property(e => e.Quantity).HasComment("數量");

                entity.Property(e => e.Status)
                    .HasComment("訂單狀態")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DisplayName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PType)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.Department).HasComment("部門");

                entity.Property(e => e.Description)
                    .HasComment("敘述")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastUpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.ProcessAlias)
                    .HasComment("代號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ProcessName)
                    .HasComment("名稱")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Property).HasComment("性質 1:廠內 2:委外");

                entity.Property(e => e.StandardTime)
                    .HasComment("標準工時")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'1'")
                    .HasComment("啟用狀態0:停用1:啟用");

                entity.Property(e => e.Visible)
                    .HasDefaultValueSql("'1'")
                    .HasComment("讓使用者不再看到");
            });

            modelBuilder.Entity<ProcessRule>(entity =>
            {
                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.ActualFinishTime).HasComment("實際完工日");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Department).HasComment("部門");

                entity.Property(e => e.FinishTime).HasComment("預計完工日");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("金額");

                entity.Property(e => e.ProcessName)
                    .HasComment("製程名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Property).HasComment("性質 1:廠內 2:委外 ");

                entity.Property(e => e.StandardTime)
                    .HasComment("工時")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.PropertyName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.BusinessId)
                    .HasComment("業務號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Cost).HasComment("總計");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("建立時間");

                entity.Property(e => e.CreateUser).HasComment("建立人員");

                entity.Property(e => e.DeliveryTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("預交日期");

                entity.Property(e => e.Fax)
                    .HasComment("傳真")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PartNo)
                    .HasComment("料號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PurchaseNo)
                    .HasComment("採購單號")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status).HasComment("狀態");

                entity.Property(e => e.Tel)
                    .HasComment("電話")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新人員")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.UpdateUser).HasComment("更新人員");

                entity.Property(e => e.VendorId).HasComment("廠商代號");
            });

            modelBuilder.Entity<PurchaseInfo>(entity =>
            {
                entity.Property(e => e.Count).HasComment("數量");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Name)
                    .HasComment("規格名稱")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price).HasComment("單價");

                entity.Property(e => e.PriceAll).HasComment("金額");

                entity.Property(e => e.PurchaseId).HasComment("採購單id");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Unit)
                    .HasComment("單位")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.StatusName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<System>(entity =>
            {
                entity.HasComment("系統參數");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.Name)
                    .HasComment("功能名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Value)
                    .HasComment("值")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("使用者");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.AuthAction)
                    .HasComment("授權行為")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Department).HasComment("部門");

                entity.Property(e => e.Password)
                    .HasComment("密碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Permission).HasComment("權限");

                entity.Property(e => e.Realname)
                    .HasComment("真實姓名")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'1'")
                    .HasComment("啟用狀態0:停用1:啟用 ");

                entity.Property(e => e.Username)
                    .HasComment("使用者名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.HasComment("使用者紀錄");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_logs_ibfk_1");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.LoginTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("登入時間");

                entity.Property(e => e.LogoutTime).HasComment("登出時間");

                entity.Property(e => e.UserId).HasComment("使用者ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_logs_ibfk_1");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasIndex(e => e.VendorNo)
                    .HasName("business_id");

                entity.Property(e => e.Id).HasComment("唯一碼");

                entity.Property(e => e.ContactName)
                    .HasComment("聯絡人")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.CreateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("新增時間");

                entity.Property(e => e.Fax)
                    .HasComment("傳真號碼")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Mail)
                    .HasComment("信箱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Remark)
                    .HasComment("備註")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Tel)
                    .HasComment("連絡電話")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.UpdateTime)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasComment("更新時間")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.VendorData)
                    .HasComment("廠商作業內容")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.VendorName)
                    .HasComment("廠商名稱")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.VendorNo)
                    .HasComment("廠商代號")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Visible)
                    .HasDefaultValueSql("'1'")
                    .HasComment("讓使用者不再看到 ");
            });

            modelBuilder.Entity<WebSession>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.Timestamp)
                    .HasName("web_sessions_timestamp");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.IpAddress)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");
            });

            modelBuilder.Entity<WorkTimeGroup>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.GroupName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<WorkTimeSpec>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.TimeType)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Value)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}