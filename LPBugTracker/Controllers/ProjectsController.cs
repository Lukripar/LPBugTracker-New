using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LPBugTracker.Helpers;
using LPBugTracker.Models;

namespace LPBugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
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
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            var pms = roleHelper.UsersInRole("Project Manager");
            var devs = roleHelper.UsersInRole("Developer");
            ViewBag.Developers = new MultiSelectList(devs, "Id", "FullName");
            ViewBag.ProjectManager = new SelectList(pms, "Id", "FullName");

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Project project, List<string> Developers, string ProjectManager)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();

                projHelper.AddUserToProject(ProjectManager, project.Id);
                foreach (var dev in Developers)
                {
                    projHelper.AddUserToProject(dev, project.Id);
                }
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
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

            var pms = roleHelper.UsersInRole("Project Manager");
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

            ViewBag.ProjectManager = new SelectList(pms, "Id", "FullName", assignedPM);
            ViewBag.Submitter = new SelectList(subs, "Id", "FullName", assignedSub);
            ViewBag.Developers = new MultiSelectList(devs, "Id", "FullName", AssignedDevs);
            return View(project);

            
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project, string Submitter, string ProjectManager, List<string>Developers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();

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
                if(Developers != null)
                {
                    foreach (var user in Developers)
                    {
                        projHelper.AddUserToProject(user, project.Id);
                    }
                }
                

                
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
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
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
