using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    /// <summary>
    /// Bus Tracker Bus Stop Model.
    /// </summary>
    public class StopModel
    {
        /// <summary>
        /// The PTV Stop Id.
        /// </summary>
        public int StopId { get; set; }

        /// <summary>
        /// The stop name.
        /// </summary>
        public string StopName { get; set; }

        /// <summary>
        /// The stop GPS latitude.
        /// </summary>
        public decimal StopLatitude { get; set; }

        /// <summary>
        /// The stop GPS longitude.
        /// </summary>
        public decimal StopLongitude { get; set; }

        /// <summary>
        /// The sto pdistace from the current geolocation.
        /// </summary>
        public decimal StopDistance { get; set; }
    }
}