﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#38936;&#26009;&#36039;&#26009;&#20027;&#27284;
    /// </summary>
    [Table("requisition")]
    public partial class Requisition
    {
        public Requisition()
        {
            RequisitionDetails = new HashSet<RequisitionDetail>();
        }

        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        /// <summary>
        /// &#24037;&#21934;&#20027;&#27284;ID
        /// </summary>
        [Column("work_order_head_id", TypeName = "int(11)")]
        public int WorkOrderHeadId { get; set; }
        /// <summary>
        /// &#38936;&#26009;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("requisition_no", TypeName = "varchar(50)")]
        public string RequisitionNo { get; set; }
        /// <summary>
        /// &#38936;&#26009;&#21934;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;
        /// </summary>
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        /// <summary>
        /// &#24288;&#20839;&#25104;&#21697;&#34399;
        /// </summary>
        [Column("material_number", TypeName = "varchar(50)")]
        public string MaterialNumber { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#29983;&#29986;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        /// <summary>
        /// &#38936;&#26009;&#21934;&#39006;&#22411;(0:&#38936;&#20986;&#21934;,1:&#36864;&#24235;&#21934;)
        /// </summary>
        [Column("type", TypeName = "tinyint(4)")]
        public sbyte Type { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("receive_user", TypeName = "int(11)")]
        public int? ReceiveUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("Requisitions")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(WorkOrderHeadId))]
        [InverseProperty("Requisitions")]
        public virtual WorkOrderHead WorkOrderHead { get; set; }
        [InverseProperty(nameof(RequisitionDetail.Requisition))]
        public virtual ICollection<RequisitionDetail> RequisitionDetails { get; set; }
    }
}
