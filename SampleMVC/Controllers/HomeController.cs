using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SampleMVC.Models;
using SampleMVC.Services;

namespace SampleMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;

        public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> Weather()
        {
            using var client = new HttpClient();
            var tokenResp = await _tokenService.GetToken("weatherapi.read");
            client.SetBearerToken(tokenResp.AccessToken);
            
            var res = client
                .GetAsync("https://localhost:5444/weatherforecast")
                .Result;

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Unable to get content");
            }
                
            var model = res.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
                    
            return View(data);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}