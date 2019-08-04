﻿using FootballScoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballScoreAPI.Services
{
    public class RefreshService : IRefreshService
    {
        FootballScoreContext context;
        IScrapingService scrapingService;

        public RefreshService(FootballScoreContext context, IScrapingService scrapingService)
        {
            this.context = context;
            this.scrapingService = scrapingService;
        }

        public void Refresh()
        {
            context.Goals.RemoveRange(context.Goals);
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Goals', RESEED, 0)");

            DateTime endDate = DateTime.Now.Date;
            DateTime startDate = endDate.AddDays(-10);

            for (DateTime date = startDate; date <= endDate; date.AddDays(1))
            {
                var goals = scrapingService.ScrapeGoals(date);
                context.Goals.AddRange(goals);
            }

            context.SaveChanges();
        }
    }
}
