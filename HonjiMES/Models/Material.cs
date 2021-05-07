using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#21407;&#26009;&#24235;&#23384;
    /// </summary>
    [Table("material")]
    public partial class Material
    {
        public Material()
        {
            MaterialLogs = new HashSet<MaterialLog>();
            OrderDetails = new HashSet<OrderDetail>();
            SaleDetailNews = new HashSet<SaleDetailNew>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("material_basic_id", TypeName = "int(11)")]
        public int MaterialBasicId { get; set; }
        /// <summary>
        /// &#21697;&#34399;
        /// </summary>
        [Required]
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        /// <summary>
        /// &#22580;&#20839;&#21697;&#34399;
        /// </summary>
        [Column("material_number", TypeName = "varchar(50)")]
        public string MaterialNumber { get; set; }
        /// <summary>
        /// &#21697;&#21517;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#24235;&#23384;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,1)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#24235;&#23384;&#26997;&#38480;
        /// </summary>
        [Column("quantity_limit", TypeName = "int(11)")]
        public int? QuantityLimit { get; set; }
        /// <summary>
        /// &#38928;&#20808;&#25187;&#24235;&#25976;&#37327;
        /// </summary>
        [Column("quantity_adv", TypeName = "int(11)")]
        public int QuantityAdv { get; set; }
        /// <summary>
        /// &#35215;&#26684;
        /// </summary>
        [Column("specification", TypeName = "varchar(50)")]
        public string Specification { get; set; }
        /// <summary>
        /// &#23660;&#24615;
        /// </summary>
        [Required]
        [Column("property", TypeName = "varchar(50)")]
        public string Property { get; set; }
        /// <summary>
        /// &#21407;&#20729;&#26684;
        /// </summary>
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Column("unit", TypeName = "varchar(50)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#21407;&#26009;&#38656;&#27714;&#37327;	
        /// </summary>
        [Column("material_require", TypeName = "int(11)")]
        public int? MaterialRequire { get; set; }
        /// <summary>
        /// &#32068;&#25104;&#29992;&#37327;
        /// </summary>
        [Column("composition", TypeName = "int(11)")]
        public int? Composition { get; set; }
        /// <summary>
        /// &#24213;&#25976;
        /// </summary>
        [Column("base_quantity", TypeName = "int(11)")]
        public int? BaseQuantity { get; set; }
        /// <summary>
        /// &#20379;&#25033;&#21830;
        /// </summary>
        [Column("supplier", TypeName = "int(11)")]
        public int? Supplier { get; set; }
        /// <summary>
        /// &#23384;&#25918;&#24235;&#21029;
        /// </summary>
        [Column("sub_inventory", TypeName = "varchar(50)")]
        public string SubInventory { get; set; }
        [Column("warehouse_id", TypeName = "int(11)")]
        public int WarehouseId { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
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

        [ForeignKey(nameof(MaterialBasicId))]
        [InverseProperty("Materials")]
        public virtual MaterialBasic MaterialBasic { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        [InverseProperty("Materials")]
        public virtual Warehouse Warehouse { get; set; }
        [InverseProperty(nameof(MaterialLog.Material))]
        public virtual ICollection<MaterialLog> MaterialLogs { get; set; }
        [InverseProperty(nameof(OrderDetail.Material))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(SaleDetailNew.Material))]
        public virtual ICollection<SaleDetailNew> SaleDetailNews { get; set; }
    }
}
