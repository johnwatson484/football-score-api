using FootballScoreAPI.Data;
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

        public void Refresh()
        {
            ReseedDatabase();

            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddDays(-10);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                context.Fixtures.AddRange(scrapingService.ScrapeScores(date));
            }

            context.SaveChanges();
        }

        private void ReseedDatabase()
        {
            context.Database.ExecuteSqlCommand("DELETE FROM dbo.Fixtures");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Fixtures', RESEED, 0)");
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Goals', RESEED, 0)");
        }

        public void RefreshDay()
        {
            DateTime date = DateTime.Now.Date;

            context.Fixtures.RemoveRange(context.Fixtures.Where(x => x.Date == date));
            context.Fixtures.AddRange(scrapingService.ScrapeScores(date));
            context.SaveChanges();
        }
    }
}
