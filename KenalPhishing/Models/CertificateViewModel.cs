using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class CertificateViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleTitle { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int Score { get; set; }
    }
}