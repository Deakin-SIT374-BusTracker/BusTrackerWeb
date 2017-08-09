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
using BusTrackerWeb.Models.GoogleApi;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Google GeoCode API requests.
    /// </summary>
    public class GeoCodeApiClientController
    {
        const string MAPS_API_BASE_URL = "https://maps.googleapis.com/maps/api/geocode/json?";

        HttpClient Client { get; set; }

        /// <summary>
        /// Initialise the client controller.
        /// </summary>
        public GeoCodeApiClientController()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public string GetAddress(GeoCoordinate location)
        {
            string returnAddress = string.Empty;

            try
            {
                string urlquery = BuildAddressQuery(location);

                HttpWebRequest request = WebRequest.Create(urlquery) as HttpWebRequest;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoCodeResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    GeoCodeResponse jsonResponse = objResponse as GeoCodeResponse;

                    returnAddress = jsonResponse.results.First().formatted_address;
                }
            }
            catch (Exception e)
            {
            }

            return returnAddress;
        }

        public string BuildAddressQuery(GeoCoordinate location)
        {
            StringBuilder queryBuilder = new StringBuilder();
            
            // Build coordinate parameters.
            string locationParam = string.Format("latlng={0},{1}", location.Latitude, location.Longitude);

            // Set the API key.
            string apiKey = Properties.Settings.Default.MapsApiDeveloperKey;

            // Build the API query.
            queryBuilder.AppendFormat("{0}{1}&key={2}",
                MAPS_API_BASE_URL, locationParam, apiKey);

            return queryBuilder.ToString();
        }
    }
}