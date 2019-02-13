using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LPBugTracker.Models;

namespace LPBugTracker.Controllers
{
    public class TicketPrioritiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketPriorities
        public ActionResult Index()
        {
            return View(db.Priorities.ToList());
        }

        // GET: TicketPriorities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPriority ticketPriority = db.Priorities.Find(id);
            if (ticketPriority == null)
            {
                return HttpNotFound();
            }
            return View(ticketPriority);
        }

        // GET: TicketPriorities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketPriorities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TicketPriority ticketPriority)
        {
            if (ModelState.IsValid)
            {
                db.Priorities.Add(ticketPriority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketPriority);
        }

        // GET: TicketPriorities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPriority ticketPriority = db.Priorities.Find(id);
            if (ticketPriority == null)
            {
                return HttpNotFound();
            }
            return View(ticketPriority);
        }

        // POST: TicketPriorities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TicketPriority ticketPriority)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketPriority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketPriority);
        }

        // GET: TicketPriorities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPriority ticketPriority = db.Priorities.Find(id);
            if (ticketPriority == null)
            {
                return HttpNotFound();
            }
            return View(ticketPriority);
        }

        // POST: TicketPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketPriority ticketPriority = db.Priorities.Find(id);
            db.Priorities.Remove(ticketPriority);
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
