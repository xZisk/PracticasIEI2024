using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace IEIPracticas.APIs_Scrapper
{
    public class SeleniumScraper
    {
        public static (string Longitude, string Latitude) Scraper(IWebDriver driver, string UTMEste, string UTMNorte)
        {
            if (driver.Url.ToString() != "https://padeepro.com/converterutm.html")
            {
                driver.Navigate().GoToUrl("https://padeepro.com/converterutm.html");
                Thread.Sleep(2000);
                driver.FindElement(By.CssSelector(".fc-cta-do-not-consent > .fc-button-label")).Click();
            }
            driver.FindElement(By.Id("UTMeBox1")).Clear();
            driver.FindElement(By.Id("UTMeBox1")).SendKeys(UTMNorte);
            driver.FindElement(By.Id("UTMnBox1")).Clear();
            driver.FindElement(By.Id("UTMnBox1")).SendKeys(UTMEste);
            driver.FindElement(By.Id("UTMzBox1")).Clear();
            driver.FindElement(By.Id("UTMzBox1")).SendKeys("30");
            driver.FindElement(By.Id("Calcularlatlon")).Click();
            string lon = driver.FindElement(By.Id("DDLonBox0")).GetAttribute("value");
            string lat = driver.FindElement(By.Id("DDLatBox0")).GetAttribute("value");
            return (lat, lon);
        }
    }
}
