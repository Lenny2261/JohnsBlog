namespace BlogProject.Migrations
{
    using BlogProject.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BlogProject.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if(!context.Roles.Any(r => r.Name == "Mod"))
            {
                roleManager.Create(new IdentityRole { Name = "Mod" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(r => r.Email == "araynor@Coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser { UserName = "araynor@Coderfoundry.com",
                                                        Email = "araynor@Coderfoundry.com",
                                                        firstName = "Antonio",
                                                        lastName = "Raynor",
                                                        displayName = "ANIVRA"
                }, "Abc&123!");
            }

            var userId = userManager.FindByEmail("araynor@Coderfoundry.com").Id;
            userManager.AddToRole(userId, "Mod");
        }
    }
}
