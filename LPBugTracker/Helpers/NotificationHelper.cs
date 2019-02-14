using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task Notify(Ticket oldTicket, Ticket newTicket)
        {
            var oldUserId = oldTicket.AssignedUserId;
            var newUserId = newTicket.AssignedUserId;

            var type = DetermineNotificationType(oldUserId, newUserId);
            if (type != NotificationType.None)
            {
                await SendEmailNotification(oldTicket, newTicket, type);
                AddNotification(oldTicket, newTicket);
            }
        }

        public void AddNotification(Ticket oldTicket, Ticket newTicket)
        {
            var oldUserId = oldTicket.AssignedUserId;
            var newUserId = newTicket.AssignedUserId;

            if (oldUserId == newUserId)
                return;

            var notification = new TicketNotification
            {
                Created = DateTime.Now,
                TicketId = newTicket.Id
            };

            if ((oldUserId == null || oldUserId == "") && (newUserId != null || newUserId != ""))
            {
                //This condition needs to trigger an Assignment Notification record
                notification.UserId = newUserId;
                notification.Message = $"You have been assigned to Ticket {newTicket.Id}";
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

            else if ((oldUserId != null || oldUserId != "") && (newUserId == null || newUserId == ""))
            {
                notification.UserId = oldUserId;
                notification.Message = $"You have been unassigned from Ticket {newTicket.Id}";
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

            else
            {
                notification.UserId = newUserId;
                notification.Message = $"You have been assigned to Ticket {newTicket.Id}";
                db.Notifications.Add(notification);

                var newNotification = new TicketNotification
                {
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    UserId = oldUserId,
                    Message = $"You have been unassigned from Ticket {newTicket.Id}"
                };
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }
        }

        public NotificationType DetermineNotificationType(string oldUserId, string newUserId)
        {
            var type = NotificationType.None;
            if (oldUserId == null && newUserId != null)
            {
                type = NotificationType.Assigned;
            }
            else if (oldUserId != null && newUserId == null)
            {
                type = NotificationType.Unassigned;
            }
            else if (oldUserId != null && newUserId != null && oldUserId != newUserId)
            {
                type = NotificationType.Reassigned;
            }

            return type;
        }

        public async Task SendEmailNotification(Ticket oldTicket, Ticket newTicket, NotificationType type)
        {
            string body = "", subject = "", toName = "", toEmail = "";
            string fromEmail = "System <System@lpbugtracker.com>";

            switch (type)
            {
                case NotificationType.Assigned:
                    subject = "Ticket Assignement";
                    body = $"You have been assigned to a ticket with an ID of {newTicket.Id}.";
                    toName = newTicket.AssignedUser.FullName;
                    toEmail = newTicket.AssignedUser.Email;

                    var message = new EmailModel
                    {
                        Body = body,
                        FromEmail = fromEmail,
                        ToEmail = toEmail,
                        Subject = subject,
                        ToName = toName
                    };

                    await SendEmail(message);
                    
                        break;

                case NotificationType.Unassigned:
                    subject = "Unassigned from a Ticket";
                    body = $"You have been unassigned from a ticket with an ID of {newTicket.Id}.";
                    toName = oldTicket.AssignedUser.FullName;
                    toEmail = oldTicket.AssignedUser.Email;
                    message = new EmailModel
                    {
                        Body = body,
                        FromEmail = fromEmail,
                        ToEmail = toEmail,
                        Subject = subject,
                        ToName = toName
                    };

                    await SendEmail(message);
                    break;

                case NotificationType.Reassigned:
                    subject = "Ticket Assignement";
                    body = $"You have been assigned to a ticket with an ID of {newTicket.Id}.";
                    toName = newTicket.AssignedUser.FullName;
                    toEmail = newTicket.AssignedUser.Email;
                    message = new EmailModel
                    {
                        Body = body,
                        FromEmail = fromEmail,
                        ToEmail = toEmail,
                        Subject = subject,
                        ToName = toName
                    };

                    await SendEmail(message);

                    subject = "Unassigned from a Ticket";
                    body = $"You have been unassigned from a ticket with an ID of {newTicket.Id}.";
                    toName = oldTicket.AssignedUser.FullName;
                    toEmail = oldTicket.AssignedUser.Email;
                    var message2 = new EmailModel
                    {
                        Body = body,
                        FromEmail = fromEmail,
                        ToEmail = toEmail,
                        Subject = subject,
                        ToName = toName
                    };

                    await SendEmail(message2);
                    break;
            }
        }

        public async Task SendEmail(EmailModel emailData)
        {
            var message = new MailMessage(emailData.FromEmail, emailData.ToEmail)
            {
                Subject = emailData.Subject,
                Body = emailData.Body,
                IsBodyHtml = true

            };

            var svc = new PersonalEmail();
            await svc.SendAsync(message);

        }

        public enum NotificationType
        {
            None,
            Unassigned,
            Reassigned,
            Assigned,
        }
        
        public void commentNotify(TicketComment comment)
        {
            if (comment.UserId != comment.Ticket.AssignedUserId)
            {
                var notification = new TicketNotification
                {
                    Created = DateTime.Now,
                    Message = db.Users.Find(comment.UserId).FullName + " left a comment on ticket: " + comment.Ticket.Title,
                    UserId = comment.Ticket.AssignedUserId,
                    TicketId = comment.TicketId
                };
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
            
        }
        public void attachmentNotify(TicketComment comment)
        {
            if (comment.UserId != comment.Ticket.AssignedUserId)
            {
                var notification = new TicketNotification
                {
                    Created = DateTime.Now,
                    Message = db.Users.Find(comment.UserId).FullName + " added an attachment on ticket: " + comment.Ticket.Title,
                    UserId = comment.Ticket.AssignedUserId,
                    TicketId = comment.TicketId
                };
                db.Notifications.Add(notification);
                db.SaveChanges();
            }

        }
    }
}