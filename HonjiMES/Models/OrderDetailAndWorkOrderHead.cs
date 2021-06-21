using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("order_detail_and_work_order_head")]
    public partial class OrderDetailAndWorkOrderHead
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("order_detail_id", TypeName = "int(11)")]
        public int OrderDetailId { get; set; }
        [Column("work_head_id", TypeName = "int(11)")]
        public int WorkHeadId { get; set; }
        /// <summary>
        /// &#26009;&#34399;&#31278;&#39006;(1&#21407;&#26009; 2&#25104;&#21697; 3 &#21322;&#25104;&#21697;)
        /// </summary>
        [Column("data_type", TypeName = "int(11)")]
        public int? DataType { get; set; }
        /// <summary>
        /// &#26009;&#34399;Id
        /// </summary>
        [Column("data_id", TypeName = "int(11)")]
        public int DataId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#25976;&#37327;
        /// </summary>
        [Column("order_count", TypeName = "decimal(10,2)")]
        public decimal OrdeCount { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }

        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("OrderDetailAndWorkOrderHeads")]
        public virtual OrderDetail OrderDetail { get; set; }
        [ForeignKey(nameof(WorkHeadId))]
        [InverseProperty(nameof(WorkOrderHead.OrderDetailAndWorkOrderHeads))]
        public virtual WorkOrderHead WorkHead { get; set; }
    }
}
