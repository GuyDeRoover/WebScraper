using WebScraper.Views;
using WebScraper.DAL;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                IntroPage.Print();
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice == 1)
                {
                    YouTubeScraperPage.Print();
                    string? keywords = Console.ReadLine();
                    YouTubeScraper.ScrapeYouTube(keywords);

                }
                else if (choice == 2)
                {
                    IctjobScraperPage.Print();
                    string? keywords = Console.ReadLine();
                    IctjobScraper.ScrapeIctjob(keywords);
                }
                else if (choice == 3)
                {
                    DevianArtScraperPage.Print();
                    string? keywords = Console.ReadLine();
                    DevianArtScraper.ScrapeDevianArt(keywords);
                }
            } while (false);
        }

    }
}