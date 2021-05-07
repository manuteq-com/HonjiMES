using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    [Table("machine_workdate")]
    public partial class MachineWorkdate
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#35373;&#23450;
        /// </summary>
        [Column("setting", TypeName = "int(11)")]
        public int Setting { get; set; }
        /// <summary>
        /// &#26085;&#26399;
        /// </summary>
        [Column("work_date", TypeName = "date")]
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// &#36215;&#22987;&#26178;&#38291;
        /// </summary>
        [Column("work_time_start", TypeName = "timestamp")]
        public DateTime WorkTimeStart { get; set; }
        /// <summary>
        /// &#32080;&#26463;&#26178;&#38291;
        /// </summary>
        [Column("work_time_end", TypeName = "timestamp")]
        public DateTime WorkTimeEnd { get; set; }
        [Column("delete_flag", TypeName = "int(11)")]
        public int DeleteFlag { get; set; }
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        [Column("update_user", TypeName = "int(11)")]
        public int? UpdateUser { get; set; }
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
    }
}
