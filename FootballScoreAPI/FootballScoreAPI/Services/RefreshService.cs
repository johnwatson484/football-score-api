using FootballScoreAPI.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FootballScoreAPI.Services
{
    public class RefreshService : IRefreshService
    {
        readonly FootballScoreContext context;
        readonly IScrapingService scrapingService;

        public RefreshService(FootballScoreContext context, IScrapingService scrapingService)
        {
            this.context = context;
            this.scrapingService = scrapingService;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(10)]
        public void Refresh()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM dbo.Fixtures");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Fixtures', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Goals', RESEED, 0)");

            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddDays(-10);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var fixtures = scrapingService.ScrapeScores(date);
                context.Fixtures.AddRange(fixtures);
            }

            context.SaveChanges();
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(10)]
        public void RefreshDay()
        {
            DateTime date = DateTime.Now.Date;

            context.Fixtures.RemoveRange(context.Fixtures.Where(x => x.Date == date));

            var fixture = scrapingService.ScrapeScores(date);
            context.Fixtures.AddRange(fixture);

            context.SaveChanges();
        }
    }
}
