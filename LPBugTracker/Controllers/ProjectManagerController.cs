using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Project Manager, Admin")]
    public class ProjectManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectHelper projHelper = new ProjectHelper();
        private TicketHelper ticketHelper = new TicketHelper();

        // GET: ProjectManager
        public ActionResult Index()
        {
            var user = User.Identity.GetUserId();
            var userProjects = projHelper.ListUserProjects(user);
            

            return View(userProjects);
        }

        [Authorize(Roles = "Project Manager, Admin")]
        public ActionResult ProjectCreate()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.User = currentUser;
            var devs = roleHelper.UsersInRole("Developer");
            ViewBag.Developers = new MultiSelectList(devs, "Id", "FullName");
            var subs = roleHelper.UsersInRole("Submitter");
            ViewBag.Submitters = new MultiSelectList(subs, "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectCreate(Project project, List<string> Developers, List<string> Submitters)
        {
            if (ModelState.IsValid)
            {

                db.Projects.Add(project);
                db.SaveChanges();

                foreach (var dev in Developers)
                {
                    projHelper.AddUserToProject(dev, project.Id);
                }
                foreach (var sub in Submitters)
                {
                    projHelper.AddUserToProject(sub, project.Id);
                }
                projHelper.AddUserToProject(User.Identity.GetUserId(), project.Id);
                

                return RedirectToAction("Index");
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignTicket(int ticketId, string Developers)
        {

            await ticketHelper.AssignUserToTicket(Developers, ticketId);
            TempData["Message"] = "Ticket Assigned.";
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return RedirectToAction("Index", "ProjectManager");
        }


        public ActionResult Edit (int projectId)
        {
            var project = db.Projects.Find(projectId);
            if (!projHelper.IsUserOnProject(User.Identity.GetUserId(), projectId) && !roleHelper.IsUserInRole(User.Identity.GetUserId(), "Admin"))
            {
                return RedirectToAction("Index");
            }
            
            var devs = roleHelper.UsersInRole("Developer");
            var AssignedDevs = new List<string>();
            foreach (var user in project.Users)
            {
                if (roleHelper.IsUserInRole(user.Id, "Developer"))
                {
                    AssignedDevs.Add(user.Id);
                }
            }

            ViewBag.Developers = new MultiSelectList(devs, "Id", "FullName", AssignedDevs);

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit (Project project, List<string> Developers)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //remove devs from project
                    var AssignedDevs = projHelper.GetProjectUsersInRole("Developer", project.Id);
                    foreach (var user in AssignedDevs)
                    {
                        await projHelper.RemoveUserFromProject(user.Id, project.Id);
                    }

                    //add them back on.
                    foreach (var user in Developers)
                    {
                        projHelper.AddUserToProject(user, project.Id);
                    }
                    TempData["Message"] = "Successfully changed the Assigned Developers";
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    TempData["Message"] = "An Error Occured when trying to update assigned Devs.";
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    return RedirectToAction("Index");
                }
            }

            return View(project);


            
        }

        public ActionResult EditProjectDetails(int projectId)
        {
            var userId = User.Identity.GetUserId();
            if (!roleHelper.IsUserInRole(userId, "Admin") && !projHelper.IsUserOnProject(userId, projectId))
            {
                return RedirectToAction("Index");
            }
            var project = db.Projects.Find(projectId);

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectDetails(int projectId, string ProjectName, string ProjectDescription)
        {
            var project = db.Projects.Find(projectId);
            var oldName = project.Name;
            project.Name = ProjectName;
            project.Description = ProjectDescription;
            db.Projects.Attach(project);
            db.Entry(project).Property(p => p.Name).IsModified = true;
            db.Entry(project).Property(p => p.Description).IsModified = true;
            db.SaveChanges();

            TempData["Message"] = "Project Name and Description Updated Successfully!";
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return RedirectToAction("Index");
        }

        public ActionResult UnassignedTickets()
        {
            var userId = User.Identity.GetUserId();
            var unassignedTickets = new List<Ticket>();
            if (roleHelper.IsUserInRole(userId, "Admin"))
            {
                unassignedTickets = db.Tickets.Where(t => t.Status.Name == "Unassigned").ToList();
            }
            else
            {
                
                var userProjects = projHelper.ListUserProjects(userId);
                foreach (var project in userProjects)
                {
                    foreach (var ticket in project.Tickets)
                    {
                        if (ticket.Status.Name == "Unassigned")
                        {
                            unassignedTickets.Add(ticket);
                        }
                    }
                }
            }
            
            return View(unassignedTickets);
        }

        public ActionResult AllProjects()
        {
            var projects = db.Projects;

            return View(projects);
        }

        public ActionResult ProjectDetails(int projectId)
        {
            var project = db.Projects.Find(projectId);

            return View(project);
        }

    }
}