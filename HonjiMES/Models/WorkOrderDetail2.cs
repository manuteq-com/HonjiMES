using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    
    public partial class WorkOrderDetail2
    {
        public WorkOrderDetail2()
        {
            WorkOrderQcLogs = new HashSet<WorkOrderQcLog>();
            WorkOrderReportLogs = new HashSet<WorkOrderReportLog>();
        }

        [Key]
        [Column("id", TypeName = "varchar(100)")]
        public string Id { get; set; }
        /// <summary>
        /// &#24037;&#21934;ID
        /// </summary>
        [Column("work_order_head_id", TypeName = "int(11)")]
        public int WorkOrderHeadId { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#38918;&#24207;	
        /// </summary>
        [Column("serial_number", TypeName = "int(11)")]
        public int SerialNumber { get; set; }
        [Column("process_id", TypeName = "int(11)")]
        public int ProcessId { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#20195;&#34399;	
        /// </summary>
        [Required]
        [Column("process_no", TypeName = "varchar(50)")]
        public string ProcessNo { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#21517;&#31281;	
        /// </summary>
        [Required]
        [Column("process_name", TypeName = "varchar(50)")]
        public string ProcessName { get; set; }
        /// <summary>
        /// &#21069;&#32622;&#26178;&#38291;	
        /// </summary>
        [Column("process_lead_time", TypeName = "decimal(10,2)")]
        public decimal ProcessLeadTime { get; set; }
        /// <summary>
        /// &#27161;&#28310;&#24037;&#26178;	
        /// </summary>
        [Column("process_time", TypeName = "decimal(10,2)")]
        public decimal ProcessTime { get; set; }
        /// <summary>
        /// &#25104;&#26412;	
        /// </summary>
        [Column("process_cost", TypeName = "decimal(10,2)")]
        public decimal ProcessCost { get; set; }
        /// <summary>
        /// &#38656;&#27714;&#37327;
        /// </summary>
        [Column("count", TypeName = "int(11)")]
        public int Count { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;ID
        /// </summary>
        [Column("purchase_id", TypeName = "int(11)")]
        public int? PurchaseId { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;id
        /// </summary>
        [Column("supplier_id", TypeName = "int(11)")]
        public int? SupplierId { get; set; }
        /// <summary>
        /// &#22294;&#34399;
        /// </summary>
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#21152;&#24037;&#31243;&#24335;
        /// </summary>
        [Column("code_no", TypeName = "varchar(50)")]
        public string CodeNo { get; set; }
        /// <summary>
        /// &#25152;&#38656;&#20154;&#21147;	
        /// </summary>
        [Column("manpower", TypeName = "int(11)")]
        public int? Manpower { get; set; }
        /// <summary>
        /// &#27231;&#21488;	
        /// </summary>
        [Column("producing_machine", TypeName = "varchar(50)")]
        public string ProducingMachine { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#31278;&#39006;
        /// </summary>
        [Column("type", TypeName = "int(11)")]
        public int Type { get; set; }
        /// <summary>
        /// &#20633;&#35387;	
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#21487;&#35069;&#36896;&#25976;&#37327;
        /// </summary>
        [Column("m_count", TypeName = "int(11)")]
        public int? MCount { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#23436;&#24037;&#25976;&#37327;
        /// </summary>
        [Column("re_count", TypeName = "int(11)")]
        public int? ReCount { get; set; }
        /// <summary>
        /// &#23526;&#38555;&#22238;&#22577;&#37329;&#38989;
        /// </summary>
        [Column("re_price", TypeName = "decimal(10,2)")]
        public decimal? RePrice { get; set; }
        /// <summary>
        /// NG&#25976;&#37327;
        /// </summary>
        [Column("ng_count", TypeName = "int(11)")]
        public int NgCount { get; set; }
        /// <summary>
        /// NC&#26410;&#21152;&#24037;
        /// </summary>
        [Column("nc_count", TypeName = "int(11)")]
        public int NcCount { get; set; }
        /// <summary>
        /// &#32317;&#24037;&#26178;
        /// </summary>
        [Column("total_time", TypeName = "timestamp")]
        public DateTime? TotalTime { get; set; }
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

        [ForeignKey(nameof(ProcessId))]
        [InverseProperty("WorkOrderDetails")]
        public virtual Process Process { get; set; }
        [ForeignKey(nameof(PurchaseId))]
        [InverseProperty(nameof(PurchaseHead.WorkOrderDetails))]
        public virtual PurchaseHead Purchase { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("WorkOrderDetails")]
        public virtual Supplier Supplier { get; set; }
        [ForeignKey(nameof(WorkOrderHeadId))]
        [InverseProperty("WorkOrderDetails")]
        public virtual WorkOrderHead WorkOrderHead { get; set; }
        [InverseProperty(nameof(WorkOrderQcLog.WorkOrderDetail))]
        public virtual ICollection<WorkOrderQcLog> WorkOrderQcLogs { get; set; }
        [InverseProperty(nameof(WorkOrderReportLog.WorkOrderDetail))]
        public virtual ICollection<WorkOrderReportLog> WorkOrderReportLogs { get; set; }
    }
}
