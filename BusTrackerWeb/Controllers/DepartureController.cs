using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
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
        
        public ActionResult GetAddress(double latitude, double longitude)
        {
            GeoCoordinate geolocation = new GeoCoordinate(latitude, longitude);

            string googleAddress = WebApiApplication.GeocodeApiControl.GetAddress(geolocation);

            ViewBag.DepartureAddress = googleAddress;

            return PartialView("~/Views/Departure/_DepartureAddress.cshtml");
        }

        public async Task<ActionResult> GetDepartures(int routeId, int directionId, double latitude, double longitude)
        {
            // Get a list of bus stops in proximity to user location.
            List<StopModel> proximiytStops = await WebApiApplication.PtvApiControl.GetStopsByDistanceAsync(Convert.ToDecimal(latitude), Convert.ToDecimal(longitude), Properties.Settings.Default.BusStopMaxResults, Properties.Settings.Default.ProximityStopMaxDistance);

            // Get a list of bus stops for the current route.
            List<StopModel> routeStops = await WebApiApplication.PtvApiControl.GetRouteStopsAsync(new RouteModel { RouteId = routeId });

            // Find a common set of stops.
            List<StopModel> commonStops = new List<StopModel>();
            int[] routeStopIdSet = routeStops.Select(s => s.StopId).ToArray();
            foreach(StopModel stop in proximiytStops)
            {
                if (routeStopIdSet.Contains(stop.StopId))
                {
                    commonStops.Add(stop);
                }
            }

            // Find the closest stop from the common set of stops.
            StopModel departureStop = commonStops.OrderBy(s => s.StopDistance).First();

            // Get the route departures from the closest stop.
            List<DepartureModel> departures = await WebApiApplication.PtvApiControl.GetDeparturesAsync(routeId, departureStop);

            // Filter for correct directions.
            departures = departures.Where(d => d.DirectionId == directionId).ToList();

            // Filter for future runs only.
            departures = departures.Where(d => d.ScheduledDeparture >= DateTime.Now).ToList();

            return PartialView("~/Views/Departure/_DepartureRuns.cshtml", departures);
        }
        
        public ActionResult SelectRun(int runId)
        {
            int run = runId;

            return View("~/Views/Journey/Journey.cshtml");
        }
    }
}