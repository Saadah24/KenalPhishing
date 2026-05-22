using KenalPhihsing.Data;
using KenalPhihsing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Controllers
{
    public class DashboardController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            // Security Check: If session is empty, go to login
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get category from session
            string category = Session["UserCategory"]?.ToString();
            ViewBag.UserCategory = category;

            // Return the Index view (the one that contains your @Html.Partial logic)
            return View();
        }

        public ActionResult Progress()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            string category = Session["UserCategory"]?.ToString() ?? "Adult";
            ViewBag.Category = category;
            ViewBag.UserName = Session["UserName"];

            // Data Simulasi - Gantikan dengan query DB sebenar nanti
            if (category == "Child")
            {
                ViewBag.Level = "Kapten Siber";
                ViewBag.Stars = 45;
                ViewBag.RankColor = "#00d2ff";
            }
            else if (category == "Elder")
            {
                ViewBag.SafetyStatus = "Selamat";
                ViewBag.ModulesFinished = 3;
            }
            else
            {
                ViewBag.AvgScore = 82;
                ViewBag.WeakTopic = "Penipuan SMS";
            }

            return View();
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

        public ActionResult Study(int id, int page = 1)
        {
            // 1. Ambil data halaman (page) sekarang
            // Kita guna .Include(m => m.Module) supaya data Icon & Title Modul boleh dibaca di View
            var currentPage = db.ModulePages
                                .Include(m => m.Module)
                                .FirstOrDefault(p => p.ModuleId == id && p.PageNumber == page);

            if (currentPage == null) return RedirectToAction("Modules");

            // 2. KIRA JUMLAH HALAMAN (INI PENTING!)
            // Kita kira ada berapa banyak baris dalam database yang mempunyai ModuleId yang sama
            int totalPages = db.ModulePages.Count(p => p.ModuleId == id);

            // 3. Hantar nilai ke View melalui ViewBag
            ViewBag.TotalPages = totalPages;

            return View(currentPage);
        }

        public ActionResult UserProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 1. Get the current user's ID
            int userId = Convert.ToInt32(Session["UserID"]);

            // 2. Fetch the user's REAL data from the database
            var user = db.Users.Find(userId);

            if (user != null)
            {
                // 3. Send the database values to the View
                ViewBag.FullName = user.FullName;
                ViewBag.Email = user.Email;
                ViewBag.Category = user.Category;

                // This will now show whatever is saved in your DB table column
                ViewBag.Phone = user.Phone ?? "Tiada Maklumat";
            }

            return View();
        }


        public ActionResult Simulation()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // 1. Show the Quiz
        public ActionResult Quiz()
        {
            return View();
        }

        // 2. Process the Answer
        [HttpPost]
        public ActionResult SubmitQuiz(string q1)
        {
            // The correct answer is 'C'
            if (q1 == "C")
            {
                return RedirectToAction("QuizCorrect");
            }
            else
            {
                return RedirectToAction("QuizWrong");
            }
        }

        // 3. The Correct Page
        public ActionResult QuizCorrect()
        {
            return View();
        }

        // 4. The Wrong Page
        public ActionResult QuizWrong()
        {
            return View();
        }

        public ActionResult Certificates()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult CertificateDetail()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            // Data simulasi untuk sijil
            ViewBag.StudentName = Session["UserName"] ?? "Rabiatul";
            ViewBag.Module = "Asas Phishing";
            ViewBag.Date = "11 Januari 2025";
            ViewBag.Score = "100%";

            return View();
        }

        public ActionResult Activity()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // Papar Borang Lapor Scam
        public ActionResult ReportScam()
        {
            return View();
        }

        // Simpan Laporan ke Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReport(ScamReport report)
        {
            // 1. Semak Session menggunakan ejaan yang betul "UserID"
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                report.DateReported = DateTime.Now;
                report.Status = "Pending";

                // 2. Tukar string kepada int dengan selamat
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
            // Hanya ambil laporan yang sudah DISAHKAN (Verified)
            var verifiedScams = db.ScamReports
                                  .Where(r => r.Status == "Verified")
                                  .OrderByDescending(r => r.DateReported)
                                  .ToList();
            return View(verifiedScams);
        }




        public ActionResult ActivitySelection()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            ViewBag.ModuleTitle = "Pengenalan Kepada Phishing"; // Tajuk modul semasa
            return View();
        }

        public ActionResult ChildReport()
        {
            // 1. Get the ChildEmail stored in the Parent's session
            string childEmail = Session["ChildEmail"]?.ToString();

            // IF NO CHILD EMAIL: Just return the view but set a flag
            if (string.IsNullOrEmpty(childEmail))
            {
                ViewBag.NoChild = true;
                return View();
            }

            // 2. Fetch the Child's data from the database
            var childUser = db.Users.FirstOrDefault(u => u.Email == childEmail);

            if (childUser != null)
            {
                ViewBag.NoChild = false;
                ViewBag.ChildName = childUser.FullName;
                ViewBag.ChildEmail = childUser.Email;
                ViewBag.ChildLastQuiz = 85;
                ViewBag.ChildProgress = 65;
            }
            else
            {
                // If email exists in session but user deleted from DB
                ViewBag.NoChild = true;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkChildAccount(string ChildEmail)
        {
            if (string.IsNullOrEmpty(ChildEmail))
            {
                TempData["ErrorMessage"] = "Sila masukkan e-mel akaun anak.";
                TempData["ActiveTab"] = "link-child"; // Helps us reopen the right tab
                return RedirectToAction("Settings", "Dashboard");
            }

            // CHECK IF THE CHILD EMAIL EXISTS IN THE DATABASE AND IS A "CHILD"
            var childAccount = db.Users.FirstOrDefault(u => u.Email == ChildEmail && u.Category == "Child");

            if (childAccount == null)
            {
                // THIS IS THE NEW ERROR MESSAGE
                TempData["ErrorMessage"] = "E-mel tidak sah. Sila pastikan anak anda telah mendaftar menggunakan akaun berstatus 'Kanak-Kanak'.";
                TempData["ActiveTab"] = "link-child"; // Helps us reopen the right tab
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
                return View("AlertDetail", alert); // Pergi ke Detail View
            }

            // Untuk Senarai Radar
            var allAlerts = db.SecurityAlerts.OrderByDescending(a => a.DatePublished).ToList();

            // Tarik data laporan komuniti yang dah verified
            ViewBag.VerifiedReports = db.ScamReports
                                        .Where(r => r.Status == "Verified")
                                        .OrderByDescending(r => r.DateReported)
                                        .ToList();

            return View(allAlerts);
        }

        // ==========================================
        // 1. PAPARKAN HALAMAN TETAPAN (GET)
        // ==========================================
        public ActionResult Settings()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);

            // Tarik data paling tepat dari Database (bukan Session)
            var user = db.Users.Find(userId);

            if (user == null) return HttpNotFound();

            return View(user); // Hantar objek 'user' ke View
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
                // 1. Handle Image
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

                // 2. Update Info (Guna Trim untuk elak ralat ruang kosong)
                user.FullName = FullName.Trim();
                user.Phone = Phone?.Trim();

                // Hanya kemaskini email jika parameter Email tidak null
                if (!string.IsNullOrEmpty(Email))
                {
                    user.Email = Email.Trim();
                    Session["UserEmail"] = user.Email;
                }

                // 3. PAKSA DATABASE SIMPAN
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // Update Session untuk paparan di sidebar
                Session["UserName"] = user.FullName;

                TempData["SuccessMessage"] = "Profil berjaya dikemaskini ke dalam pangkalan data!";
            }

            // Kembali ke halaman asal (Profil atau Tetapan)
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
                // Gunakan .Trim() untuk pastikan tiada 'space' yang tidak sengaja
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

        // ==========================================
        // 4. PADAM AKAUN (POST)
        // ==========================================
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