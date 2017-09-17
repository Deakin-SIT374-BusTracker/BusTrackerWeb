using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    public class JourneyStopModel
    {
        public int StopId { get; set; }

        public string StopName { get; set; }

        public DateTime DepartureTime { get; set; }

        public double DepartureMinutes { get; set; }

        public double StopLatitude { get; set; }

        public double StopLongitude { get; set; }
    }
}