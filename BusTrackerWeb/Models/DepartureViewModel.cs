using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusTrackerWeb.Models
{
    public class DepartureViewModel
    {
        public List<DepartureModel> Departures { get; set; }

        public DepartureViewModel()
        { }

        public DepartureViewModel(List<DepartureModel> departures)
        {
            Departures = departures;
        }
    }
}