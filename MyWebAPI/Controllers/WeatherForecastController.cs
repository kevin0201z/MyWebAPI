using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 天气预报
        /// </summary>
        /// <returns>天气</returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 方程式(x * 10 + y - 1)
        /// </summary>
        /// <param name="x">第一个参数</param>
        /// <param name="y">第二个参数</param>
        /// <returns>结果</returns>
        [HttpGet("Math")]
        public int Get(int x,int y)
        {
            return x * 10 + y - 1;
        }

        [HttpGet("{gid}", Name = "OwnerById")]
        public IActionResult GetById(int gid)
        {
            return Ok(new { thing = gid + 1 });
        }
    }
}
