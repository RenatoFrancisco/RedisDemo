using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedisDemo.Data;
using RedisDemo.Models;

namespace RedisDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private const string CountriesKey = "Countries";

        private readonly ILogger<CountriesController> _logger;
        private readonly ApplicationContext _context;
        private readonly IDistributedCache _distributedCache;

        public CountriesController(ILogger<CountriesController> logger,
                                   ApplicationContext context,
                                   IDistributedCache distributedCache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countriesObject = await _distributedCache.GetStringAsync(CountriesKey);
            // await _distributedCache.RemoveAsync(CountriesKey);

            if (!string.IsNullOrWhiteSpace(countriesObject))
            {
                _logger.LogInformation("The Redis contains the key");
                return Ok(countriesObject);
            }

            _logger.LogWarning("The Redis does not contains the key");

            var countries = _context.Countries.ToList();
            var json = JsonSerializer.Serialize<List<Country>>(countries);

            var memoryCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(10),
            };

            await _distributedCache.SetStringAsync(CountriesKey, json, memoryCacheEntryOptions);

            return Ok(countries);
        }
    }
}
