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
    public class TicketNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketHelper ticketHelper = new TicketHelper();
        // GET: TicketNotifications
        public ActionResult Index()
        {
            var userNotifications = db.Notifications.Include(t => t.Ticket).Include(t => t.User).Where(n => n.UserId == User.Identity.GetUserId());
            return View(userNotifications.ToList());
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
