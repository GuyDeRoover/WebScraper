using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Net;
using System.Text.Json;
using WebScraper.Models;

namespace WebScraper.DAL
{
    internal class DevianArtScraper
    {
        public static void ScrapeDevianArt(string keywords)
        {
            int totalScrapes = 5;
            string filePath = "C:\\Users\\deroo\\Downloads\\";
            string query = "https://www.deviantart.com/search?q=" + keywords;
            // Headless scraping
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless=new");
            // Comment out the 2 lines above and remove "option" argument in line below to open browser when scraping
            IWebDriver driver = new ChromeDriver(option);
            driver.Navigate().GoToUrl(query);
            driver.Navigate().Refresh();
            // Pull data from elements
            var title = driver.FindElements(By.XPath("//*/a[@data-hook=\"deviation_link\"]/div/img"));
            var uploader = driver.FindElements(By.XPath("//*/a[@data-hook=\"user_link\"]"));
            var link = driver.FindElements(By.XPath("//*/div/div[3]/div/a[@data-hook=\"deviation_link\"]"));

            // Checking the existence of the specified csv file (for first line)
            if (!File.Exists("DevianArtScrape.csv"))
            {
                File.AppendAllText(filePath + "DevianArtScrape.csv", "Title;Uploader;Link;Source Link\n");
            }

            // Looping over the data
            for (int i = 0; i < totalScrapes; i++)
            {
                string csv = String.Format("{0};{1};{2}\n", title[i].GetAttribute("alt"), uploader[i].GetAttribute("data-username"), link[i].GetAttribute("href"), title[i].GetAttribute("src"));
                var jsonString = new DevianArtJson
                {
                    Title = title[i].GetAttribute("alt"),
                    Uploader = uploader[i].GetAttribute("data-username"),
                    Link = link[i].GetAttribute("href"),
                    SourceLink = title[i].GetAttribute("src"),
                };

                Console.WriteLine(" ");
                Console.WriteLine("Title: " + title[i].GetAttribute("alt"));
                Console.WriteLine("Uploader: " + uploader[i].GetAttribute("data-username"));
                Console.WriteLine("Link: " + link[i].GetAttribute("href"));
                Console.WriteLine("Source Link: " + title[i].GetAttribute("src"));

                var options = new JsonSerializerOptions { WriteIndented = true, AllowTrailingCommas = true, PropertyNameCaseInsensitive = true };
                string json = System.Text.Json.JsonSerializer.Serialize(jsonString, options);

                File.AppendAllText(filePath + "DevianArtScrape.csv", csv);
                File.AppendAllText(filePath + "DevianArtScrape.json", json);

                // Download and save the images
                var source = title[i].GetAttribute("src");
                var imageName = title[i].GetAttribute("alt");
                WebClient Downloader = new WebClient();
                Downloader.DownloadFileTaskAsync(source, "C:\\Users\\deroo\\Downloads\\" + imageName + ".jpeg");
            }

            driver.Quit();
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Your images, .cvs and .json files can be found in " + filePath);
            Console.WriteLine("------------------------------------------------------------------------------");
        }

    }
}
