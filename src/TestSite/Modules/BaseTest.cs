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
//using System.Xml;
using System.Text.RegularExpressions;

namespace UnitSite
{
    public abstract class BaseTest
    {
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

            public enum TypeParameter { Good, Nothing, Error }
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

        protected void LoadSite(WebBrowser Web, string url, bool delay = false, string tag = "body")
        {
            if (Navigate(Web, url) != true)
                throw new Exception("Не корректный url.");

            while (Web.ReadyState != WebBrowserReadyState.Complete)
                // || Web.IsBusy || Web.Url.AbsoluteUri != url)
                Application.DoEvents();

            if (delay)
            {
                var d = new Delay(Web);
            }

            /*if(Web.Document.Body == null)
                throw new Exception("Документ не заргружен.");*/
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
                        result += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                result += "\n\nОшибка: " + ex.Message;
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

                        if (add)
                            AddNewAttributesToList(Attributes);
                        foreach (var tag in hsList)
                        {
                            if (!FindAttribute(Attributes, tag))
                            {
                                error += "\nСайт: " + el + "\t Тэг: " + tag;
                            }
                        }
                        error += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                error += "\n\nОшибка: " + ex.Message;
            }
            finally
            {
                Application.ExitThread();
            }
        }

        protected void GetP2C(ref string result)
        {
            try
            {
                using (var Web = new WebBrowser())
                {
                    var SiteData = new List<string>() { "Google кэш", "PR", "Alexa Rank", "Quant Rank",
                                                        "ТИЦ", "DMOZ", "Google индекс", "Yahoo индекс", 
                                                        "Bing индекс", "Яндекс индекс", "Yahoo BL", "Referring domains",
                                                        "Majestic BL", "Alexa BL", "Digg", "delicious" };
                    var SiteResult = new List<string>();
                    var siteadd = "http://push2check.net/";

                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        string site = siteadd + CreateAddress(el);
                        LoadSite(Web, site, true);
                        if (Web.Url.ToString() != site)
                            throw new Exception("Адрес загруженного сайта не совпадает с адресом запрашеваемого: \n" +
                                Web.Url.ToString());

                        //Web.Document.InvokeScript("func");

                        /*string strStart0 = "<TR bgColor=#ffffff>\r\n<TD>сегодня</TD>";
                        string strStart = "<TR bgColor=#ffffff>\r\n<TD>сегодня</TD>";
                        string strStart1 = "<TR bgColor=#ffffff>\r\n<TD>today</TD>";
                        string strEnd = "</TR>";
                        var tmp = GetSubString(Web, strStart, strEnd);
                        var res = Regex.Split(@tmp, @"\r\n<TD class=\w*>|</TD>", 
                                              RegexOptions.IgnoreCase);*/
                        
                        //ToLower()

                        var tmp = Regex.Split(@Web.Document.Body.InnerHtml,
                                              @"<TR bgColor=#ffffff>\r\n<TD>\w*</TD>",
                                              RegexOptions.IgnoreCase);
                        if (tmp.Length < 2)
                            throw new Exception("Ошибка во время прасинга начала строки.");
                        tmp = Regex.Split(@tmp[1],
                                          @"</TR>",
                                          RegexOptions.IgnoreCase);
                        if (tmp.Length == 0)
                            throw new Exception("Ошибка во время прасинга конца строки.");
                        var res = Regex.Split(@tmp[0], @"\r\n<TD class=\w*>|</TD>",
                                              RegexOptions.IgnoreCase);

                        if (SiteData.Count != (int)(res.Count() / 2))
                            throw new Exception("Ошибка в формате данных во время парсинга.");
                        int N = res.Count();
                        for (int i = 1; i < N; i += 2)
                        {
                            SiteResult.Add(res[i]);
                        }

                        for (int i = 0; i < SiteData.Count; ++i)
                        {
                            result += "\nСайт: " + el + "\tАтрибут: " + SiteData[i] + "\t Значение: " + SiteResult[i];
                        }
                        result += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                result += "\n\nОшибка: " + ex.Message;
            }
            finally
            {
                Application.ExitThread();
            }
        }

        //No
        private string GetSubString(WebBrowser Web, string strstart0, string strstart1, string strend)
        {
            var strbody = Web.Document.Body.InnerHtml;
            var start = strbody.IndexOf(strstart0);
            int length = 0;
            
            if (start < 0)
            {
                start = strbody.IndexOf(strstart1);
                if (start < 0)
                    throw new Exception("Начало строки не найдено.");
                else
                    length = strstart1.Length;
            }
            else
                length = strstart0.Length;
            start += length;

            //start += strstart.Length;
            var end = strbody.IndexOf(strend, start);
            if (end < 0)
                throw new Exception("Конец строки не найден.");

            return strbody.Substring(start, end - start);
        }

        //No
        private string GetSubString(WebBrowser Web, string strstart, string strend)
        {
            var strbody = Web.Document.Body.InnerHtml;
            var start = strbody.IndexOf(strstart);

            if (start < 0)
                throw new Exception("Начало строки не найдено.");

            start += strstart.Length;

            var end = strbody.IndexOf(strend, start);
            if (end < 0)
                throw new Exception("Конец строки не найден.");

            return strbody.Substring(start, end - start);
        }

        //No
        protected string GetElelemntInnerTextById(WebBrowser Web, string Id)
        {
            return Web.Document.GetElementById(Id).InnerText;
        }

        //No
        protected int GetElelemntTextByText(WebBrowser Web, string text)
        {
            return Web.Document.Body.InnerText.IndexOf(text);
        }

        protected string CreateAddress(string address)
        {
            if (address.StartsWith(HttpData.http))
                address = address.Replace(HttpData.http, "");
            if (address.StartsWith(HttpData.https))
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
                    string prefsite = "https://www.google.com.ua/interstitial?url=";

                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        string newaddress = el;
                        if (!el.StartsWith(HttpData.http) &&
                            !el.StartsWith(HttpData.https))
                        {
                            newaddress = HttpData.http + el;
                        }
                        LoadSite(Web, prefsite +
                                 newaddress);

                        //or Like str OR Containe str
                        string tmp = GetDocumentTitle(Web);
                        var t = Web.Document;
                        if (tmp == title)
                            error += "\nСайт: " + el;
                        if(error != "")
                            error += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                error += "\n\nОшибка: " + ex.Message;
            }
            finally
            {
                Application.ExitThread();
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
                        error += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                error += "\n\nОшибка: " + ex.Message;
            }
            finally
            {
                Application.ExitThread();
            }
        }

