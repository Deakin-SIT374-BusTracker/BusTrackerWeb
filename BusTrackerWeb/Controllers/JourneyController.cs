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
    /// This controller handles all Journey View functions.
    /// </summary>
    public class JourneyController : Controller
    {
        /// <summary>
        /// Open the Journey Index View.
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

            ViewBag.Departures = pattern.Departures;

            for (int i = 0; i < departureArray.Count(); i++)
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

            // Get directions between stops.
            List<Route> runRoutes = WebApiApplication.DirectionsApiControl.GetDirections(stopCoordinates.ToArray());
            List<string> polyLines = new List<string>();
            runRoutes.ForEach(r => polyLines.Add(r.overview_polyline.points.Replace(@"\\", @"\\\\")));
            
            ViewBag.EncodePolylines = polyLines.ToArray();

            return View("~/Views/Journey/Index.cshtml", pattern.Departures);
        }
    }
}
