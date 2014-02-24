#define WEB
#undef NUNIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit.Framework;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace UnitSite.Modules
{

    /// <summary>
    /// Класс для тестов
    /// </summary>
#if (NUNIT)
    [TestFixture, Category("Regression Tests")]
#else
    [TestClassAttribute]
#endif
    public sealed class UnitTest : BaseTest
    {
        public UnitTest()
        {
            ClassInitialize();
        }

        ~UnitTest()
        {
            ClassClean();
        }
        
        protected delegate void TestMethod(ref string str);

        protected string RunMethod(TestMethod d)
        {
            string str = "";

            if (d != null)
            {
                try
                {
                    var th = new Thread(obj => d(ref str));
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                    th.Join();
                }
                catch (Exception ex)
                {
                    Assert.Fail("Ошибка: \n", ex.Message);
                }
            }

            return str;
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyAllWebPageAttributes()
        {
            string result = RunMethod(GetAllPageMetaTags);

#if (WEB)
            throw new Exception("\nНайдены следующие метатеги:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyWebPageAttributes()
        {
            string error = RunMethod(GetAddPageMetaTags);

            if (error != "")
                Assert.Fail("\nТэги не найдены на следующих страницах:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("\nТэги найдены на всех страницах.\n");
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyWebPageWebAttributes()
        {
            string error = RunMethod(GetPageMetaTags);

            if (error != "")
                Assert.Fail("\nТэги не найдены на следующих страницах:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("\nТэги найдены на всех страницах.\n");
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyP2C()
        {
            string result = RunMethod(GetP2C);

#if (WEB)
            throw new Exception("\nДанные по сайтам:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyWebGoogleBlocked()
        {
            string error = RunMethod(VerifyGooglePageTitle);

            if (error != "")
                Assert.Fail("\nGoogle заблокирвоал следующие страницы/сайты:\n" + error);
            
#if (WEB)
            if (error == "")  
                throw new Exception("\nGoogle не заблокирвоал ни одну из страниц (не один из сайтов).\n");
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void IsPageLoad()
        {
            string error = RunMethod(VerifyIsPageLoad);

            if (error != "")
                Assert.Fail("Следующие страницы не загружаются:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("\nВсе страницы загружаются.\n");
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void IsPageUpdatedSixMonth()
        {
            string error = RunMethod(VerifyIsPageUpdatedSixMonth);

            if (error != "")
                Assert.Fail("\nСледующие страницы не обновлялись последние 6 месяцев:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("\nВсе страницы обновлялись за последние 6 месяцев.\n");
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void NoMethod()
        {
            string error = "\nДанный метод еще не реализован.\n".ToUpper();

            Assert.Fail(error);
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyWebPageDate()
        {
            string result = RunMethod(GetWebPageDate);

#if (WEB)
            throw new Exception("\nДата прошлой модификации страниц:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyLocalFilesDate()
        {
            //OR without thread
            string result = RunMethod(GetLocalFilesDate);

#if (WEB)
            throw new Exception("\nДата прошлой модификации файлов в диерктории:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyFileType()
        {
            string result = RunMethod(GetCountFiles);

#if (WEB)  
            throw new Exception("\nКоличетсво файлов на сайте:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyPageLoadTime()
        {
            /*string result = "";

            try
            {
                var th = new Thread(obj => GetLoadTime(ref result));
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
            catch (Exception ex)
            {
                Assert.Fail("Ошибка: \n", ex.Message);
            }*/

            string result = RunMethod(GetPageLoadTime);

#if (WEB)
            throw new Exception("\nВремя загрузки страницы:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void AllPageUrls()
        {
            string result = RunMethod(GetAllPageUrls);

#if (WEB)
            throw new Exception("\nСсылки:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void AllSitePagesUrls()
        {
            string result = RunMethod(GetAllSitePageUrls);

#if (WEB)
            throw new Exception("\nСсылки:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyPageDBEdit()
        {
            string result = RunMethod(GetPageEdit);

#if (WEB)
            throw new Exception("\nИзменения на странице:\n" + result);
#endif
        }

        [STAThread]
#if (NUNIT)
        [TestFixture]
#else
        [TestMethod, TestCategory("Regression Tests")]
#endif
        public void VerifyRobotsTxt()
        {
            string result = RunMethod(GetRobotsTxt);

#if (WEB)
            throw new Exception("\nДанные из robots.txt:\n" + result);
#endif
        }
    }
}
