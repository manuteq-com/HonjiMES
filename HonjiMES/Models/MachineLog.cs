using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#27231;&#21488;&#35686;&#31034;
    /// </summary>
    [Table("machine_log")]
    public partial class MachineLog
    {
        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27231;&#21488;ID
        /// </summary>
        [Column("machine_id", TypeName = "int(11)")]
        public int MachineId { get; set; }
        /// <summary>
        /// &#35686;&#22577;&#34399;&#30908;
        /// </summary>
        [Column("alarm_number", TypeName = "int(11)")]
        public int AlarmNumber { get; set; }
        /// <summary>
        /// &#35686;&#22577;&#35338;&#24687;
        /// </summary>
        [Required]
        [Column("alarm_message", TypeName = "varchar(50)")]
        public string AlarmMessage { get; set; }
        /// <summary>
        /// &#20633;&#35387;
        /// </summary>
        [Column("remarks", TypeName = "varchar(50)")]
        public string Remarks { get; set; }
        /// <summary>
        /// &#38283;&#22987;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// &#32080;&#26463;&#26178;&#38291;
        /// </summary>
        [Column("end_time", TypeName = "timestamp")]
        public DateTime EndTime { get; set; }
        [Column("create_user", TypeName = "int(11)")]
        public int CreateUser { get; set; }
        /// <summary>
        /// &#21034;&#38500;&#35387;&#35352;
        /// </summary>
        [Column("delete_flag", TypeName = "tinyint(4)")]
        public sbyte DeleteFlag { get; set; }
    }
}
