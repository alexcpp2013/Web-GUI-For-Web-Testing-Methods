using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Threading;
using System.Text;

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

            try
            {
                var th = new Thread(obj => GetP2C(ref result));
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
                //OR without thread
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

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyFileType()
        {
            string result = "";

            try
            {
                var th = new Thread(obj => GetCountFiles(ref result));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }

            if (result != "")
                throw new Exception("Количетсво pdf файлов на сайте:\n" + result);
        }
    }
}
