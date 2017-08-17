using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Bus Tracker API Route action requests.
    /// </summary>
    public class RouteController : ApiController
    {
        /// <summary>
        /// Get a list of all PTV bus routes.
        /// </summary>
        /// <returns>An array of RouteModel objects.</returns>
        [ActionName("GetRoutes")]
        public async Task<List<RouteModel>> GetRoutes()
        {
            return await WebApiApplication.PtvApiControl.GetRoutesAsync();
        }

        /// <summary>
        /// Get a list of all PTV bus routes filtered by route name.
        /// </summary>
        /// <returns>An array of RouteModel objects.</returns>
        [ActionName("GetRoutesByName")]
        public async Task<List<RouteModel>> GetRoutesByName(string routeName)
        {
            return await WebApiApplication.PtvApiControl.GetRoutesByNameAsync(routeName);
        }


        /// <summary>
        /// Get a specific PTV bus route.
        /// </summary>
        /// <param name="routeId">PTV Bus Route Id.</param>
        /// <returns>A RouteModel object.</returns>
        [ActionName("GetRoute")]
        public async Task<RouteModel> GetRoute(int routeId)
        {
            RouteModel route = new RouteModel();

            try
            {
                route = await WebApiApplication.PtvApiControl.GetRouteAsync(routeId);
            }
            catch(Exception e)
            {
                Trace.TraceError("GetRoute Exception: {0}", e.Message);
            }

            return route;
        }

        /// <summary>
        /// Get a list of PTV directions for a specific route.
        /// </summary>
        /// <param name="routeId">PTV Bus Route Id.</param>
        /// <returns>An array of DirectionModel objects.</returns>
        [ActionName("GetRouteDirections")]
        public async Task<List<DirectionModel>> GetRouteDirections(int routeId)
        {
            List<DirectionModel> routeDirections = new List<Models.DirectionModel>();

            try
            {
                // Create a new route.
                RouteModel route = await WebApiApplication.PtvApiControl.GetRouteAsync(routeId);

                // Get all stops for a given route.
                routeDirections = await WebApiApplication.PtvApiControl.
                    GetRouteDirectionsAsync(route);
            }
            catch(Exception e)
            {
                Trace.TraceError("GetRouteDirections Exception: {0}", e.Message);
            }

            return routeDirections;
        }

        /// <summary>
        /// Get the next PTV run scheduled for specific route and direction.
        /// </summary>
        /// <param name="routeId">PTV Bus Route Id.</param>
        /// <param name="directionId">PTV Direction Id.</param>
        /// <returns>A RunModel object.</returns>
        [ActionName("GetRouteNextRun")]
        public async Task<RunModel> GetRouteNextRun(int routeId, int directionId)
        {
            RunModel nextRun = new RunModel();

            try
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

                nextRun = currentRuns.First();

                // Build a collection of run geo locations.
                List<GeoCoordinate> routePoints = new List<GeoCoordinate>();
                foreach(DepartureModel departure in nextRun.StoppingPattern.Departures)
                {
                    GeoCoordinate geoLocation = 
                        new GeoCoordinate (Convert.ToDouble(departure.Stop.StopLatitude), 
                        Convert.ToDouble(departure.Stop.StopLongitude));

                    routePoints.Add(geoLocation);  
                }

                // Query Google Directions API for ETA.
                List<Route> routes = WebApiApplication.DirectionsApiControl.GetDirections(routePoints.ToArray());
                List<Leg> routeLegs = new List<Leg>();
                routes.ForEach(r => routeLegs.AddRange(r.legs));

                // Update each stop on the run with ETA.

                // Initialise the first stop estimated departure time.
                nextRun.StoppingPattern.Departures.First().EstimatedDeparture = 
                    nextRun.StoppingPattern.Departures.First().ScheduledDeparture;

                // Calculate and update optimum ETA for each leg of the run.
                int legCount = routeLegs.Count();
                for (int i = 0; i < legCount; i++)    
                {
                    // Estimate departure of next stop = last stop estimated departure time plus travel time.
                    DateTime estimatedDeparture = nextRun.StoppingPattern.Departures[i].EstimatedDeparture.AddSeconds(routeLegs[i].duration.value);

                    // Set estimated departure time based on bus ahead or behind schedule
                    // NB: A bus can't leave a bus stop ahead of schedule.
                    DateTime scheduledDeparture = nextRun.StoppingPattern.Departures[i + 1].ScheduledDeparture;
                    if (estimatedDeparture < scheduledDeparture)
                    {
                        nextRun.StoppingPattern.Departures[i + 1].EstimatedDeparture = scheduledDeparture;
                    }
                    else
                    {
                        nextRun.StoppingPattern.Departures[i + 1].EstimatedDeparture = estimatedDeparture;
                    }
                }

                // Find the last scheduled stop the bus should have reached.
                StopModel lastScheduledStop = nextRun.StoppingPattern.Departures.First(d => d.ScheduledDeparture >= DateTime.Now).Stop;

                // Check if that bus has reached the last scheduled stop.
                int busPreviousStopId = WebApiApplication.TrackedBuses.First(b => b.RouteId == route.RouteId).BusPreviousStop.StopId;
                if (busPreviousStopId != lastScheduledStop.StopId)
                {
                    // If the bus is late use the leg durations to estimate how late the bus is.
                    // Find the index of the actual stop.
                    int actualStopIndex = nextRun.StoppingPattern.Departures.FindIndex(d => d.Stop.StopId == busPreviousStopId);

                    // Find the index of the scheduled stop.
                    int scheduledStopIndex = nextRun.StoppingPattern.Departures.FindIndex(d => d.Stop.StopId == lastScheduledStop.StopId);

                    // Take departures between actual and scheduled.
                    List<Leg> lateLegs = routeLegs.Skip(actualStopIndex).Take(scheduledStopIndex - actualStopIndex).ToList();

                    // Sum leg durations, this is how late the bus is.
                    int busDelaySeconds = lateLegs.Sum(l => l.duration.value);

                    // From the current stop, offset the optimum estimated departure times by how late the bus is.
                    for(int i = (actualStopIndex + 1); i < nextRun.StoppingPattern.Departures.Count(); i++)
                    {
                        nextRun.StoppingPattern.Departures[i].EstimatedDeparture = 
                            nextRun.StoppingPattern.Departures[i].EstimatedDeparture.AddSeconds(busDelaySeconds);
                    }
                }
                
            }
            catch(Exception e)
            {
                Trace.TraceError("GetRouteNextRun Exception: {0}", e.Message);
            }

            return nextRun;
        }
    }
}