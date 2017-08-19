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
            ViewBag.RunId = runId;
            ViewBag.RouteId = routeId;

            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };
            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            ViewBag.Departures = pattern.Departures;

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

            // If route simulation enabled, pass route legs.
            List<Leg> routeLegs = new List<Leg>();
            runRoutes.ForEach(r => routeLegs.AddRange(r.legs));
            if(Properties.Settings.Default.SimulateRuns)
            {
                ViewBag.RouteLegs = routeLegs;
            }

            return View("~/Views/Journey/Index.cshtml", pattern.Departures);
        }


        public async Task SimulateLocation(int runId, int routeId)
        {
            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };
            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            // Get the first stop departure time.
            DateTime startTime = pattern.Departures.First().ScheduledDeparture;

            // Get the final stop arrival time. 
            DateTime endTime = pattern.Departures.Last().ScheduledDeparture;

            // Journey total timespan.
            TimeSpan spanTotal = endTime - startTime;
            double secondsTotal = spanTotal.TotalSeconds;

            // Journey elapsed timespan.
            TimeSpan spanElapsed = DateTime.Now - startTime;
            double secondsElapsed = spanElapsed.TotalSeconds;

            // Calculate percentage journey complete.
            double progressRatio = secondsElapsed / secondsTotal;

            // Build and array of stop coordinates.
            List<GeoCoordinate> stopCoordinates = new List<GeoCoordinate>();
            foreach (DepartureModel departure in pattern.Departures)
            {
                stopCoordinates.Add(new GeoCoordinate((double)departure.Stop.StopLatitude, (double)departure.Stop.StopLongitude));
            }

            // Get directions between stops.
            List<Route> runRoutes = WebApiApplication.DirectionsApiControl.GetDirections(stopCoordinates.ToArray());

            // Build a collection of legs.
            List<Leg> legs = new List<Leg>();
            runRoutes.ForEach(r => legs.AddRange(r.legs));

            // Build a collection of steps.
            List<Step> steps = new List<Step>();
            legs.ForEach(l => steps.AddRange(l.steps));

            // Select current step index based on progess.
            double stepIndex = steps.Count() * progressRatio;

            // Select current step.
            Step currentStep = steps[(int)stepIndex];

            // Get simulated coordinates.
            GeoCoordinate simulatedLocation = new GeoCoordinate(currentStep.end_location.lat, currentStep.end_location.lng);

        }
    }
}
