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
    public class GoalsController : ControllerBase
    {
        private readonly FootballScoreContext context;

        public GoalsController(FootballScoreContext context)
        {
            this.context = context;
        }

        // GET: api/Goals
        [HttpGet]
        public IEnumerable<Goal> GetGoals(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null)
            {
                endDate = startDate;
            }

            return context.Goals.Where(x => x.Date >= startDate && x.Date <= endDate)
                .OrderBy(x=>x.Date).ThenBy(x=>x.Minute);
        }
    }
}