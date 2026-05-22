using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhihsing.Models
{
    public class ScamReport
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Siapa yang lapor

        public string Category { get; set; } // WhatsApp, SMS, Emel, Panggilan
        public string ScammerInfo { get; set; } // Pautan atau No. Telefon
        public string Description { get; set; } // Kronologi
        public DateTime DateReported { get; set; }
        public string Status { get; set; } // Pending, Verified, Rejected

        // Relasi ke User
        public virtual User User { get; set; }
    }
}