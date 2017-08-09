using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace BusTrackerWeb.Models.GoogleApi
{
    [DataContract]
    public class DirectionsResponse
    {
        [DataMember]
        public Geocoded_Waypoints[] geocoded_waypoints { get; set; }
        [DataMember]
        public Route[] routes { get; set; }
        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class Geocoded_Waypoints
    {
        [DataMember]
        public string geocoder_status { get; set; }
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public string[] types { get; set; }
        [DataMember]
        public bool partial_match { get; set; }
    }

    [DataContract]
    public class Route
    {
        [DataMember]
        public Bounds bounds { get; set; }
        [DataMember]
        public string copyrights { get; set; }
        [DataMember]
        public Leg[] legs { get; set; }
        [DataMember]
        public Overview_Polyline overview_polyline { get; set; }
        [DataMember]
        public string summary { get; set; }
        [DataMember]
        public object[] warnings { get; set; }
        [DataMember]
        public object[] waypoint_order { get; set; }
    }

    [DataContract]
    public class Bounds
    {
        [DataMember]
        public Northeast northeast { get; set; }
        [DataMember]
        public Southwest southwest { get; set; }
    }

    [DataContract]
    public class Northeast
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

    [DataContract]
    public class Southwest
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

    [DataContract]
    public class Overview_Polyline
    {
        [DataMember]
        public string points { get; set; }
    }

    [DataContract]
    public class Leg
    {
        [DataMember]
        public Distance distance { get; set; }
        [DataMember]
        public Duration duration { get; set; }
        [DataMember]
        public Duration_In_Traffic duration_in_traffic { get; set; }
        [DataMember]
        public string end_address { get; set; }
        [DataMember]
        public End_Location end_location { get; set; }
        [DataMember]
        public string start_address { get; set; }
        [DataMember]
        public Start_Location start_location { get; set; }
        [DataMember]
        public Step[] steps { get; set; }
        [DataMember]
        public object[] via_waypoint { get; set; }
    }

    [DataContract]
    public class Distance
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class Duration
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class Duration_In_Traffic
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    [DataContract]
    public class End_Location
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

    [DataContract]
    public class Start_Location
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

    [DataContract]
    public class Step
    {
        [DataMember]
        public Distance1 distance { get; set; }
        [DataMember]
        public Duration1 duration { get; set; }
        [DataMember]
        public End_Location1 end_location { get; set; }
        [DataMember]
        public string html_instructions { get; set; }
        [DataMember]
        public Polyline polyline { get; set; }
        [DataMember]
        public Start_Location1 start_location { get; set; }
        [DataMember]
        public string travel_mode { get; set; }
        [DataMember]
        public string maneuver { get; set; }
    }

    [DataContract]
    public class Distance1
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class Duration1
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class End_Location1
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

    [DataContract]
    public class Polyline
    {
        [DataMember]
        public string points { get; set; }
    }

    [DataContract]
    public class Start_Location1
    {
        [DataMember]
        public float lat { get; set; }
        [DataMember]
        public float lng { get; set; }
    }

}