﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("process")]
    public partial class Process
    {
        public Process()
        {
            MBillOfMaterials = new HashSet<MBillOfMaterial>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        [Column("lead_time", TypeName = "decimal(10,2)")]
        public decimal? LeadTime { get; set; }
        [Column("work_time", TypeName = "decimal(10,2)")]
        public decimal? WorkTime { get; set; }
        [Column("cost", TypeName = "decimal(10,2)")]
        public decimal? Cost { get; set; }
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        [Column("manpower", TypeName = "int(11)")]
        public int? Manpower { get; set; }
        [Column("producing_machine", TypeName = "varchar(50)")]
        public string ProducingMachine { get; set; }
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [InverseProperty(nameof(MBillOfMaterial.Process))]
        public virtual ICollection<MBillOfMaterial> MBillOfMaterials { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.Process))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
    }
}