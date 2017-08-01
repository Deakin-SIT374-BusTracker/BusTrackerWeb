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
            ViewBag.Title = "BusHop > Search";

            return View();
        }

        /// <summary>
        /// Return the search results as a partial view.
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchRoutes()
        {
            return PartialView("~/Views/Journey/_SearchRoutes.cshtml");
        }

        /// <summary>
        /// Open the Journeys View.
        /// </summary>
        /// <returns>Journeys View.</returns>
        public ActionResult Journeys()
        {
            ViewBag.Title = "BusHop > Journeys";

            return View();
        }

        /// <summary>
        /// Open the Your Journey View.
        /// </summary>
        /// <returns>Your Journey View.</returns>
        public ActionResult YourJourney()
        {
            ViewBag.Title = "BusHop > Your Journey";

            return View();
        }
    }
}
