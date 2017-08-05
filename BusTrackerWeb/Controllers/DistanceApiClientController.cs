using BusTrackerWeb.Models;
using BusTrackerWeb.Models.DirectionsApi;
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
using BusTrackerWeb.Models.DistanceApi;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Google Maps Distance Matrix API requests.
    /// </summary>
    public class DistanceApiClientController
    {
        const string MAPS_API_BASE_URL = "https://maps.googleapis.com/maps/api/distancematrix/json?";
        const int MAPS_MAX_WAYPOINTS = 23;

        HttpClient Client { get; set; }

        /// <summary>
        /// Initialise the client controller.
        /// </summary>
        public DistanceApiClientController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public decimal GetDistance(GeoCoordinate origin , GeoCoordinate destination)
        {
            decimal returnDistance = 0.0M;

            try
            {
                string urlquery = BuildDistanceQuery(origin, destination);

                HttpWebRequest request = WebRequest.Create(urlquery) as HttpWebRequest;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DistanceResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    DistanceResponse jsonResponse = objResponse as DistanceResponse;

                    returnDistance = Convert.ToDecimal(jsonResponse.rows.First().elements.First().distance.value)/1000;
                }
            }
            catch (Exception e)
            {
            }

            return returnDistance;
        }

        public string BuildDistanceQuery(GeoCoordinate origin, GeoCoordinate destination)
        {
            StringBuilder queryBuilder = new StringBuilder();
            
            // Build coordinate parameters.
            string originParam = string.Format("origins={0},{1}", origin.Latitude, origin.Longitude);
            string destinationParam = string.Format("&destinations={0},{1}", destination.Latitude, destination.Longitude);

            // Set the API key.
            string apiKey = Properties.Settings.Default.MapsApiDeveloperKey;

            // Build the API query.
            queryBuilder.AppendFormat("{0}{1}{2}&key={3}",
                MAPS_API_BASE_URL, originParam, destinationParam, apiKey);


            return queryBuilder.ToString();
        }
    }
}