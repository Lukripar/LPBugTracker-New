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
    [Authorize(Roles = "Admin, Submitter, Developer")]
    public class TicketNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper ticketHelper = new TicketHelper();
        // GET: TicketNotifications
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userNotifications = db.Notifications.Include(t => t.Ticket).Include(t => t.User).Where(n => n.UserId == userId && n.Read != true);
            return View(userNotifications.ToList());
        }
        
        public ActionResult AllNotifications()
        {
            var userId = User.Identity.GetUserId();
            var userNotifications = db.Notifications.Include(t => t.Ticket).Include(t => t.User).Where(n => n.UserId == userId);
            return View(userNotifications.ToList());
        }

        public ActionResult MarkAsRead(int notificationId)
        {
            
            var notification = db.Notifications.Find(notificationId);
            if (User.Identity.GetUserId() != notification.UserId)
                return RedirectToAction("Index", "Profile");
            notification.Read = true;
            db.Notifications.Attach(notification);
            db.Entry(notification).Property(n => n.Read).IsModified = true;
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
