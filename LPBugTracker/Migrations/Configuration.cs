namespace LPBugTracker.Migrations
{
    using LPBugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LPBugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LPBugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            // Add users to the system
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "lukascparsons@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "lukascparsons@gmail.com",
                    Email = "lukascparsons@gmail.com",
                    FirstName = "Lukas",
                    LastName = "Parsons",
                    DisplayName = "LParsons"
                }, "martin95");
            }

            //assign users to roles.
            var userId = userManager.FindByEmail("lukascparsons@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");


            //Project Managers
            if (!context.Users.Any(u => u.Email == "DemoProj@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoProj@mailinator.com",
                    Email = "DemoProj@mailinator.com",
                    FirstName = "Demo",
                    LastName = "PM",
                    DisplayName = "DemoPM1"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoProj@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //

            if (!context.Users.Any(u => u.Email == "DemoProj2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoProj2@mailinator.com",
                    Email = "DemoProj2@mailinator.com",
                    FirstName = "Demo2",
                    LastName = "PM2",
                    DisplayName = "DemoPM2"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoProj2@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //

            if (!context.Users.Any(u => u.Email == "DemoProj3@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoProj3@mailinator.com",
                    Email = "DemoProj3@mailinator.com",
                    FirstName = "Demo3",
                    LastName = "PM3",
                    DisplayName = "DemoPM3"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoProj3@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            // Developers

            if (!context.Users.Any(u => u.Email == "DemoDev@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoDev@mailinator.com",
                    Email = "DemoDev@mailinator.com",
                    FirstName = "Demo",
                    LastName = "Dev",
                    DisplayName = "DemoDev1"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoDev@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //

            if (!context.Users.Any(u => u.Email == "DemoDev2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoDev2@mailinator.com",
                    Email = "DemoDev2@mailinator.com",
                    FirstName = "Demo2",
                    LastName = "Dev2",
                    DisplayName = "DemoDev2"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoDev2@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //

            if (!context.Users.Any(u => u.Email == "DemoDev3@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoDev3@mailinator.com",
                    Email = "DemoDev3@mailinator.com",
                    FirstName = "Demo3",
                    LastName = "Dev3",
                    DisplayName = "DemoDev3"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoDev3@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            // Submitters

            if (!context.Users.Any(u => u.Email == "DemoSubmitter@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoSubmitter@mailinator.com",
                    Email = "DemoSubmitter@mailinator.com",
                    FirstName = "Demo",
                    LastName = "Submitter",
                    DisplayName = "DemoSub1"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoSubmitter@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            //

            if (!context.Users.Any(u => u.Email == "DemoSubmitter2@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoSubmitter2@mailinator.com",
                    Email = "DemoSubmitter2@mailinator.com",
                    FirstName = "Demo2",
                    LastName = "Submitter2",
                    DisplayName = "DemoSub2"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoSubmitter2@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            //

            if (!context.Users.Any(u => u.Email == "DemoSubmitter3@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoSubmitter3@mailinator.com",
                    Email = "DemoSubmitter3@mailinator.com",
                    FirstName = "Demo3",
                    LastName = "Submitter3",
                    DisplayName = "DemoSub3"
                }, "martin95");
            }

            userId = userManager.FindByEmail("DemoSubmitter3@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");


            //Demo users for interview.

            if (!context.Users.Any(u => u.Email == "SolutionCenterDemoAdmin@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "SolutionCenterDemoAdmin@mailinator.com",
                    Email = "SolutionCenterDemoAdmin@mailinator.com",
                    FirstName = "Lukas",
                    LastName = "Parsons",
                    DisplayName = "LParsons (Admin)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("SolutionCenterDemoAdmin@mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            //

            if (!context.Users.Any(u => u.Email == "SolutionCenterDemoPM@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "SolutionCenterDemoPM@mailinator.com",
                    Email = "SolutionCenterDemoPM@mailinator.com",
                    FirstName = "Carlos",
                    LastName = "Corea",
                    DisplayName = "JCCorea (Project Manager)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("SolutionCenterDemoPM@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //

            if (!context.Users.Any(u => u.Email == "SolutionCenterDemoDev@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "SolutionCenterDemoDev@mailinator.com",
                    Email = "SolutionCenterDemoDev@mailinator.com",
                    FirstName = "Brent",
                    LastName = "Davis",
                    DisplayName = "BDavis (Developer)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("SolutionCenterDemoDev@mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //

            if (!context.Users.Any(u => u.Email == "SolutionCenterDemoSub@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "SolutionCenterDemoSub@mailinator.com",
                    Email = "SolutionCenterDemoSub@mailinator.com",
                    FirstName = "Natosha",
                    LastName = "Sanders",
                    DisplayName = "NSanders (Submitter)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("SolutionCenterDemoSub@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            //

            if (!context.Users.Any(u => u.Email == "jtwichell@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtwichell@mailinator.com",
                    Email = "jtwichell@mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "JTwichell (Project Manager)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("jtwichell@mailinator.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            //

            if (!context.Users.Any(u => u.Email == "amyparsons@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "amyparsons@mailinator.com",
                    Email = "amyparsons@mailinator.com",
                    FirstName = "Amy",
                    LastName = "Parsons",
                    DisplayName = "AParsons (Submitter)"
                }, "martin95!");
            }

            userId = userManager.FindByEmail("amyparsons@mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            context.Priorities.AddOrUpdate(
                t => t.Name,
                    new TicketPriority { Name = "None" },
                    new TicketPriority { Name = "Low" },
                    new TicketPriority { Name = "Medium" },
                    new TicketPriority { Name = "High" },
                    new TicketPriority { Name = "Urgent" }
                );
            context.Statuses.AddOrUpdate(
                t => t.Name,
                    new TicketStatus { Name = "Unassigned" },
                    new TicketStatus { Name = "Open" },
                    new TicketStatus { Name = "Resolved" },
                    new TicketStatus { Name = "Need More Info" }
                );
            context.Types.AddOrUpdate(
                t => t.Name,
                    new TicketType { Name = "Bug" },
                    new TicketType { Name = "Feature Request" },
                    new TicketType { Name = "Documentation" },
                    new TicketType { Name = "Other" }
                );

            context.Projects.AddOrUpdate(
                p => p.Name,
                    new Project { Name = "Sunset View Inventory Application", Description = "IMS Application for managing Inventory for our client, Sunset View." },
                    new Project { Name = "Wayward Moose Logistics System", Description = "Application for tracking shipments for Wayward Moose Logistics." },
                    new Project { Name = "Goal Works Point-of-Sale", Description = "A POS System Designed for Goal Works." }
                );

        }
    }
}

