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
    public class AjaxDataController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();
        // GET: AjaxData
        public JsonResult GetTicketTypes()
        {
            var tickets = db.Tickets;
            var typeList = new List<PieData>();

            var ticketTypes = db.Types.ToList();
            foreach (var type in ticketTypes)
            {
                typeList.Add(new PieData
                {
                    name = type.Name,
                    value = tickets.Where(t => t.Type.Name == type.Name).Count()
                });

            }
            //var data = Json(typeList);
            //return Json(data.Data);
            return Json(typeList);
        }

        public JsonResult GetTicketPriorities()
        {
            var tickets = db.Tickets;
            var prioList = new List<PieData>();

            var ticketPriorities = db.Priorities.ToList();
            foreach (var prio in ticketPriorities)
            {
                prioList.Add(new PieData
                {
                    name = prio.Name,
                    value = tickets.Where(t => t.Priority.Name == prio.Name).Count()
                });
            }
            //var data = Json(prioList);
            //return Json(data.Data);
            return Json(prioList);
        }

        public JsonResult GetDevTicketTypes()
        {
            var userId = User.Identity.GetUserId();
            var ticketList = db.Tickets.Where(t => t.AssignedUserId == userId);
            var ticketTypes = db.Types.ToList();
            var typeList = new List<PieData>();
            foreach (var type in ticketTypes)
            {
                typeList.Add(new PieData
                {
                    name = type.Name,
                    value = ticketList.Where(t => t.Type.Name == type.Name).Count()
                });
            }
            return Json(typeList);
        }

        public JsonResult GetPmTicketTypes()
        {
            var userId = User.Identity.GetUserId();
            var userProjects = projHelper.ListUserProjects(userId);
            var ticketList = new List<Ticket>();
            var typeList = new List<PieData>();
            var ticketTypes = db.Types.ToList();
            foreach (var project in userProjects)
            {
                foreach (var ticket in project.Tickets)
                {
                    ticketList.Add(ticket);
                }
            }
            foreach (var type in ticketTypes)
            {
                typeList.Add(new PieData
                {
                    name = type.Name,
                    value = ticketList.Where(t => t.Type.Name == type.Name).Count()
                });
            }
            return Json(typeList);
        }

        public JsonResult GetPmTicketPriorities()
        {
            var userId = User.Identity.GetUserId();
            var userProjects = projHelper.ListUserProjects(userId);
            var ticketList = new List<Ticket>();
            var priorityList = new List<PieData>();
            var ticketPriorities = db.Priorities.ToList();
            foreach (var project in userProjects)
            {
                foreach (var ticket in project.Tickets)
                {
                    ticketList.Add(ticket);
                }
            }
            foreach (var priority in ticketPriorities)
            {
                priorityList.Add(new PieData
                {
                    name = priority.Name,
                    value = ticketList.Where(t => t.Priority.Name == priority.Name).Count()
                });
            }
            return Json(priorityList);
        }
    }
}