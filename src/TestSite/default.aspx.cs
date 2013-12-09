using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.JScript;
using System.IO;
using System.Text;
using System.Net.Mail;

namespace UnitSite
{
    public partial class TestPage : System.Web.UI.Page
    {
        private static class WebItems
        {
            static public string uploadDir = "Upload\\";
            static public string uploadFiletype = ".xml";
            static public string downloadFiletype = "txt";
            static public string downloadFileName = "result";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ;
        }

        protected delegate void SomeMethod();

        protected void RunMethod(UnitTest test, SomeMethod d,
            ref string error, string add)
        {
            if (d == null)
                return;

            try
            {
                error += add;
                test.TestStart();
                d();
                test.TestClean();
            }
            catch (Exception ex)
            {
                error += ex.Message;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var start = DateTime.Now;

            string error = "";
            tbResult.Text = "";

            try
            {
                GetData();

                var test = new UnitTest();
                test.ClassInitialize();

                switch (ddList.SelectedIndex)
                {
                    case 0: RunMethod(test, test.VerifyWebGoogleBlocked, ref error,
                                     "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 1: RunMethod(test, test.VerifyAllWebSiteAttributes, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 2: RunMethod(test, test.VerifyWebSiteAttributes, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 3: RunMethod(test, test.VerifyWebSiteWebAttributes, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 4: RunMethod(test, test.VerifyWebSiteDate, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 5: RunMethod(test, test.VerifyLocalFilesDate, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 6: RunMethod(test, test.VerifyP2C, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    case 7: RunMethod(test, test.VerifyFileType, ref error,
                                    "\n" + ddList.SelectedItem.ToString() + "\n");
                        break;
                    default: RunAlertScript("Ошибка во время определения метода для тестирования.");
                             error += "Ошибка во время определения метода для тестирования.";
                        break;
                }

                test.ClassClean();

                var finish = DateTime.Now;
                var delta = finish - start;
                error += "\n\nВремя выполнения: " + delta;
            }
            catch (Exception ex)
            {
                error += "\n" + ex.Message;
            }
            finally
            {
                tbResult.Text = error;
            }
        }

        private void GetData()
        {
            SiteItems.WebSites.Clear();
            SiteItems.WebAttributes.Clear();

            string[] splitS = tbSites.Text.Split(new Char[] { '\n' });
            string[] splitA = tbTags.Text.Split(new Char[] { '\n' });

            foreach (var s in splitS)
            {
                if (s.Trim() != "")
                    SiteItems.WebSites.Add(s);
            }

            foreach (var attr in splitA)
            {
                if (attr.Trim() != "")
                    SiteItems.WebAttributes.Add(attr);
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            UploadFileToText(FileUpload1, tbSites, WebItems.uploadDir, WebItems.uploadFiletype,
                SiteItems.WebSites, "site");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            UploadFileToText(FileUpload2, tbTags, WebItems.uploadDir, WebItems.uploadFiletype,
                SiteItems.WebAttributes, "tag");
        }

        protected string GetName()
        {
            string n = DateTime.Now.Date.Year.ToString() + "-" +
                       DateTime.Now.Date.Month.ToString() + "-" +
                       DateTime.Now.Day.ToString() + "-" +
                       DateTime.Now.TimeOfDay.Hours.ToString() + "-" +
                       DateTime.Now.TimeOfDay.Minutes.ToString() + "-" +
                       DateTime.Now.TimeOfDay.Seconds.ToString() + "-" +
                       DateTime.Now.TimeOfDay.Milliseconds.ToString() + "-";

            return n;
        }

        private void UploadFileToText(FileUpload file, TextBox text,
            string dir, string filetype,
            HashSet<string> hsList, string attribute)
        {
            if (file.HasFile)
            {
                try
                {
                    string filename = file.FileName;
                    string extension = System.IO.Path.GetExtension(filename);

                    if (extension == filetype)
                    {
                        string filefullname = Server.MapPath("~/") + dir + GetName() + filename;

                        file.SaveAs(filefullname);

                        var r = new XmlReaderConfig();
                        r.GetParameters(filefullname, hsList, attribute);

                        if (File.Exists(filefullname))
                            File.Delete(filefullname);

                        text.Text = "";
                        foreach (var s in hsList)
                        {
                            text.Text += s + "\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    text.Text += ex.Message;
                }
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string filefullname = GetName() + WebItems.downloadFileName +
                "." + WebItems.downloadFiletype;

            try
            {
                SaveClientFile(filefullname);
            }
            catch (Exception ex)
            {
                string script = "Ошибка во время сохранения результата: " + ex.Message;
                RunAlertScript(script);
            }
        }

        private void SaveClientFile(string filefullname)
        {
            //Response.Charset = "windows-1251";
            Response.HeaderEncoding = Encoding.GetEncoding("windows-1251");
            Response.ContentType = "application/" + WebItems.downloadFiletype;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filefullname);
            //Response.ContentEncoding = Encoding.UTF8;
            Response.Write(tbResult.Text);
            Response.End();
        }

        private void RunAlertScript(string script)
        {
            Controls.Add(new LiteralControl("<script>alert('" +
                script.Replace('\'', ' ').Replace('\n', ' ') +
                "');</script>"));
        }

        protected void SendEmail(MailData md)
        {
            MailMessage Message = new MailMessage();
            Message.Subject = md.Subject;
            Message.Body = md.Message;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.From = new System.Net.Mail.MailAddress(md.From);
            Message.To.Add(new MailAddress(md.To));
            System.Net.Mail.SmtpClient Smtp = new SmtpClient(md.Client);
            Smtp.EnableSsl = true; // актуально для почтовых служб с SSL, например Gmail
            Smtp.Credentials = new System.Net.NetworkCredential(md.From, md.Password);
            Smtp.Send(Message);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            try
            {
                var md = new MailData();

                md.Message = "Ваш результат:\n\n" +
                    "Сайты: \n" + tbSites.Text + "\n\n" +
                    "Результат: \n" + tbResult.Text;
                md.Subject = "Результат тестов";
                md.From = "testdp0mail@gmail.com";
                md.Client = "smtp.gmail.com";
                md.Password = "Qwerty!123";
                md.To = tbMail.Text;

                SendEmail(md);
            }
            catch (Exception ex)
            {
                RunAlertScript(ex.Message);
            } 
            RunAlertScript("Письмо отправлено.");
        }

        protected void ScriptManager1_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs ex)
        {
            RunAlertScript(ex.Exception.Message);
        }
    }
}