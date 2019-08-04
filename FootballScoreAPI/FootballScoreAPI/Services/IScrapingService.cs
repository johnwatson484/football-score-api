using System;
using System.Collections.Generic;
using FootballScoreAPI.Models;

namespace FootballScoreAPI.Services
{
    public interface IScrapingService
    {
        List<Goal> ScrapeGoals(DateTime date);
    }
}