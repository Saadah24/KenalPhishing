using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Progress()
        {
            ViewBag.Title = "Kemajuan Saya";
            return View();
        }

        public ActionResult Modules()
        {
            // If session exists, it will NOT redirect to Login.
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // This automatically pulls the category they chose during registration
            string category = Session["UserCategory"]?.ToString() ?? "Adult";
            ViewBag.UserCategory = category;

            return View();
        }

        public ActionResult Study()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kita boleh hantar data tajuk modul ke View
            ViewBag.ModuleTitle = "Pengenalan Kepada Phishing";
            return View();
        }

        public ActionResult Profile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Mengambil data simulasi dari Session
            ViewBag.FullName = Session["UserName"]?.ToString() ?? "Pengguna KenalPhishing";
            ViewBag.Email = Session["UserEmail"]?.ToString() ?? "user@email.com";
            ViewBag.Category = Session["UserCategory"]?.ToString() ?? "Dewasa";
            ViewBag.Phone = "012-3456789"; // Contoh data tambahan

            return View();
        }

        public ActionResult Settings()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Simulasi data pengguna
            ViewBag.UserName = Session["UserName"] ?? "Rabiatul";
            ViewBag.UserEmail = Session["UserEmail"] ?? "rabiatul@email.com";

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

        public ActionResult ReportScam()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult ActivitySelection()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            ViewBag.ModuleTitle = "Pengenalan Kepada Phishing"; // Tajuk modul semasa
            return View();
        }

        public ActionResult ChildReport()
        {
            // 1. Semak jika pengguna sudah login dan adakah dia seorang 'Parent'
            if (Session["UserID"] == null || Session["UserCategory"]?.ToString() != "Adult")
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Data Simulasi Akaun Anak (Dalam realiti, anda akan tarik dari Database guna emel anak)
            ViewBag.ChildName = "Ahmad Syauqi";
            ViewBag.ChildEmail = Session["LinkedChildEmail"] ?? "syauqi@emel.com";
            ViewBag.ChildProgress = 65; // Peratus siap
            ViewBag.ChildLastQuiz = 80; // Skor kuiz terakhir

            return View();
        }

        public ActionResult SecurityAlert()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            // Kita simulasikan data amaran terkini
            ViewBag.AlertTitle = "Amaran: Taktik Scam SMS 'Akaun Disekat' Kembali Menular";
            ViewBag.AlertDate = "9 Januari 2025";
            ViewBag.Severity = "Tinggi";

            return View();
        }
    }
}