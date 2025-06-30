using System;

namespace BulkOperationsEntityFramework.Models
{
    public class Sesjon
    {

        public Guid Key { get; set; } // Primary key by convention

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }
    }
}
