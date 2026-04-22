using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        
        public ActionResult Register()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Register(string fullname, string email, string password, string userCategory, bool isParent = false, string childEmail = "")
        {
            // 1. Simpan ke database (Sila pautkan emel anak dengan ID user ini jika isParent true)

            // 2. Simpan dalam Session untuk kegunaan Dashboard
            Session["UserID"] = "123";
            Session["UserName"] = fullname;
            Session["UserCategory"] = userCategory;
            Session["IsParent"] = isParent;
            Session["LinkedChildEmail"] = childEmail;

            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // === KREDENTIAL ADMIN SPESIFIK ===
            if (email == "admin@kenalphishing.com" && password == "Admin123")
            {
                Session["UserID"] = "ADMIN001";
                Session["UserName"] = "Super Admin";
                Session["UserRole"] = "Admin";
                return RedirectToAction("Index", "Admin");
            }

            // === KREDENTIAL USER BIASA (MOCKUP) ===
            // Sila gantikan dengan logic database kelak
            if (email == "user@email.com" && password == "user123")
            {
                Session["UserID"] = "USER123";
                Session["UserName"] = "Rabiatul";
                Session["UserCategory"] = "Adult"; // Ambil dari database
                Session["UserRole"] = "Member";
                return RedirectToAction("Index", "Dashboard");
            }

            // JIKA SALAH
            ViewBag.Error = "Emel atau Kata Laluan yang anda masukkan adalah salah.";
            return View();
        }
    }
}