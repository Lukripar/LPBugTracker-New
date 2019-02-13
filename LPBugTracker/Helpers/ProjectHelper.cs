using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private readonly TicketHelper ticketHelper = new TicketHelper();

        public ICollection<string> GetProjectUserEmailsInRole(string roleName, int projectId)
        {
            var users = UsersOnProject(projectId);
            var usersInRole = new List<string>();

            
            foreach (var user in users)
            {
                if (roleHelper.IsUserInRole(user.Id, roleName))
                {
                    usersInRole.Add(user.Email);
                }

            }
            return usersInRole;

        }

        public ICollection<ApplicationUser> GetProjectUsersInRole(string roleName, int projectId)
        {
            var users = UsersOnProject(projectId);
            var usersInRole = new List<ApplicationUser>();


            foreach (var user in users)
            {
                if (roleHelper.IsUserInRole(user.Id, roleName))
                {
                    usersInRole.Add(user);
                }

            }
            return usersInRole;

        }

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return flag;
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return projects;
        }

        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);

                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }
        public async Task RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified; //just saves this obj instance.

                var userTickets = db.Tickets.Where(t => t.AssignedUserId == delUser.Id);
                if (userTickets.Count() > 0)
                {
                    foreach (var ticket in userTickets)
                    {
                        await ticketHelper.UnassignUserFromTicket(ticket.Id, delUser.Id);
                    }
                }
                
                
                db.SaveChanges();
            }
        }

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
    }
}