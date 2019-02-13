using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Submitter, Admin")]
    public class SubmitterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectHelper projHelper = new ProjectHelper();
        private TicketHelper ticketHelper = new TicketHelper();

        // GET: Submitter
        public ActionResult Index()
        {
            var user = User.Identity.GetUserId();
            var userProjects = projHelper.ListUserProjects(user);
            
            return View(userProjects);
        }


        public ActionResult TicketCreate(int projectId)
        {
            var project = db.Projects.Find(projectId);
            if(!projHelper.IsUserOnProject(User.Identity.GetUserId(), projectId) && !roleHelper.IsUserInRole(User.Identity.GetUserId(), "Admin"))
            {
                return RedirectToAction("Index");
            }
            var defaultPriority = db.Priorities.FirstOrDefault(m => m.Name == "Low");
            var newTicket = new Ticket()
            {
                StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Unassigned").Id,
                TypeId = db.Types.FirstOrDefault(t => t.Name == "Bug").Id,
                ProjectId = projectId
            };
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", defaultPriority.Id);
            ViewBag.ProjectName = project.Name;
            return View(newTicket);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketCreate(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTime.Now;

                db.Tickets.Add(ticket);
                db.SaveChanges();
                TempData["Message"] = "Ticket submitted to " + db.Projects.Find(ticket.ProjectId).Name + " successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();

            }
            
            return View(ticket);
        }

        public ActionResult MyTicketsIndex()
        {
            var userId = User.Identity.GetUserId();
            var userTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();


            return View(userTickets);
        }

    }
}
