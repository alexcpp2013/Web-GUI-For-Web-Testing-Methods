#define WEB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.Threading;
using System.Text;

namespace UnitSite.Modules
{
    [TestClassAttribute]
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
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyAllWebSiteAttributes()
        {
            string result = RunMethod(GetAllMetaTags);

#if (WEB)
            throw new Exception("Найдены следующие мета-теги:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteAttributes()
        {
            string error = RunMethod(GetAddMetaTags);

            if (error != "")
                Assert.Fail("Тэги не найдены на следующих страницах:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("Тэги найдены на всех страницах.\n");
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteWebAttributes()
        {
            string error = RunMethod(GetMetaTags);

            if (error != "")
                Assert.Fail("Тэги не найдены на следующих страницах:\n" + error);

#if (WEB)
            if (error == "")
                throw new Exception("Тэги найдены на всех страницах.\n");
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyP2C()
        {
            string result = RunMethod(GetP2C);

#if (WEB)
            throw new Exception("Данные по сайтам:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebGoogleBlocked()
        {
            string error = RunMethod(VerifyGoogleSiteTitle);

            if (error != "")
                Assert.Fail("Google заблокирвоал следующие сайты:\n" + error);
            
#if (WEB)
            if (error == "")  
                throw new Exception("Google не заблокирвоал ни один из сайтов.\n");
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyWebSiteDate()
        {
            string result = RunMethod(GetWebSiteDate);

#if (WEB)
            throw new Exception("Дата прошлой модификации сайтов:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyLocalFilesDate()
        {
            //OR without thread
            string result = RunMethod(GetLocalFilesDate);

#if (WEB)
            throw new Exception("Дата прошлой модификации файлов в диерктории:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyFileType()
        {
            string result = RunMethod(GetCountFiles);

#if (WEB)  
            throw new Exception("Количетсво файлов на сайте:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyLoadTime()
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

            string result = RunMethod(GetLoadTime);

#if (WEB)
            throw new Exception("Время загрузки сайтов:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifySiteEdit()
        {
            string result = RunMethod(GetSiteEdit);

#if (WEB)
            throw new Exception("Изменения на странице:\n" + result);
#endif
        }

        [STAThread]
        [TestMethod, TestCategory("Regression Tests")]
        public void VerifyRobotsTxt()
        {
            string result = RunMethod(GetRobotsTxt);

#if (WEB)
            throw new Exception("Данные из robots.txt:\n" + result);
#endif
        }
    }
}
