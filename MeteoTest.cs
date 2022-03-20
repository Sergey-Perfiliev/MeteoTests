using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace MeteoTests
{
    [TestClass]
    public class MeteoTest
    {
        IWebDriver? driver;
        string? homeUrl;

        By linkDiagramByText = By.LinkText("Skew-T log-P diagram");

        Configuration appConfig = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);

        [TestInitialize]
        public void SetupTest()
        {
            string browserName = appConfig.AppSettings.Settings["browser"].Value;

            try
            {
                switch (browserName)
                {
                    case "chrome":
                        driver = new ChromeDriver();
                        break;

                    case "firefox":
                        driver = new FirefoxDriver();
                        break;

                    case "edge":
                        driver = new EdgeDriver();
                        break;

                    default:
                        driver = new ChromeDriver();
                        break;
                }
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("Problem with browser installation " + browserName
                    + ". Change property 'browser' in App.config. " + ex.Message);
                throw;
            }
            
            homeUrl = "https://meteo.paraplan.net/en/";
            driver.Navigate().GoToUrl(homeUrl);
            driver.Manage().Window.Maximize();
        }

        [TestMethod("Check aerological diagram link")]
        public void Meteo_DiagramLink_Visited()
        {
            // extected value
            string expectedValue_CurrentActiveLink = "Skew-T log-P diagram";

            // await page loading and click
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

            driver
                .FindElement(By.XPath("//a[text()='Five-day weather forecast']"))
                .Click();

            // await page new link exists
            WaitUntilElementExists(linkDiagramByText);

            //check text contains and link displayed
            if (IsElementPresent(By.XPath("//*[text()='Five-day weather forecast']")) &&
                IsElementPresent(linkDiagramByText))
            {
                driver.FindElement(linkDiagramByText).Click();
            }

            IWebElement visitedDiagramLink = driver.FindElement(By.XPath("//div[@class='sub_nav-item active']/span"));

            Assert.AreEqual(expectedValue_CurrentActiveLink, visitedDiagramLink.Text);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            try
            {
                driver.Close();
                driver.Quit();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("Problem with driver clean up"
                    + ". Change property 'browser' in App.config. " 
                    + ex.Message);
                throw;
            }
        }

        // check if element displayed
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // search for the element until a timeout is reached
        public void WaitUntilElementExists(By elementLocator, int timeout = 10)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(timeout))
                    .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(elementLocator));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Element with xpath: '" + elementLocator + "' was not found in current context page.");
                throw;
            }
        }
    }
}
