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
            var connection = context.GetHttpContext().Connection;

            if (connection.RemoteIpAddress == null)
            {
                return false;
            }

            if (connection.RemoteIpAddress.ToString() == "127.0.0.1" || connection.RemoteIpAddress.ToString() == "::1")
            {
                return true;
            }

            if (connection.RemoteIpAddress == connection.LocalIpAddress)
            {
                return true;
            }

            return false;
        }
    }
}

