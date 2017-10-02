using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace Financial_Journal
{
    public class Shipment_Tracking
    {
        public string Tracking_Number { get; set; }
        public string Ref_Order_Number { get; set; }
        public DateTime Expected_Date { get; set; }
        public DateTime Last_Alert_Date { get; set; }
        public DateTime Received_Date { get; set; }
        public bool Alert_Active { get; set; }
        public bool Email_Active { get; set; }
        public int Status { get; set; }
        public bool Temp_Toggle { get; set; } // Toggle for redundant copies : internal use only

        public Shipment_Tracking()
        {
            Tracking_Number = "";
            Ref_Order_Number = "";
            Alert_Active = true;
            Email_Active = true;
            Expected_Date = DateTime.Now;
            Received_Date = DateTime.MinValue;
            Last_Alert_Date = DateTime.Now.AddDays(-1);
            Status = 1;
        }

        /// <summary>
        /// Attempt using current default browser; to look on google for tracking (automatically selects first link)
        /// </summary>
        public void Show_Tracking_Information()
        {
            /*
            // Assume same chrome path for all
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            System.Environment.SetEnvironmentVariable("webdriver.chrome.driver", Directory.GetCurrentDirectory());
            service.Port = 4444;
            IWebDriver Driver = new ChromeDriver(service);

            Driver.Navigate().GoToUrl("https://www.google.com");
            //Driver.Manage().Window.Maximize();

            IWebElement searchInput = Driver.FindElement(By.Id("lst-ib"));
            searchInput.SendKeys(Tracking_Number);
            searchInput.SendKeys(Keys.Enter);
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            By linkLocator = By.CssSelector("cite._Rm");
            wait.Until(ExpectedConditions.ElementToBeClickable(linkLocator));
            IReadOnlyCollection<IWebElement> links = Driver.FindElements(linkLocator);
            foreach (IWebElement link in links)
            {
                Diagnostics.WriteLine(link.Text);
            }
             * */

            string tracking_no = Tracking_Number;
            string URL = "http://google.ca//?gws_rd=ssl#q=";
            Process.Start(URL + tracking_no);
        }

        private string Parse_Tracking(string input)
        {
            string parsed = "";
            bool parsing = false;
            if (input.Contains("#")) // track after '#'
            {
                foreach (char c in input)
                {
                    if (parsing)
                    {
                        parsed = parsed + c.ToString();
                    }
                    if (c.ToString() == "#")
                    {
                        parsing = true;
                    }
                }
            }
            else // if no # found, try and find tracking number
            {
                foreach (char c in input)
                {
                    if ((char.IsDigit(c) || c.ToString() == " ") && parsing)
                    {
                        parsed = parsed + c.ToString();
                    }
                    else if (char.IsDigit(c))
                    {
                        parsing = true;
                        parsed = c.ToString();
                    }
                    else
                    {
                        parsing = false;
                    }
                }
            }
            while (parsed.EndsWith(" "))
            {
                parsed = parsed.TrimEnd();
            }
            return parsed;
        }
    }

    public static class Comparisons
    {
        public static Comparison<T> Then<T>(this Comparison<T> primary,
                                            Comparison<T> secondary)
        {
            // TODO: Nullity validation
            return (x, y) =>
            {
                int first = primary(x, y);
                return first != 0 ? first : secondary(x, y);
            };
        }
    }
}
