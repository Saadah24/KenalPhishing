using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string ActivityType { get; set; } // Contoh: 'Modul', 'Sijil', 'Laporan'

        [Required]
        public string Title { get; set; } // Contoh: 'Selesai Modul Anti-Phishing'

        public string Description { get; set; } // Contoh: 'Mendapat skor 10/10'

        public DateTime CreatedAt { get; set; }

        public string ActionUrl { get; set; } // Contoh: '/Dashboard/Certificates'

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}