using KenalPhishing.Data;
using KenalPhishing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration; // Added for reading connection string
using System.Data.Entity;
using System.Data.SqlClient;

namespace KenalPhishing.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Login() { return View(); }
        public ActionResult Register() { return View(); }

        // ==========================================
        // FUNGSI REGISTER (REAL DATABASE)
        // ==========================================
        [HttpPost]
        public ActionResult Register(User model)
        {
            // If Model Binding fails for some reason, we can manually check the checkbox
            if (Request.Form["IsParent"] == "true")
            {
                model.IsParent = true;
            }

            if (ModelState.IsValid)
            {
                // 1. Check email
                var exist = db.Users.Any(u => u.Email == model.Email);
                if (exist)
                {
                    ViewBag.Error = "Emel ini sudah berdaftar.";
                    return View(model);
                }

                // 2. Parent Verification Logic
                if (model.Category == "Adult" && model.IsParent)
                {
                    if (string.IsNullOrEmpty(model.ChildEmail))
                    {
                        ViewBag.Error = "Sila masukkan emel anak.";
                        return View(model);
                    }

                    // Check if child exists in DB
                    var child = db.Users.FirstOrDefault(u => u.Email == model.ChildEmail && u.Category == "Child");
                    if (child == null)
                    {
                        ViewBag.Error = "Emel anak tidak dijumpai. Anak mesti daftar dahulu sebagai 'Golongan Muda'.";
                        return View(model);
                    }
                }

                // 3. Save
                model.Role = "Member";

                // ✅ ADD THIS: If parent, resolve ChildEmail to LinkedChildId before saving
                if (model.IsParent && !string.IsNullOrEmpty(model.ChildEmail))
                {
                    var child = db.Users.FirstOrDefault(u => u.Email == model.ChildEmail && u.Category == "Child");
                    if (child != null)
                    {
                        model.LinkedChildId = child.Id; // ✅ Save the FK
                    }
                }

                db.Users.Add(model);
                db.SaveChanges();

                // 4. Session
                Session["UserID"] = model.Id.ToString();
                Session["UserName"] = model.FullName;
                Session["UserEmail"] = model.Email;
                Session["UserCategory"] = model.Category;
                Session["IsParent"] = model.IsParent;
                Session["ChildEmail"] = model.ChildEmail;

                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        // ==========================================
        // FUNGSI LOGIN (REAL DATABASE)
        // ==========================================
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Use "UserID" (Capital ID) consistently across the whole app
                Session["UserID"] = user.Id;
                Session["UserName"] = user.FullName;
                Session["UserEmail"] = user.Email;
                Session["UserCategory"] = user.Category;
                Session["UserRole"] = user.Role;
                Session["IsParent"] = user.IsParent;
                Session["ChildEmail"] = user.ChildEmail;

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Emel atau Kata Laluan salah.";
            return View();
        }
    }
}