using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class LookupHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetNameFromId(string propertyName, string id)
        {
            var name = "";
            switch (propertyName)
            {
                case "PriorityId":
                    name = db.Priorities.Find(Convert.ToInt32(id)).Name;
                    break;
                case "StatusId":
                    name = db.Statuses.Find(Convert.ToInt32(id)).Name;
                    break;
                case "TypeId":
                    name = db.Types.Find(Convert.ToInt32(id)).Name;
                    break;
                case "AssignedToUserId":
                    name = db.Users.Find(id).FullName;
                    break;
            }
            return name;
        }
    }
}