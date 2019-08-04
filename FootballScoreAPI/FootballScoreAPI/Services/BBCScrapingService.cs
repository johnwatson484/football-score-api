using FootballScoreAPI.Models;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FootballScoreAPI.Services
{
    public class BBCScrapingService : IScrapingService
    {
        private static ScrapingBrowser browser;

        public List<Goal> ScrapeGoals(DateTime date)
        {
            SetSecurityProtocols();
            SetBrowser();
            return GetGoals(date);
        }

        private void SetBrowser()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Setting browser");
            Console.ResetColor();
            browser = new ScrapingBrowser();
            browser.AllowAutoRedirect = true;
            browser.AllowMetaRedirect = true;
        }

        private void SetSecurityProtocols()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Setting security protocols");
            Console.ResetColor();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
        }

        private List<Goal> GetGoals(DateTime date)
        {
            List<Goal> goals = new List<Goal>();

            string root = "https://www.bbc.com";

            WebPage page = browser.NavigateToPage(new Uri(string.Format("{0}/sport/football/scores-fixtures/{1}", root, date.ToString("yyyy-MM-dd"))));

            var matches = page.Html.CssSelect(".sp-c-fixture__block-link");

            Parallel.ForEach(matches, match =>
            {
                var link = match.Attributes["href"].Value;

                WebPage matchPage = browser.NavigateToPage(new Uri(string.Format("{0}/{1}", root, link)));

                string competition = matchPage.Html.CssSelect(".fixture__title").First().InnerText;

                if (competition.ToUpper() == "CHAMPIONSHIP" || competition.ToUpper() == "LEAGUE ONE" || competition.ToUpper() == "LEAGUE TWO"
                || competition.ToUpper() == "FA CUP" || competition.ToUpper() == "EFL CUP")
                {
                    var teams = matchPage.Html.CssSelect(".fixture__wrapper").First().CssSelect("abbr").ToList();

                    string homeTeam = teams[0].Attributes["title"].Value;
                    string awayTeam = teams[1].Attributes["title"].Value;

                    var homeGoals = matchPage.Html.CssSelect(".fixture__scorers-home").First().SelectNodes("li");

                    if (homeGoals != null)
                    {
                        foreach (var homeGoal in homeGoals)
                        {
                            var goalDetails = homeGoal.SelectNodes("span");

                            if (!goalDetails[2].FirstChild.InnerText.Contains("Dismissed"))
                            {
                                var scorer = goalDetails[0].InnerText;
                                var minute = goalDetails[2].SelectNodes("span")[0].InnerText;
                                var ownGoal = goalDetails[2].SelectNodes("span")[2].InnerText.Replace(" ", string.Empty) == "og" ? true : false;

                                var goal = new Goal
                                {
                                    Date = date,
                                    Competition = competition,
                                    HomeTeam = homeTeam,
                                    AwayTeam = awayTeam,
                                    For = homeTeam,
                                    Scorer = scorer,
                                    Minute = int.Parse(minute.Substring(0, minute.IndexOf("&"))),
                                    OwnGoal = ownGoal
                                };

                                goals.Add(goal);
                            }
                        }
                    }

                    var awayGoals = matchPage.Html.CssSelect(".fixture__scorers-away").First().SelectNodes("li");

                    if (awayGoals != null)
                    {
                        foreach (var awayGoal in awayGoals)
                        {
                            var goalDetails = awayGoal.SelectNodes("span");

                            if (!goalDetails[2].FirstChild.InnerText.Contains("Dismissed"))
                            {
                                var scorer = goalDetails[0].InnerText;
                                var minute = goalDetails[2].SelectNodes("span")[0].InnerText.Replace("'", string.Empty);
                                var ownGoal = goalDetails[2].SelectNodes("span")[2].InnerText.Replace(" ", string.Empty) == "og" ? true : false;

                                var goal = new Goal
                                {
                                    Date = date,
                                    Competition = competition,
                                    HomeTeam = homeTeam,
                                    AwayTeam = awayTeam,
                                    For = awayTeam,
                                    Scorer = scorer,
                                    Minute = int.Parse(minute.Substring(0, minute.IndexOf("&"))),
                                    OwnGoal = ownGoal
                                };

                                goals.Add(goal);
                            }
                        }
                    }
                }
            });

            return goals;
        }
    }
}
