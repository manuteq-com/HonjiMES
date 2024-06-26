﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("process")]
    public partial class Process
    {
        public Process()
        {
            MBillOfMaterials = new HashSet<MBillOfMaterial>();
            StaffManagements = new HashSet<StaffManagement>();
            Toolsets = new HashSet<Toolset>();
            WorkOrderDetails = new HashSet<WorkOrderDetail>();
        }

        /// <summary>
        /// id
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#21517;&#31281;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; }
        /// <summary>
        /// &#20195;&#34399;
        /// </summary>
        [Required]
        [Column("code", TypeName = "varchar(50)")]
        public string Code { get; set; }
        /// <summary>
        /// &#24037;&#24207;&#31278;&#39006;
        /// </summary>
        [Column("type", TypeName = "int(11)")]
        public int? Type { get; set; }
        /// <summary>
        /// &#21069;&#32622;&#26178;&#38291;	
        /// </summary>
        [Column("lead_time", TypeName = "decimal(10,2)")]
        public decimal? LeadTime { get; set; }
        /// <summary>
        /// &#27161;&#28310;&#24037;&#26178;	
        /// </summary>
        [Column("work_time", TypeName = "decimal(10,2)")]
        public decimal? WorkTime { get; set; }
        /// <summary>
        /// &#25104;&#26412;
        /// </summary>
        [Column("cost", TypeName = "decimal(10,2)")]
        public decimal? Cost { get; set; }
        /// <summary>
        /// &#22294;&#34399;	
        /// </summary>
        [Column("draw_no", TypeName = "varchar(50)")]
        public string DrawNo { get; set; }
        /// <summary>
        /// &#25152;&#38656;&#20154;&#21147;	
        /// </summary>
        [Column("manpower", TypeName = "int(11)")]
        public int? Manpower { get; set; }
        /// <summary>
        /// &#27231;&#21488;	
        /// </summary>
        [Column("producing_machine", TypeName = "varchar(50)")]
        public string ProducingMachine { get; set; }
        /// <summary>
        /// &#20633;&#35387;&#27396;&#20301;
        /// </summary>
        [Column("remark", TypeName = "varchar(50)")]
        public string Remark { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#24314;&#31435;&#20154;&#21729;
        /// </summary>
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }

        [InverseProperty(nameof(MBillOfMaterial.Process))]
        public virtual ICollection<MBillOfMaterial> MBillOfMaterials { get; set; }
        [InverseProperty(nameof(StaffManagement.Process))]
        public virtual ICollection<StaffManagement> StaffManagements { get; set; }
        [InverseProperty(nameof(Toolset.Process))]
        public virtual ICollection<Toolset> Toolsets { get; set; }
        [InverseProperty(nameof(WorkOrderDetail.Process))]
        public virtual ICollection<WorkOrderDetail> WorkOrderDetails { get; set; }
    }
}
