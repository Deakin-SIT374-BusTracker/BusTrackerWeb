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
        public async Task<ActionResult> Index(int runId, int routeId)
        {
            ViewBag.Title = "BusHop > Your Journey";

            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };

            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            // Set map coordinates.
            DepartureModel[] departureArray = pattern.Departures.ToArray();

            ViewBag.LatPath = new double[departureArray.Count()];
            ViewBag.LngPath = new double[departureArray.Count()];

            for(int i = 0; i < departureArray.Count(); i++)
            {
                ViewBag.LatPath[i] = (double)departureArray[i].Stop.StopLatitude;
                ViewBag.LngPath[i] = (double)departureArray[i].Stop.StopLongitude;
            }

            return View("~/Views/Journey/Index.cshtml", pattern.Departures);
        }

        public ActionResult TestMap()
        {
            return View("~/Views/Journey/TestMap.cshtml");
        }

    }
}
