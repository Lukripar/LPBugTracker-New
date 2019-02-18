using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LPBugTracker.Controllers
{
    [Authorize(Roles = "Developer, Admin")]
    public class DeveloperController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();

        // GET: Developer
        public ActionResult Index(int? page, string searchStr)
        {
            var userProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            var openTicketProjects = userProjects.Where(p => p.Tickets.Where(t => t.Status.Name != "Unassigned").Count() > 0).ToList();
            ViewBag.Search = searchStr;
            var projList = SearchHelper.ProjectSearch(searchStr, openTicketProjects);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var projects = openTicketProjects.OrderByDescending(p => p.Id).ToPagedList(pageNumber, pageSize);
            return View(projList.ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult AllProjectsIndex(int? page, string searchStr)
        {
            var userProjects = projHelper.ListUserProjects(User.Identity.GetUserId());

            ViewBag.Search = searchStr;
            var projList = SearchHelper.ProjectSearch(searchStr, userProjects);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var projects = userProjects.OrderByDescending(p => p.Name).ToPagedList(pageNumber, pageSize);
            return View(projList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult MyTicketsIndex()
        {
            var userId = User.Identity.GetUserId();
            var userTickets = db.Tickets.Where(t => t.AssignedUserId == userId && t.Status.Name != "Resolved").ToList();
                

            return View(userTickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NotificationRead(int notificationId)
        {
            var notification = db.Notifications.Find(notificationId);
            notification.Read = true;
            db.Entry(notification).Property(n => n.Read).IsModified = true;
            db.SaveChanges();
            var returnUrl = Request.ServerVariables["http_referer"];
            //return Redirect(returnUrl);
            return RedirectToAction("Details", "Tickets", new { id = notification.TicketId });
        }
    }
}