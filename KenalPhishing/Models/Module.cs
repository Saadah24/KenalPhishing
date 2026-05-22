using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhihsing.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Category { get; set; } // "Elder", "Adult", "Child"
        public string ModuleCode { get; set; } // "Intro", "A", "B"
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; } // contoh: "shield-outline"
        public string ThemeColor { get; set; } // contoh: "bg-primary"

        // Relasi ke muka surat (pages)
        public virtual ICollection<ModulePage> Pages { get; set; }
    }
}