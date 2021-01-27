﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.CncModels
{
    /// <summary>
    /// &#21152;&#24037;&#31243;&#24335;&#36039;&#35338;
    /// </summary>
    [Table("nc_file_informations")]
    public partial class NcFileInformation
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(5)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27284;&#26696;&#20358;&#28304;
        /// </summary>
        [Column("source", TypeName = "int(5)")]
        public int Source { get; set; }
        /// <summary>
        /// &#27284;&#26696;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }
        /// <summary>
        /// &#31243;&#24335;&#35387;&#35299;
        /// </summary>
        [Column("comment", TypeName = "varchar(255)")]
        public string Comment { get; set; }
        /// <summary>
        /// &#27284;&#26696;&#22823;&#23567;
        /// </summary>
        [Column("size", TypeName = "varchar(255)")]
        public string Size { get; set; }
        /// <summary>
        /// &#27284;&#26696;&#24314;&#31435;&#26178;&#38291;
        /// </summary>
        [Column("file_create_time", TypeName = "timestamp")]
        public DateTime FileCreateTime { get; set; }
        /// <summary>
        /// &#27161;&#31844;JSON
        /// </summary>
        [Column("tag", TypeName = "text")]
        public string Tag { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        [ForeignKey(nameof(Source))]
        [InverseProperty(nameof(MachineInformation.NcFileInformations))]
        public virtual MachineInformation SourceNavigation { get; set; }
    }
}