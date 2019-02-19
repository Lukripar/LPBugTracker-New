using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper ticketHelper = new TicketHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectHelper projHelper = new ProjectHelper();
        private NotificationHelper notifyHelper = new NotificationHelper();

        // GET: TicketAttachments
        

        // GET: TicketAttachments/Details/5
        

        // GET: TicketAttachments/Create
        

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,FilePath")] TicketAttachment ticketAttachment, string AttachmentDescription, HttpPostedFileBase file)
        {

            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(ticketAttachment.TicketId);
            var pmId = projHelper.GetProjectUsersInRole("Project Manager", ticket.ProjectId).FirstOrDefault().Id;
            if (!ticketHelper.IsUserAssignedToTicket(userId, ticket.Id) && !ticketHelper.IsUserTicketOwner(userId, ticket.Id) && !roleHelper.IsUserInRole(userId, "Admin") && userId != pmId)
            {
                return RedirectToAction("Index", "Profile");
            }
            if (ModelState.IsValid)
            {
                if (AttachmentHelper.IsWebFriendlyFile(file))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Path.Combine(Server.MapPath("~/Attachments/"), fileName));
                    ticketAttachment.FilePath = "/Attachments/" + fileName;
                }
                else
                {
                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }
                ticketAttachment.Created = DateTime.Now;
                ticketAttachment.Description = AttachmentDescription;
                ticketAttachment.UserId = userId;
                db.Attachments.Add(ticketAttachment);

                var comment = new TicketComment
                {
                    CommentBody = $"<small>Uploaded an Attachment titled: <i>{Path.GetFileName(file.FileName)}</i> at {DateTime.Now.ToString("h:mm tt")}" +
                    $"<br />Attachment Description: <i>{ticketAttachment.Description}</i></small>",
                    Created = DateTime.Now,
                    TicketId = ticketAttachment.TicketId,
                    UserId = ticketAttachment.UserId
                };
                db.Comments.Add(comment);
                db.SaveChanges();
                notifyHelper.attachmentNotify(comment);
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.Attachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.Attachments.Find(id);
            db.Attachments.Remove(ticketAttachment);
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
