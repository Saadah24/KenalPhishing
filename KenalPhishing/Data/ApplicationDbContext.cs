using KenalPhishing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KenalPhishing.Data
{
    public class ApplicationDbContext : DbContext
    {
        // "DefaultConnection" merujuk kepada sambungan dalam fail Web.config
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        // Senaraikan jadual (Tables) yang kita nak dalam Database
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModulePage> ModulePages { get; set; }


        // TAMBAHKAN BARIS INI:
        public DbSet<User> Users { get; set; }

       
        public DbSet<ScamReport> ScamReports { get; set; }
        public DbSet<SecurityAlert> SecurityAlerts { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
    }
}