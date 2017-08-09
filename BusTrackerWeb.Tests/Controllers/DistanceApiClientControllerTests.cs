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
    public class DistanceApiClientControllerTests
    {
        [TestMethod()]
        public void BuildDistanceQueryTest()
        {
            DistanceApiClientController controller = new DistanceApiClientController();

            GeoCoordinate origin = new GeoCoordinate(30.3449153, -81.860543);
            GeoCoordinate destination = new GeoCoordinate(33.7676932, -84.4906437);

            string apiQuery = controller.BuildDistanceQuery(origin, destination);

            Assert.IsTrue(apiQuery == "https://maps.googleapis.com/maps/api/distancematrix/json?origins=30.3449153,-81.860543&destinations=33.7676932,-84.4906437&key=AIzaSyAyEgv4_85K8azHU2fYz78xxGAT3ne3egU");
        }

        [TestMethod()]
        public void GetDistanceTest()
        {
            DistanceApiClientController controller = new DistanceApiClientController();

            GeoCoordinate origin = new GeoCoordinate(30.3449153, -81.860543);
            GeoCoordinate destination = new GeoCoordinate(33.7676932, -84.4906437);

            decimal distance = controller.GetDistance(origin, destination);

            Assert.IsTrue(distance == 554.617M);

        }
    }
}