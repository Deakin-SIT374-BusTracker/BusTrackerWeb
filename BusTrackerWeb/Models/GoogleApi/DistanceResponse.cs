using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace BusTrackerWeb.Models.GoogleApi
{
    [DataContract]
    public class DistanceResponse
    {
        [DataMember]
        public string[] destination_addresses { get; set; }

        [DataMember]
        public string[] origin_addresses { get; set; }
            
        [DataMember]
        public Row[] rows { get; set; }

        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class Row
    {
        [DataMember]
        public Element[] elements { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember]
        public string text { get; set; }

        [DataMember]
        public int value { get; set; }
    }

    [DataContract]
    public class Element
    {
        [DataMember]
        public Item distance { get; set; }

        [DataMember]
        public Item duration { get; set; }

        [DataMember]
        public string status { get; set; }
    }
}