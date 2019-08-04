using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballScoreAPI.Data
{
    public class DbInitializer
    {
        public static void Initialize(FootballScoreContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
