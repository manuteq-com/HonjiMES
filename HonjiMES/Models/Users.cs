using System;
using System.Collections.Generic;

namespace HonjiMES.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Realname { get; set; }
        public string Password { get; set; }
        public string Permission { get; set; }
        public string Department { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateTime { get; set; }
        public int CreateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? UpdateUser { get; set; }
    }
}
