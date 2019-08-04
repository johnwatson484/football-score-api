using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FootballScoreAPI.Models;
using FootballScoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballScoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Goal>> Get()
        {
            BBCScrapingService bbcScrapingService = new BBCScrapingService();
            return bbcScrapingService.ScrapeScores(new DateTime(2019, 8, 3));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
