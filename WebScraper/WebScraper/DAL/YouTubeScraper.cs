using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Text.Json;
using WebScraper.Models;

namespace WebScraper.DAL
{
    internal class YouTubeScraper {

        public static void ScrapeYouTube(string keywords) 
        {
            int totalScrapes = 5;
            string filePath = "C:\\Users\\deroo\\Downloads\\";
            // Adding "&sp=CAI%253D" is to sort based on upload date (most recent videos)
            string query = "https://www.youtube.com/results?search_query=" + keywords + "&sp=CAI%253D";
            // Headless scraping
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless=new");
            // Comment out the 2 lines above and remove "option" argument in line below to open browser when scraping
            IWebDriver driver = new ChromeDriver(option);
            driver.Navigate().GoToUrl(query);
            // Wait for cookies popup to appear
            WebDriverWait waitPopupCookies = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
            waitPopupCookies.Until(d => d.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button")).Displayed);
            // Accept cookies
            driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button")).Click();
            driver.Navigate().Refresh();
            // Pull data from elements
            var title = driver.FindElements(By.XPath("//*/a[@id=\"video-title\"]"));
            var uploader = driver.FindElements(By.XPath("//*//yt-formatted-string[@class=\"style-scope ytd-channel-name\"]/a[@class=\"yt-simple-endpoint style-scope yt-formatted-string\"]"));
            var views = driver.FindElements(By.XPath("//*/ytd-video-meta-block[@class=\"style-scope ytd-video-renderer byline-separated\"]/div/div[@id=\"metadata-line\"]/span[1]"));

            // Checking the existence of the specified csv file (for first line)
            if (!File.Exists("YouTubeScrape.csv"))
            {
                File.AppendAllText(filePath + "YouTubeScrape.csv", "Link;Title;Uploader;Views\n");
            }

            // Looping over the data
            for (int i = 0; i < totalScrapes; i++)
            {
                string csv = String.Format("{0};{1};{2};{3}\n", title[i].GetAttribute("href"), title[i].GetAttribute("title"), uploader[i].Text, views[i].Text);
                var jsonString = new YouTubeJson
                {
                    Link = title[i].GetAttribute("href"),
                    Title = title[i].GetAttribute("title"),
                    Uploader = uploader[i].Text,
                    Views = views[i].Text,
                };

                Console.WriteLine(" ");
                Console.WriteLine("URL: " + title[i].GetAttribute("href"));
                Console.WriteLine("Title: " + title[i].GetAttribute("title"));
                Console.WriteLine("Uploader: " + uploader[i].Text);
                Console.WriteLine("Views: " + views[i].Text);

                var options = new JsonSerializerOptions { WriteIndented = true, AllowTrailingCommas = true, PropertyNameCaseInsensitive = true };
                string json = System.Text.Json.JsonSerializer.Serialize(jsonString, options);

                File.AppendAllText(filePath + "YouTubeScrape.csv", csv);
                File.AppendAllText(filePath + "YouTubeScrape.json", json);  
            }

            driver.Quit();
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Your .cvs and .json files can be found in " + filePath);
            Console.WriteLine("------------------------------------------------------------------------------");
        }
    }
}