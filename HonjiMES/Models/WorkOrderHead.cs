﻿using System;
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
            OrderDetailAndWorkOrderHeads = new HashSet<OrderDetailAndWorkOrderHead>();
            Requisitions = new HashSet<Requisition>();
            StaffManagements = new HashSet<StaffManagement>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
            WorkOrderQcLogs = new HashSet<WorkOrderQcLog>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("work_order_no", TypeName = "varchar(50)")]
        public string WorkOrderNo { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#26126;&#32048;&#38364;&#32879;
        /// </summary>
        [Column("order_detail_id", TypeName = "int(11)")]
        public int? OrderDetailId { get; set; }
        /// <summary>
        /// &#27231;&#34399;
        /// </summary>
        [Column("machine_no", TypeName = "varchar(50)")]
        public string MachineNo { get; set; }
        /// <summary>
        /// &#22294;&#34399;
        /// </summary>
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#31278;&#39006;(1&#21407;&#26009; 2&#25104;&#21697; 3 &#21322;&#25104;&#21697;)
        /// </summary>
        [Column("data_type", TypeName = "int(11)")]
        public int? DataType { get; set; }
        /// <summary>
        /// &#26009;&#34399;ID
        /// </summary>
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        /// <summary>
        /// &#26009;&#34399;
        /// </summary>
        [Required]
        [Column("data_no", TypeName = "varchar(50)")]
        public string DataNo { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#21517;&#31281;
        /// </summary>
        [Column("data_name", TypeName = "varchar(50)")]
        public string DataName { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("count", TypeName = "int(11)")]
        public int Count { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#25976;&#37327;
        /// </summary>
        [Column("order_count", TypeName = "int(11)")]
        public int? OrderCount { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#23436;&#24037;&#25976;&#37327;
        /// </summary>
        [Column("re_count", TypeName = "int(11)")]
        public int? ReCount { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#32317;&#24037;&#26178;
        /// </summary>
        [Column("total_time", TypeName = "timestamp")]
        public DateTime? TotalTime { get; set; }
        /// <summary>
        /// &#27966;&#24037;&#26178;&#38291;
        /// </summary>
        [Column("dispatch_time", TypeName = "timestamp")]
        public DateTime? DispatchTime { get; set; }
        /// <summary>
        /// &#38928;&#35336;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("due_start_time", TypeName = "timestamp")]
        public DateTime? DueStartTime { get; set; }
        /// <summary>
        /// &#38928;&#35336;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("due_end_time", TypeName = "timestamp")]
        public DateTime? DueEndTime { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("actual_start_time", TypeName = "timestamp")]
        public DateTime? ActualStartTime { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("actual_end_time", TypeName = "timestamp")]
        public DateTime? ActualEndTime { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#23526;&#38555;&#38283;&#24037;&#26085;
        /// </summary>
        [Column("machine_start_time", TypeName = "timestamp")]
        public DateTime? MachineStartTime { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#23526;&#38555;&#23436;&#24037;&#26085;
        /// </summary>
        [Column("machine_end_time", TypeName = "timestamp")]
        public DateTime? MachineEndTime { get; set; }
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
        [InverseProperty(nameof(OrderDetailAndWorkOrderHead.WorkHead))]
        public virtual ICollection<OrderDetailAndWorkOrderHead> OrderDetailAndWorkOrderHeads { get; set; }
        [InverseProperty(nameof(Requisition.WorkOrderHead))]
        public virtual ICollection<Requisition> Requisitions { get; set; }
        [InverseProperty(nameof(StaffManagement.WorkOrder))]
        public virtual ICollection<StaffManagement> StaffManagements { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.WorkOrderHead))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
        [InverseProperty(nameof(WorkOrderQcLog.WorkOrderHead))]
        public virtual ICollection<WorkOrderQcLog> WorkOrderQcLogs { get; set; }
    }
}
