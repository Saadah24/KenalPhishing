namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KenalPhihsing.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "KenalPhihsing.Data.ApplicationDbContext";
        }

        protected override void Seed(KenalPhihsing.Data.ApplicationDbContext context)
        {
            /// Cek jika emel admin sudah wujud dalam DB
            if (!context.Users.Any(u => u.Email == "admin@kenalphishing.com"))
            {
                context.Users.Add(new Models.User
                {
                    FullName = "Super Admin",
                    Email = "admin@kenalphishing.com",
                    Password = "Admin123", // Anda boleh tukar nanti
                    Role = "Admin",
                    Category = "Adult"
                });
            }
        }
    }

}
