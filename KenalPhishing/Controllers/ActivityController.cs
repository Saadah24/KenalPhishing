using KenalPhishing.Data;
using KenalPhishing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace KenalPhishing.Controllers
{
    public class ActivityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            // 1. Dapatkan UserID menggunakan fungsi yang telah dibetulkan
            int userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var user = db.Users.Find(userId);
            if (user == null) return RedirectToAction("Login", "Account");

            RecordLoginActivity(userId);

            // Fetch progress records (Gunakan CompletedAt != null sebagai penanda siap)
            var progressListAll = db.UserProgresses
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CompletedAt)
                .Include(p => p.Module)
                .ToList() ?? new List<UserProgress>();

            ViewBag.RecentProgress = progressListAll.Take(5).ToList();

            // Fetch top 5 Security Alerts
            var amaranList = db.SecurityAlerts
                .OrderByDescending(a => a.DatePublished)
                .Take(5)
                .ToList() ?? new List<SecurityAlert>();
            ViewBag.RadarAlerts = amaranList;

            // 2. Streak Calculation
            var distinctDates = progressListAll
                .Where(p => p.CompletedAt.HasValue)
                .Select(p => p.CompletedAt.Value.Date)
                .OrderByDescending(d => d)
                .Distinct()
                .ToList();

            int streakCount = 0;
            if (distinctDates.Contains(DateTime.Today) || distinctDates.Contains(DateTime.Today.AddDays(-1)))
            {
                var checkDate = distinctDates.Contains(DateTime.Today) ? DateTime.Today : DateTime.Today.AddDays(-1);
                foreach (var date in distinctDates)
                {
                    if (date == checkDate)
                    {
                        streakCount++;
                        checkDate = checkDate.AddDays(-1);
                    }
                    else break;
                }
            }
            ViewBag.StreakCount = streakCount;

            // 3. Konsistensi (7 hari terakhir)
            var sevenDaysAgo = DateTime.Today.AddDays(-7);
            var distinctStudyDays = distinctDates.Count(d => d >= sevenDaysAgo);
            ViewBag.ConsistencyStatus = distinctStudyDays >= 3 ? "Sangat Aktif" : (distinctStudyDays >= 1 ? "Sederhana" : "Perlu Ditingkatkan");

            // 4. Pencapaian
            string category = Session["UserCategory"]?.ToString() ?? user.Category ?? "Adult";

            // Ambil jumlah modul yang wujud dalam DB bagi kategori pengguna ini
            int totalModulesInDb = db.Modules.Count(m => m.Category == category);

            // Ambil jumlah modul yang telah disiapkan (Unique ID)
            int completedUniqueModules = progressListAll.Select(p => p.ModuleId).Distinct().Count();

            // Pengiraan Peratusan
            double completionPercentage = totalModulesInDb > 0 ? ((double)completedUniqueModules / totalModulesInDb) * 100 : 0;
            ViewBag.CompletionPercentage = Math.Round(completionPercentage, 2);

            // Populate ActivitiesList
            var activitiesList = new List<ActivityLog>();
            foreach (var prog in progressListAll.Take(15))
            {
                activitiesList.Add(new ActivityLog
                {
                    ActivityType = "Selesai",
                    Title = prog.Module?.Title ?? "Modul",
                    Description = $"Menamatkan modul dengan skor {prog.QuizScore}/{prog.QuizTotal}",
                    CreatedAt = prog.CompletedAt ?? DateTime.Now
                });
            }

            // Populate HeatmapData
            var heatmapData = new Dictionary<int, int>();
            for (int i = 0; i < 28; i++)
            {
                var targetDate = DateTime.Today.AddDays(-i);
                heatmapData[i] = progressListAll.Count(p => p.CompletedAt.HasValue && p.CompletedAt.Value.Date == targetDate);
            }

            var vm = new ActivityDashboardViewModel
            {
                UserCategory = category,
                FullName = user.FullName ?? user.Email,
                ProfilePicture = user.ProfilePicture,
                CurrentStreak = streakCount,

                // KEMAS KINI DI SINI:
                CompletedModules = completedUniqueModules,
                TotalModules = totalModulesInDb, // Dinamik ikut DB

                // Sijil dikira berdasarkan jumlah modul yang siap
                CompletedCertificates = completedUniqueModules,
                TotalCertificates = totalModulesInDb, // Dinamik ikut DB (1 modul = 1 sijil)

                ActivitiesList = activitiesList,
                HeatmapData = heatmapData
            };

            return View(vm);
        }

        private void RecordLoginActivity(int userId)
        {
            try
            {
                var today = DateTime.Today;
                bool alreadyToday = db.UserLogins.Any(l => l.UserId == userId && l.LoginDate == today);
                if (!alreadyToday)
                {
                    db.UserLogins.Add(new UserLogin { UserId = userId, LoginDate = today });
                    db.SaveChanges();
                }
            }
            catch { }
        }

        // FIX APPLIED HERE: Menggunakan Convert.ToInt32 untuk mengelakkan InvalidCastException
        private int GetCurrentUserId()
        {
            var raw = Session["UserID"];
            if (raw == null) return 0;
            try
            {
                return Convert.ToInt32(raw);
            }
            catch
            {
                return 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}