using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        //private readonly ProjectHelper projHelper = new ProjectHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private NotificationHelper notifyHelper = new NotificationHelper();

        internal object ListAssignedTickets(object getuserId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Ticket> ListTicketsOnProject(string projectId)
        {
            var project = db.Projects.Find(projectId);

            var list = project.Tickets.ToList();

            return list;
        }

        public ICollection<Ticket> GetTicketsOwned(string userId)
        {

            var list = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();

            return list;
        }

        public string GetAssignedUserId(int ticketId)
        {
            return db.Tickets.Find(ticketId).AssignedUserId;
        }

        public string GetTicketOwner(int ticketId)
        {
            return db.Tickets.Find(ticketId).OwnerUserId;
        }

        public ICollection<Ticket> ListAssignedTickets (string userId)
        {
            var user = db.Users.Find(userId);

            var tickets = db.Tickets.Where(t => t.AssignedUserId == userId).ToList();
            return tickets;
        }

        public ICollection<Ticket> ListTicketsOwned (string userId)
        {
            var user = db.Users.Find(userId);

            var tickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();

            return tickets;
        }

        public ICollection<Ticket> GetOpenTickets (int projectId)
        {
            var project = db.Projects.Find(projectId);
            var openTickets = project.Tickets.Where(t => t.Status.Name == "Open");

            return openTickets.ToList();
        }

        public ICollection<Ticket> GetClosedTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var closedTickets = project.Tickets.Where(t => t.Status.Name == "Resolved");

            return closedTickets.ToList();
        }

        public ICollection<Ticket> GetUnassignedTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var unassignedTickets = project.Tickets.Where(t => t.Status.Name == "Unassigned");

            return unassignedTickets.ToList();
        }

        public ICollection<Ticket> GetInfoNeededTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var tickets = project.Tickets.Where(t => t.Status.Name == "Need More Info");

            return tickets.ToList();
        }

        public ICollection<Ticket> GetUrgentTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var urgentTickets = project.Tickets.Where(t => t.Priority.Name == "Open" || t.Priority.Name == "High");

            return urgentTickets.ToList();
        }

        public bool IsUserTicketOwner(string userId, int ticketId)
        {
            var ticket = db.Tickets.Find(ticketId);
            if (ticket.OwnerUserId != userId || ticket.OwnerUserId == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsUserAssignedToTicket(string userId, int ticketId)
        {
            var ticket = db.Tickets.Find(ticketId);
            if (ticket.AssignedUserId != userId || ticket.AssignedUserId == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task AssignUserToTicket (string userId, int ticketId)
        {
            if(!IsUserAssignedToTicket(userId, ticketId))
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticketId);
                var ticket = db.Tickets.Find(ticketId);
                ticket.AssignedUserId = userId;
                db.Entry(ticket).Property(t => t.AssignedUserId).IsModified = true;
                ChangeTicketStatus(ticketId, "Open");
                if (!HttpContext.Current.User.IsInRole("Demo"))
                {
                    db.SaveChanges();
                    await notifyHelper.Notify(oldTicket, ticket);
                    historyHelper.AddHistories(oldTicket, ticket);
                };
            }
        }

        public void ChangeTicketStatus(int ticketId, string statusName)
        {
            var ticket = db.Tickets.Find(ticketId);
            ticket.StatusId = db.Statuses.FirstOrDefault(s => s.Name == statusName).Id;
            db.SaveChanges();
        }

        public async Task UnassignUserFromTicket(int ticketId, string userId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticketId);
            var ticket = db.Tickets.Find(ticketId);
            ticket.AssignedUserId = null;
            db.Entry(ticket).Property(t => t.AssignedUserId).IsModified = true;
            ChangeTicketStatus(ticket.Id, "Unassigned");
            if(!HttpContext.Current.User.IsInRole("Demo"))
            {
                await notifyHelper.Notify(oldTicket, ticket);
                historyHelper.AddHistories(oldTicket, ticket);
                db.SaveChanges();
            }
            
        }
    }
}