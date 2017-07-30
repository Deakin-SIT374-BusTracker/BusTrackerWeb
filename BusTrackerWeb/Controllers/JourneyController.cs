using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Open the Search View.
        /// </summary>
        /// <returns>Search View.</returns>
        public ActionResult Search()
        {
            ViewBag.Title = "Search Page";

            return View();
        }

        /// <summary>
        /// Open the Journeys View.
        /// </summary>
        /// <returns>Journeys View.</returns>
        public ActionResult Journeys()
        {
            ViewBag.Title = "Journeys Page";

            return View();
        }

        /// <summary>
        /// Open the Your Journey View.
        /// </summary>
        /// <returns>Your Journey View.</returns>
        public ActionResult YourJourney()
        {
            ViewBag.Title = "Your Journey Page";

            return View();
        }

    }
}
