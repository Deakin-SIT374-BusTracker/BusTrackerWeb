using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using System;
using System.Collections.Generic;
using System.Device.Location;
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

            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };
            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            // Build an array of stops and pass to the view as marker coordinates.
            DepartureModel[] departureArray = pattern.Departures.ToArray();
            ViewBag.StopsLatitudeArray = new double[departureArray.Count()];
            ViewBag.StopsLongitudeArray = new double[departureArray.Count()];

            for(int i = 0; i < departureArray.Count(); i++)
            {
                ViewBag.StopsLatitudeArray[i] = (double)departureArray[i].Stop.StopLatitude;
                ViewBag.StopsLongitudeArray[i] = (double)departureArray[i].Stop.StopLongitude;
            }

            // Build and array of stop coordinates.
            List<GeoCoordinate> stopCoordinates = new List<GeoCoordinate>();
            foreach(DepartureModel departure in pattern.Departures)
            {
                stopCoordinates.Add(new GeoCoordinate((double)departure.Stop.StopLatitude, (double)departure.Stop.StopLongitude));
            }

            // Get the snapped points between stops.
            List<SnappedPoints> snappedPoints = WebApiApplication.SnappedApiControl.GetSnappedPoints(stopCoordinates.ToArray());

            // Pass the snapped points to the view as a road path.
            ViewBag.PathLatitudeArray = new double[snappedPoints.Count()];
            ViewBag.PathLongitudeArray = new double[snappedPoints.Count()];

            for (int i = 0; i < snappedPoints.Count(); i++)
            {
                ViewBag.PathLatitudeArray[i] = snappedPoints[i].location.latitude;
                ViewBag.PathLongitudeArray[i] = snappedPoints[i].location.longitude;
            }
            
            return View("~/Views/Journey/Index.cshtml", pattern.Departures);
        }

        public ActionResult TestMap()
        {
            return View("~/Views/Journey/TestMap.cshtml");
        }

    }
}
