using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.Json;
using WebScraper.Models;
using OpenQA.Selenium.Support.UI;

namespace WebScraper.DAL
{
    internal class IctjobScraper
    {
        public static void ScrapeIctjob(string keyword) 
        { 
            int totalScrapes = 5;
            string filePath = "C:\\Users\\deroo\\Downloads\\";
            string query = "https://www.ictjob.be/nl/it-vacatures-zoeken?keywords=" + keyword;
            // Headless scraping
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless=new");
            // Comment out the 2 lines above and remove "option" argument in line below to open browser when scraping
            IWebDriver driver = new ChromeDriver(option);
            driver.Navigate().GoToUrl(query);
            // Wait for elememt to show up and then click to sort for most recent
            WebDriverWait waitPopupCookies = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
            waitPopupCookies.Until(d => d.FindElement(By.Id("sort-by-date")).Displayed);
            driver.FindElement(By.Id("sort-by-date")).Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            // Pull data from elements
            var title = driver.FindElements(By.XPath("//*/a[@class=\"job-title search-item-link\"]/h2[@class=\"job-title\"]"));
            var company = driver.FindElements(By.ClassName("job-company"));
            var location = driver.FindElements(By.XPath("//*/span[@class=\"job-location\"]/span/span"));
            var keywords = driver.FindElements(By.ClassName("job-keywords"));
            var detailPage = driver.FindElements(By.ClassName("search-item-link"));

            // Checking the existence of the specified csv file (for first line)
            if (!File.Exists("IctjobScrape.csv"))
            {
                File.AppendAllText(filePath + "IctjobScrape.csv", "Title;Company;Location;Keywords;Detail Page\n");
            }

            // Looping over the data
            for (int i = 0; i<totalScrapes; i++)
            {
                string csv = String.Format("{0};{1};{2};{3};{4}\n", title[i].Text, company[i].Text, location[i].Text, keywords[i].Text, detailPage[i].GetAttribute("href"));
                var jsonString = new IctjobJson
                {
                    Title = title[i].Text,
                    Company = company[i].Text,
                    Location = location[i].Text,
                    Keywords = keywords[i].Text,
                    DetailPage = detailPage[i].GetAttribute("href"),
                };

                Console.WriteLine(" ");
                Console.WriteLine("Title: " + title[i].Text);
                Console.WriteLine("Company: " + company[i].Text);
                Console.WriteLine("Location: " + location[i].Text);
                Console.WriteLine("Keywords: " + keywords[i].Text);
                Console.WriteLine("Detail Page: " + detailPage[i].GetAttribute("href")); ;

                var options = new JsonSerializerOptions { WriteIndented = true, AllowTrailingCommas = true, PropertyNameCaseInsensitive = true };
                string json = System.Text.Json.JsonSerializer.Serialize(jsonString, options);

                File.AppendAllText(filePath + "IctjobScrape.csv", csv);
                File.AppendAllText(filePath + "IctjobScrape.json", json);
            }

            driver.Quit();
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Your .cvs and .json files can be found in " + filePath);
            Console.WriteLine("------------------------------------------------------------------------------");
        }
    }
}
