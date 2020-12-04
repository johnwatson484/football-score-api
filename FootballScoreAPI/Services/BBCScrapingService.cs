using FootballScoreAPI.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace FootballScoreAPI.Services
{
    public class BBCScrapingService : IScrapingService
    {
        private const string root = "https://www.bbc.com";
        private static ChromeDriver driver;
        private WebDriverWait wait;

        private static readonly List<string> leagues = new List<string>
        {
            "PREMIER LEAGUE",
            "CHAMPIONSHIP",
            "LEAGUE ONE",
            "LEAGUE TWO",
            "THE FA CUP",
            "EFL CUP"
        };

        public List<Fixture> ScrapeScores(DateTime date)
        {
            SetSecurityProtocols();
            SetBrowser();
            SetWait();
            var fixtures = GetScores(date);
            CloseBrowser();
            return fixtures;
        }

        private void SetBrowser()
        {
            driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void SetWait()
        {
            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
        }

        private void CloseBrowser()
        {
            driver.Quit();
        }

        private void SetSecurityProtocols()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
        }

        private List<Fixture> GetScores(DateTime date)
        {
            NavigateToPage(date);

            List<Fixture> fixtures = new List<Fixture>();

            var competitions = driver.FindElementsByClassName("qa-match-block");

            foreach (var competition in competitions)
            {
                var name = competition.FindElement(By.CssSelector("h3")).Text?.ToUpper();

                if (!leagues.Exists(x => x == name))
                {
                    continue;
                }

                var matches = competition.FindElements(By.ClassName("gs-o-list-ui__item"));

                foreach (var match in matches)
                {
                    AddFixture(date, fixtures, name, match);
                }
            }

            return fixtures;
        }

        private static void AddFixture(DateTime date, List<Fixture> fixtures, string name, IWebElement match)
        {
            try
            {
                string homeScore = match.FindElement(By.ClassName("sp-c-fixture__number--home")).Text;
                string awayScore = match.FindElement(By.ClassName("sp-c-fixture__number--away")).Text;

                // If match postphoned or abandoned then ignore
                if (char.IsLetter(homeScore[0]))
                {
                    return;
                }

                string homeTeam = match.FindElement(By.ClassName("sp-c-fixture__team-name--home")).FindElement(By.CssSelector("abbr")).GetAttribute("title");
                string awayTeam = match.FindElement(By.ClassName("sp-c-fixture__team-name--away")).FindElement(By.CssSelector("abbr")).GetAttribute("title");

                Fixture fixture = new Fixture
                {
                    Date = date,
                    Competition = name,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    HomeScore = int.Parse(homeScore),
                    AwayScore = int.Parse(awayScore),
                    Goals = new List<Goal>()
                };

                AddGoals(match, homeTeam, awayTeam, fixture);

                fixtures.Add(fixture);
            }
            catch (NoSuchElementException)
            {
                return;
            }
        }

        private static void AddGoals(IWebElement match, string homeTeam, string awayTeam, Fixture fixture)
        {
            foreach (var homeGoal in GetGoalElements(match, "sp-c-fixture__scorers-home"))
            {
                AddGoal(homeTeam, fixture, homeGoal);
            }

            foreach (var awayGoal in GetGoalElements(match, "sp-c-fixture__scorers-away"))
            {
                AddGoal(awayTeam, fixture, awayGoal);
            }
        }

        private void NavigateToPage(DateTime date)
        {
            driver.Navigate().GoToUrl(new Uri(string.Format("{0}/sport/football/scores-fixtures/{1}", root, date.ToString("yyyy-MM-dd"))));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".qa-match-block")));
            driver.FindElementByClassName("qa-show-scorers-button").Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".qa-match-block")));
        }

        private static ReadOnlyCollection<IWebElement> GetGoalElements(IWebElement match, string className)
        {
            try
            {
                return match.FindElement(By.ClassName(className)).FindElements(By.CssSelector("li"));
            }
            catch (NoSuchElementException)
            {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
        }

        private static void AddGoal(string team, Fixture fixture, IWebElement goalElement)
        {
            var goalDetails = goalElement.FindElements(By.CssSelector("span"));

            var dismissed = goalDetails[2].FindElements(By.CssSelector("i"));
            if (dismissed.Count == 0)
            {
                var scorer = goalDetails[0].Text;
                var minute = goalDetails[2].FindElements(By.CssSelector("span"))[0].Text.Replace("'", string.Empty);
                var ownGoal = goalDetails[2].FindElements(By.CssSelector("span"))[2].Text.Replace(" ", string.Empty) == "og" ? true : false;

                fixture.Goals.Add(new Goal
                {
                    For = team,
                    Scorer = scorer,
                    Minute = int.Parse(minute.Substring(0, minute.IndexOf("+") != -1 ? minute.IndexOf("+") : minute.Length)),
                    OwnGoal = ownGoal
                });
            }
        }
    }
}
