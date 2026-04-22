using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Controllers
{
    public class AdminController : Controller
    {
        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        public ActionResult Index()
        {
            if (!IsAdmin())
            {
                // Jika bukan admin, tendang balik ke Login
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult ManageModules()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // 3. Tambah/Edit Modul (input_file_14 - Admin-1)
        public ActionResult EditModule(int? id)
        {
            ViewBag.IsEdit = id.HasValue;
            return View();
        }
    }
}