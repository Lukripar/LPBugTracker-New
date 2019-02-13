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
    [Authorize(Roles = "Developer, Admin")]
    public class DeveloperController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();

        // GET: Developer
        public ActionResult Index()
        {
            var userProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            var openTicketProjects = userProjects.Where(p => p.Tickets.Where(t => t.Status.Name != "Unassigned").Count() > 0).ToList();
            return View(openTicketProjects);
        }

        public ActionResult AllProjectsIndex()
        {
            var userProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            return View(userProjects);
        }

        public ActionResult MyTicketsIndex()
        {
            var userId = User.Identity.GetUserId();
            var userTickets = db.Tickets.Where(t => t.AssignedUserId == userId && t.Status.Name != "Resolved").ToList();
                

            return View(userTickets);
        }
    }
}