using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    public class DepartureEstimateController : Controller
    {
        public void EstimateDepartures(List<DepartureModel> departures, List<Leg> routeLegs, string busRegoNumber)
        {
            // Initialise the first stop estimated departure time.
            departures.First().EstimatedDeparture = departures.First().ScheduledDeparture;

            // Calculate and update optimum ETA for each leg of the run.
            for (int i = 0; i < departures.Count(); i++)
            {
                // Estimate departure of next stop = last stop estimated departure time plus travel time.
                DateTime estimatedDeparture = departures[i].EstimatedDeparture.AddSeconds(routeLegs[i].duration.value);

                departures[i + 1].EstimatedDeparture = estimatedDeparture;
            }

            // Find the last scheduled stop the bus should have reached.
            StopModel lastScheduledStop = departures.First(d => d.ScheduledDeparture >= DateTime.Now).Stop;

            // Check if that bus has reached the last scheduled stop.
            BusModel trackedBus = WebApiApplication.TrackedBuses.First(b => (b.RouteId == departures.First().RouteId) && (b.BusRegoNumber == busRegoNumber));
            int busPreviousStopId = trackedBus.BusPreviousStop.StopId;
            if (busPreviousStopId != lastScheduledStop.StopId)
            {
                // If the bus is late use the leg durations to estimate how late the bus is.
                // Find the index of the actual stop.
                int actualStopIndex = departures.FindIndex(d => d.Stop.StopId == busPreviousStopId);

                // Find the index of the scheduled stop.
                int scheduledStopIndex = departures.FindIndex(d => d.Stop.StopId == lastScheduledStop.StopId);

                // Take departures between actual and scheduled.
                List<Leg> lateLegs = routeLegs.Skip(actualStopIndex).Take(scheduledStopIndex - actualStopIndex).ToList();

                // Sum leg durations, this is how late the bus is.
                int busDelaySeconds = lateLegs.Sum(l => l.duration.value);

                // From the current stop, offset the optimum estimated departure times by how late the bus is.
                for (int i = (actualStopIndex + 1); i < departures.Count(); i++)
                {
                    departures[i].EstimatedDeparture =
                        departures[i].EstimatedDeparture.AddSeconds(busDelaySeconds);
                }
            }
        }
    }
}