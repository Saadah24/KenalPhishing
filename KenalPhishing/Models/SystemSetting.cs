using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public bool IsMaintenanceMode { get; set; }
        public string GlobalAnnouncement { get; set; }
        public int ScoreChild { get; set; }
        public int ScoreAdult { get; set; }
        public int ScoreElder { get; set; }
        public int QuizAttempts { get; set; }
        public int SessionTimeout { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
    }
}