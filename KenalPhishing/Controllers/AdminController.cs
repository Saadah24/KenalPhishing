using KenalPhishing.Data;
using KenalPhishing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace KenalPhishing.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // --- FUNGSI KESELAMATAN ---
        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // 1. STATISTIK RINGKAS
            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.TotalModules = db.Modules.Count();
            ViewBag.TotalScamReports = db.ScamReports.Count();
            ViewBag.PendingReports = db.ScamReports.Count(r => r.Status == "Pending");

            // Sijil dikira berdasarkan jumlah modul yang berjaya disiapkan dalam UserProgress
            ViewBag.TotalCertificates = db.UserProgresses.Count();

            // 2. AKTIVITI TERKINI (Live Feed Dashboard)
            // KEMASKINI: Hanya mengambil 3 aktiviti terbaru untuk dashboard yang lebih bersih
            ViewBag.RecentActivities = db.UserActivities
                                         .Include(u => u.User)
                                         .OrderByDescending(a => a.CreatedAt)
                                         .Take(3)
                                         .ToList();

            // 3. DATA CARTA (Pendaftaran Pelajar)
            var registrationTrend = db.Users
                .GroupBy(u => u.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ChartLabels = registrationTrend.Select(x => x.Category).ToList();
            ViewBag.ChartValues = registrationTrend.Select(x => x.Count).ToList();

            return View();
        }

        // ==========================================
        // 1. PAPARKAN SENARAI MODUL
        // ==========================================
        public ActionResult ManageModules()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var modules = db.Modules.OrderBy(m => m.Category).ToList();
            return View(modules);
        }

        public ActionResult EditModule(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.IsEdit = id.HasValue;
            if (id.HasValue)
            {
                var module = db.Modules.Find(id);
                if (module == null) return HttpNotFound();
                return View(module);
            }
            return View(new Module());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveModule(Module model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    db.Modules.Add(model);
                }
                db.SaveChanges();
                return RedirectToAction("ManagePages", new { moduleId = model.Id });
            }
            return View("EditModule", model);
        }

        [HttpPost]
        public ActionResult DeleteModule(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var module = db.Modules.Find(id);
            if (module != null)
            {
                db.Modules.Remove(module);
                db.SaveChanges();
            }
            return RedirectToAction("ManageModules");
        }

        public ActionResult EditPage(int? id, int moduleId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.ModuleId = moduleId;
            if (id.HasValue)
            {
                var page = db.ModulePages.Find(id);
                return View(page);
            }
            return View(new ModulePage { ModuleId = moduleId, PageType = "Content" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SavePage(ModulePage model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    db.ModulePages.Add(model);
                }
                db.SaveChanges();
                return RedirectToAction("ManageModules");
            }
            return View("EditPage", model);
        }

        public ActionResult ManagePages(int moduleId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var module = db.Modules.Include(m => m.Pages).FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return HttpNotFound();
            return View(module);
        }

        public ActionResult ManageUsers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var users = db.Users.OrderBy(u => u.FullName).ToList();
            return View(users);
        }

        public ActionResult ManageReports()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var reports = db.ScamReports.OrderByDescending(r => r.DateReported).ToList();
            return View(reports);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            var report = db.ScamReports.Find(id);
            if (report != null)
            {
                report.Status = status;
                db.SaveChanges();
            }
            return RedirectToAction("ManageReports");
        }

        public ActionResult ManageAlerts()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var alerts = db.SecurityAlerts.OrderByDescending(a => a.DatePublished).ToList();
            return View(alerts);
        }

        public ActionResult EditAlert(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id.HasValue)
            {
                var alert = db.SecurityAlerts.Find(id);
                return View(alert);
            }
            return View(new SecurityAlert { DatePublished = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SaveAlert(SecurityAlert alert)
        {
            if (ModelState.IsValid)
            {
                var exists = db.SecurityAlerts.Any(a => a.Id == alert.Id);
                if (alert.Id > 0 && exists)
                {
                    db.Entry(alert).State = EntityState.Modified;
                }
                else
                {
                    alert.Id = 0;
                    alert.DatePublished = DateTime.Now;
                    db.SecurityAlerts.Add(alert);
                }
                db.SaveChanges();
                return RedirectToAction("ManageAlerts");
            }
            return View("EditAlert", alert);
        }

        [HttpPost]
        public ActionResult DeleteAlert(int id)
        {
            var alert = db.SecurityAlerts.Find(id);
            if (alert != null)
            {
                db.SecurityAlerts.Remove(alert);
                db.SaveChanges();
            }
            return RedirectToAction("ManageAlerts");
        }

        public ActionResult PromoteToAlert(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var report = db.ScamReports.Find(id);
            if (report == null) return HttpNotFound();

            var newAlert = new SecurityAlert
            {
                Title = "AMARAN: Taktik Scam " + report.Category,
                ScammerInfo = report.ScammerInfo,
                ExampleMessage = report.Description,
                Severity = "Sederhana",
                DatePublished = DateTime.Now,
                Content = "Analisis: Taktik ini dikesan melalui laporan komuniti."
            };
            return View("EditAlert", newAlert);
        }

        public ActionResult SystemConfig()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var settings = db.SystemSettings.FirstOrDefault();
            if (settings == null)
            {
                settings = new SystemSetting
                {
                    ScoreChild = 70,
                    ScoreAdult = 85,
                    ScoreElder = 80,
                    QuizAttempts = 3,
                    SessionTimeout = 30,
                    IsMaintenanceMode = false
                };
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSystemSettings(SystemSetting model, bool IsMaintenanceMode = false)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                var dbSettings = db.SystemSettings.FirstOrDefault();
                if (dbSettings != null)
                {
                    dbSettings.IsMaintenanceMode = IsMaintenanceMode;
                    dbSettings.GlobalAnnouncement = model.GlobalAnnouncement;
                    dbSettings.ScoreChild = model.ScoreChild;
                    dbSettings.ScoreAdult = model.ScoreAdult;
                    dbSettings.ScoreElder = model.ScoreElder;
                    dbSettings.QuizAttempts = model.QuizAttempts;
                    dbSettings.SessionTimeout = model.SessionTimeout;
                    dbSettings.LastUpdated = DateTime.Now;
                    dbSettings.UpdatedBy = Session["UserName"]?.ToString() ?? "Administrator";
                    db.Entry(dbSettings).State = EntityState.Modified;
                }
                else
                {
                    model.IsMaintenanceMode = IsMaintenanceMode;
                    model.LastUpdated = DateTime.Now;
                    model.UpdatedBy = Session["UserName"]?.ToString() ?? "Administrator";
                    db.SystemSettings.Add(model);
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "Tetapan sistem berjaya dikemaskini!";
            }
            return RedirectToAction("SystemConfig");
        }

        // HALAMAN DEDIKASI UNTUK SEMUA LOG AKTIVITI
        public ActionResult PortalActivity()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var allActivities = db.UserActivities
                                  .Include(u => u.User)
                                  .OrderByDescending(a => a.CreatedAt)
                                  .ToList();

            return View(allActivities);
        }
    }
}