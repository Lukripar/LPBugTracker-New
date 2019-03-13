using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LPBugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedUserId { get; set; }
        public int ProjectId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        
        [MaxLength(150)]
        public string Title { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        // Navigational Properties
        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<TicketNotification> Notifications { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; } 
        public virtual Project Project { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketType Type { get; set; }


        public Ticket()
        {
            Comments = new HashSet<TicketComment>();
            Attachments = new HashSet<TicketAttachment>();
            Notifications = new HashSet<TicketNotification>();
            Histories = new HashSet<TicketHistory>();
        }

    }
}