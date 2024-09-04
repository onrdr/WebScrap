using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WebScrap;

public class FetchService
{
    public static void FetchData()
    {
        var pageCounter = 1;
        var chromeDriver = new ChromeDriver();
        chromeDriver.Manage().Window.Maximize();

        var coreUrl = $"https://skinsort.com";

        while (pageCounter <= 1383)
        {
            var siteUrl = $"{coreUrl}/ingredients/page/{pageCounter++}";

            chromeDriver.Navigate().GoToUrl(siteUrl);

            // Set up a wait to look for the button
            var wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            try
            {
                // Try to find and click the button if it appears
                var button = wait.Until(driver => driver.FindElement(By.ClassName("dismiss-drawer-button")));
                button.Click();
                Console.WriteLine("Button clicked.");
            }
            catch (Exception e)
            {
                // Handle the case where the button does not appear
                Console.WriteLine("Button not found within the timeout period.");
            }

            // Get the page source after the site has loaded
            string pageSource = chromeDriver.PageSource;

            // Create an HtmlDocument object and load the page source into it
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);

            var urlList = new List<string>();

            var aTags = htmlDoc.DocumentNode.SelectNodes("//a[@class='w-1/2 lg:w-1/3 group cursor-pointer p-3']");

            if (aTags != null)
            {
                foreach (var aTag in aTags)
                {
                    var hrefValue = aTag.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(hrefValue))
                    {
                        urlList.Add($"{coreUrl}{hrefValue}");
                    }
                }
            }


            if (urlList is not null && urlList.Count != 0)
            {
                foreach (var link in urlList)
                {
                    chromeDriver.Navigate().GoToUrl(link);

                    try
                    {
                        Thread.Sleep(3000);
                        // Try to find and click the button if it appears
                        var button = wait.Until(driver => driver.FindElement(By.ClassName("dismiss-drawer-button")));
                        button.Click();
                        Console.WriteLine("Button clicked.");
                    }
                    catch (Exception e)
                    {
                        // Handle the case where the button does not appear
                        Console.WriteLine("Button not found within the timeout period.");
                    }

                    Console.ReadLine();
                }
            }
        }
    }
}
