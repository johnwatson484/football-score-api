using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballScoreAPI.Models
{
    public class Fixture
    {
        public int FixtureId { get; set; }

        public DateTime Date { get; set; }

        public string Competition { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public virtual IList<Goal> Goals { get; set; }
    }
}
