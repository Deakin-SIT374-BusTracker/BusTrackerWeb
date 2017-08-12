using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models.GoogleApi
{
    public class GeoCodeResponse
    {
        public results[] results { get; set; }

        public string status { get; set; }
    }

    public class results
    {
        public address_component[] address_components { get; set; }

        public string formatted_address { get; set; }

        public geometry geometry { get; set; }

        public string partial_match { get; set; }

        public string[] types { get; set; }
    }

    public class geometry
    {
        public bounds bounds { get; set; }

        public location location { get; set; }

        public string location_type { get; set; }

        public viewport viewport { get; set; }
    }

    public class bounds
    {
        public northeast northeast { get; set; }

        public southwest southwest { get; set; }
    }

    public class viewport
    {
        public northeast northeast { get; set; }

        public southwest southwest { get; set; }
    }

    public class northeast : location
    {
    }

    public class southwest : location
    {
    }

    public class location
    {
        public decimal lat { get; set; }

        public decimal lng { get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }

        public string short_name { get; set; }

        public string[] types { get; set; }
    }
}