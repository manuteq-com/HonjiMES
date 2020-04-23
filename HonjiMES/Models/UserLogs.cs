using System;
using System.Collections.Generic;

namespace HonjiMES.Models
{
    public partial class UserLogs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public int? CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
