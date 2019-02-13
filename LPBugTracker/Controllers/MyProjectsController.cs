using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Project Manager, Developer, Submitter")]
    public class MyProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();


        // GET: ProjectManager
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var projects = db.Projects.ToList();
            var userProjects = new List<Project>();
            foreach (var project in projects)
            {
                if (projHelper.IsUserOnProject(userId, project.Id))
                {
                    userProjects.Add(project);
                }
            }

            return View(userProjects);
        }
        [Authorize(Roles = "Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            var projUsers = projHelper.UsersOnProject(id ?? 0);

            string assignedPM = "";
            string assignedSub = "";
            List<string> AssignedDevs = new List<string>();
            
            var subs = roleHelper.UsersInRole("Submitter");
            var devs = roleHelper.UsersInRole("Developer");
            foreach (var user in projUsers)
            {
                if (roleHelper.IsUserInRole(user.Id, "Project Manager"))
                {
                    assignedPM = user.Id;
                }
                if (roleHelper.IsUserInRole(user.Id, "Submitter"))
                {
                    assignedSub = user.Id;
                }
                if (roleHelper.IsUserInRole(user.Id, "Developer"))
                {
                    AssignedDevs.Add(user.Id);
                }
            }
            
            ViewBag.Submitter = new SelectList(subs, "Id", "FullName", assignedSub);
            ViewBag.Developers = new MultiSelectList(devs, "Id", "FullName", AssignedDevs);
            ViewBag.ProjectManager = assignedPM;
            return View(project);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project, string Submitter, string ProjectManager, List<string> Developers)
        {
            if (ModelState.IsValid)
            {
                //Remove Everyone from the Project and add back the chosen ones
                foreach (var user in projHelper.UsersOnProject(project.Id).ToList())
                {
                    projHelper.RemoveUserFromProject(user.Id, project.Id);
                }

                //add back the PM
                projHelper.AddUserToProject(ProjectManager, project.Id);

                //add back the submitter?
                projHelper.AddUserToProject(Submitter, project.Id);

                //add developers
                if (Developers != null)
                {
                    foreach (var user in Developers)
                    {
                        projHelper.AddUserToProject(user, project.Id);
                    }
                }
                db.Projects.Attach(project);
                db.Entry(project).Property(p => p.Name).IsModified = true;
                db.Entry(project).Property(p => p.Description).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        [Authorize(Roles = "Project Manager")]
        public ActionResult Create()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.User = currentUser;
            var devs = roleHelper.UsersInRole("Developer");
            ViewBag.Developers = new SelectList(devs, "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project, List<string> Developers)
        {
            if (ModelState.IsValid)
            {

                db.Projects.Add(project);
                db.SaveChanges();

                foreach (var dev in Developers)
                {
                    projHelper.AddUserToProject(dev, project.Id);
                }
                projHelper.AddUserToProject(User.Identity.GetUserId(), project.Id);

                
                return RedirectToAction("Index");
            }

            return View(project);
        }

        public ActionResult Details(int Id)
        {

            var project = db.Projects.Find(Id);

            string assignedPM = "";
            string assignedSub = "";
            List<string> AssignedDevs = new List<string>();
            var projUsers = projHelper.UsersOnProject(Id);

            var subs = roleHelper.UsersInRole("Submitter");
            var devs = roleHelper.UsersInRole("Developer");
            foreach (var user in projUsers)
            {
                if (roleHelper.IsUserInRole(user.Id, "Project Manager"))
                {
                    assignedPM = user.FullName;
                }
                if (roleHelper.IsUserInRole(user.Id, "Submitter"))
                {
                    assignedSub = user.FullName;
                }
                if (roleHelper.IsUserInRole(user.Id, "Developer"))
                {
                    AssignedDevs.Add(user.FullName);
                }
            }

            if (!projHelper.IsUserOnProject(User.Identity.GetUserId(), Id))
            {
                return RedirectToAction("Index");
            }

            var tickets = project.Tickets;
            var openTickets = tickets.Where(t => t.Status.Name == "Open");
            var closedTickets = tickets.Where(t => t.Status.Name == "Resolved");
            var unassignedTickets = tickets.Where(t => t.Status.Name == "Unassigned");
            var moreInfoTickets = tickets.Where(t => t.Status.Name == "Need More Info");

            ViewBag.Tickets = tickets;
            ViewBag.OpenTickets = openTickets;
            ViewBag.closedTickets = closedTickets;
            ViewBag.UnassignedTickets = unassignedTickets;
            ViewBag.MoreInfoTickets = moreInfoTickets;

            ViewBag.TicketsCount = tickets.Count();
            ViewBag.OpenTicketsCount = openTickets.Count();
            ViewBag.closedTicketsCount = closedTickets.Count();
            ViewBag.UnassignedTicketsCount = unassignedTickets.Count();
            ViewBag.MoreInfoTicketsCount = moreInfoTickets.Count();

            ViewBag.Submitters = assignedSub;
            ViewBag.Developers = AssignedDevs;
            ViewBag.PM = assignedPM;
            return View(project);
        }

    }
}