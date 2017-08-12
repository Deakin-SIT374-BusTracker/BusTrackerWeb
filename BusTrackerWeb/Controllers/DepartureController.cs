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

            return PartialView("~/Views/Departure/_DeparureAddress.cshtml");
        }

        public ActionResult GetDepartures(int routeId, int directionId, double latitude, double longitude)
        {

            return PartialView("~/Views/Departure/_DepartureRuns.cshtml");
        }
        
        public ActionResult SelectRun(int runId)
        {
            int run = runId;

            return View("~/Views/Journey/Journey.cshtml");
        }
    }
}