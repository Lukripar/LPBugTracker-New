using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private NotificationHelper notifyHelper = new NotificationHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private ProjectHelper projHelper = new ProjectHelper();

        

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            var project = ticket.Project;
            var pms = roleHelper.UsersInRole("Project Manager");
            var assignedPM = "";
            var pmId = "";
            foreach (var user in ticket.Project.Users)
            {
                if (roleHelper.IsUserInRole(user.Id, "Project Manager"))
                {
                    assignedPM = user.FullName;
                    pmId = user.Id;
                }
            }
            ViewBag.ProjectManagerName = assignedPM;
            var priorities = db.Priorities;
            var currentPriority = ticket.PriorityId;
            ViewBag.Priorities = new SelectList(priorities, "Id", "Name", currentPriority);
            var statuses = db.Statuses.Where(s => s.Name != "Unassigned");
            var currentStatus = ticket.StatusId;
            ViewBag.Statuses = new SelectList(statuses, "Id", "Name", currentStatus);
            var types = db.Types;
            var currentType = ticket.TypeId;
            ViewBag.Types = new SelectList(types, "Id", "Name", currentType);
            if(User.IsInRole("Admin") || userId == pmId || userId == ticket.AssignedUserId || userId == ticket.OwnerUserId)
            {
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        }

        public ActionResult TitleEdit(int id)
        {
            var ticket = db.Tickets.Find(id);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TitleEdit(string Title, string Description, int ticketId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticketId);
            var ticket = db.Tickets.Find(ticketId);
            if (ticket == null || Title == null || Description == null)
            {
                return View(ticket);
            }

            ticket.Title = Title;
            ticket.Description = Description;

            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property(t => t.Title).IsModified = true;
            db.Entry(ticket).Property(t => t.Description).IsModified = true;
            if(!User.IsInRole("Demo"))
            {
                db.SaveChanges();
                await notifyHelper.Notify(oldTicket, ticket);
                historyHelper.AddHistories(oldTicket, ticket);
            };

            TempData["TicketMessage"] = "Ticket Updated Successfully.";
            return RedirectToAction("Details", new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateTicketDetails(int Types, int? Statuses, int Priorities, int ticketId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticketId);
            var ticket = db.Tickets.Find(ticketId);
            if (Statuses != null)
            {
                ticket.StatusId = Statuses.GetValueOrDefault();
            }
            ticket.TypeId = Types;
            ticket.PriorityId = Priorities;
            db.Tickets.Attach(ticket);
            if (Statuses != null)
            {
                db.Entry(ticket).Property(t => t.StatusId).IsModified = true;
            }
            db.Entry(ticket).Property(t => t.TypeId).IsModified = true;
            db.Entry(ticket).Property(t => t.PriorityId).IsModified = true;
            if (!User.IsInRole("Demo"))
            {
                db.SaveChanges();
                await notifyHelper.Notify(oldTicket, ticket);
                historyHelper.AddHistories(oldTicket, ticket);
            };
            TempData["TicketMessage"] = "Ticket Details Updated.";


            return RedirectToAction("Details", new { id = ticketId });
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
