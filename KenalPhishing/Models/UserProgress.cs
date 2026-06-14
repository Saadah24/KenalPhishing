using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class UserProgress
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ModuleId { get; set; }

        // Property yang diperlukan oleh Controller
        public bool IsCompleted { get; set; }
        public int ProgressPercent { get; set; }
        public bool CertificateIssued { get; set; }
        public string CertificateUrl { get; set; }

        // Simpan "BERJAYA" atau "GAGAL"
        public string SimResult { get; set; }

        public int QuizScore { get; set; }
        public int QuizTotal { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime? LastAccessedAt { get; set; }

        // Navigation Property untuk .Include(p => p.PhihsingModule)
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
    }
}