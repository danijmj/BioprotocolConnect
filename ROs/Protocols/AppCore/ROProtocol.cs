using System;
using System.Collections.Generic;
using BioprotocolConnect.ROs.Protocols.Models;
using OpenQA.Selenium;

namespace BioprotocolConnect.ROs.Protocols.AppCore
{

    public class ROProtocol
    {

        private IWebDriver _driver;

        public ROProtocol(IWebDriver driver)
        {
            var title = driver.Title;
            _driver = driver;
        }


        /// <summary>
        /// Read the RO main data
        /// </summary>
        /// <returns></returns>
        public Protocol GetROInfo()
        {
            Protocol protocol = new Protocol();
            var val = "";
            var section = "";
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> ros = null;

            try
            {
                try
                {
                    ros = _driver.FindElements(By.CssSelector("#porotoclAll > p"));
                }
                catch (System.Exception) { }

                foreach (var item in ros)
                {
                    if (item.Text.Length > 0)
                    {

                        val = item.Text.Trim();

                        if (item.GetAttribute("class").Contains("pptt"))
                        {
                            section = val;
                        }
                        else
                        {

                            switch (section)
                            {
                                case "Abstract":
                                    protocol.abstract_ += val + " ";
                                    break;
                                case "Background":
                                    protocol.background += val + " ";
                                    break;
                                case "Categories":
                                    protocol.categories = getListVal(item);
                                    break;
                            }
                        }


                        // CategoriaId = Convert.ToInt32(url.Substring(inicio, largo)),
                        // Nombre = item.Text,
                        // Url = item.GetAttribute("href")

                    }
                }

                // Get the list elements
                try
                {
                    ros = _driver.FindElements(By.CssSelector("#porotoclAll > #div_protocol > *"));
                }
                catch (System.Exception) { }

                foreach (var item in ros)
                {
                    if (item.Text.Length > 0)
                    {
                        // var url = item.GetAttribute("href");
                        val = item.Text.Trim();

                        if (item.GetAttribute("class").Contains("pptt"))
                        {
                            section = val;
                        }
                        else
                        {

                            switch (section)
                            {
                                case "Materials and Reagents":
                                    protocol.materials = getListVal(item);
                                    break;
                                case "Equipment":
                                    protocol.equipment = getListVal(item);
                                    break;
                                case "Software":
                                    protocol.software = getListVal(item);
                                    break;
                                case "Procedure":
                                    protocol.procedure = getListVal(item);
                                    break;
                                case "Data analysis":
                                    protocol.dataAnalysis += val + " ";
                                    break;
                                case "Recipes":
                                    protocol.recipies += val + " ";
                                    break;
                                case "Acknowledgments":
                                    protocol.acknowledgments += val;
                                    break;
                                case "Competing interests":
                                    protocol.competingInterest += val + " ";
                                    break;
                                case "References":
                                    protocol.references = getListVal(item);
                                    break;
                                case "Categories":
                                    protocol.categories = getListVal(item);
                                    break;
                            }
                        }


                        // CategoriaId = Convert.ToInt32(url.Substring(inicio, largo)),
                        // Nombre = item.Text,
                        // Url = item.GetAttribute("href")

                    }


                }

            }
            catch (System.Exception) { }


            protocol.categories = GetCathegories();
            protocol.title = GetTitle();

            // Get the metadata
            GetMetaData(ref protocol);
            protocol.keywords = GetkeyWords();
            
            // Get the url
            protocol.url = _driver.Url;
            
            // Get the users
            GetUsers(ref protocol);
            
            
            return protocol;
        }

        /// <summary>
        /// Runs over each list and get all data
        /// </summary>
        /// <param name="item">The parent item from which to loop the item </param>
        /// <returns></returns>
        protected List<Listado> getListVal(IWebElement item)
        {

            var val = new List<Listado>();

            try
            {

                // Select all li children elements
                var contentItem = item.FindElements(By.XPath("./li"));
                foreach (var el in contentItem)
                {
                    if (el.Text.Length > 0 && el.TagName == "li")
                    {

                        dynamic children = null;

                        try
                        {
                            // Try to get all "ol" children for new a new list elements
                            try
                            {
                                children = el.FindElement(By.XPath("./ol"));
                            }
                            catch (System.Exception) { }

                            if (children == null)
                            {
                                // Try to get all "ul" children for new a new list elements
                                try
                                {
                                    children = el.FindElement(By.XPath("./ul"));
                                }
                                catch (System.Exception) { }
                            }

                            if (children != null)
                            {
                                // Add the element text and the list element 
                                val.Add(new Listado()
                                {
                                    text = el.Text,
                                    listado = getListVal(children)
                                });
                            }
                        }
                        catch (System.Exception) { }


                        if (children == null)
                        {
                            // Add the element text 
                            val.Add(new Listado()
                            {
                                text = el.Text
                            });
                        }

                    }
                }
            }
            catch (System.Exception) { }

            return val;
        }

