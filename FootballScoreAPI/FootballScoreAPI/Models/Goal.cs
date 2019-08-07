using System;

namespace FootballScoreAPI.Models
{
    public class Goal
    {
        public int GoalId { get; set; }

        public DateTime Date { get; set; }

        public string Competition { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public string For { get; set; }

        public string Against
        {
            get
            {
                return For == HomeTeam ? AwayTeam : HomeTeam;
            }
        }

        public string Scorer { get; set; }

        public int Minute { get; set; }

        public bool OwnGoal { get; set; }
    }
}
