using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Threading;

namespace UnitSite
{
    [TestClassAttribute]
    public sealed class UnitTest : BaseTest
    {
        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyAllWebSiteAttributes()
        {
            string result = "";

            List<Tuple<string, string>> Attributes = new List<Tuple<string, string>>();

            try
            {
                var th = new Thread(obj => GetAllSiteAttributes(ref result, Attributes,
                                                              "META", "NAME", "CONTENT"));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (result != "")
                throw new Exception("Найдены следующие мета-теги:\n" + result);
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteAttributes()
        {
            string error = "";

            List<Tuple<string, string>> Attributes = new List<Tuple<string, string>>();

            try
            {
                var th = new Thread(obj => GetSiteAttributes(ref error, Attributes, SiteItems.Attributes,
                                                              "META", "NAME", "CONTENT", true));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (error != "")
                Assert.Fail("Тэги не найдены на следующих страницах:\n" + error);
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteWebAttributes()
        {
            string error = "";

            List<Tuple<string, string>> Attributes = new List<Tuple<string, string>>();

            try
            {
                var th = new Thread(obj => GetSiteAttributes(ref error, Attributes, SiteItems.WebAttributes,
                                                              "META", "NAME", "CONTENT", false));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (error != "")
                Assert.Fail("Тэги не найдены на следующих страницах:\n" + error);
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyP2C()
        {
            string result = "";

            List<Tuple<string, string>> SiteId = new List<Tuple<string, string>>();
            SiteId.Add(new Tuple<string, string>("Google кэш", "google_cache"));
            SiteId.Add(new Tuple<string, string>("PR", "pagerank"));
            SiteId.Add(new Tuple<string, string>("Quant Rank", "quantcast_rank"));
            SiteId.Add(new Tuple<string, string>("ТИЦ", "tcy"));
            SiteId.Add(new Tuple<string, string>("DMOZ", "dmoz"));
            SiteId.Add(new Tuple<string, string>("Google индекс", "google_index"));
            SiteId.Add(new Tuple<string, string>("Yahoo индекс", "yahoo_index"));
            SiteId.Add(new Tuple<string, string>("Bing индекс", "bing_index"));
            SiteId.Add(new Tuple<string, string>("Яндекс индекс", "yandex_index"));
            SiteId.Add(new Tuple<string, string>("Yahoo BL", "yahoo_backlinks"));
            SiteId.Add(new Tuple<string, string>("Referring domains", "majestic_referring_domains"));
            SiteId.Add(new Tuple<string, string>("Majestic BL", "majestic_backlinks"));
            SiteId.Add(new Tuple<string, string>("Digg", "digg"));
            SiteId.Add(new Tuple<string, string>("delicious", "delicious"));

            List<Tuple<string, string, string>> SiteData = new List<Tuple<string, string, string>>();

            try
            {
                var th = new Thread(obj => GetP2C(ref result, SiteId, SiteData, 
                    "http://push2check.net/", "<TR bgColor=#ffffff>\r\n<TD>сегодня</TD>","</TR>"));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (result != "")
                throw new Exception("Данные по сайтам:\n" + result);
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebGoogleBlocked()
        {
            string error = "";

            try
            {
                var th = new Thread(obj => VerifySiteTitle(ref error,
                    "Предупреждение о вредоносном ПО"));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (error != "")
                Assert.Fail("Google заблокирвоал следующие сайты:\n" + error);
            else
                throw new Exception("Google не заблокирвоал ни один из сайтов.\n");
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteDate()
        {
            string result = "";

            try
            {
                var th = new Thread(obj => GetWebSiteDate(ref result));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (result != "")
                throw new Exception("Дата прошлой модификации сайтов:\n" + result);
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyLocalFilesDate()
        {
            string result = "";

            try
            {
                var th = new Thread(obj => GetLocalFilesDate(ref result));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (result != "")
                throw new Exception("Дата прошлой модификации файлов в диерктории:\n" + result);
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
                Assert.Fail("Ошибка: \n", ex.Message);
            }
        }
    }
}