        /// <summary>
        /// Get all cathegories from the webpage
        /// </summary>
        /// <returns></returns>
        protected List<Listado> GetCathegories()
        {
            Protocol protocol = new Protocol();
            var val = new List<Listado>();
            var val2 = new List<Listado>();

            try
            {
                // Get all tags
                var contentItem = _driver.FindElements(By.CssSelector("#mediaContent .categories_a > .topbarh4"));
                foreach (var el in contentItem)
                {
                    // Get all links
                    var links = el.FindElements(By.XPath("./a"));
                    foreach (var item in links)
                    {
                        if (item.Text.Length > 0)
                        {
                            val2.Add(new Listado()
                            {
                                text = item.Text,
                                listado = getListVal(item)
                            });
                        }
                    }
                    val.Add(new Listado()
                    {
                        listado = val2
                    });

                    val2 = new List<Listado>();

                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return val;
        }

        /// <summary>
        /// Get the keywords
        /// </summary>
        /// <returns></returns>
        protected List<string> GetkeyWords()
        {
            Protocol protocol = new Protocol();
            var val = new List<string>();
            string[] list;

            try
            {
                var contentItem = _driver.FindElements(By.CssSelector("#porotoclAll > .p_keyword"));
                foreach (var el in contentItem)
                {
                    if (el.Text.Length > 0)
                    {

                        var textoFull = el.Text.ToString();
                        list = textoFull.Split(",");

                        foreach (var item in list)
                        {
                            val.Add(item.Trim().Replace("Keywords: ", ""));
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            

            return val;
        }

        /// <summary>
        /// Get the title from the page
        /// </summary>
        /// <returns></returns>
        public string GetTitle()
        {
            return _driver.FindElement(By.Id("div_title_protocol")).Text;
        }

        /// <summary>
        /// Read the RO main data and get the users from each repository, after of this go to each user and get the data 
        /// </summary>
        /// <param name="protocol">Referenced protocol where add the user to him</param>
        public void GetUsers(ref Protocol protocol)
        {
            var users = new List<User>();
            var tmpUsers = new List<User>();

            try
            {
                // Select all li children elements
                var contentItem = _driver.FindElements(By.CssSelector("#mediaContent > .media-body > .topbarcon .media > .media-body .authordiv > a"));

                foreach (var el in contentItem)
                {
                    if (el != null && el.Text.Length > 0)
                    {

                        string user = el.Text.Trim();
                        
                        var currentUser = new User()
                        {
                            name = el.Text.Trim()
                        };

                        var clickAttr = el.GetAttribute("onclick");

                        // showAffiliation([ID])
                        // Get the id into the html attr
                        string[] breakedString = clickAttr.Split("showAffiliation(");

                        if (breakedString.Length > 0)
                        {
                            string[] breakedStringLast = breakedString[1].Split(")");
                            if (breakedStringLast.Length > 0)
                            {
                                // userId.Add(breakedStringLast[0]);
                                var falseId = breakedStringLast[0];

                                // Get the id & the url
                                try
                                {
                                    var el_url = _driver.FindElements(By.CssSelector("#tcaid_" + falseId + " > .paffiliationdiv > div > a"));
                                    var uri = el_url[0].GetAttribute("href");

                                    currentUser.bioProtocolId = uri.Substring(uri.LastIndexOf('=') + 1);
                                    currentUser.url = ROProtocolUser.GetUserUrl(currentUser.bioProtocolId);
                                }
                                catch (System.Exception) {}

                                // Get the emails
                                try
                                {
                                    var el_email = _driver.FindElements(By.CssSelector("#tcaid_" + falseId + " > .media > .tanchutitle > a"));

                                    // var div = "#tcaid_" + falseId + " > .media > .tanchutitle > a";
                                    var emails = new List<string>();
                                    foreach (var elem in el_email)
                                    {
                                        var atr = elem.GetAttribute("href");
                                        emails.Add(atr.Replace("mailto:",""));
                                        // emails.Add(elem.Text.Trim());
                                    }

                                    currentUser.emails = emails;
                                }
                                catch (System.Exception) {}

                            }
                        }
                        tmpUsers.Add(currentUser);
                    }
                }

                // Get the users from each protocol
                ROProtocolUser protocolUser = new ROProtocolUser(_driver);
                protocol.authors = users;

                foreach (var us in tmpUsers)
                {
                    // Redirect to the user url
                    _driver.Navigate().GoToUrl((us.url));

                    // Search data into the current user dir
                    var currentUser = protocolUser.GetUserInfo();

                    // Set the emais data
                    currentUser.emails = us.emails;
                    
                    // Add the data into the protocol authors 
                    protocol.authors.Add(currentUser);
                }


            }
            catch (System.Exception) {}
        }


        /// <summary>
        /// Get all metadata, like the RO DOI, the published date or the published info
        /// </summary>
        /// <param name="protocol">Referenced protocol where add the user to him</param>
        public void GetMetaData(ref Protocol protocol)
        {

            var val = new List<Listado>();

            try
            {
                
                // Select all li children elements
                var contentItem = _driver.FindElements(By.CssSelector("#mediaContent > .media-body > .topbarcon .media > .media-body .media > .doclink"));

                foreach (var el in contentItem)
                {
                    if (el.Text.Length > 0)
                    {

                        System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> children = null;

                        try
                        {
                            // Try to get the "title" & "value" of the current element
                            try
                            {
                                children = el.FindElements(By.XPath("./*"));
                            }
                            catch (System.Exception) {}
                        

                            if (children != null)
                            {
                                switch (children[0].Text.Trim())
                                {
                                    case "DOI:":
                                        protocol.doi = children[1].Text.Trim();
                                    break;
                                
                                    case "Published:":
                                        DateTime newDate = new DateTime();

                                        // Sanitize the text and extract the date
                                        string iso8601String = children[1].Text.Trim();
                                        string[] breakedString = iso8601String.Split(',');
                                        
                                        // Create the date string format
                                        string date = breakedString[2].Trim() + ", " + breakedString[3].Trim();
                                        
                                        // Convert the date (in string format) to DateTime format
                                        try {
                                            newDate = DateTime.Parse(date);
                                        }
                                        catch (System.Exception) {}

                                        protocol.published = newDate;
                                        protocol.publicationInformation = iso8601String;
                                        break;
                                
                                    case "Views:":
                                    break;
                                }
                            }
                        }
                        catch (System.Exception) {}
                    }
                }
            }
            catch (System.Exception) {}
        }

    }
}
