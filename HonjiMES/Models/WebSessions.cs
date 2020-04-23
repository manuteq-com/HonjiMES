using System;
using System.Collections.Generic;

namespace HonjiMES.Models
{
    public partial class WebSessions
    {
        public string Id { get; set; }
        public string IpAddress { get; set; }
        public uint Timestamp { get; set; }
        public byte[] Data { get; set; }
        public int? CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
