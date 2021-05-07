using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#37559;&#36008;&#21934;
    /// </summary>
    [Table("sale_head")]
    public partial class SaleHead
    {
        public SaleHead()
        {
            SaleDetailNews = new HashSet<SaleDetailNew>();
            WorkOrderQcLogs = new HashSet<WorkOrderQcLog>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#37559;&#36008;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("sale_no", TypeName = "varchar(100)")]
        public string SaleNo { get; set; }
        [Column("temp", TypeName = "int(11)")]
        public int? Temp { get; set; }
        /// <summary>
        /// &#37559;&#36008;&#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#37559;&#25110;&#26085;&#26399;
        /// </summary>
        [Column("sale_date", TypeName = "timestamp")]
        public DateTime? SaleDate { get; set; }
        /// <summary>
        /// &#32317;&#37329;&#38989;
        /// </summary>
        [Column("price_all", TypeName = "decimal(10,2)")]
        public decimal PriceAll { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(SaleDetailNew.Sale))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
        [InverseProperty(nameof(WorkOrderQcLog.SaleHead))]
        public virtual ICollection<WorkOrderQcLog> WorkOrderQcLogs { get; set; }
    }
}
