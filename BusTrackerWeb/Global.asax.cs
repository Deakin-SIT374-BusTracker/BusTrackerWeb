using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BusTrackerWeb.Controllers;
using BusTrackerWeb.Models;

namespace BusTrackerWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static PtvApiClientController PtvApiControl;

        public static DirectionsApiClientController DirectionsApiControl;

        public static SnappedApiClientController SnappedApiControl;

        public static GeoCodeApiClientController GeocodeApiControl;

        public static List<BusModel> TrackedBuses;

        public static List<RunModel> RunsCache;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Setup a single app PTV API Client i.e. to prevent http port exhaustion.
            PtvApiControl = new Controllers.PtvApiClientController();

            // Setup a single app Google Directions API Client i.e. to prevent http port exhaustion.
            DirectionsApiControl = new Controllers.DirectionsApiClientController();

            // Setup a single app Google Geolocation API Client i.e. to prevent http port exhaustion.
            GeocodeApiControl = new Controllers.GeoCodeApiClientController();

            // Setup a single app Google Snap to Road API Client i.e. to prevent http port exhaustion.
            SnappedApiControl = new Controllers.SnappedApiClientController();

            // Setup a static collection of tracked buses.
            TrackedBuses = new List<BusModel>();

            // Setup a static collection of cached runs.
            RunsCache = new List<RunModel>();
        }
    }
}
