using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Bus Tracker API Bus action requests.
    /// </summary>
    public class BusController : ApiController
    {       
        /// <summary>
        /// Update the longitude and latitude of a bus.
        /// </summary>
        /// <param name="bus">The bus object to be updated.</param>
        /// <returns>200 if updated.</returns>
        [ActionName("PutBusOnRouteLocation")]
        public async Task<int> PutBusOnRouteLocation([FromBody]BusModel bus)
        {
            // Set default HTTP return code.
            int httpResult = 400;

            try
            {
                // Check for any stops in close proximity to the bus location.
                List<StopModel> proximiytStops = await WebApiApplication.PtvApiControl.
                    GetStopsByDistanceAsync(bus.BusLatitude, bus.BusLongitude,
                    Properties.Settings.Default.BusStopMaxResults,
                    Properties.Settings.Default.BusStopMaxDistance);

                // Get the stops on route.
                List<StopModel> routeStops = await WebApiApplication.PtvApiControl.
                    GetRouteStopsAsync(new RouteModel { RouteId = bus.RouteId });

                // If the bus is in proximity to a stop on route, update the bus last stop.
                StopModel previousStop = new StopModel();
                if (proximiytStops.Count != 0)
                {
                    StopModel closestStop = proximiytStops.First();

                    if (routeStops.Exists(s => s.StopId == closestStop.StopId))
                    {
                        previousStop = closestStop;
                    }
                }

                // Check for the bus in the current collection
                if (WebApiApplication.TrackedBuses.Exists(b => b.BusRegoNumber == bus.BusRegoNumber))
                {
                    // If exists, update the bus location.
                    BusModel trackedBus = WebApiApplication.TrackedBuses.
                        Single(b => b.BusRegoNumber == bus.BusRegoNumber);

                    trackedBus.BusLatitude = bus.BusLatitude;
                    trackedBus.BusLongitude = bus.BusLongitude;
                    trackedBus.RouteId = bus.RouteId;

                    // Only update the previous stop if a valid stop is found.
                    if (previousStop.StopId != 0)
                    {
                        trackedBus.BusPreviousStop = previousStop;
                    }
                }
                else
                {
                    // If new, add the new bus to the collection.
                    BusModel newBus = new BusModel
                    {
                        BusRegoNumber = bus.BusRegoNumber,
                        BusLatitude = bus.BusLatitude,
                        BusLongitude = bus.BusLongitude,
                        RouteId = bus.RouteId,
                        BusPreviousStop = previousStop
                    };

                    WebApiApplication.TrackedBuses.Add(newBus);
                }
                
                httpResult = 200;
            }
            catch(Exception e)
            {
                Trace.TraceError("PutBusOnRouteLocation Exception: {0}", e.Message);
            }

            return httpResult;
        }

        /// <summary>
        /// Get all buses that are curretly on the defined route.
        /// </summary>
        /// <param name="routeId">The PTV Route Id of the buses to be returned.</param>
        /// <returns>A collection of Bus Model objects.</returns>
        [ActionName("GetBusOnRouteLocation")]
        public List<BusModel> GetBusOnRouteLocation(int routeId)
        {
            List<BusModel> busesOnRoute = new List<BusModel>();

            try
            {
                busesOnRoute = WebApiApplication.TrackedBuses.Where(b => b.RouteId == routeId).ToList();
            }
            catch (Exception e)
            {
                Trace.TraceError("GetBusOnRouteLocation Exception: {0}", e.Message);
            }
             
            return busesOnRoute;
        }
    }
}