using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult> SearchRoutes(string destination)
        {
            List<RouteModel> routes = await WebApiApplication.PtvApiControl.GetRoutesByNameAsync(destination);

            List<SearchRouteModel> searchRoutes = new List<SearchRouteModel>();

            foreach (RouteModel route in routes)
            {
                List<DirectionModel> directions = await WebApiApplication.PtvApiControl.GetRouteDirectionsAsync(route);

                searchRoutes.Add(new SearchRouteModel(route, directions));
            }

            return PartialView("~/Views/Journey/_SearchRoutes.cshtml", searchRoutes);
        }

        public ActionResult SelectRoute(int routeId, int directionId)
        {
            int route = routeId;
            int dest = directionId;

            return PartialView("~/Views/Journey/Journeys.cshtml");
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
