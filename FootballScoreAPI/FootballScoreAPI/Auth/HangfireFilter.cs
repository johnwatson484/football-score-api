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
            if (string.IsNullOrEmpty(context.Request.RemoteIpAddress))
            {
                return false;
            }

            if (context.Request.RemoteIpAddress == "127.0.0.1" || context.Request.RemoteIpAddress == "::1")
            {
                return true;
            }

            if (context.Request.RemoteIpAddress == context.Request.LocalIpAddress)
            {
                return true;
            }

            return false;
        }
    }
}

