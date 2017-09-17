using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    public class JourneyDashboardModel
    {
        public JourneyStopModel UserStop { get; set; }

        public string BusDepartureMinutes { get; set; }

        public string WalkingDepartureMinutes { get; set; }
    }
}