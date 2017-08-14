using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace BusTrackerWeb.Models.GoogleApi
{
    [DataContract]
    public class SnappedResponse
    {
        [DataMember]
        public SnappedPoints[] snappedPoints { get; set; }
    }

    [DataContract]
    public class SnappedPoints
    {
        [DataMember]
        public Location location { get; set; }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public double latitude { get; set; }
        [DataMember]
        public double longitude { get; set; }
    }
}