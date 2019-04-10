using System;
using System.Linq;
using System.Net;
using WebScraper.Builders;
using WebScraper.Data;
using WebScraper.Folders;

namespace WebScraper
{
    class Program
    {
        private const string Method = "search";

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter which city you would like to scrape:");

            var craigsListCity = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Please enter the CraigsList category you would like to scrape:");

            var craigsListCategory = Console.ReadLine() ?? string.Empty;

            using (WebClient client = new WebClient())
            {
                string content = client.DownloadString($"http://{craigsListCity.Replace(" ", string.Empty)}.craigslist.org/{Method}/{craigsListCategory}");

                ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                    .WithData(content)
                    .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result=title hdrlnk\"">(.*?)</a>")
                .WithRegexOption(System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                .WithPart(new ScrapeCriteriaPartBuilder()
                    .WithRegex(@">(.*?)</a>")
                    .WithRegexOption(System.Text.RegularExpressions.RegexOptions.Singleline)
                    .Build())
                .WithPart(new ScrapeCriteriaPartBuilder()
                    .WithRegex(@"href=\""(.*?)\""")
                    .WithRegexOption(System.Text.RegularExpressions.RegexOptions.Singleline)
                    .Build())
                .Build();

                Scraper scraper = new Scraper();

                var scrapedElements = scraper.Scrape(scrapeCriteria);

                if (scrapedElements.Any())
                {
                    foreach (var scrapedElement in scrapedElements)
                    {
                        Console.WriteLine(scrapedElement);
                    }

                }
                else
                {
                    Console.WriteLine("no matches");
                }
            }

        }
    }
}
