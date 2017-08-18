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
    public class DirectionsApiClientControllerTests
    {
        [TestMethod()]
        public void BuildDirectionsQueryTest()
        {
            DirectionsApiClientController controller = new DirectionsApiClientController();

            List<GeoCoordinate> routePoints = new List<GeoCoordinate>();

            routePoints.Add(new GeoCoordinate(-38.145, 144.355));
            routePoints.Add(new GeoCoordinate(-38.198, 144.300));
            routePoints.Add(new GeoCoordinate(-38.157, 144.356));
            routePoints.Add(new GeoCoordinate(-38.189, 144.343));

            string apiQuery = controller.BuildDirectionsQuery(routePoints.ToArray());

            Assert.IsTrue(apiQuery == "https://maps.googleapis.com/maps/api/directions/json?origin=-38.145,144.355&destination=-38.189,144.343&waypoints=-38.198,144.3|-38.157,144.356&departure_time=now&traffic_model=best_guess&key=AIzaSyAyEgv4_85K8azHU2fYz78xxGAT3ne3egU");
        }

        [TestMethod()]
        public void GetDirectionsTest()
        {
            DirectionsApiClientController controller = new DirectionsApiClientController();

            List<GeoCoordinate> routePoints = new List<GeoCoordinate>();

            // Stops 1 - 5
            routePoints.Add(new GeoCoordinate(-38.14525, 144.355118));
            routePoints.Add(new GeoCoordinate(-38.1485825, 144.3601));
            routePoints.Add(new GeoCoordinate(-38.15226, 144.358566));
            routePoints.Add(new GeoCoordinate(-38.1552, 144.357224));
            routePoints.Add(new GeoCoordinate(-38.1571426, 144.356277));

            // Stops 6 - 10
            routePoints.Add(new GeoCoordinate(-38.1594658, 144.35524));
            routePoints.Add(new GeoCoordinate(-38.16136, 144.354416));
            routePoints.Add(new GeoCoordinate(-38.163475, 144.353409));
            routePoints.Add(new GeoCoordinate(-38.1700668, 144.348251));
            routePoints.Add(new GeoCoordinate(-38.1715355, 144.3465));

            // Stops 11 - 15
            routePoints.Add(new GeoCoordinate(-38.1731, 144.344849));
            routePoints.Add(new GeoCoordinate(-38.1751976, 144.342682));
            routePoints.Add(new GeoCoordinate(-38.1779861, 144.345016));
            routePoints.Add(new GeoCoordinate(-38.1802177, 144.3452));
            routePoints.Add(new GeoCoordinate(-38.1856, 144.34436));

            // Stops 16 - 20
            routePoints.Add(new GeoCoordinate(-38.18911, 144.343414));
            routePoints.Add(new GeoCoordinate(-38.1933022, 144.342651));
            routePoints.Add(new GeoCoordinate(-38.1968346, 144.34021));
            routePoints.Add(new GeoCoordinate(-38.19655, 144.3378));
            routePoints.Add(new GeoCoordinate(-38.1960068, 144.333542));

            // Stops 21 - 25
            routePoints.Add(new GeoCoordinate(-38.19619, 144.33046));
            routePoints.Add(new GeoCoordinate(-38.1966972, 144.32785));
            routePoints.Add(new GeoCoordinate(-38.1992569, 144.327164));  // Record 23
            routePoints.Add(new GeoCoordinate(-38.2012825, 144.327454));
            routePoints.Add(new GeoCoordinate(-38.19777, 144.319763));

            // Stops 26 - 30
            routePoints.Add(new GeoCoordinate(-38.19745, 144.317886));
            routePoints.Add(new GeoCoordinate(-38.2046, 144.312454));
            routePoints.Add(new GeoCoordinate(-38.2086754, 144.31192));
            routePoints.Add(new GeoCoordinate(-38.2134056, 144.310852));
            routePoints.Add(new GeoCoordinate(-38.2154732, 144.306046));

            // Stops 31 - 35
            routePoints.Add(new GeoCoordinate(-38.2154655, 144.30365));
            routePoints.Add(new GeoCoordinate(-38.2117424, 144.301712));
            routePoints.Add(new GeoCoordinate(-38.2084846, 144.3023));
            routePoints.Add(new GeoCoordinate(-38.20561, 144.302887));
            routePoints.Add(new GeoCoordinate(-38.2001266, 144.305328));

            // Stops 36
            routePoints.Add(new GeoCoordinate(-38.1985359, 144.30014));
            
            List<Route> routes = controller.GetDirections(routePoints.ToArray());

            List<Leg> legs = new List<Leg>();
            routes.ForEach(r => legs.AddRange(r.legs));

            Assert.IsTrue(legs.Count() == 35);

        }
    }
}