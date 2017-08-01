using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Bus Tracker About View actions.
    /// </summary>
    public class AboutController : Controller
    {
        /// <summary>
        /// Open the About View.
        /// </summary>
        /// <returns>Index View.</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "BusHop > About";

            return View();
        }
    }
}
