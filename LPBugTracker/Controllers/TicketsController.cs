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

namespace LPBugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private NotificationHelper notifyHelper = new NotificationHelper();
        private HistoryHelper historyHelper = new HistoryHelper();

        // GET: Tickets
        public ActionResult Index()
        {

            var tickets = db.Tickets.Include(t => t.AssignedUser).Include(t => t.OwnerUser).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {

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
            foreach (var user in pms)
            {
                if (roleHelper.IsUserInRole(user.Id, "Project Manager"))
                {
                    assignedPM = user.FullName;
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
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerUserId,AssignedUserId,ProjectId,PriorityId,StatusId,TypeId,Title,Description,Created,Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerUserId,AssignedUserId,ProjectId,PriorityId,StatusId,TypeId,Title,Description,Created,Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
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
