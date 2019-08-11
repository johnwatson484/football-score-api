using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballScoreAPI.Data;
using FootballScoreAPI.Models;

namespace FootballScoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixturesController : ControllerBase
    {
        private readonly FootballScoreContext context;

        public FixturesController(FootballScoreContext context)
        {
            this.context = context;
        }

        // GET: api/Goals
        [HttpGet]
        public IEnumerable<Fixture> GetScores(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null)
            {
                endDate = startDate;
            }

            return context.Fixtures.Include(x => x.Goals).Where(x => x.Date >= startDate && x.Date <= endDate)
                .OrderBy(x => x.Date);
        }
    }
}