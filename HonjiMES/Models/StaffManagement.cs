﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#20154;&#21729;&#25490;&#29677;
    /// </summary>
    [Table("staff_management")]
    public partial class StaffManagement
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20154;&#21729;ID
        /// </summary>
        [Column("user_id", TypeName = "int(11)")]
        public int UserId { get; set; }
        /// <summary>
        /// &#27231;&#21488;ID
        /// </summary>
        [Column("machine_id", TypeName = "int(11)")]
        public int MachineId { get; set; }
        /// <summary>
        /// &#24037;&#21934;ID
        /// </summary>
        [Column("work_order_id", TypeName = "int(11)")]
        public int WorkOrderId { get; set; }
        /// <summary>
        /// &#24037;&#24207;ID
        /// </summary>
        [Column("process_id", TypeName = "int(11)")]
        public int ProcessId { get; set; }
        /// <summary>
        /// &#38283;&#22987;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#32080;&#26463;&#26178;&#38291;
        /// </summary>
        [Column("end_time", TypeName = "timestamp")]
        public DateTime EndTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [ForeignKey(nameof(MachineId))]
        [InverseProperty(nameof(MachineInformation.StaffManagements))]
        public virtual MachineInformation Machine { get; set; }
        [ForeignKey(nameof(ProcessId))]
        [InverseProperty("StaffManagements")]
        public virtual Process Process { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("StaffManagements")]
        public virtual User User { get; set; }
        [ForeignKey(nameof(WorkOrderId))]
        [InverseProperty(nameof(WorkOrderHead.StaffManagements))]
        public virtual WorkOrderHead WorkOrder { get; set; }
    }
}
