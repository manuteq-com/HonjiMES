using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonjiMES.Models
{
    /// <summary>
    /// &#27231;&#21488;&#36039;&#35338;
    /// </summary>
    [Table("machine_informations")]
    public partial class MachineInformation
    {
        public MachineInformation()
        {
            MachineMaintenances = new HashSet<MachineMaintenance>();
            StaffManagements = new HashSet<StaffManagement>();
        }

        /// <summary>
        /// 唯一碼
        /// </summary>
        [Key]
        [Column("id", TypeName = "int(5)")]
        public int Id { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#21517;&#31281;
        /// </summary>
        [Required]
        [Column("name", TypeName = "varchar(12)")]
        public string Name { get; set; }
        /// <summary>
        /// &#36899;&#32218;&#22320;&#22336;
        /// </summary>
        [Required]
        [Column("url", TypeName = "varchar(100)")]
        public string Url { get; set; }
        /// <summary>
        /// &#36899;&#32218;&#22496;&#34399;
        /// </summary>
        [Column("port", TypeName = "int(5)")]
        public int Port { get; set; }
        /// <summary>
        /// &#27231;&#21488;&#31278;&#39006;ID
        /// </summary>
        [Column("model_id", TypeName = "int(5)")]
        public int ModelId { get; set; }
        /// <summary>
        /// &#25511;&#21046;&#22120;&#31278;&#39006;ID
        /// </summary>
        [Column("control_brand_id", TypeName = "int(5)")]
        public int ControlBrandId { get; set; }
        /// <summary>
        /// &#36000;&#36012;&#20154;ID
        /// </summary>
        [Column("user_id", TypeName = "int(5)")]
        public int? UserId { get; set; }
        /// <summary>
        /// &#29305;&#27530;&#35373;&#23450;JSON
        /// </summary>
        [Column("special_setting", TypeName = "text")]
        public string SpecialSetting { get; set; }
        /// <summary>
        /// &#27284;&#26696;&#20659;&#36865;&#35373;&#23450;JSON
        /// </summary>
        [Column("transfer_setting", TypeName = "text")]
        public string TransferSetting { get; set; }
        [Column("plan_setting", TypeName = "text")]
        public string PlanSetting { get; set; }
        [Column("enable_state", TypeName = "int(1)")]
        public int? EnableState { get; set; }
        /// <summary>
        /// &#26356;&#26032;&#26178;&#38291;
        /// </summary>
        [Column("update_time", TypeName = "timestamp")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// &#26032;&#22686;&#26178;&#38291;
        /// </summary>
        [Column("create_time", TypeName = "timestamp")]
        public DateTime CreateTime { get; set; }

        [InverseProperty(nameof(MachineMaintenance.Machine))]
        public virtual ICollection<MachineMaintenance> MachineMaintenances { get; set; }
        [InverseProperty(nameof(StaffManagement.Machine))]
        public virtual ICollection<StaffManagement> StaffManagements { get; set; }
    }
}
