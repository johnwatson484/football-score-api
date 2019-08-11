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
        private static ChromeDriver driver;

        private static readonly List<string> leagues = new List<string>
        {
            "CHAMPIONSHIP",
            "LEAGUE ONE",
            "LEAGUE TWO"
        };

        public List<Fixture> ScrapeScores(DateTime date)
        {
            SetSecurityProtocols();
            SetBrowser();
            var fixtures = GetScores(date);
            CloseBrowser();
            return fixtures;
        }

        private void SetBrowser()
        {
            driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
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
            List<Fixture> fixtures = new List<Fixture>();

            string root = "https://www.bbc.com";

            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));

            driver.Navigate().GoToUrl(new Uri(string.Format("{0}/sport/football/scores-fixtures/{1}", root, date.ToString("yyyy-MM-dd"))));

            driver.FindElementByClassName("qa-show-scorers-button").Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".qa-match-block")));

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
                    string homeTeam = match.FindElement(By.ClassName("sp-c-fixture__team-name--home")).FindElement(By.CssSelector("abbr")).GetAttribute("title");
                    string awayTeam = match.FindElement(By.ClassName("sp-c-fixture__team-name--away")).FindElement(By.CssSelector("abbr")).GetAttribute("title");

                    string homeScore = match.FindElement(By.ClassName("sp-c-fixture__number--home")).Text;
                    string awayScore = match.FindElement(By.ClassName("sp-c-fixture__number--away")).Text;

                    if (char.IsLetter(homeScore[0]))
                    {
                        continue;
                    }

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

                    ReadOnlyCollection<IWebElement> homeGoals;

                    try
                    {
                        homeGoals = match.FindElement(By.ClassName("sp-c-fixture__scorers-home")).FindElements(By.CssSelector("li"));
                    }
                    catch (NoSuchElementException)
                    {
                        homeGoals = new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
                    }

                    foreach (var homeGoal in homeGoals)
                    {
                        var goalDetails = homeGoal.FindElements(By.CssSelector("span"));

                        var dismissed = goalDetails[2].FindElements(By.CssSelector("i"));
                        if (dismissed.Count == 0)
                        {
                            var scorer = goalDetails[0].Text;
                            var minute = goalDetails[2].FindElements(By.CssSelector("span"))[0].Text.Replace("'", string.Empty);
                            var ownGoal = goalDetails[2].FindElements(By.CssSelector("span"))[2].Text.Replace(" ", string.Empty) == "og" ? true : false;

                            var goal = new Goal
                            {
                                For = homeTeam,
                                Scorer = scorer,
                                Minute = int.Parse(minute.Substring(0, minute.IndexOf("+") != -1 ? minute.IndexOf("+") : minute.Length)),
                                OwnGoal = ownGoal
                            };

                            fixture.Goals.Add(goal);
                        }
                    }

                    ReadOnlyCollection<IWebElement> awayGoals;

                    try
                    {
                        awayGoals = match.FindElement(By.ClassName("sp-c-fixture__scorers-away")).FindElements(By.CssSelector("li"));
                    }
                    catch (NoSuchElementException)
                    {
                        awayGoals = new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
                    }

                    foreach (var awayGoal in awayGoals)
                    {
                        var goalDetails = awayGoal.FindElements(By.CssSelector("span"));

                        var dismissed = goalDetails[2].FindElements(By.CssSelector("i"));
                        if (dismissed.Count == 0)
                        {
                            var scorer = goalDetails[0].Text;
                            var minute = goalDetails[2].FindElements(By.CssSelector("span"))[0].Text.Replace("'", string.Empty);
                            var ownGoal = goalDetails[2].FindElements(By.CssSelector("span"))[2].Text.Replace(" ", string.Empty) == "og" ? true : false;

                            var goal = new Goal
                            {
                                For = awayTeam,
                                Scorer = scorer,
                                Minute = int.Parse(minute.Substring(0, minute.IndexOf("+") != -1 ? minute.IndexOf("+") : minute.Length)),
                                OwnGoal = ownGoal
                            };

                            fixture.Goals.Add(goal);
                        }
                    }
                    fixtures.Add(fixture);
                }
            }

            return fixtures;
        }
    }
}
