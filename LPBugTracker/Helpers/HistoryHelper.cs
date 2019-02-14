using LPBugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace LPBugTracker.Helpers
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void AddHistories(Ticket oldTicket, Ticket newTicket)
        {
            foreach (var propertyInfo in oldTicket.GetType().GetProperties())
            {
                var trackedProperties = WebConfigurationManager.AppSettings["propertyList"].Split(',').ToList();
                if (!trackedProperties.Contains(propertyInfo.Name))
                    continue;

                var oldProp = oldTicket.GetType().GetProperty(propertyInfo.Name);
                var newProp = newTicket.GetType().GetProperty(propertyInfo.Name);

                var oldPropValue = oldProp.GetValue(oldTicket, null);
                var newPropValue = newProp.GetValue(newTicket, null);
                if (oldPropValue == null || newPropValue == null)
                {
                    Console.WriteLine("Is Null");
                }
                oldPropValue = oldPropValue ?? "";
                newPropValue = newPropValue ?? "";
                if (oldPropValue.ToString() != newPropValue.ToString())
                {
                    var history = new TicketHistory
                    {
                        Changed = DateTime.Now,
                        OldValue = oldPropValue.ToString(),
                        NewValue = newPropValue.ToString(),
                        Property = propertyInfo.Name,
                        TicketId = newTicket.Id,
                        UserId = HttpContext.Current.User.Identity.GetUserId()
                    };

                    db.Histories.Add(history);

                    if (newProp.Name == "Status" && newProp.GetValue(newTicket).ToString() == "Resolved")
                    {
                        var notification = new TicketNotification
                        {
                            Created = DateTime.Now,
                            Message = $"Your ticket with ID of {newTicket.Id} has been marked CLOSED.",
                            TicketId = newTicket.Id,
                            UserId = newTicket.OwnerUserId
                        };
                        db.Notifications.Add(notification);
                    }
                    else if (newProp.Name == "Status" && newProp.GetValue(newTicket).ToString() == "Need More Info")
                    {
                        var notification = new TicketNotification
                        {
                            Created = DateTime.Now,
                            Message = $"More Info is required for your ticket with an ID of {newTicket.Id}. Please view the Ticket Details and contact the Assigned Dev for more info.",
                            TicketId = newTicket.Id,
                            UserId = newTicket.OwnerUserId
                        };
                        db.Notifications.Add(notification);
                    }

                    db.SaveChanges();
                }
            }
        }


    }
}