using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mshtml;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace UnitSite
{
    public abstract class BaseTest
    {
        protected enum TypeParameter { Good, Nothing, Error }

        //------------------------------------------------

        [ClassInitializeAttribute]
        virtual public void ClassInitialize()
        {
            ;
        }

        [ClassCleanupAttribute]
        virtual public void ClassClean()
        {
            ;
        }

        [TestInitializeAttribute]
        public void TestStart()
        {
            TestClean();
        }

        [TestCleanupAttribute]
        public void TestClean()
        {
            ;
        }

        //------------------------------------------------

        private static class HttpData
        {
            public static string http = "http://";
            public static string https = "https://";
            public static string www = "www.";
        }

        protected bool Navigate(WebBrowser Web, String address)
        {
            if (String.IsNullOrEmpty(address)) return false;
            if (address.Equals("about:blank")) return false;
            if (!address.StartsWith(HttpData.http) &&
                !address.StartsWith(HttpData.https))
            {
                address = HttpData.http + address;
            }

            try
            {
                Web.Navigate(new Uri(address));
                return true;
            }
            catch (System.UriFormatException ex)
            {
                throw new Exception("Ошибка в uri. \n" + ex.Message);
                //return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при переходе к документу. \n" + ex.Message);
                //return false;
            }
        }

        protected void GetAttributes(WebBrowser Web, List<Tuple<string, string>> Attributes,
            string head = "META", string name = "NAME", string content = "CONTENT")
        {
            Attributes.Clear();

            var elems = Web.Document.GetElementsByTagName(head);
            try
            {
                if (elems != null)
                {
                    foreach (HtmlElement elem in elems)
                    {
                        String nameStr = elem.GetAttribute(name);
                        if (nameStr != null && nameStr.Length != 0)
                        {
                            String contentStr = elem.GetAttribute(content);
                            Attributes.Add(new Tuple<string, string>(nameStr, contentStr));
                        }
                    }
                }
                else
                    throw new Exception("Не удалось считать страницу указанного сайта.");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка во время париснга документа. \n" + ex.Message);
            }
        }

        protected void GetAttributesId(WebBrowser Web, List<Tuple<string, string>> SiteId, 
            List<Tuple<string, string, string>> SiteData, string id)
        {
            try
            {
                String contentStr = "";
                SiteData.Clear();

                foreach (var el in SiteId)
                {
                    var element = Web.Document.GetElementById(el.Item2);
                    contentStr = element.GetAttribute(id);
                    if (contentStr != null && contentStr.Length != 0)
                    {
                        SiteData.Add(new Tuple<string, string, string>(el.Item1, el.Item2, contentStr));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка во время париснга документа. \n" + ex.Message);
            }
        }

        protected void GetAttributesTextId(WebBrowser Web, List<Tuple<string, string>> SiteId,
            List<Tuple<string, string, string>> SiteData, string strstart, string strend)
        {
            try
            {
                String contentStr = "";
                SiteData.Clear();

                foreach (var el in SiteId)
                {
                    string strbody = "";
                    int start = -1;
    
                    const int N = 1000;
                    int T = 60;
                    while (start < 0 && T > 0)
                    {
                        strbody = Web.Document.Body.InnerHtml;
                        start = strbody.IndexOf(strstart);

                        Thread.Sleep(N);
                        --T;
                    }
                    if (start < 0)
                        throw new Exception("Страница не загрузилась.");
                    
                    start += strstart.Length;
                    int end = strbody.IndexOf(strend, start);
                    string string2 = strbody.Substring(start, end - start);


                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка во время париснга документа. \n" + ex.Message);
            }
        }

        private void SetWebBrowserOptions(WebBrowser Web)
        {
            Web.ScriptErrorsSuppressed = true;
            Web.Visible = false;
        }

        protected void ClearWebBrowser(WebBrowser Web)
        {
            if (Web != null)
                Web.Dispose();

            Web = null;
        }

        protected void LoadSite(WebBrowser Web, string url)
        {
            if (Navigate(Web, url) != true)
                throw new Exception("Не корректный url.");

            while (Web.ReadyState != WebBrowserReadyState.Complete)// || Web.IsBusy || Web.Url.AbsoluteUri != url)
                Application.DoEvents();
        }

        protected bool FindAttribute(List<Tuple<string, string>> Attributes, string tag)
        {
            bool flag = false;

            if (tag != "")
            {
                Parallel.ForEach(Attributes,
                (curValue, loopstate) =>
                {
                    if (tag.ToLower() == curValue.Item1.ToLower())
                    {
                        loopstate.Stop();
                        flag = true;
                        return;
                    }
                });
            }

            return flag;
        }

        protected void GetAllSiteAttributes(ref string result,
            List<Tuple<string, string>> Attributes,
            string head = "META", string name = "NAME", string content = "CONTENT")
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        LoadSite(Web, el);
                        GetAttributes(Web, Attributes, head, name, content);

                        foreach (var tag in Attributes)
                        {
                            result += "\nСайт: " + el + "\t Тэг: " + tag.Item1 + "\t Значение: " + tag.Item2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected void GetSiteAttributes(ref string error,
            List<Tuple<string, string>> Attributes, HashSet<string> hsList,
            string head = "META", string name = "NAME", string content = "CONTENT", bool add = false)
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        LoadSite(Web, el);
                        GetAttributes(Web, Attributes, head, name, content);

                        if(add)
                            AddNewAttributesToList(Attributes);
                        foreach (var tag in hsList)
                        {
                            if (!FindAttribute(Attributes, tag))
                            {
                                error += "\nСайт: " + el + "\t Тэг: " + tag;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected void GetP2C(ref string result, List<Tuple<string, string>> SiteId, 
            List<Tuple<string, string, string>> SiteData, string siteadd, 
            string strstart, string strend)
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        
                        LoadSite(Web, siteadd + CreateAddress(el));
                        //Web.Document.InvokeScript("func");
                        GetAttributesTextId(Web, SiteId, SiteData, strstart, strend);

                        foreach (var attr in SiteData)
                        {
                            result += "\nАтрибут: " + attr.Item1 + "\t Значение: " + attr.Item3;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected string CreateAddress(string address)
        { 
            if(address.StartsWith(HttpData.http))
                address = address.Replace(HttpData.http, "");
            if(address.StartsWith(HttpData.https))
                address = address.Replace(HttpData.https, "");
            if (address.StartsWith(HttpData.www))
                address = address.Replace(HttpData.www, "");

            return address;
        }
        
        private void AddNewAttributesToList(List<Tuple<string, string>> Attributes)
        {
            foreach (var el in Attributes)
            {
                if (!SiteItems.Attributes.Contains(el.Item1.ToLower()))
                {
                    SiteItems.Attributes.Add(el.Item1.ToLower());
                }
            }
        }

        protected void VerifySiteTitle(ref string error, string title)
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        string newaddress = el;
                        if (!el.StartsWith(HttpData.http) &&
                            !el.StartsWith(HttpData.https))
                        {
                            newaddress = HttpData.http + el;
                        }
                        LoadSite(Web, "https://www.google.com.ua/interstitial?url=" +
                                 newaddress);

                        //or Like str OR Containe str
                        string tmp = GetDocumentTitle(Web);
                        var t = Web.Document;
                        if (tmp == title)
                            error += "\nСайт: " + el;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected string GetDocumentTitle(WebBrowser Web)
        {
            return Web.DocumentTitle;
        }

        protected void GetWebSiteDate(ref string error)
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var s in SiteItems.WebSites)
                    {
                        LoadSite(Web, s);

                        if (Web.Document != null)
                        {
                            IHTMLDocument2 currentDoc =
                                (IHTMLDocument2)Web.Document.DomDocument;
                            error += "\nСайт: " + s + "\t Дата прошлой модификации: " + currentDoc.lastModified;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected void GetLocalFilesDate(ref string error)
        {
            try
            {
                foreach (var d in SiteItems.WebSites)
                {
                    string[] filePaths = Directory.GetFiles(d);
                    foreach (var el in filePaths)
                    {
                        var dt = File.GetLastWriteTime(el);
                        error += "\nДиреткория: " + d + "\t Файл: "
                            + Path.GetFileName(el) + "\t Дата прошлой модификации: "
                            + dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }

        protected void RunScript(string script)
        {
            try
            {
                //ClearWebBrowser(Web);
                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    Web.Url = new Uri(script);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Application.ExitThread();   // Stops the thread
            }
        }
    }
}


