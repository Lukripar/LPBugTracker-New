using LPBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LPBugTracker.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }
        
        public ActionResult DemoIndex()
        {
            return View();
        }

        
    }
}