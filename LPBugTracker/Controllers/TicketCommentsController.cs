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
using Microsoft.AspNet.Identity;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper ticketHelper = new TicketHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectHelper projHelper = new ProjectHelper();
        private NotificationHelper notifyHelper = new NotificationHelper();
        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComments = db.Comments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.Comments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int TicketId, [ModelBinder(typeof(AllowHtmlBinder))] string CommentBody)
        {

            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(TicketId);
            var pmId = projHelper.GetProjectUsersInRole("Project Manager", ticket.ProjectId).FirstOrDefault().Id;
            if (!ticketHelper.IsUserAssignedToTicket(userId, TicketId) && !ticketHelper.IsUserTicketOwner(userId, TicketId) && !roleHelper.IsUserInRole(userId, "Admin") && userId != pmId)
            {
                return RedirectToAction("Index", "Profile");
            }
                var newComment = new TicketComment()
            {
                CommentBody = CommentBody,
                TicketId = TicketId,
                UserId = User.Identity.GetUserId(),
                Created = DateTime.Now
            };

            db.Comments.Add(newComment);
            db.SaveChanges();
            notifyHelper.commentNotify(newComment);
            return RedirectToAction("Details", "Tickets", new { id = TicketId });
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.Comments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,CommentBody,Created")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.Comments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.Comments.Find(id);
            db.Comments.Remove(ticketComment);
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