        protected void GetCountFiles(ref string result)
        {
            try
            {
                //var prefsite = "https://www.google.com.ua/#q=site:";
                var prefsite = "https://www.google.com.ua/search?q=site:";
                var suffix = "filetype:";
                var id = "resultStats";
                var iesuffix = "&cad=h";
                var noprefics = "По запросу site:";
                var nosuffix = "ничего не найдено.";

                using (var Web = new WebBrowser())
                {
                    SetWebBrowserOptions(Web);

                    foreach (var el in SiteItems.WebSites)
                    {
                        foreach (var filetypes in SiteItems.WebAttributes)
                        {
                            var site = prefsite + el + "+" + suffix + filetypes + iesuffix;
                            LoadSite(Web, site);

                            if (Web.Url.ToString() != site)
                                throw new Exception("Адрес загруженного сайта не совпадает с адресом запрашеваемого: \n" +
                                    Web.Url.AbsoluteUri +
                                    "\n" + site);

                            string res = "";
                            var data = Web.Document.GetElementById(id);

                            if (data != null)
                            {
                                if (data.InnerHtml != null)
                                    res = data.InnerText;
                                else
                                    res = NoCountFiles(suffix, noprefics, nosuffix, Web, el, filetypes, res);
                            }
                            else
                            {
                                res = NoCountFiles(suffix, noprefics, nosuffix, Web, el, filetypes, res);
                            }

                            result += "\nСайт: " + el + "\tколичество " + filetypes + " файлов: " + res;
                        }
                        result += "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                result += "\n\nОшибка: " + ex.Message; 
                //+ "\nСайт: " + Web.Url
            }
            finally
            {
                Application.ExitThread();
            }
        }

        private static string NoCountFiles(string suffix, string noprefics, string nosuffix, 
                                           WebBrowser Web, string el, string filetypes, string res)
        {
            string nores = noprefics + el + " " + suffix + filetypes + " " + nosuffix;
            if (Web.Document.Body.InnerText.IndexOf(nores) < 0)
                throw new Exception("Не найдено ключевых элементов на странице.");
            else
                res = nores;
            return res;
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
                    error += "\n";
                }
            }
            catch (Exception ex)
            {
                error += "\n\nОшибка: " + ex.Message;
                //throw; IF not thread
            }
            finally
            {
                Application.ExitThread();
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
                ;
            }
            finally
            {
                Application.ExitThread();
            }
        }

        protected void Script(string script)
        {
            try
            {
                var th = new Thread(obj => RunScript(script));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка: \n" + ex.Message);
            }
        }
    }
}


