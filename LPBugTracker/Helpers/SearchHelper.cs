using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class SearchHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static ProjectHelper projHelper = new ProjectHelper();
        
        public static IQueryable<Project> IndexSearch(string searchStr)
        {
            IQueryable<Project> result = null;
            if (searchStr != null)
            {
                result = db.Projects.AsQueryable();
                result = result.Where(p => p.Name.Contains(searchStr) ||
                    p.Description.Contains(searchStr) ||

                    p.Users.Any(u => u.Email.Contains(searchStr)) ||
                    p.Users.Any(u => u.FirstName.Contains(searchStr)) ||
                    p.Users.Any(u => u.LastName.Contains(searchStr)) ||
                    p.Tickets.Any(u => u.Title.Contains(searchStr))
                );
            }
            else
            {
                result = db.Projects.AsQueryable();
            }
            return result.OrderByDescending(p => p.Id);
        }

        public static IQueryable<Project> ProjectSearch(string searchStr, ICollection<Project> projects)
        {
            
            IQueryable<Project> result = null;
            if (searchStr != null)
            {
                searchStr = searchStr.ToLower();
                result = projects.AsQueryable();
                result = result.Where(p => p.Name.ToLower().Contains(searchStr) ||
                    p.Description.Contains(searchStr) ||

                    p.Users.Any(u => u.Email.ToLower().Contains(searchStr)) ||
                    p.Users.Any(u => u.FirstName.ToLower().Contains(searchStr)) ||
                    p.Users.Any(u => u.LastName.ToLower().Contains(searchStr)) ||
                    p.Tickets.Any(u => u.Title.ToLower().Contains(searchStr))
                );
            }
            else
            {
                result = projects.AsQueryable();
            }
            return result.OrderByDescending(p => p.Id);
        }
        

        
    }

}