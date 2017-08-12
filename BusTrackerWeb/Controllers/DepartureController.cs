using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    public class DepartureController : Controller
    {
        /// <summary>
        /// Open the Departures View.
        /// </summary>
        /// <returns>Journeys View.</returns>
        public ActionResult Index(int routeId, int directionId)
        {
            ViewBag.Title = "BusHop > Departures";

            ViewBag.RouteId = routeId;
            ViewBag.DirectionId = directionId;

            return View();
        }
        
        public ActionResult GetAddress(decimal latitude, decimal longitude)
        {
            ViewBag.DepartureAddress = "1 Smith Street";

            return PartialView("~/Views/Departure/_DeparureAddress.cshtml");
        }

        public ActionResult GetDepartures(int routeId, int directionId, decimal latitude, decimal longitude)
        {

            return PartialView("~/Views/Departure/_DepartureRuns.cshtml");
        }
        
        /// <summary>
        /// Get the next departures for the selected route and diraction.
        /// </summary>
        /// <param name="routeId">Selected Route Id.</param>
        /// <param name="directionId">Selected Direction Id.</param>
        /// <returns></returns>
        //public async Task<ActionResult> GetDepartures(int routeId, int directionId)
        //{
        //    // Get the route.
        //    RouteModel route = await WebApiApplication.PtvApiControl.GetRouteAsync(routeId);

        //    // Get route direction.
        //    DirectionModel direction = await WebApiApplication.PtvApiControl.
        //        GetDirectionAsync(directionId, route);

        //    // Get all runs for a route.
        //    List<RunModel> routeRuns = await WebApiApplication.PtvApiControl.
        //        GetRouteRunsAsync(route);

        //    // Index through all runs to find those that have not expired.
        //    List<RunModel> currentRuns = new List<RunModel>();
        //    foreach (RunModel run in routeRuns)
        //    {
        //        // Check if the run is new or already cached.
        //        if (!WebApiApplication.RunsCache.Exists(r => r.RunId == run.RunId))
        //        {
        //            // Get run info from the PTV API.
        //            run.StoppingPattern = await WebApiApplication.PtvApiControl.
        //                GetStoppingPatternAsync(run);

        //            // Update the local cache to minimise future API calls.
        //            WebApiApplication.RunsCache.Add(run);
        //        }
        //        else
        //        {
        //            // Get the cached run.
        //            run.StoppingPattern = WebApiApplication.RunsCache.
        //                Single(r => r.RunId == run.RunId).StoppingPattern;
        //        }


        //        // Check the current run for optimisation sentinals.
        //        DateTime runLastStoptime = run.StoppingPattern.Departures.Last().ScheduledDeparture;
        //        int runDirectionId = run.StoppingPattern.Departures.Last().DirectionId;

        //        // Add current and future runs to a collection.
        //        if ((runDirectionId == directionId) && (runLastStoptime > DateTime.Now))
        //        {
        //            run.Direction = direction;
        //            currentRuns.Add(run);
        //        }
        //    }

        //    // Order the current run collection by Last Stop Scheduled Departure Time, the first 
        //    // run in the ordered collection will be the next run.
        //    currentRuns = currentRuns.
        //        OrderBy(r => r.StoppingPattern.Departures.Last().ScheduledDeparture).ToList();


        //    return View("~/Views/Departure/Departure.cshtml", currentRuns);
        //}


        public ActionResult SelectRun(int runId)
        {
            int run = runId;

            return View("~/Views/Journey/Journey.cshtml");
        }
    }
}