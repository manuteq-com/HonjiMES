using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#36914;&#36008;&#21934;
    /// </summary>
    [Table("billof_purchase")]
    public partial class BillofPurchase
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("billof_purchase_no", TypeName = "varchar(50)")]
        public string BillofPurchaseNo { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;
        /// </summary>
        [Column("supplier", TypeName = "int(11)")]
        public int Supplier { get; set; }
        /// <summary>
        /// &#36914;&#36008;&#26085;&#26399;
        /// </summary>
        [Required]
        [Column("date", TypeName = "varchar(50)")]
        public string Date { get; set; }
        /// <summary>
        /// &#21697;&#34399;
        /// </summary>
        [Column("material_no", TypeName = "int(11)")]
        public int MaterialNo { get; set; }
        /// <summary>
        /// &#21697;&#21517;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Required]
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#39511;&#25910;/&#36864;&#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#21934;&#20729;
        /// </summary>
        [Column("price", TypeName = "int(11)")]
        public int Price { get; set; }
        /// <summary>
        /// &#25505;&#36092;&#21934;&#34399;
        /// </summary>
        [Required]
        [Column("purchase_no", TypeName = "varchar(50)")]
        public string PurchaseNo { get; set; }
        /// <summary>
        /// &#29376;&#24907;
        /// </summary>
        [Required]
        [Column("status", TypeName = "varchar(50)")]
        public string Status { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#32773;id
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int? CreateUser { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#32773;id
        /// </summary>
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
