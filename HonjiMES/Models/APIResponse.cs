using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class APIResponse
    {
        public bool success { get; set; }
        public string timestamp { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public string message { get; set; }
        public object data { get; set; }
    }
}
