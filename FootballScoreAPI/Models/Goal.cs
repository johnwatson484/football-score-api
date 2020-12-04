using System;

namespace FootballScoreAPI.Models
{
    public class Goal
    {
        public int GoalId { get; set; }

        public int FixtureId { get; set; }

        public string For { get; set; }

        public string Scorer { get; set; }

        public int Minute { get; set; }

        public bool OwnGoal { get; set; }

        public virtual Fixture Fixture { get; set; }
    }
}
