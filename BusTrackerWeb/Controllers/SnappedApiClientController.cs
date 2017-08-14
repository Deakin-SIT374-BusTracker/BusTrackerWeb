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
    /// Controls all Google Maps Snap to Road API requests.
    /// </summary>
    public class SnappedApiClientController
    {
        const string MAPS_API_BASE_URL = "https://roads.googleapis.com/v1/snapToRoads?path=";
        const int MAPS_MAX_WAYPOINTS = 100;

        HttpClient Client { get; set; }

        /// <summary>
        /// Initialise the client controller.
        /// </summary>
        public SnappedApiClientController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public List<SnappedPoints> GetSnappedPoints(GeoCoordinate[] routePoints)
        {
            List<SnappedPoints> snappedPoints = new List<SnappedPoints>();

            try
            {
                // Determine how many requests are required i.e. each request has a maximum number
                // of waypoints allowed.
                int requestsMax = (routePoints.Count() / MAPS_MAX_WAYPOINTS) + 1; 
                
                for(int i = 0; i < requestsMax; i++)
                {
                    GeoCoordinate[] requestPoints = routePoints.Skip((i * MAPS_MAX_WAYPOINTS)-1).Take(MAPS_MAX_WAYPOINTS).ToArray();

                    string requestQuery = BuildSnappedPointsQuery(requestPoints);

                    HttpWebRequest request = WebRequest.Create(requestQuery) as HttpWebRequest;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription)); 
                        }

                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SnappedResponse));
                        object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                        SnappedResponse jsonResponse = objResponse as SnappedResponse;

                        snappedPoints.AddRange(jsonResponse.snappedPoints);
                    }
                }
            }
            catch (Exception)
            {
            }

            return snappedPoints;
        }

        public string BuildSnappedPointsQuery(GeoCoordinate[] routePoints)
        {
            string origin = string.Empty;
            string destination = string.Empty;

            StringBuilder waypointBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder();

            // Determine API query parameters from route points.
            int pointCount = routePoints.Count();
            for (int i = 0; i < pointCount; i++)
            {
                string latitude = routePoints[i].Latitude.ToString();
                string longitude = routePoints[i].Longitude.ToString();

                if (i >= (pointCount - 1))
                {
                    waypointBuilder.AppendFormat("{0},{1}", latitude, longitude);
                }
                else
                {
                    waypointBuilder.AppendFormat("{0},{1}|", latitude, longitude);
                }
            }

            // Set the API key.
            string apiKey = Properties.Settings.Default.MapsApiDeveloperKey;

            // Build the API query.
            queryBuilder.AppendFormat("{0}{1}&interpolate=true&key={2}",
                MAPS_API_BASE_URL, waypointBuilder, apiKey);

            return queryBuilder.ToString();
        }
    }
}