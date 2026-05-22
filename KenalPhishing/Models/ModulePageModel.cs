using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KenalPhihsing.Models
{
    public class ModulePageModel
    {
        public int PageNumber { get; set; }
        public string PageTitle { get; set; }

        [AllowHtml] // This tells ASP.NET to allow HTML tags for this property only
        public string BodyContent { get; set; }
    }
}