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

            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(r => r.Email == "jmahoney2261@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser { UserName = "jmahoney2261@Mailinator.com",
                                                        Email = "jmahoney2261@Mailinator.com",
                                                        firstName = "John",
                                                        lastName = "Mahoney",
                                                        displayName = "Lenny2261"}, "password");
            }

            var userId = userManager.FindByEmail("jmahoney2261@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}
