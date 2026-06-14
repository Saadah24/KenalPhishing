using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhishing.Controllers
{
    public class LearningController : Controller
    {
        // GET: Learning
        public ActionResult Explore(string category)
        {
            // Hantar kategori yang dipilih ke View
            ViewBag.SelectedCategory = category;
            return View();
        }

        
    }
    
}