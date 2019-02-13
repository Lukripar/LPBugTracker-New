using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LPBugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        //public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<TicketNotification> Notifications { get; set; }
        public virtual ICollection<TicketHistory> Histories { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }

        public ApplicationUser()
        {
            Comments = new HashSet<TicketComment>();
            Projects = new HashSet<Project>();
            //Tickets = new HashSet<Ticket>();
            Notifications = new HashSet<TicketNotification>();
            Histories = new HashSet<TicketHistory>();
            Attachments = new HashSet<TicketAttachment>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> Comments { get; set; }
        public DbSet<TicketAttachment> Attachments { get; set; }
        public DbSet<TicketHistory> Histories { get; set; }
        public DbSet<TicketNotification> Notifications { get; set; }
        public DbSet<TicketPriority> Priorities { get; set; }
        public DbSet<TicketStatus> Statuses { get; set; }
        public DbSet<TicketType> Types { get; set; }
    }
}