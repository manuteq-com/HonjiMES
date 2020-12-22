using System;

namespace HonjiMES.Models
{
    public class HubMessage
    {
        public string clientuniqueid { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
    }
}
