﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("bill_of_material_ver")]
    public partial class BillOfMaterialVer
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("product_basic_id", TypeName = "int(11)")]
        public int ProductBasicId { get; set; }
        /// <summary>
        /// &#29256;&#26412;
        /// </summary>
        [Column("version", TypeName = "decimal(10,2)")]
        public decimal Version { get; set; }
        /// <summary>
        /// bomID
        /// </summary>
        [Key]
        [Column("bomid", TypeName = "int(11)")]
        public int Bomid { get; set; }
        /// <summary>
        /// &#29238;bomID	
        /// </summary>
        [Column("bompid", TypeName = "int(11)")]
        public int? Bompid { get; set; }
        [Column("product_no", TypeName = "varchar(50)")]
        public string ProductNo { get; set; }
        [Column("product_name", TypeName = "varchar(50)")]
        public string ProductName { get; set; }
        [Column("material_no", TypeName = "varchar(50)")]
        public string MaterialNo { get; set; }
        [Column("material_name", TypeName = "varchar(50)")]
        public string MaterialName { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#25976;&#37327;
        /// </summary>
        [Column("quantity", TypeName = "decimal(10,3)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// &#21934;&#20301;
        /// </summary>
        [Column("unit", TypeName = "varchar(10)")]
        public string Unit { get; set; }
        /// <summary>
        /// &#23652;&#25976;
        /// </summary>
        [Column("lv", TypeName = "int(11)")]
        public int Lv { get; set; }
        /// <summary>
        /// &#22806;&#21253;&#35387;&#35352;
        /// </summary>
        [Column("outsource", TypeName = "tinyint(4)")]
        public sbyte? Outsource { get; set; }
        /// <summary>
        /// &#32068;&#25104;&#34920;&#32676;&#32068;
        /// </summary>
        [Column("group", TypeName = "int(11)")]
        public int Group { get; set; }
        /// <summary>
        /// BOM&#30340;&#39006;&#22411;
        /// </summary>
        [Column("type", TypeName = "tinyint(4)")]
        public sbyte? Type { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
