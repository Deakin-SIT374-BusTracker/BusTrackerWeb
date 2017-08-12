using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusTrackerWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusTrackerWeb.Models;


namespace BusTrackerWeb.Controllers.Tests
{
    [TestClass()]
    public class PtvApiClientControllerTests
    {
        [TestMethod()]
        public async Task GetRoutesAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();

            List<RouteModel> routes = await apiControl.GetRoutesAsync();

            RouteModel testRoute =
                routes.Find(r => r.RouteName.Contains("Geelong Station") &&
                r.RouteName.Contains("Deakin University") &&
                r.RouteNumber == "41");

            Assert.IsTrue(testRoute.RouteId == 10846);
        }

        [TestMethod()]
        public async Task GetRoutesByNameAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();

            List<RouteModel> routes = await apiControl.GetRoutesByNameAsync("Deakin");

            Assert.IsTrue(routes.Count == 9);
        }


        [TestMethod()]
        public async Task GetRouteRunsAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();
            RouteModel route = new RouteModel { RouteId = 10846 };

            List<RunModel> runs = await apiControl.GetRouteRunsAsync(route);

            Assert.IsNotNull(runs.Count != 0);
        }


        [TestMethod()]
        public async Task GetRouteStopsAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();
            RouteModel route = new RouteModel { RouteId = 10846 };

            List<StopModel> stops = await apiControl.GetRouteStopsAsync(route);

            Assert.IsTrue(stops.Count == 52);
        }


        [TestMethod()]
        public async Task GetRouteDirectionsAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();
            RouteModel route = new RouteModel { RouteId = 10846 };

            List<DirectionModel> directions = await apiControl.GetRouteDirectionsAsync(route);

            Assert.IsTrue(directions.Count == 2);
        }

        [TestMethod()]
        public async Task GetRouteAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();

            RouteModel route = await apiControl.GetRouteAsync(10846);

            Assert.IsTrue(route.RouteName == "Geelong Station - Deakin University via Grovedale");
        }

        [TestMethod()]
        public async Task GetDirectionAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();
            RouteModel route = new RouteModel { RouteId = 10846 };

            DirectionModel direction = await apiControl.GetDirectionAsync(8, route);

            Assert.IsTrue(direction.DirectionName == "Deakin University");
        }

        [TestMethod()]
        public async Task GetRunPatternAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();
            RunModel run = new RunModel { RunId = 94095 };
            run.Route = new RouteModel { RouteId = 10846 };

            StoppingPatternModel pattern = await apiControl.GetStoppingPatternAsync(run);

            Assert.IsTrue(pattern.Departures.Count != 0);
        }

        [TestMethod()]
        public async Task GetStopsByDistanceAsyncTest()
        {
            PtvApiClientController apiControl = new PtvApiClientController();

            decimal latitude = -38.145M;
            decimal longitude = 144.354M;
            int maxDistance = 150;
            int maxStops = 30;

            List<StopModel> stops = 
                await apiControl.GetStopsByDistanceAsync(latitude, longitude, maxStops, maxDistance);

            Assert.IsTrue(stops.Count == 2);
        }
    }
}