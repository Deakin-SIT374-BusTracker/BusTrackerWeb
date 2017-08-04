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
        /// Get all routes and associated directions for the given destination.
        /// Return a summary table as a partial view.
        /// </summary>
        /// <param name="destination">Route name filter.</param>
        /// <returns></returns>
        public async Task<ActionResult> SearchRoutes(string destination)
        {
            // Get all routes matching the destination.
            List<RouteModel> routes = await WebApiApplication.PtvApiControl.GetRoutesByNameAsync(destination);

            // For each route get the associated directions and build a new 
            // collection.
            List<SearchRouteModel> searchRoutes = new List<SearchRouteModel>();
            foreach (RouteModel route in routes)
            {
                List<DirectionModel> directions = await WebApiApplication.PtvApiControl.GetRouteDirectionsAsync(route);
                searchRoutes.Add(new SearchRouteModel(route, directions));
            }

            return PartialView("~/Views/Journey/_SearchRoutes.cshtml", searchRoutes);
        }

        /// <summary>
        /// Get the next departures for the selected route and diraction.
        /// </summary>
        /// <param name="routeId">Selected Route Id.</param>
        /// <param name="directionId">Selected Direction Id.</param>
        /// <returns></returns>
        public async Task<ActionResult> SelectRoute(int routeId, int directionId)
        {
            // Get the route.
            RouteModel route = await WebApiApplication.PtvApiControl.GetRouteAsync(routeId);

            // Get route direction.
            DirectionModel direction = await WebApiApplication.PtvApiControl.
                GetDirectionAsync(directionId, route);

            // Get all runs for a route.
            List<RunModel> routeRuns = await WebApiApplication.PtvApiControl.
                GetRouteRunsAsync(route);

            // Index through all runs to find those that have not expired.
            List<RunModel> currentRuns = new List<RunModel>();
            foreach (RunModel run in routeRuns)
            {
                // Check if the run is new or already cached.
                if (!WebApiApplication.RunsCache.Exists(r => r.RunId == run.RunId))
                {
                    // Get run info from the PTV API.
                    run.StoppingPattern = await WebApiApplication.PtvApiControl.
                        GetStoppingPatternAsync(run);

                    // Update the local cache to minimise future API calls.
                    WebApiApplication.RunsCache.Add(run);
                }
                else
                {
                    // Get the cached run.
                    run.StoppingPattern = WebApiApplication.RunsCache.
                        Single(r => r.RunId == run.RunId).StoppingPattern;
                }


                // Check the current run for optimisation sentinals.
                DateTime runLastStoptime = run.StoppingPattern.Departures.Last().ScheduledDeparture;
                int runDirectionId = run.StoppingPattern.Departures.Last().DirectionId;

                // Add current and future runs to a collection.
                if ((runDirectionId == directionId) && (runLastStoptime > DateTime.Now))
                {
                    run.Direction = direction;
                    currentRuns.Add(run);
                }
            }

            // Order the current run collection by Last Stop Scheduled Departure Time, the first 
            // run in the ordered collection will be the next run.
            currentRuns = currentRuns.
                OrderBy(r => r.StoppingPattern.Departures.Last().ScheduledDeparture).ToList();


            return View("~/Views/Journey/Journeys.cshtml", currentRuns);
        }

        public ActionResult SelectRun(int runId)
        {
            int run = runId;

            return View("~/Views/Journey/YourJourney.cshtml");
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
