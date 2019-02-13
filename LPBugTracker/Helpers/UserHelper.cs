using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class UserHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static ApplicationUser User = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
        public static string GetDisplayName()
        {
            return User.DisplayName;
        }
        public static string GetEmail()
        {
            return User.Email;
        }
        public static string GetAvatar()
        {
            return User.AvatarPath;
        }
        public static string GetFirstName()
        {
            return User.FirstName;
        }
        public static string GetLastName()
        {
            return User.LastName;
        }
        public static string GetFullName()
        {
            return User.FirstName + " " + User.LastName;
        }
        public static string GetId()
        {
            return User.Id;
        }
    }
}