﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
[Table("purchase_head")]
    public partial class PurchaseHead
    {
        public PurchaseHead()
        {
            PurchaseDetails = new HashSet<PurchaseDetail>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("purchase_no", TypeName = "varchar(100)")]
        public string PurchaseNo { get; set; }
        [Column("supplier_id")]
        public int SupplierId { get; set; }
        [Column("type")]
        public int? Type { get; set; }
        [Column("status")]
        public int Status { get; set; }
        [Column("remarks", TypeName = "varchar(100)")]
        public string Remarks { get; set; }
        [Column("purchase_date", TypeName = "timestamp")]
        public DateTime? PurchaseDate { get; set; }
        [Column("price_all")]
        public int PriceAll { get; set; }
        [Column("delete_flag")]
        public sbyte DeleteFlag { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user")]
        public int? UpdateUser { get; set; }

        [InverseProperty("Purchase")]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}