using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedisDemo.Data;
using RedisDemo.Infra;
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

        private readonly ICacheProvider _cache;

        public CountriesController(ILogger<CountriesController> logger,
                                   ApplicationContext context,
                                   IDistributedCache distributedCache,
                                   ICacheProvider cache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countriesObject = await _cache.GetFromCacheAsync<List<Country>>(CountriesKey);
            
            if (countriesObject is not null)
            {
                _logger.LogInformation("Redis contains the key");
                return Ok(countriesObject);
            }

            _logger.LogWarning("Redis does not contains the key");

            var countries = _context.Countries.ToList();

            var memoryCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(50),
                SlidingExpiration = TimeSpan.FromSeconds(30),
            };

            await _cache.SetCacheAsync<List<Country>>(CountriesKey, countries, memoryCacheEntryOptions);

            return Ok(countries);
        }
    }
}
