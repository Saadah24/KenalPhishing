using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class ActivityDashboardViewModel
    {
        public string UserCategory { get; set; } = "Adult";
        public string FullName { get; set; } = "Pengguna";
        public int CurrentStreak { get; set; }

        public string ProfilePicture { get; set; }

        // Stats
        public int CompletedCertificates { get; set; }
        public int TotalCertificates { get; set; }
        public int CompletedModules { get; set; }
        public int TotalModules { get; set; }

        // The Activity List (Fixed duplicate)
        public List<ActivityLog> ActivitiesList { get; set; } = new List<ActivityLog>();

        // Heatmap data: Key is the date offset (0 for today, 1 for yesterday, etc.), Value is activity count
        public Dictionary<int, int> HeatmapData { get; set; } = new Dictionary<int, int>();
    }

    public class ActivityLog
    {
        public int Id { get; set; }
        public string ActivityType { get; set; } // Sijil, Amaran, Simulasi
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionUrl { get; set; }
    }
}