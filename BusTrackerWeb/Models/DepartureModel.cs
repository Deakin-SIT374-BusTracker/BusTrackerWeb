using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    /// <summary>
    /// Bus Tracker Departure Model.
    /// </summary>
    public class DepartureModel
    {
        /// <summary>
        /// The bus stop associated with this departure.
        /// </summary>
        public StopModel Stop { get; set; }

        /// <summary>
        /// The PTV Route Id associated with this departure.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// The PTV Run Id associated with this departure.
        /// </summary>
        public int RunId { get; set; }

        /// <summary>
        /// The PTV Direction Id associated with this departure.
        /// </summary>
        public int DirectionId { get; set; }

        /// <summary>
        /// A collection of PTV Disruption Id's associated with this departure.
        /// </summary>
        public List<int> Disruptions { get; set; }

        /// <summary>
        /// The scheduled departure time from the associated bus stop.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        public DateTime ScheduledDeparture { get; set; }

        /// <summary>
        /// The estimated departure time from the associated bus stop.
        /// </summary>
        public DateTime EstimatedDeparture { get; set; }

        /// <summary>
        /// A summary of PTV flags for this departure.
        /// </summary>
        public string Flags { get; set; }
    }
}