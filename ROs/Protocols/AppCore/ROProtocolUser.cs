using System;
using System.Collections.Generic;
using BioprotocolConnect.ROs.Protocols.Models;
using OpenQA.Selenium;

namespace BioprotocolConnect.ROs.Protocols.AppCore
{

    public class ROProtocolUser
    {

        private IWebDriver _driver;

        public ROProtocolUser(IWebDriver driver)
        {
            var title = driver.Title;
            _driver = driver;
        }


        /// <summary>
        /// Method to search throw a webpage in a user profile page
        /// </summary>
        /// <returns></returns>
        public User GetUserInfo()
        {
            User user = new User();
            user.emails = new List<string>();
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> rows;
            try
            {
                rows = _driver.FindElements(By.CssSelector("#centerMainBody > #userLeft > *"));
                
                foreach (var item in rows)
                {
                    
                    var cssClass = item.GetAttribute("class");
                    var id = item.GetAttribute("id");

                    // Get the name of the user
                    // strong.text-20.margin-top
                    if (cssClass.Contains("text-20") && cssClass.Contains("margin-top"))
                    {
                        user.name = item.Text.Trim();
                    }

                    // Search and get each email from the user & the institution
                    // Really, the email appears only if is the current log-in account, them, the emails never has filled here
                    // author-li
                    if (cssClass.Contains("author-li") && item.TagName == "ul")
                    {
                        try
                        {
                            var els = item.FindElements(By.XPath("./li"));
                            if (els.Count > 0 && els[0].GetAttribute("class") != "lire" && els[0].GetAttribute("id") != "ContentPlaceHolder1_liau")
                            {
                                List<string> institution = new List<string>();
                                institution.Add(els[0].Text);
                                // Get user name
                                user.institutions = institution;

                                // Get email
                                if (els.Count > 1 && els[1].GetAttribute("id") == "ContentPlaceHolder1_liemail")
                                {
                                    user.emails.Add(els[1].Text);
                                }
                            }
                        }
                        catch (System.Exception) {}
                    }

                    // Get the research fields & the research focus
                    if (id == "ContentPlaceHolder1_divff")
                    {
                        try
                        {
                            var els = item.FindElements(By.XPath("./ul/li"));
                            List<string> reserarchFocus = new List<string>();
                            List<string> reserarchFields = new List<string>();
                            foreach (var el in els)
                            {
                                if (el.GetAttribute("id") == "ContentPlaceHolder1_lifields")
                                {
                                    reserarchFields.Add(el.Text.Trim());
                                    user.reserarchFields = reserarchFields;
                                } else if (el.GetAttribute("id") == "ContentPlaceHolder1_lifocus")
                                {
                                    reserarchFocus.Add(el.Text.Trim());
                                    user.reserarchFocus = reserarchFocus;
                                }
                            }
                        }
                        catch (System.Exception) {
                            throw;
                        }
                    }

                    // Get the orcid code (if exist)
                    if (id == "ContentPlaceHolder1_panIDTwitter")
                    {
                        try
                        {
                            var el = item.FindElement(By.XPath("./*[@id=\"ContentPlaceHolder1_lkorcid\"]"));
                            
                            var orcidUrl = el.GetAttribute("href");

                            user.orcid = orcidUrl.Substring(orcidUrl.LastIndexOf('/') + 1);
                            
                        }
                        catch (System.Exception) {}
                    }


                    // var url = item.GetAttribute("href");
                    
                    // var largo = url.LastIndexOf('/');

                    // user.actualWork = "";

                }
                
                user.studies = new List<string>();

                try
                {
                    rows = _driver.FindElements(By.CssSelector("#ContentPlaceHolder1_Content3_panBio > p"));
                    var title = "";
                    foreach (var item in rows)
                    {
                        if (item.GetAttribute("class") == "pptt")
                        {
                            title = item.Text.Trim();
                        } else if (title == "Education" && item.GetAttribute("class") == "ppzz")
                        {
                            user.studies.Add(item.Text.Trim());
                        }
                    }
                }
                catch (System.Exception) {}


                // Set the bioProtocolId
                user.bioProtocolId = _driver.Url.Substring(_driver.Url.LastIndexOf('=') + 1);
                user.url = ROProtocolUser.GetUserUrl(user.bioProtocolId);
            }
            catch (System.Exception) {
                throw;
            }

            return user;
        }

        /// <summary>
        /// Get all ROs urls from ROs section 
        /// </summary>
        /// <returns></returns>
        private List<string> GetROsUrl(IWebElement el)
        {
            List<String> res = new List<string>();

            var link = el.FindElements(By.CssSelector(".media-body > a"));
            foreach (var item in link)
            {
                if (item.Text.Length > 0)
                {
                    res.Add(item.GetAttribute("href"));

                    var strongs = el.FindElements(By.CssSelector("strong"));

                    foreach (var itm in strongs)
                    {
                        res.Add(itm.Text);
                    }
                }
            }


            return res;
        }

        /// <summary>
        /// Get all Protocols basic info to go over each element lather
        /// </summary>
        /// <returns></returns>
        public List<Protocol> GetROsUser()
        {
            List<Protocol> protocols = new List<Protocol>();
            var ros = _driver.FindElements(By.CssSelector("#ContentPlaceHolder1_Content3_panPublished > div > .publish-main"));
            foreach (var item in ros)
            {
                if (item.Text.Length > 0)
                {
                    // var url = item.GetAttribute("href");
                    var url = GetROsUrl(item);
                    var id = url[0].Substring(url[0].LastIndexOf('/') + 1);
                    var protocol = new Protocol()
                    {
                        id = id.ToString(),
                        url = url[0],
                        title = url[1]
                    };
                    protocols.Add(protocol);
                }
            }
            return protocols;
        }

        /// <summary>
        /// Static function to get a user url with their user id
        /// </summary>
        /// <returns></returns>
        public static string GetUserUrl(string userId)
        {
            string url = $@"https://bio-protocol.org/UserHome.aspx?id={userId}";
            return url;
        }

    }
}
