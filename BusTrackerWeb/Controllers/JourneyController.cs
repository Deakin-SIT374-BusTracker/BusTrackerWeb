using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        public async Task<ActionResult> Index(int runId, int routeId, int stopId)
        {
            ViewBag.Title = "BusHop > Your Journey";
            ViewBag.RunId = runId;
            ViewBag.RouteId = routeId;
            ViewBag.StopId = stopId;

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

            // Get any buses on route.
            BusController busControl = new BusController();
            List<BusModel> busesOnRoute = busControl.GetBusOnRouteLocation(routeId);
            ViewBag.BusesOnRoute = busesOnRoute;

            // If route simulation enabled, pass route legs.
            List<Leg> routeLegs = new List<Leg>();
            runRoutes.ForEach(r => routeLegs.AddRange(r.legs));
            if(Properties.Settings.Default.SimulateRuns)
            {
                ViewBag.RouteLegs = routeLegs;
            }

            DepartureViewModel view = new DepartureViewModel(pattern.Departures);

            return View("~/Views/Journey/Index.cshtml", view);
        }


        [HttpPost]
        public ActionResult GetStops(JourneyStopModel[] stops)
        {
            List<JourneyStopModel> journeyStops = new List<JourneyStopModel>();

            foreach (JourneyStopModel jStop in stops)
            {
                double departureMintues = (jStop.DepartureTime - DateTime.Now).TotalMinutes;
                departureMintues = Math.Round(departureMintues, 0);

                if (departureMintues >= 0)
                {
                    journeyStops.Add(
                        new JourneyStopModel
                        {
                            StopId = jStop.StopId,
                            StopName = jStop.StopName,
                            DepartureTime = jStop.DepartureTime,
                            DepartureMinutes = departureMintues
                        });
                }
            }

            return PartialView("~/Views/Journey/_JourneyStops.cshtml", journeyStops);
        }


        [HttpPost]
        public ActionResult GetDashboard(JourneyStopModel[] stops, int stopId, double userLatitude, double userLongitude)
        {
            // Get the users selected pickup stop.
            JourneyDashboardModel dashboardModel = new JourneyDashboardModel();
            dashboardModel.UserStop = stops.First(s => s.StopId == stopId);

            // Calculate the time until stop departure.
            double departureMintues = (dashboardModel.UserStop.DepartureTime - DateTime.Now).TotalMinutes;
            departureMintues = Math.Round(departureMintues, 0);

            // Get the time required to walk from the user location to the bus stop.
            GeoCoordinate[] waypoints = {
            new GeoCoordinate(userLatitude, userLongitude),
            new GeoCoordinate(dashboardModel.UserStop.StopLatitude, dashboardModel.UserStop.StopLongitude)
            };

            List<Route> walkingRoutes = WebApiApplication.DirectionsApiControl.GetDirections(waypoints, true);

            double walkingDuration = 0.0;
            
            foreach(Route route in walkingRoutes)
            {
                walkingDuration += route.legs.Sum(rl => rl.duration.value / 60);
            }

            // Calculate walking departure time, aim to get there 1 minute early.
            double walkMinutes = (departureMintues - 1) - walkingDuration;

            // Calculate Walking status
            if(walkMinutes > 0)
            {
                dashboardModel.WalkingDepartureMinutes = string.Format("{0} Mins", walkMinutes);
            }
            else if (walkMinutes <= 0)
            {
                dashboardModel.WalkingDepartureMinutes = "Now";
            }

            // Calculate Bus Departing status.
            if(departureMintues > 0.0)
            {
                dashboardModel.BusDepartureMinutes = string.Format("{0:0} Mins", departureMintues);
            }
            else if(departureMintues == 0.0)
            {
                dashboardModel.BusDepartureMinutes = "Now";
            }
            else
            {
                dashboardModel.BusDepartureMinutes = "Departed";
            }

            return PartialView("~/Views/Journey/_JourneyDashboard.cshtml", dashboardModel);
        }


        public async Task<JsonResult> SimulateBusLocation(int runId, int routeId)
        {
            BusModel simulatedBus = new BusModel();

            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };

            try
            {
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

                if (stepIndex > 0)
                {
                    // Select current step.
                    Step currentStep = steps[(int)stepIndex];

                    // Create bus with simulated coordinates.
                    simulatedBus = new BusModel
                    {
                        RouteId = routeId,
                        BusLatitude = Convert.ToDecimal(currentStep.end_location.lat),
                        BusLongitude = Convert.ToDecimal(currentStep.end_location.lng),
                        BusRegoNumber = "SIM001"
                    };
                }
                else
                {
                    // Select first step.
                    Step currentStep = steps[0];

                    // Create bus with simulated coordinates.
                    simulatedBus = new BusModel
                    {
                        RouteId = routeId,
                        BusLatitude = Convert.ToDecimal(pattern.Departures[0].Stop.StopLatitude),
                        BusLongitude = Convert.ToDecimal(pattern.Departures[0].Stop.StopLongitude),
                        BusRegoNumber = "SIM001"
                    };
                }

                // Update the bus location.
                BusController busControl = new BusController();
                await busControl.PutBusOnRouteLocation(simulatedBus);
            }
            catch (Exception e)
            { }

            return Json(simulatedBus, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PutBusLocation(int routeId, double latitude, double longitude)
        {
            BusModel bus = new BusModel {
                RouteId = routeId,
                BusLatitude = Convert.ToDecimal(latitude),
                BusLongitude = Convert.ToDecimal(longitude),
                BusRegoNumber = "SIM001"
            };
            BusController busControl = new BusController();
            await busControl.PutBusOnRouteLocation(bus);

            return Json(bus, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> PutBusOnRoute(int runId, int routeId, double busLatitude, double busLongitude)
        {
            BusModel trackedBus = new BusModel();
            
            try
            {
                // Create bus with simulated coordinates.
                trackedBus = new BusModel
                {
                    RouteId = routeId,
                    BusLatitude = Convert.ToDecimal(busLatitude),
                    BusLongitude = Convert.ToDecimal(busLongitude),
                    BusRegoNumber = "SIM001"
                };

                // Update the bus location.
                BusController busControl = new BusController();
                await busControl.PutBusOnRouteLocation(trackedBus);
            }
            catch (Exception e)
            { }

            return Json(trackedBus, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateDepartures(int routeId, int runId)
        {
            DepartureEstimateController estimateControl = new DepartureEstimateController();

            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };
            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            // Build and array of stop coordinates.
            List<GeoCoordinate> stopCoordinates = new List<GeoCoordinate>();
            foreach (DepartureModel departure in pattern.Departures)
            {
                stopCoordinates.Add(new GeoCoordinate((double)departure.Stop.StopLatitude, (double)departure.Stop.StopLongitude));
            }

            // Get directions between stops.
            List<Route> runRoutes = WebApiApplication.DirectionsApiControl.GetDirections(stopCoordinates.ToArray());

            // If route simulation enabled, pass route legs.
            List<Leg> routeLegs = new List<Leg>();
            runRoutes.ForEach(r => routeLegs.AddRange(r.legs));

            List<DepartureModel> updatedDeaprtures = estimateControl.EstimateDepartures(pattern.Departures.ToList(), routeLegs, "SIM001");

            return Json(updatedDeaprtures, JsonRequestBehavior.AllowGet);
        }
    }
}
