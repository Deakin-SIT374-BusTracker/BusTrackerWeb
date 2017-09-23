
using BusTrackerWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// This controller handles all Search View functions.
    /// </summary>
    public class SearchController : Controller
    {
        /// <summary>
        /// Open the Search Index View.
        /// </summary>
        /// <returns>Search View.</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "BusHop > Search";

            return View();
        }

        /// <summary>
        /// Return all PTV routes as JSON formatted hints for the Search 
        /// Typeahead.
        /// </summary>
        /// <param name="q">Route name filter.</param>
        /// <returns></returns>
        public async Task<JsonResult> SearchHints(string q)
        {
            List<RouteModel> routes = 
                await WebApiApplication.PtvApiControl.GetRoutesAsync();

            string[] hints = routes.Select(r => r.RouteName).ToArray();

            return Json(hints, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all routes and associated directions for the given destination.
        /// Return a summary table as a partial view.
        /// </summary>
        /// <param name="destination">Route name filter.</param>
        /// <returns></returns>
        public async Task<ActionResult> SearchRoutes(string destination)
        {
            // Encode the search string before passing as a URL parameter.
            StringWriter writer = new StringWriter();
            Server.UrlEncode(destination, writer);
            String EncodedString = writer.ToString();

            // Get all routes matching the destination.
            List<RouteModel> routes = await WebApiApplication.PtvApiControl
                .GetRoutesByNameAsync(EncodedString);

            // For each route get the associated directions and build a new 
            // collection.
            List<SearchRouteModel> searchRoutes = new List<SearchRouteModel>();
            foreach (RouteModel route in routes)
            {
                List<DirectionModel> directions = await WebApiApplication
                    .PtvApiControl.GetRouteDirectionsAsync(route);
                searchRoutes.Add(new SearchRouteModel(route, directions));
            }

            if (searchRoutes.Count() != 0)
            {
                return PartialView("~/Views/Search/_SearchRoutes.cshtml", searchRoutes);
            }
            else
            {
                return PartialView("~/Views/Search/_SearchNoRoutes.cshtml", searchRoutes);
            }
        }
    }
}