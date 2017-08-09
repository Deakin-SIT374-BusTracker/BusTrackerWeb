using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusTrackerWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusTrackerWeb.Models;
using System.Device.Location;
using BusTrackerWeb.Models.GoogleApi;

namespace BusTrackerWeb.Controllers.Tests
{
    [TestClass()]
    public class GeoCodeApiClientControllerTests
    {
        [TestMethod()]
        public void BuildAddressQueryTest()
        {
            GeoCodeApiClientController controller = new GeoCodeApiClientController();

            GeoCoordinate location = new GeoCoordinate(30.3449153, -81.860543);

            string apiQuery = controller.BuildAddressQuery(location);

            Assert.IsTrue(apiQuery == "https://maps.googleapis.com/maps/api/geocode/json?latlng=30.3449153,-81.860543&key=AIzaSyAyEgv4_85K8azHU2fYz78xxGAT3ne3egU");
        }

        [TestMethod()]
        public void GetDistanceTest()
        {
            GeoCodeApiClientController controller = new GeoCodeApiClientController();

            GeoCoordinate location = new GeoCoordinate(30.3449153, -81.860543);

            string address = controller.GetAddress(location);

            Assert.IsTrue(address == "Jacksonville-Baldwin Trail, Jacksonville, FL 32220, USA");
        }
    }
}