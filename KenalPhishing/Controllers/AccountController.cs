using KenalPhihsing.Data;
using KenalPhihsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Controllers
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
            // 1. Cari user dalam database yang sepadan emel & password
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // 2. Simpan data dalam Session
                Session["UserID"] = user.Id.ToString();
                Session["UserName"] = user.FullName;
                Session["UserEmail"] = user.Email;
                Session["UserCategory"] = user.Category;
                Session["UserRole"] = user.Role;

                // --- TAMBAH DUA BARIS INI (WAJIB) ---
                Session["IsParent"] = user.IsParent;
                Session["ChildEmail"] = user.ChildEmail;
                // ------------------------------------

                // 3. Redirect ikut Role
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            // Jika gagal
            ViewBag.Error = "Emel atau Kata Laluan salah.";
            return View();
        }
    }
}