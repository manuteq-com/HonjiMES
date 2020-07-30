﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("work_order_head")]
    public partial class WorkOrderHead
    {
        public WorkOrderHead()
        {
            Requisitions = new HashSet<Requisition>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("work_order_no", TypeName = "varchar(50)")]
        public string WorkOrderNo { get; set; }
        [Column("order_detail_id", TypeName = "int(11)")]
        public int? OrderDetailId { get; set; }
        [Column("machine_no", TypeName = "varchar(50)")]
        public string MachineNo { get; set; }
        [Column("data_type", TypeName = "int(11)")]
        public int DataType { get; set; }
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        [Column("count", TypeName = "int(11)")]
        public int Count { get; set; }
        [Column("re_count", TypeName = "int(11)")]
        public int? ReCount { get; set; }
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        [Column("total_time", TypeName = "timestamp")]
        public DateTime? TotalTime { get; set; }
        [Column("dispatch_time", TypeName = "timestamp")]
        public DateTime? DispatchTime { get; set; }
        [Column("due_start_time", TypeName = "timestamp")]
        public DateTime? DueStartTime { get; set; }
        [Column("due_end_time", TypeName = "timestamp")]
        public DateTime? DueEndTime { get; set; }
        [Column("actual_start_time", TypeName = "timestamp")]
        public DateTime? ActualStartTime { get; set; }
        [Column("actual_end_time", TypeName = "timestamp")]
        public DateTime? ActualEndTime { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("WorkOrderHeads")]
        public virtual OrderDetail OrderDetail { get; set; }
        [InverseProperty(nameof(Requisition.WorkOrderHead))]
        public virtual ICollection<Requisition> Requisitions { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.WorkOrderHead))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
    }
}