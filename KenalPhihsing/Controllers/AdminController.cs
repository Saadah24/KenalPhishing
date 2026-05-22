using KenalPhihsing.Data;
using KenalPhihsing.Models;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace KenalPhihsing.Controllers
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

            return View();
        }

        // ==========================================
        // 1. PAPARKAN SENARAI MODUL
        // ==========================================
        public ActionResult ManageModules()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Ambil semua modul dari Database dan susun mengikut Kategori
            var modules = db.Modules.OrderBy(m => m.Category).ToList();
            return View(modules);
        }

        // ==========================================
        // 2. PAPARKAN BORANG (TAMBAH / SUNTING)
        // ==========================================
        public ActionResult EditModule(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.IsEdit = id.HasValue;

            if (id.HasValue)
            {
                // Jika URL ada ID (Maksudnya Admin nak Edit)
                var module = db.Modules.Find(id);
                if (module == null) return HttpNotFound();

                return View(module); // Hantar data sedia ada ke form
            }

            // Jika URL tiada ID (Maksudnya Admin nak Tambah Baru)
            return View(new Module());
        }

        // ==========================================
        // 3. PROSES SIMPAN DATA (POST)
        // ==========================================
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

                // SELEPAS SIMPAN, terus pergi ke page urus kandungan (halaman)
                return RedirectToAction("ManagePages", new { moduleId = model.Id });
            }
            return View("EditModule", model);
        }

        // ==========================================
        // 4. PROSES PADAM MODUL (POST)
        // ==========================================
        [HttpPost]
        public ActionResult DeleteModule(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var module = db.Modules.Find(id);
            if (module != null)
            {
                db.Modules.Remove(module);
                db.SaveChanges(); // Buang dari database
            }
            return RedirectToAction("ManageModules");
        }

        // ==========================================
        // 5. PAPARKAN BORANG EDIT "MUKA SURAT" (KANDUNGAN / SIMULASI)
        // ==========================================
        public ActionResult EditPage(int? id, int moduleId)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.ModuleId = moduleId; // Hantar Module ID ke View

            if (id.HasValue)
            {
                var page = db.ModulePages.Find(id);
                return View(page); // Edit page sedia ada
            }

            // Tambah page baru
            return View(new ModulePage { ModuleId = moduleId, PageType = "Content" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // <--- ADD THIS LINE HERE
        public ActionResult SavePage(ModulePage model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
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

        // 6. PAPARKAN SENARAI HALAMAN UNTUK SESUATU MODUL
        public ActionResult ManagePages(int moduleId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Ambil data modul sekali dengan senarai Pages di dalamnya
            var module = db.Modules
                           .Include(m => m.Pages)
                           .FirstOrDefault(m => m.Id == moduleId);

            if (module == null) return HttpNotFound();

            return View(module);
        }

        // ==========================================
        // 7. PAPARKAN SENARAI PELAJAR
        // ==========================================
        public ActionResult ManageUsers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Ambil semua pengguna kecuali Admin
            var users = db.Users.OrderBy(u => u.FullName).ToList();
            return View(users);
        }

        // --- BAHAGIAN ADMIN ---

        // Papar senarai laporan untuk Admin
        public ActionResult ManageReports()
        {
            // Hanya Admin boleh akses (tambah logic check session role di sini)
            var reports = db.ScamReports.OrderByDescending(r => r.DateReported).ToList();
            return View(reports);
        }

        // Update status laporan (Sahkan/Tolak)
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

        // --- PENGURUSAN SECURITY ALERTS (ADMIN) ---

        // 1. Senarai Amaran
        public ActionResult ManageAlerts()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var alerts = db.SecurityAlerts.OrderByDescending(a => a.DatePublished).ToList();
            return View(alerts);
        }

        // 2. Borang Tambah/Edit
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

        // 3. Simpan Amaran
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // <--- ADD THIS LINE HERE
        public ActionResult SaveAlert(SecurityAlert alert)
        {
            if (ModelState.IsValid)
            {
                // Semak adakah ID ini benar-benar wujud dalam table SecurityAlerts
                var exists = db.SecurityAlerts.Any(a => a.Id == alert.Id);

                if (alert.Id > 0 && exists)
                {
                    // Jika ID wujud, maksudnya ini adalah PROSES EDIT (Update)
                    db.Entry(alert).State = EntityState.Modified;
                }
                else
                {
                    // Jika ID adalah 0 ATAU ID tidak wujud dalam table (Promote dari ScamReport)
                    // Kita paksa ID jadi 0 supaya Database buat ID baru (Auto-increment)
                    alert.Id = 0;
                    alert.DatePublished = DateTime.Now;
                    db.SecurityAlerts.Add(alert);
                }

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("ManageAlerts");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ralat semasa menyimpan: " + ex.Message);
                }
            }
            return View("EditAlert", alert);
        }

        // 4. Padam Amaran
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

        // Fungsi untuk 'promote' laporan user kepada Buletin Rasmi
        public ActionResult PromoteToAlert(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var report = db.ScamReports.Find(id);
            if (report == null) return HttpNotFound();

            // Cipta objek SecurityAlert baru dan isi dengan data dari ScamReport
            var newAlert = new SecurityAlert
            {
                Title = "AMARAN: Taktik Scam " + report.Category,
                ScammerInfo = report.ScammerInfo,
                ExampleMessage = report.Description, // Masukkan kronologi sebagai contoh mesej
                Severity = "Sederhana", // Default severity
                DatePublished = DateTime.Now,
                Content = "Analisis: Taktik ini dikesan melalui laporan komuniti. Modus operandi melibatkan..."
            };

            // Bawa ke View EditAlert dengan data yang sudah diisi
            return View("EditAlert", newAlert);
        }


        // 1. DISPLAY THE CONFIG PAGE (GET)
        public ActionResult SystemConfig()
        {
            // Fetch the first row of settings. 
            // If the table is empty, create a default one so the page doesn't crash.
            var settings = db.SystemSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new SystemSetting
                {
                    ScoreChild = 70,
                    ScoreAdult = 85,
                    ScoreElder = 80,
                    QuizAttempts = 3
                };
            }

            return View(settings);
        }

        // 2. SAVE THE SETTINGS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSystemSettings(SystemSetting model, bool IsMaintenanceMode = false)
        {
            var dbSettings = db.SystemSettings.FirstOrDefault();

            if (dbSettings != null)
            {
                // Update existing row
                dbSettings.IsMaintenanceMode = IsMaintenanceMode;
                dbSettings.GlobalAnnouncement = model.GlobalAnnouncement;
                dbSettings.ScoreChild = model.ScoreChild;
                dbSettings.ScoreAdult = model.ScoreAdult;
                dbSettings.ScoreElder = model.ScoreElder;
                dbSettings.QuizAttempts = model.QuizAttempts;
                dbSettings.SessionTimeout = model.SessionTimeout;

                db.SaveChanges();
                TempData["SuccessMessage"] = "Tetapan sistem telah berjaya dikemaskini!";
            }
            else
            {
                // If table is empty, add the first row
                model.IsMaintenanceMode = IsMaintenanceMode;
                db.SystemSettings.Add(model);
                db.SaveChanges();
            }

            return RedirectToAction("SystemConfig");
        }


    }
}