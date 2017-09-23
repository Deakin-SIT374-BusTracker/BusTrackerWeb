using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// This controller handles all Departure View functions.
/// </summary>
namespace BusTrackerWeb.Controllers
{
    public class DepartureController : Controller
    {
        /// <summary>
        /// Open the Departure Index View.
        /// </summary>
        /// <param name="routeId">The selected route.</param>
        /// <param name="directionId">The selected departure.</param>
        /// <returns>Departures Index View.</returns>
        public ActionResult Index(int routeId, int directionId)
        {
            ViewBag.Title = "BusHop > Departures";

            // Pass parameters to the view.
            ViewBag.RouteId = routeId;
            ViewBag.DirectionId = directionId;

            return View();
        }

        /// <summary>
        /// Retreives a Goole API address based on the coordinate paramters.
        /// </summary>
        /// <param name="latitude">The address latitude coordinate.</param>
        /// <param name="longitude">The address longitude coordinate.</param>
        /// <returns>Departures DepartureAddress Partial View.</returns>
        public ActionResult GetAddress(double latitude, double longitude)
        {
            // Get the reverse geolocation address.
            GeoCoordinate geolocation = new GeoCoordinate(latitude, longitude);
            string googleAddress = WebApiApplication.GeocodeApiControl
                .GetAddress(geolocation);

            // Pass the parameter to the view.
            ViewBag.DepartureAddress = googleAddress;

            return PartialView("~/Views/Departure/_DepartureAddress.cshtml");
        }

        /// <summary>
        /// For the given route and direction find the closest bus stop, then 
        /// get all the future departures for the stop.
        /// </summary>
        /// <param name="routeId">The selected route.</param>
        /// <param name="directionId">The selected route direction.</param>
        /// <param name="latitude">The users' current latitude.</param>
        /// <param name="longitude">The users' current longitude.</param>
        /// <returns>Departures DepartureRuns Partial View.</returns>
        public async Task<ActionResult> GetDepartures(int routeId, 
            int directionId, double latitude, double longitude)
        {
            // Get a list of bus stops in proximity to user location.
            List<StopModel> proximiytStops = 
                await WebApiApplication.PtvApiControl.GetStopsByDistanceAsync(
                    Convert.ToDecimal(latitude), 
                Convert.ToDecimal(longitude), 
                Properties.Settings.Default.BusStopMaxResults, 
                Properties.Settings.Default.ProximityStopMaxDistance);

            // Get a list of bus stops for the current route.
            List<StopModel> routeStops = 
                await WebApiApplication.PtvApiControl.GetRouteStopsAsync(
                    new RouteModel { RouteId = routeId });

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
            StopModel departureStop = 
                commonStops.OrderBy(s => s.StopDistance).First();

            // Get the routes' departures from the closest stop.
            List<DepartureModel> departures = 
                await WebApiApplication.PtvApiControl.GetDeparturesAsync(
                    routeId, departureStop);

            // Filter for correct directions.
            departures = departures.Where(
                d => d.DirectionId == directionId).ToList();

            // Filter for future runs only.
            departures = departures.Where(
                d => d.ScheduledDeparture >= DateTime.Now).ToList();

            if (departures.Count != 0)
            {
                return PartialView("~/Views/Departure/_DepartureRuns.cshtml",
                    departures);
            }
            else
            {
                return PartialView("~/Views/Departure/_DepartureNoRuns.cshtml",
                    departures);
            }
        }
    }
}