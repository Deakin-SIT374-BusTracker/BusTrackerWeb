using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Bus Tracker Journey View actions.
    /// </summary>
    public class JourneyController : Controller
    {
        /// <summary>
        /// Open the Your Journey View.
        /// </summary>
        /// <returns>Your Journey View.</returns>
        public ActionResult GetJourney(int runId)
        {
            ViewBag.Title = "BusHop > Your Journey";

            return View("~/Views/Journey/Journey.cshtml");
        }
    }
}
