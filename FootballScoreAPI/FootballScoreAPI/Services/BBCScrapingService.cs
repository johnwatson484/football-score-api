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

        public List<Goal> ScrapeGoals(DateTime date)
        {
            SetSecurityProtocols();
            SetBrowser();
            var goals = GetGoals(date);
            CloseBrowser();
            return goals;
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

        private List<Goal> GetGoals(DateTime date)
        {
            List<Goal> goals = new List<Goal>();

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
                                Date = date,
                                Competition = name,
                                HomeTeam = homeTeam,
                                AwayTeam = awayTeam,
                                For = homeTeam,
                                Scorer = scorer,
                                Minute = int.Parse(minute.Substring(0, minute.IndexOf("+") != -1 ? minute.IndexOf("+") : minute.Length)),
                                OwnGoal = ownGoal
                            };

                            goals.Add(goal);
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
                                Date = date,
                                Competition = name,
                                HomeTeam = homeTeam,
                                AwayTeam = awayTeam,
                                For = awayTeam,
                                Scorer = scorer,
                                Minute = int.Parse(minute.Substring(0, minute.IndexOf("+") != -1 ? minute.IndexOf("+") : minute.Length)),
                                OwnGoal = ownGoal
                            };

                            goals.Add(goal);
                        }
                    }
                }
            }

            return goals;
        }
    }
}
