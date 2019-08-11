using FootballScoreAPI.Models;
using System;
using System.Collections.Generic;

namespace FootballScoreAPI.Services
{
    public interface IScrapingService
    {
        List<Fixture> ScrapeScores(DateTime date);
    }
}