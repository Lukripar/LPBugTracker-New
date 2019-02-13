using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Models
{
    public class ProjectTicketViewModel
    {

        public Project Project { get; set; }
        public IEnumerable<Ticket> tickets { get; set; }

    }
}