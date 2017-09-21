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
        public List<DepartureModel> EstimateDepartures(List<DepartureModel> departures, List<Leg> routeLegs, string busRegoNumber)
        {
            // Initialise the first stop estimated departure time.
            departures.First().EstimatedDeparture = departures.First().ScheduledDeparture;

            // Calculate and update optimum ETA for each leg of the run.
            for (int i = 0; i < routeLegs.Count(); i++)
            {
                // Estimate departure of next stop = last stop estimated departure time plus travel time.
                DateTime estimatedDeparture = departures[i].EstimatedDeparture.AddSeconds(routeLegs[i].duration.value);

                departures[i + 1].EstimatedDeparture = estimatedDeparture;
            }

            // Find the last scheduled stop the bus should have reached.
            StopModel lastScheduledStop = departures.First(d => d.ScheduledDeparture >= DateTime.Now).Stop;

            // Check if that bus has reached the last scheduled stop.
            int routeId = departures.First().RouteId;
            BusModel trackedBus = WebApiApplication.TrackedBuses.Single(b => b.RouteId == routeId);
            int busPreviousStopId = trackedBus.BusPreviousStop.StopId;
            if (busPreviousStopId != lastScheduledStop.StopId)
            {
                // If the bus is late use the leg durations to estimate how late the bus is.
                // Find the index of the actual stop.
                int actualStopIndex = departures.FindIndex(d => d.Stop.StopId == busPreviousStopId);

                // Find the index of the scheduled stop.
                int scheduledStopIndex = departures.FindIndex(d => d.Stop.StopId == lastScheduledStop.StopId);

                // Determine if early or late.
                List<Leg> delayLegs = new List<Leg>();
                int busDelaySeconds = 0;
                if (actualStopIndex < scheduledStopIndex)
                {
                    // Take all legs running later than scheduled.
                    delayLegs = routeLegs.Skip(actualStopIndex).Take(scheduledStopIndex - actualStopIndex).ToList();

                    // Delay estimated departures for all remaining stops.
                    busDelaySeconds = delayLegs.Sum(l => l.duration.value);
                }
                else
                {
                    // Take all legs running ahead of schedule.
                    delayLegs = routeLegs.Skip(scheduledStopIndex).Take(actualStopIndex - scheduledStopIndex).ToList();

                    // Advance estimted departures fro all reamining stops.
                    busDelaySeconds = delayLegs.Sum(l => l.duration.value);
                    busDelaySeconds *= -1;
                }

                // From the current stop, offset the optimum estimated departure times by how late the bus is.
                for (int i = (actualStopIndex); i < departures.Count(); i++)
                {
                    departures[i].EstimatedDeparture =
                        departures[i].EstimatedDeparture.AddSeconds(busDelaySeconds);
                }

                // Discard all past stops.
                departures = departures.Skip(actualStopIndex).Take(departures.Count()).ToList();
            }

            return departures;
        }
    }
}