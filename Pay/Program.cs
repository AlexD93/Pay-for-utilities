using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Pay
{
    class Program
    {
        private static IWebDriver driver;

        static void Main()
        {
            try
            {
                driver = new ChromeDriver();

                driver.Navigate().GoToUrl("https://www.complatezh.info/viewpage.php?page_id=9");
                driver.Manage().Window.Maximize();

                driver.FindElement(By.Name("user_name")).SendKeys("");//UserName goes here
                driver.FindElement(By.Name("user_pass")).SendKeys("");//Password goes here

                Thread.Sleep(1500);

                driver.FindElement(By.XPath("/html/body/table[3]/tbody/tr/td[1]/table[1]/tbody/tr[2]/td/div/form/input[3]")).Click();

                Thread.Sleep(3000);

                driver.Navigate().GoToUrl("https://www.complatezh.info/viewpage.php?page_id=9");

                Thread.Sleep(3000);

                driver.Navigate().GoToUrl("https://www.complatezh.info/eao/show.php");

                Thread.Sleep(3000);

                driver.Navigate().GoToUrl("https://mncp-dp.oschadbank.ua/pay");

                List<string> placeholderForMoneyAmount = new List<string>();
                List<string> payAmountToXPaths = new List<string>();

                //Collect all the XPaths to the boxes on the website where amount will go.
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[2]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[3]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[5]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[6]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[7]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[8]/td[8]/input");
                payAmountToXPaths.Add("/html/body/form/table[2]/tbody/tr[9]/td[8]/input");

                //Get and parse entire table since each roow is in a separate <tr> and run it through Regex to make sure that it is a valid value that we need.
                //This will basically help us out to filter out other gibberish from the <table>.
                IWebElement table = driver.FindElement(By.ClassName("border"));
                IList<IWebElement> tableRow = table.FindElements(By.TagName("td"));

                foreach (var item in tableRow)
                {
                    if (Regex.Match(item.Text.Substring(0, item.Text.Length), @"\b[0-9][0-9][0-9][.][0-9][0-9]|[0-9][0-9][.][0-9][0-9]|[0-9][.][0-9][0-9]\b").Success)
                    {
                        string value = item.Text.Trim();

                        if (Convert.ToDecimal(value) != 0)
                        {
                            placeholderForMoneyAmount.Add(value);
                        }
                        else
                        {
                            placeholderForMoneyAmount.Add("0");
                        }
                    }
                }

                for (int i = 0; i < placeholderForMoneyAmount.Count; i++)
                {
                    driver.FindElement(By.XPath(payAmountToXPaths[i])).SendKeys(placeholderForMoneyAmount[i]);
                }

                Thread.Sleep(5000);

                driver.FindElement(By.XPath("/html/body/form/table[2]/tbody/tr[13]/td[2]/input")).Click();

                Thread.Sleep(3000);

                driver.FindElement(By.CssSelector("input[type='submit']")).Click();

                Thread.Sleep(5000);

                driver.FindElement(By.Name("single_portmone_pay_card")).SendKeys("");//Credit Card number
                driver.FindElement(By.Name("single_portmone_pay_exp_date")).SendKeys("");//CC exparation date
                driver.FindElement(By.Name("single_portmone_pay_cvv2")).SendKeys("");//CVV2 code
                driver.FindElement(By.Name("emailAddress")).SendKeys("");//email address for a receipt

                driver.FindElement(By.XPath("/html/body/main/section/div[1]/div[5]/div[1]/form/div[3]/button")).Click();

                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                Console.WriteLine("Payment was successfull. Please see your email for a receipt.");
                driver.Close();
                driver.Quit();
            }
        }
    }
}
