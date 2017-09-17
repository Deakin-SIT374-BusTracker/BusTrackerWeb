using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using System.Device.Location;
using System.Text;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Google Maps Directions API requests.
    /// </summary>
    public class DirectionsApiClientController
    {
        const string MAPS_API_BASE_URL = "https://maps.googleapis.com/maps/api/directions/json?";
        const int MAPS_MAX_WAYPOINTS = 22;

        HttpClient Client { get; set; }

        /// <summary>
        /// Initialise the client controller.
        /// </summary>
        public DirectionsApiClientController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public List<Route> GetDirections(GeoCoordinate[] routePoints, bool isWalking = false)
        {
            List<Route> routes = new List<Route>();

            try
            {
                // Determine how many requests are required i.e. each request has a maximum number
                // of waypoints allowed.
                int requestsMax = (routePoints.Count() / MAPS_MAX_WAYPOINTS) + 1; 
                
                for(int i = 0; i < requestsMax; i++)
                {
                    // For the first route.
                    GeoCoordinate[] requestPoints;
                    if (i == 0)
                    {
                        // Take the maximum number of waypoints.
                        requestPoints = routePoints.Take(MAPS_MAX_WAYPOINTS).ToArray();
                    }
                    else
                    {
                        // Take the last waypoint of the previous set and the maximum number of waypoints.
                        // This is required to maintain route continuity between requests.
                        requestPoints = routePoints.Skip((i * MAPS_MAX_WAYPOINTS) - 1).Take(MAPS_MAX_WAYPOINTS + 1).ToArray();
                    }


                    string requestQuery = BuildDirectionsQuery(requestPoints, isWalking);

                    HttpWebRequest request = WebRequest.Create(requestQuery) as HttpWebRequest;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription)); 
                        }

                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DirectionsResponse));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                        DirectionsResponse jsonResponse = objResponse as DirectionsResponse;

                        routes.Add(jsonResponse.routes.First());
                    }
                }
            }
            catch (Exception)
            {
            }

            return routes;
        }

        public string BuildDirectionsQuery(GeoCoordinate[] routePoints, bool isWalking = false)
        {
            string origin = string.Empty;
            string destination = string.Empty;

            StringBuilder waypointBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder();

            // Determine API query parameters from route points.
            waypointBuilder.Append("&waypoints=");
            int pointCount = routePoints.Count();
            for (int i = 0; i < pointCount; i++)
            {
                string latitude = routePoints[i].Latitude.ToString();
                string longitude = routePoints[i].Longitude.ToString();

                // Set the origin.
                if (i == 0)
                {
                    origin = string.Format("origin={0},{1}", latitude, longitude);
                }
                // Set the destination.
                else if (i == pointCount - 1)
                {
                    destination = string.Format("&destination={0},{1}", latitude, longitude);
                }
                // Add last waypoint.
                else if (i == pointCount - 2)
                {
                    waypointBuilder.AppendFormat("{0},{1}", latitude, longitude);
                }
                // Add midwaypoint.
                else
                {
                    waypointBuilder.AppendFormat("{0},{1}|", latitude, longitude);
                }
            }

            // Set the API key.
            string apiKey = Properties.Settings.Default.MapsApiDeveloperKey;

            // Build the API query.
            if (isWalking)
            {
                queryBuilder.AppendFormat("{0}{1}{2}{3}&mode=walking&departure_time=now&traffic_model=best_guess&key={4}",
                    MAPS_API_BASE_URL, origin, destination, waypointBuilder, apiKey);
            }
            else
            {
                queryBuilder.AppendFormat("{0}{1}{2}{3}&departure_time=now&traffic_model=best_guess&key={4}",
                    MAPS_API_BASE_URL, origin, destination, waypointBuilder, apiKey);
            }

            return queryBuilder.ToString();
        }
    }
}