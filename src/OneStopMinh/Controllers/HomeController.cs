using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneStopMinh.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneStopMinh.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Wiki(string place)
        {
            var client = new RestClient("https://en.wikipedia.org/w/api.php");

            var request = new RestRequest(Method.GET);
            request.AddParameter("format", "json");
            request.AddParameter("action","parse");
            request.AddParameter("page", place);
            request.AddParameter("prop", "text");
            request.AddParameter("section", 0);
            request.AddHeader("Api-User-Agent", "Travel-App");

            var response = new RestResponse();

            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Coord(string place)
        {
            var client = new RestClient("https://geocoder.cit.api.here.com/6.2/");

            var request = new RestRequest("geocode.json?",Method.GET);
            request.AddParameter("searchtext", place);
            request.AddParameter("app_id", env.HereKey);
            request.AddParameter("app_code", env.HereSecret);
            request.AddParameter("gen", "8");

            var response = new RestResponse();

            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Restaurants(string place)
        {
            var client = new RestClient("http://api.yelp.com/v3");
            var request = new RestRequest("/businesses/search", Method.GET);
            request.AddParameter("term", "restaurant");
            request.AddParameter("location", place);
            request.AddHeader("Authorization", "Bearer " + env.YelpToken);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Hotels(string place)
        {
            var client = new RestClient("http://api.yelp.com/v3");
            var request = new RestRequest("/businesses/search", Method.GET);
            request.AddParameter("term", "hotel");
            request.AddParameter("location", place);
            request.AddHeader("Authorization", "Bearer " + env.YelpToken);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Weather(string place)
        {
            var client = new RestClient("http://api.openweathermap.org/data/2.5/");
            var request = new RestRequest("forecast?q=" + place, Method.GET);
            request.AddParameter("appid", env.apiWeatherKey);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult CurrencyCode(string countryCode)
        {
            var client = new RestClient("https://restcountries.eu/rest/v2/alpha/" + countryCode);
            var request = new RestRequest(Method.GET);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }


        [HttpPost]
        public IActionResult Exchange(string currencyCode)
        {
            var client = new RestClient("http://apilayer.net/api/live?");
            var request = new RestRequest(Method.GET);
            request.AddParameter("access_key", env.currencyKey);
            request.AddParameter("currencies", "USD," + currencyCode);
            request.AddParameter("format", "1");
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Attractions(string latitude, string longitude)
        {
            var client = new RestClient("https://places.demo.api.here.com/places/v1/discover/here?");
            var request = new RestRequest(Method.GET);
            string location = latitude + ',' + longitude;
            request.AddParameter("at", location);
            request.AddParameter("app_id", env.HereKey);
            request.AddParameter("app_code", env.HereSecret);
            request.AddParameter("gen", "8");
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Airport(string latitude, string longitude)
        {
            var client = new RestClient("https://places.cit.api.here.com/places/v1/discover/around");
            var request = new RestRequest(Method.GET);
            string location = latitude + ',' + longitude;
            request.AddParameter("at", location);
            request.AddParameter("app_id", env.HereKey);
            request.AddParameter("app_code", env.HereSecret);
            request.AddParameter("cat", "airport");
            request.AddParameter("gen", "8");
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }

        [HttpPost]
        public IActionResult Flights(string queries)
        {
            var client = new RestClient("https://www.googleapis.com/qpxExpress/v1/trips/search?key=AIzaSyBIo1tB-n0yvzIBBaMubegAa52S-9uvnKY");
            var request = new RestRequest(Method.POST);
            var newPassenger = new Passengers() { adultCount = 1 };
            var newSlouse = new Slouse() { origin = "SEA", destination = "LAX", date = "2017-6-30" };
            var newRequest = new Request()
            {
                passengers = newPassenger,
                slice = new List<Slouse>() { newSlouse }
            };
            var newRootObject = new RootObject()
            {
                request = newRequest
            };
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(newRootObject);
            var response = new RestResponse();
            Task.Run(async () =>
            {
                response = await GetResponseContentAsync(client, request) as RestResponse;
            }).Wait();

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content);

            return Json(jsonResponse);
        }


        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();

            theClient.ExecuteAsync(theRequest, response =>
            {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }


        //Save to databases actions
        [HttpPost]
        public IActionResult SaveAttractions(string attractionName)
        {
            Attraction newAttraction = new Attraction();
            newAttraction.Name = attractionName;
            newAttraction.Tourist = _db.Tourists.FirstOrDefault(i => i.UserName == User.Identity.Name);
            _db.Attractions.Add(newAttraction);
            _db.SaveChanges();
            return Json(newAttraction);
        }
   
    }

    public class Passengers
    {
        public int adultCount { get; set; }
    }

    public class Slouse
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string date { get; set; }
    }

    public class Request
    {
        public Passengers passengers { get; set; }
        public List<Slouse> slice { get; set; }
    }

    public class RootObject
    {
        public Request request { get; set; }
    }
}
