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
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Goals");

            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddDays(-10);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var goals = scrapingService.ScrapeGoals(date);
                context.Goals.AddRange(goals);
            }

            context.SaveChanges();
        }

        public void RefreshDay()
        {
            DateTime date = DateTime.Now.Date;

            context.Goals.RemoveRange(context.Goals.Where(x => x.Date == date));

            var goals = scrapingService.ScrapeGoals(date);
            context.Goals.AddRange(goals);

            context.SaveChanges();
        }
    }
}
