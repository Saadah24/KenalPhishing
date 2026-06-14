using KenalPhishing.Data;
using KenalPhishing.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhishing.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            // 1. SEMAKAN KESELAMATAN
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            string category = Session["UserCategory"]?.ToString();
            ViewBag.UserCategory = category;

            // 2. TARIK TETAPAN SISTEM
            var settings = db.SystemSettings.FirstOrDefault();

            if (settings != null && settings.IsMaintenanceMode)
            {
                if (Session["UserRole"]?.ToString() != "Admin")
                    return View("MaintenanceMode");
            }

            ViewBag.GlobalAnnouncement = settings?.GlobalAnnouncement;
            ViewBag.MinScore = (category == "Child") ? settings?.ScoreChild :
                               (category == "Elder") ? settings?.ScoreElder :
                               settings?.ScoreAdult;

            // ==============================================================
            // 3. LOGIK MENGIKUT KATEGORI (DIOPTIMUMKAN KE 3 ITEM)
            // ==============================================================

            if (category == "Adult")
            {
                var progresses = db.UserProgresses.Include(p => p.Module).Where(p => p.UserId == userId).ToList();

                ViewBag.CompletedModules = progresses.Select(p => p.ModuleId).Distinct().Count();
                ViewBag.TotalModules = db.Modules.Count(m => m.Category == "Adult");

                int totalScore = progresses.Sum(p => p.QuizScore);
                int totalQuestions = progresses.Sum(p => p.QuizTotal);
                ViewBag.AvgScore = totalQuestions > 0 ? Math.Round(((double)totalScore / totalQuestions) * 100) : 0;

                // --- LOGIK LENCANA (BADGES) ---
                // Kira berapa banyak modul yang dijawab dengan skor sempurna (100%)
                ViewBag.Badges = progresses.Count(p => p.QuizScore == p.QuizTotal && p.QuizTotal > 0);

                // Hadkan kepada 3 item supaya Dashboard "Clean"
                ViewBag.LatestAlerts = db.SecurityAlerts.OrderByDescending(a => a.DatePublished).Take(3).ToList();

                var myReports = db.ScamReports.Where(r => r.UserId == userId).ToList();
                ViewBag.TotalReports = myReports.Count;

                // Hadkan Log Aktiviti kepada 3 item sahaja
                ViewBag.RecentActivities = db.UserActivities
                                             .Where(a => a.UserId == userId)
                                             .OrderByDescending(a => a.CreatedAt)
                                             .Take(3)
                                             .ToList();

                var chartData = progresses.Where(p => p.Module != null).GroupBy(p => p.Module.Title)
                    .Select(g => new {
                        Label = g.Key.Length > 15 ? g.Key.Substring(0, 15) + "..." : g.Key,
                        Score = g.Average(x => x.QuizTotal > 0 ? ((double)x.QuizScore / x.QuizTotal) * 100 : 0)
                    }).ToList();

                ViewBag.ChartLabels = chartData.Select(c => c.Label).ToList();
                ViewBag.ChartValues = chartData.Select(c => Math.Round(c.Score)).ToList();
            }
            else if (category == "Child")
            {
                var progress = db.UserProgresses.Where(p => p.UserId == userId).ToList();
                int completedCount = progress.Select(p => p.ModuleId).Distinct().Count();
                int totalScore = progress.Sum(p => p.QuizScore);

                int currentXP = (completedCount * 50) + (totalScore * 10);
                ViewBag.CurrentXP = currentXP;
                ViewBag.NextRankXP = 1000;
                ViewBag.LevelName = currentXP < 300 ? "Prajurit Muda" : (currentXP < 700 ? "Wira Muda" : "Sarjan Elit");
                ViewBag.Badges = progress.Count(p => p.QuizScore == p.QuizTotal && p.QuizTotal > 0);

                ViewBag.ChildModules = db.Modules.Where(m => m.Category == "Child").ToList();
                ViewBag.CompletedModuleIds = progress.Select(p => p.ModuleId).Distinct().ToList();

                // Hadkan Leaderboard kepada 2-3 orang sahaja di Dashboard
                ViewBag.Leaderboard = db.Users.Where(u => u.Category == "Child").OrderByDescending(u => u.Id).Take(3).ToList();
            }
            else if (category == "Elder")
            {
                // 1. Tarik semua rekod kemajuan pengguna (Warga Emas)
                var userProgress = db.UserProgresses.Include(p => p.Module).Where(p => p.UserId == userId).ToList();

                // 2. Kira jumlah modul yang telah disiapkan (Unik)
                int completedModulesCount = userProgress.Select(p => p.ModuleId).Distinct().Count();
                ViewBag.ModulesFinished = completedModulesCount;

                // 3. Sijil dikira berdasarkan jumlah modul yang tamat (1 modul = 1 sijil)
                ViewBag.Certificates = completedModulesCount;

                // 4. LOGIK LENCANA: Kira berapa banyak modul yang dijawab dengan skor sempurna (100%)
                ViewBag.Badges = userProgress.Count(p => p.QuizScore == p.QuizTotal && p.QuizTotal > 0);

                // 5. Kira Purata Markah Keseluruhan (Untuk bar kemajuan besar 0-100%)
                double avg = userProgress.Any()
                    ? userProgress.Average(p => (p.QuizTotal > 0 ? (double)p.QuizScore / p.QuizTotal * 100 : 0))
                    : 0;
                ViewBag.AvgScore = (int)Math.Round(avg);

                // 6. Tarik senarai modul khusus Elder (Hadkan kepada 4 supaya Dashboard "Clean")
                ViewBag.ElderModules = db.Modules
                                        .Where(m => m.Category == "Elder")
                                        .OrderBy(m => m.Id)
                                        .Take(4)
                                        .ToList();
            }

            return View();
        }

        // 1. CARI DAN GANTIKAN kaedah Progress() yang sedia ada:
        public ActionResult Progress()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);
            string category = user.Category ?? "Adult";

            ViewBag.Category = category;
            ViewBag.UserName = user.FullName;

            var userProgress = db.UserProgresses.Include(p => p.Module).Where(p => p.UserId == userId).ToList();
            int completedModules = userProgress.Select(p => p.ModuleId).Distinct().Count();

            int totalScore = userProgress.Sum(p => p.QuizScore);
            int totalQuestions = userProgress.Sum(p => p.QuizTotal);
            double avgScore = totalQuestions > 0 ? ((double)totalScore / totalQuestions) * 100 : 0;

            ViewBag.AvgScore = Math.Round(avgScore);

            if (category == "Child")
            {
                // PENGIRAAN XP & PANGKAT KANAK-KANAK
                // Katakan: 1 Modul Siap = 50 XP, Skor Betul = 10 XP setiap 1
                int totalXP = (completedModules * 50) + (totalScore * 10);
                ViewBag.CurrentXP = totalXP;

                // Logik Pangkat (Praktikal)
                if (totalXP < 100) { ViewBag.LevelName = "Prajurit Muda"; ViewBag.NextRankXP = 100; }
                else if (totalXP < 200) { ViewBag.LevelName = "Koperal Siber"; ViewBag.NextRankXP = 200; }
                else if (totalXP < 500) { ViewBag.LevelName = "Sarjan Elit"; ViewBag.NextRankXP = 500; }
                else { ViewBag.LevelName = "Jeneral Siber"; ViewBag.NextRankXP = totalXP; }

                // Logik Lencana: Hanya dapat jika markah 100% dan Simulasi 'BERJAYA'
                int earnedBadges = userProgress.Count(p => p.QuizScore == p.QuizTotal && p.SimResult == "BERJAYA");
                ViewBag.Badges = earnedBadges;

                // Logik Ranking: Kira di tangga ke berapa pengguna ini berbanding Child lain (berdasarkan markah/XP)
                var allChildUsers = db.Users.Where(u => u.Category == "Child").ToList();
                // Simulasi rank rawak untuk contoh jika database kosong (Anda boleh buat db query sum XP nanti)
                ViewBag.Rank = new Random().Next(1, 20);
            }
            else if (category == "Elder")
            {
                ViewBag.ModulesFinished = completedModules;
                ViewBag.Certificates = completedModules; // Anggap setiap modul = 1 sijil
            }
            else // Adult
            {
                ViewBag.TotalStudyTime = completedModules * 25;

                var radarData = userProgress.Where(p => p.Module != null).GroupBy(p => p.Module.Title)
                    .Select(g => new {
                        Label = g.Key.Length > 15 ? g.Key.Substring(0, 15) + "..." : g.Key,
                        Score = g.Average(x => x.QuizTotal > 0 ? ((double)x.QuizScore / x.QuizTotal) * 100 : 0)
                    }).ToList();
                if (!radarData.Any()) radarData.Add(new { Label = "Belum Ada Data", Score = 0.0 });

                ViewBag.RadarLabels = radarData.Select(r => r.Label).ToList();
                ViewBag.RadarValues = radarData.Select(r => Math.Round(r.Score)).ToList();

                var trendLabels = new List<string>(); var trendValues = new List<double>();
                for (int i = 5; i >= 0; i--)
                {
                    var targetMonth = DateTime.Now.AddMonths(-i);
                    trendLabels.Add(targetMonth.ToString("MMM"));
                    var monthData = userProgress.Where(p => p.CompletedAt.HasValue && p.CompletedAt.Value.Month == targetMonth.Month && p.CompletedAt.Value.Year == targetMonth.Year).ToList();
                    trendValues.Add(monthData.Any() ? Math.Round(monthData.Average(x => x.QuizTotal > 0 ? ((double)x.QuizScore / x.QuizTotal) * 100 : 0)) : 0);
                }
                ViewBag.LineLabels = trendLabels; ViewBag.LineValues = trendValues;
            }

            var completedCerts = userProgress.Where(p => p.Module != null).GroupBy(p => p.ModuleId)
                .Select(g => g.OrderByDescending(x => x.CompletedAt).FirstOrDefault()).ToList();
            ViewBag.CompletedCerts = completedCerts;

            return View();
        }

        // ==========================================
        // MUKA SURAT BAHARU UNTUK KANAK-KANAK
        // ==========================================
        public ActionResult Pangkat()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserID"]);

            // Kiraan XP (Sama macam di atas)
            var userProgress = db.UserProgresses.Where(p => p.UserId == userId).ToList();
            int completedModules = userProgress.Select(p => p.ModuleId).Distinct().Count();
            int totalScore = userProgress.Sum(p => p.QuizScore);
            ViewBag.CurrentXP = (completedModules * 50) + (totalScore * 10);

            return View();
        }

        public ActionResult Lencana()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserID"]);

            // Hantar data progress supaya View boleh tentukan mana lencana di'unlock'
            var userProgress = db.UserProgresses.Include(p => p.Module).Where(p => p.UserId == userId).ToList();
            return View(userProgress);
        }

        public ActionResult Ranking()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            // Ambil senarai murid dari jadual User yang berstatus Child
            var children = db.Users.Where(u => u.Category == "Child").ToList();
            ViewBag.CurrentUserId = Convert.ToInt32(Session["UserID"]);

            return View(children);
        }

        // 2. TAMBAH FUNGSI BAHARU INI UNTUK EKSPORT CSV
        public ActionResult ExportProgressCsv()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserID"]);

            var progresses = db.UserProgresses.Include(p => p.Module)
                               .Where(p => p.UserId == userId)
                               .OrderByDescending(p => p.CompletedAt).ToList();

            var sb = new System.Text.StringBuilder();
            // Header CSV
            sb.AppendLine("Modul,Skor Kuiz,Jumlah Soalan,Keputusan Simulasi,Tarikh Selesai");

            foreach (var p in progresses)
            {
                string title = p.Module?.Title?.Replace(",", "") ?? "Modul Tidak Diketahui";
                string dateStr = p.CompletedAt.HasValue ? p.CompletedAt.Value.ToString("dd MMM yyyy HH:mm") : "Tiada Rekod";
                sb.AppendLine($"{title},{p.QuizScore},{p.QuizTotal},{p.SimResult},{dateStr}");
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(buffer, "text/csv", "Laporan_Prestasi_Phishing.csv");
        }

        public ActionResult Modules()
        {
            // 1. Semak keselamatan: Jika user belum login, tendang ke page Login
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Ambil kategori user yang login (Elder / Adult / Child)
            string category = Session["UserCategory"]?.ToString() ?? "Adult";
            ViewBag.UserCategory = category;

            // 3. Tarik modul dari Database HANYA untuk kategori user tersebut
            var modulesList = db.Modules.Where(m => m.Category == category).ToList();

            // 4. Hantar senarai modul ke View (UI)
            return View(modulesList);
        }

        [HttpPost]
        public ActionResult SaveModuleProgress(int ModuleId, string SimResult, int QuizScore, int QuizTotal)
        {
            try
            {
                // 1. Ambil User ID sebenar daripada Session
                if (Session["UserID"] == null)
                {
                    return Json(new { success = false, message = "Sesi tamat. Sila log masuk semula." });
                }

                int currentUserId = Convert.ToInt32(Session["UserID"]);

                // 2. Simpan rekod kemajuan ke dalam database
                var progress = new UserProgress
                {
                    UserId = currentUserId,
                    ModuleId = ModuleId,
                    SimResult = SimResult ?? "Tiada",
                    QuizScore = QuizScore,
                    QuizTotal = QuizTotal,
                    CompletedAt = DateTime.Now
                };

                db.UserProgresses.Add(progress);

                // 3. Tambahan: Rekod aktiviti untuk dipaparkan di Dashboard (Feed)
                var module = db.Modules.Find(ModuleId);
                var activity = new UserActivity
                {
                    UserId = currentUserId,
                    ActivityType = "Module",
                    Title = "Selesai Modul: " + (module?.Title ?? "Phishing"),
                    Description = $"Skor Kuiz: {QuizScore}/{QuizTotal}, Simulasi: {SimResult}",
                    CreatedAt = DateTime.Now,
                    ActionUrl = "/Dashboard/Certificates"
                };
                db.UserActivities.Add(activity);

                db.SaveChanges();

                return Json(new { success = true, message = "Kemajuan anda telah disimpan. Sijil kini tersedia!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===============================================
        // FIX APPLIED HERE: Added int? id and null check
        // ===============================================
        public ActionResult Study(int? id, int page = 1)
        {
            // 1. SEMAKAN KESELAMATAN: Elak aplikasi crash jika URL tiada ID
            if (!id.HasValue)
            {
                return RedirectToAction("Modules");
            }

            // 2. Ambil data halaman (page) sekarang
            var currentPage = db.ModulePages
                                .Include(m => m.Module)
                                .FirstOrDefault(p => p.ModuleId == id.Value && p.PageNumber == page);

            if (currentPage == null) return RedirectToAction("Modules");

            // 3. KIRA JUMLAH HALAMAN (INI PENTING!)
            int totalPages = db.ModulePages.Count(p => p.ModuleId == id.Value);

            // 4. Hantar nilai ke View melalui ViewBag
            ViewBag.TotalPages = totalPages;

            return View(currentPage);
        }

        [HttpPost]
        public JsonResult CompleteModule(int moduleId, bool simResult, bool quizResult)
        {
            if (Session["UserID"] == null) return Json(new { success = false });

            int userId = Convert.ToInt32(Session["UserID"]);
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // 1. Ambil Nama Modul
                string moduleTitle = "Modul";
                using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Modules WHERE Id = @mid", conn))
                {
                    cmd.Parameters.AddWithValue("@mid", moduleId);
                    moduleTitle = cmd.ExecuteScalar()?.ToString();
                }

                // 2. Rekod ke Jadual UserActivities (Untuk Feed Dashboard)
                string activitySql = @"INSERT INTO UserActivities (UserId, ActivityType, Title, Description, CreatedAt, ActionUrl) 
                               VALUES (@uid, 'Simulasi', @title, @desc, GETDATE(), @url)";

                using (SqlCommand cmd = new SqlCommand(activitySql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@title", "Selesai " + moduleTitle);
                    cmd.Parameters.AddWithValue("@desc", simResult ? "Tahniah! Anda berjaya mengesan ancaman scam." : "Anda telah tamat simulasi ini.");
                    cmd.Parameters.AddWithValue("@url", "/Dashboard/Study/" + moduleId + "?page=1");
                    cmd.ExecuteNonQuery();
                }

                // 3. Kemaskini UserStats (Untuk Ringkasan Pencapaian 0/0 Modul)
                string statsSql = @"
            IF EXISTS (SELECT 1 FROM UserStats WHERE UserId = @uid)
                UPDATE UserStats SET CompletedModules = CompletedModules + 1 WHERE UserId = @uid
            ELSE
                INSERT INTO UserStats (UserId, CompletedCertificates, TotalCertificates, CompletedModules, TotalModules)
                VALUES (@uid, 0, 5, 1, 10)";

                using (SqlCommand cmd = new SqlCommand(statsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            return Json(new { success = true });
        }

        public ActionResult UserProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);

            if (user != null)
            {
                ViewBag.FullName = user.FullName;
                ViewBag.Email = user.Email;
                ViewBag.Category = user.Category;
                ViewBag.Phone = user.Phone ?? "Tiada Maklumat";

                // TARIK DATA LENCANA (Contoh: Skor 100% = 1 Lencana)
                var userProgress = db.UserProgresses.Where(p => p.UserId == userId).ToList();
                ViewBag.BadgesCount = userProgress.Count(p => p.QuizScore == p.QuizTotal && p.QuizTotal > 0);

                // Simpan dalam list untuk kegunaan View jika perlu butiran modul
                ViewBag.EarnedModules = userProgress.Where(p => p.QuizScore == p.QuizTotal).Select(p => p.Module.Title).ToList();
            }

            return View();
        }

        // 3. CARI DAN GANTIKAN kaedah Certificates()
        public ActionResult Certificates()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);
            string category = Session["UserCategory"]?.ToString() ?? "Adult";

            var allModules = db.Modules.Where(m => m.Category == category).ToList();
            var userProgress = db.UserProgresses.Where(p => p.UserId == userId).ToList();

            // Sediakan ViewModel untuk Sijil
            var certList = allModules.Select(m => {
                var progress = userProgress.Where(p => p.ModuleId == m.Id).OrderByDescending(p => p.CompletedAt).FirstOrDefault();
                return new CertificateViewModel
                {
                    ModuleId = m.Id,
                    ModuleTitle = m.Title,
                    IsCompleted = progress != null,
                    CompletedDate = progress?.CompletedAt,
                    Score = progress != null && progress.QuizTotal > 0
                            ? (int)Math.Round(((double)progress.QuizScore / progress.QuizTotal) * 100)
                            : 0
                };
            }).ToList();

            return View(certList);
        }


        public ActionResult CertificateDetail(int? id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            if (!id.HasValue) return RedirectToAction("Certificates");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);

            // 1. Ambil data sijil semasa
            var progress = db.UserProgresses.Include(p => p.Module)
                             .Where(p => p.UserId == userId && p.ModuleId == id.Value)
                             .OrderByDescending(p => p.CompletedAt)
                             .FirstOrDefault();

            if (progress == null) return RedirectToAction("Certificates");

            ViewBag.StudentName = user.FullName;
            ViewBag.Module = progress.Module.Title;
            ViewBag.Date = progress.CompletedAt?.ToString("dd MMMM yyyy") ?? DateTime.Now.ToString("dd MMMM yyyy");
            int score = progress.QuizTotal > 0 ? (int)Math.Round((double)progress.QuizScore / progress.QuizTotal * 100) : 0;
            ViewBag.Score = score + "%";

            // ============================================================
            // 2. LOGIK LANJUTKAN PEMBELAJARAN (CONNECT DATABASE)
            // ============================================================
            string category = user.Category ?? "Adult";

            // Ambil ID semua modul yang SUDAH disiapkan
            var completedModuleIds = db.UserProgresses
                                       .Where(p => p.UserId == userId)
                                       .Select(p => p.ModuleId)
                                       .ToList();

            // Cari modul dalam kategori yang sama yang BELUM disiapkan
            var nextModules = db.Modules
                                .Where(m => m.Category == category && !completedModuleIds.Contains(m.Id))
                                .Take(2) // Ambil 2 cadangan modul
                                .ToList();

            ViewBag.RecommendedModules = nextModules;

            return View();
        }



        public ActionResult ReportScam()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReport(ScamReport report)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                report.DateReported = DateTime.Now;
                report.Status = "Pending";
                report.UserId = Convert.ToInt32(Session["UserID"]);

                db.ScamReports.Add(report);
                db.SaveChanges();

                TempData["Success"] = "Laporan anda telah dihantar dan akan disemak.";
                return RedirectToAction("ReportScam");
            }

            return View("ReportScam", report);
        }

        public ActionResult CommunityAlerts()
        {
            var verifiedScams = db.ScamReports
                                  .Where(r => r.Status == "Verified")
                                  .OrderByDescending(r => r.DateReported)
                                  .ToList();
            return View(verifiedScams);
        }

        public ActionResult ActivitySelection()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            ViewBag.ModuleTitle = "Pengenalan Kepada Phishing";
            return View();
        }

        public ActionResult ChildReport()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int parentId = Convert.ToInt32(Session["UserID"]);

            // 1. Cari data parent untuk dapatkan ID anak yang dipautkan
            var parent = db.Users.Find(parentId);

            if (parent == null || parent.LinkedChildId == null)
            {
                ViewBag.NoChild = true;
                return View();
            }

            // 2. Dapatkan data profil anak
            int childId = parent.LinkedChildId.Value;
            var child = db.Users.Find(childId);

            if (child == null)
            {
                ViewBag.NoChild = true;
                return View();
            }

            ViewBag.NoChild = false;
            ViewBag.ChildName = child.FullName;
            ViewBag.ChildEmail = child.Email;

            // 1. Ambil SEMUA data untuk kira purata
            var allProgress = db.UserProgresses.Include(p => p.Module).Where(p => p.UserId == childId).ToList();

            // 2. HADKAN paparan di Dashboard kepada 3 TERBARU sahaja
            ViewBag.ProgressList = allProgress.OrderByDescending(p => p.CompletedAt).Take(2).ToList();

            int completedModules = allProgress.Select(p => p.ModuleId).Distinct().Count();
            double avgScore = allProgress.Any() ? allProgress.Average(p => (p.QuizTotal > 0 ? (double)p.QuizScore / p.QuizTotal * 100 : 0)) : 0;

            ViewBag.CompletedModulesCount = completedModules;
            ViewBag.ChildAvgScore = Math.Round(avgScore);

            // 3. HADKAN Log Aktiviti kepada 3 TERBARU sahaja
            ViewBag.ChildActivities = db.UserActivities
                                       .Where(a => a.UserId == childId)
                                       .OrderByDescending(a => a.CreatedAt)
                                       .Take(2)
                                       .ToList();

            return View();
        }

        // HALAMAN DEDIKASI: LIHAT SEMUA AKTIVITI ANAK
        public ActionResult FullChildActivity()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int parentId = Convert.ToInt32(Session["UserID"]);
            var parent = db.Users.Find(parentId);

            if (parent == null || parent.LinkedChildId == null) return RedirectToAction("ChildReport");

            int childId = parent.LinkedChildId.Value;
            var child = db.Users.Find(childId);

            ViewBag.ChildName = child.FullName;

            // Ambil sejarah penuh kemajuan modul
            var fullProgress = db.UserProgresses.Include(p => p.Module)
                                 .Where(p => p.UserId == childId)
                                 .OrderByDescending(p => p.CompletedAt).ToList();

            // Ambil sejarah penuh log aktiviti
            var fullActivities = db.UserActivities
                                   .Where(a => a.UserId == childId)
                                   .OrderByDescending(a => a.CreatedAt).ToList();

            ViewBag.FullProgress = fullProgress;
            ViewBag.FullActivities = fullActivities;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkChildAccount(string ChildEmail)
        {
            if (string.IsNullOrEmpty(ChildEmail))
            {
                TempData["ErrorMessage"] = "Sila masukkan e-mel akaun anak.";
                TempData["ActiveTab"] = "link-child";
                return RedirectToAction("Settings", "Dashboard");
            }

            var childAccount = db.Users.FirstOrDefault(u => u.Email == ChildEmail && u.Category == "Child");

            if (childAccount == null)
            {
                TempData["ErrorMessage"] = "E-mel tidak sah. Sila pastikan anak anda telah mendaftar menggunakan akaun berstatus 'Kanak-Kanak'.";
                TempData["ActiveTab"] = "link-child";
                return RedirectToAction("Settings", "Dashboard");
            }

            int parentId = Convert.ToInt32(Session["UserID"]);
            var parentAccount = db.Users.FirstOrDefault(u => u.Id == parentId);

            if (parentAccount != null)
            {
                parentAccount.LinkedChildId = childAccount.Id;
                db.SaveChanges();

                Session["ChildEmail"] = childAccount.Email;
                Session["ChildName"] = childAccount.FullName;
                Session["NoChild"] = false;

                TempData["SuccessMessage"] = "Akaun anak berjaya dipautkan!";
            }

            return RedirectToAction("ChildReport", "Dashboard");
        }

        public ActionResult SecurityAlert(int? id)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            if (id.HasValue)
            {
                var alert = db.SecurityAlerts.Find(id);
                if (alert == null) return HttpNotFound();
                return View("AlertDetail", alert);
            }

            var allAlerts = db.SecurityAlerts.OrderByDescending(a => a.DatePublished).ToList();

            ViewBag.VerifiedReports = db.ScamReports
                                        .Where(r => r.Status == "Verified")
                                        .OrderByDescending(r => r.DateReported)
                                        .ToList();

            return View(allAlerts);
        }

        public ActionResult Settings()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);

            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(string FullName, string Email, string Phone, HttpPostedFileBase ProfileImage)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);

            if (user != null)
            {
                if (ProfileImage != null && ProfileImage.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/Profiles/");
                    if (!System.IO.Directory.Exists(folderPath)) System.IO.Directory.CreateDirectory(folderPath);

                    string fileName = userId + "_" + System.IO.Path.GetFileName(ProfileImage.FileName);
                    string path = System.IO.Path.Combine(folderPath, fileName);
                    ProfileImage.SaveAs(path);

                    user.ProfilePicture = fileName;
                    Session["UserProfilePic"] = fileName;
                }

                user.FullName = FullName.Trim();
                user.Phone = Phone?.Trim();

                if (!string.IsNullOrEmpty(Email))
                {
                    user.Email = Email.Trim();
                    Session["UserEmail"] = user.Email;
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                Session["UserName"] = user.FullName;
                TempData["SuccessMessage"] = "Profil berjaya dikemaskini ke dalam pangkalan data!";
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.Find(userId);

            if (user != null)
            {
                if (user.Password.Trim() != CurrentPassword.Trim())
                {
                    TempData["ErrorMessage"] = "KATA LALUAN SEMASA SALAH. Sila masukkan kata laluan lama yang betul.";
                    TempData["ActiveTab"] = "edit-security";
                    return RedirectToAction("Settings");
                }

                if (NewPassword != ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "Kata laluan baharu tidak sepadan dengan pengesahan.";
                    TempData["ActiveTab"] = "edit-security";
                    return RedirectToAction("Settings");
                }

                user.Password = NewPassword.Trim();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Kata laluan anda telah ditukar dengan selamat!";
                TempData["ActiveTab"] = "edit-security";
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId;
            if (int.TryParse(Session["UserID"].ToString(), out userId))
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    Session.Clear();
                    Session.Abandon();
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Settings");
        }


    }
}