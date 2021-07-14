using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using BioprotocolConnect.ROs.Protocols.Models;
using System.Web;
using System.Text.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Linq;
using BioprotocolConnect.ROs.Protocols.AppCore;

namespace BioprotocolConnect.ROs.Protocols.Controllers
{

    public class ROProtocolLogic : ROProtocolInterface
    {

        public List<Protocol> protocols = new List<Protocol>();
        protected IWebDriver _driver; // Web driver (Chrome or similar)
        protected string _urlBase;
        protected string _userId;


        public ROProtocolLogic(string urlBase, string userId)
        {
            _urlBase = urlBase;
            _userId = userId;
        }

        /// <summary>
        /// Method to get all protocol,
        /// This method will be implemented by the function
        /// </summary>
        /// <returns></returns>
        public List<Protocol> GetListProtocols()
        {
            // Inizialize Protocol User

            BrowseTheCatalog();

            // User RO page Instance
            ROProtocolUser protocolUser = new ROProtocolUser(_driver);
            var protocolUsers = protocolUser.GetROsUser(); // Get info of the user & all ROs

            // var user = protocolUser.GetUserInfo();

            // Protocol page logic instance
            ROProtocol protocol = new ROProtocol(_driver);

            foreach (var item in protocolUsers)
            {
                // Go to the current protocol url
                try
                {
                    _driver.Navigate().GoToUrl((item.url));

                    // Get the RO info en the current url
                    protocols.Add(protocol.GetROInfo());
                    
                }
                catch (System.Exception)
                {
                    throw new Exception("Error while launch the webpage");
                }
            }

            _driver.Close();

            return protocols;
            
        }

        /// <summary>
        /// Open the browser (Chrome) and visit each of the catalog webpages of
        /// https://bio-protocol.org
        /// </summary>
        protected void BrowseTheCatalog()
        {
            // Get the navigator driver and open and maximize a windows
            _driver = GetDriver();
            _driver.Manage().Window.Maximize();

            var url = ROProtocolUser.GetUserUrl(_userId);
            // Go to the url
            try
            {
                _driver.Navigate().GoToUrl(ROProtocolUser.GetUserUrl(_userId));
                // Set the cookies to login into the web page
                setCookies().ForEach(e => _driver.Manage().Cookies.AddCookie(e));
            }
            catch (System.Exception)
            {
                throw new Exception("Error while launch the url");
            }

        }

        /// <summary>
        /// Configure and return the web driver (chrome for example)
        /// </summary>
        /// <returns></returns>
        protected IWebDriver GetDriver()
        {
            var user_agent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
            ChromeOptions options = new ChromeOptions();
            //Descomenta esta linea para usar el mode HeadLess de Chrome
            //options.AddArgument("--headless"); 
            options.AddArgument("--disable-gpu");
            options.AddArgument($"user_agent={user_agent}");
            options.AddArgument("--ignore-certificate-errors");
            IWebDriver driver = new ChromeDriver(Directory.GetCurrentDirectory(), options);
            return driver;
        }

        /// <summary>
        /// Get a url with a given id 
        /// </summary>
        /// <returns></returns>
        protected string GetProtocolUrl(string protocol)
        {
            string url = $@"https://bio-protocol.org/{protocol}";
            return url;
        }

        /// <summary>
        /// Generate some cookies to init a session 
        /// </summary>
        /// <returns></returns>
        protected List<Cookie> setCookies() 
        {
            var cookies = new List<Cookie>();
            var cookiesDic = new Dictionary<string, string>() {

                {"ASP.NET_SessionId", "rvbizze5kp53m5w5bkhzcmoj"},
                {"account", "danijmj@gmail.com"},
                {"email", "danijmj@gmail.com"},
                {"job", "UC"},
                {"lastname", "Fern%c3%a1ndez"},
                {"lj", "yhid=23508152"},
                {"middlename", "Morales"},
                {"photo", "/imagesUP/usersphoto/u1297590.jpg?v=0"},
                {"username", "Daniel"},
            };

            Cookie userCookie;


            foreach (var item in cookiesDic)
            {
                userCookie = new Cookie(item.Key, item.Value, ".bio-protocol.org", "/", DateTime.Now.AddYears(1));
                cookies.Add(userCookie);
            }

            return cookies;
        }

    }
}
