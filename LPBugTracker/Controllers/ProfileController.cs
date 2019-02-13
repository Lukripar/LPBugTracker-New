using LPBugTracker.Helpers;
using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static LPBugTracker.Controllers.ManageController;

namespace LPBugTracker.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectHelper projHelper = new ProjectHelper();
        

        // GET: Profile
        public ActionResult Index(string id, ManageMessageId? message)

        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred"
                : message == ManageMessageId.PasswordMatchError ? "Your Passwords didn't match."
                : message == ManageMessageId.ProfileUpdateSuccess ? "Your Profile has been updated."
                : "";

            var user = db.Users.Find(id);

            ViewBag.UserProjects = projHelper.ListUserProjects(id);
            ViewBag.FirstName = user.FirstName;
            ViewBag.Email = user.Email;
            ViewBag.LastName = user.LastName;
            ViewBag.DisplayName = user.DisplayName;

            
            return View(user);
        }

        public ActionResult UpdatePicture(HttpPostedFileBase image)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if(AttachmentHelper.IsWebFriendlyImage(image))
            {
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                user.AvatarPath = "/img/" + fileName;
                db.Users.Attach(user);
                db.Entry(user).Property(u => u.AvatarPath).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = userId, Message = ManageMessageId.ProfileUpdateSuccess });
            }
            return RedirectToAction("Index", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string email, string firstname, string lastname, string displayname, string oldpassword, string newpassword, string confirmpassword)
        {
            // I extended my UserHelper class to get the current user's id quickly. 
            var userId = User.Identity.GetUserId();
            // Finds the ApplicationUser in the db with the id of userId and sets it to var user.
            var user = db.Users.Find(userId);
            if (ModelState.IsValid)
            {

                
                user.FirstName = firstname;
                user.LastName = lastname;
                user.DisplayName = displayname;

                db.Users.Attach(user);
                // setting these properties as modified.
                db.Entry(user).Property(u => u.FirstName).IsModified = true;
                db.Entry(user).Property(u => u.LastName).IsModified = true;
                db.Entry(user).Property(u => u.DisplayName).IsModified = true;
                //this is where the changes SHOULD be saved. Everything runs fine. I even get the redirectToAction with the "Profile Updated" message.
                db.SaveChanges();
                //checks if the password inputs are null or empty.
                if (!string.IsNullOrEmpty(oldpassword) || !string.IsNullOrEmpty(newpassword) || !string.IsNullOrEmpty(confirmpassword))
                {
                    //makes sure newpassword and confirm password are the same.
                    if (newpassword != confirmpassword)
                    {
                        return RedirectToAction("Index", new { Message = ManageMessageId.PasswordMatchError, id = user.Id });
                    }
                    var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                    var result = manager.ChangePassword(userId, oldpassword, newpassword);
                    //if you were able to successfully change the password based on user's inputs.
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess, id = user.Id });
                    }
                    //if you were not able to, redirect to index with an error message.
                    return RedirectToAction("Index", new { Message = ManageMessageId.PasswordMatchError, id = user.Id });
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ProfileUpdateSuccess, id = user.Id });

            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error, id = user.Id });



        }
    }
}