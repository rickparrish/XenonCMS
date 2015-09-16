namespace XenonCMS.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using XenonCMS.Classes;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<XenonCMS.Classes.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(XenonCMS.Classes.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            if (!context.Roles.Any(r => r.Name == "GlobalAdmin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                manager.Create(new IdentityRole { Name = "GlobalAdmin" });
            }

            if (!context.Roles.Any(r => r.Name == "SiteAdmin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                manager.Create(new IdentityRole { Name = "SiteAdmin" });
            }

            if (context.Users.Count() == 0)
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new ApplicationUserManager(store);
                manager.UserValidator = new UserValidator<ApplicationUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

                var user = new ApplicationUser
                {
                    Email = $"XenonCMS@localhost.localdomain",
                    UserName = $"XenonCMS@localhost.localdomain"
                };

                manager.Create(user, Environment.MachineName.ToLower());
                manager.AddToRole(user.Id, "GlobalAdmin");
            }            
        }
    }
}
