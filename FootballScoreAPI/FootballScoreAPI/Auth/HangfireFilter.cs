using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballScoreAPI.Auth
{
    public class HangfireFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if(httpContext.Request.Host.Value.StartsWith("localhost:"))
            {
                return true;
            }

            return false;
        }
    }
}

