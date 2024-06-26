﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#20489;&#24235;
    /// </summary>
    [Table("warehouse")]
    public partial class Warehouse
    {
        public Warehouse()
        {
            BillofPurchaseReturns = new HashSet<BillofPurchaseReturn>();
            Materials = new HashSet<Material>();
            Products = new HashSet<Product>();
            PurchaseDetails = new HashSet<PurchaseDetail>();
            Receives = new HashSet<Receive>();
            ReturnSales = new HashSet<ReturnSale>();
            Wiproducts = new HashSet<Wiproduct>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#20839;&#37096;&#20195;&#30908;
        /// </summary>
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        /// <summary>
        /// &#20489;&#24235;&#21517;&#31281;
        /// </summary>
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#36899;&#32097;&#20154;
        /// </summary>
        [Column("contact", TypeName = "varchar(50)")]
        public string Contact { get; set; }
        /// <summary>
        /// &#38651;&#35441;
        /// </summary>
        [Column("phone", TypeName = "varchar(50)")]
        public string Phone { get; set; }
        /// <summary>
        /// &#20659;&#30495;
        /// </summary>
        [Column("fax", TypeName = "varchar(50)")]
        public string Fax { get; set; }
        /// <summary>
        /// &#38651;&#23376;&#37109;&#20214;
        /// </summary>
        [Column("email", TypeName = "varchar(50)")]
        public string Email { get; set; }
        /// <summary>
        /// &#22320;&#22336;
        /// </summary>
        [Column("address", TypeName = "varchar(50)")]
        public string Address { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
        /// <summary>
        /// &#26159;&#21542;&#35201;&#29986;&#21697;&#27298;&#26597;,&#19981;&#27298;&#26597;&#30452;&#25509;&#22238;&#23384;&#20489;&#24235;
        /// </summary>
        [Column("recheck", TypeName = "tinyint(4)")]
        public sbyte? Recheck { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime? UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }

        [InverseProperty(nameof(BillofPurchaseReturn.Warehouse))]
        public virtual ICollection<BillofPurchaseReturn> BillofPurchaseReturns { get; set; }
        [InverseProperty(nameof(Material.Warehouse))]
        public virtual ICollection<Material> Materials { get; set; }
        [InverseProperty(nameof(Product.Warehouse))]
        public virtual ICollection<Product> Products { get; set; }
        [InverseProperty(nameof(PurchaseDetail.Warehouse))]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        [InverseProperty(nameof(Receive.Warehouse))]
        public virtual ICollection<Receive> Receives { get; set; }
        [InverseProperty(nameof(ReturnSale.Warehouse))]
        public virtual ICollection<ReturnSale> ReturnSales { get; set; }
        [InverseProperty(nameof(Wiproduct.Warehouse))]
        public virtual ICollection<Wiproduct> Wiproducts { get; set; }
    }
}
