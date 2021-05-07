using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#37559;&#36008;&#26126;&#32048;
    /// </summary>
    [Table("sale_detail_new")]
    public partial class SaleDetailNew
    {
        public SaleDetailNew()
        {
            ReturnSales = new HashSet<ReturnSale>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#37559;&#36008;&#21934;&#34399;
        /// </summary>
        [Column("sale_id", TypeName = "int(11)")]
        public int SaleId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#21934;&#34399;id
        /// </summary>
        [Column("order_id", TypeName = "int(11)")]
        public int OrderId { get; set; }
        /// <summary>
        /// &#35330;&#21934;&#20839;&#23481;&#21807;&#19968;&#30908;
        /// </summary>
        [Column("order_detail_id", TypeName = "int(11)")]
        public int OrderDetailId { get; set; }
        /// <summary>
        /// &#29986;&#21697;&#22522;&#26412;&#36039;&#35338;id
        /// </summary>
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;ID
        /// </summary>
        [Column("material_id", TypeName = "int(11)")]
        public int? MaterialId { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#34399;
        /// </summary>
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        /// <summary>
        /// &#37559;&#36008;&#29376;&#24907;
        /// </summary>
        [Column("status", TypeName = "int(11)")]
        public int Status { get; set; }
        /// <summary>
        /// &#20027;&#20214;&#21697;&#21517;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }
        /// <summary>
        /// &#21407;&#21934;&#20729;	
        /// </summary>
        [Column("originPrice", TypeName = "decimal(10,2)")]
        public decimal OriginPrice { get; set; }
        /// <summary>
        /// &#20729;&#26684;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
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

        [ForeignKey(nameof(MaterialId))]
        [InverseProperty("SaleDetailNews")]
        public virtual Material Material { get; set; }
        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("SaleDetailNews")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(OrderHead.SaleDetailNews))]
        public virtual OrderHead Order { get; set; }
        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("SaleDetailNews")]
        public virtual OrderDetail OrderDetail { get; set; }
        [ForeignKey(nameof(SaleId))]
        [InverseProperty(nameof(SaleHead.SaleDetailNews))]
        public virtual SaleHead Sale { get; set; }
        [InverseProperty(nameof(ReturnSale.SaleDetailNew))]
        public virtual ICollection<ReturnSale> ReturnSales { get; set; }
    }
}
