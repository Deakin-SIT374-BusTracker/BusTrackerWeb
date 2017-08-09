using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    public class JourneysController : Controller
    {
        // GET: Journeys
        public ActionResult Index()
        {
            return View();
        }
    }
}