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
using System.Web;

namespace UnitSite
{
    public partial class testpage : System.Web.UI.Page
    {
        private static class WebItems
        {
            static public string uploadDir = "Upload\\";
            static public string uploadFiletype = ".xml";
            static public string downloadFiletype = "txt";
            static public string downloadFileName = "result";
        }

        public class MailData
        {
            public string subject;
            public string password;
            public string message;
            public string from;
            public string to;
            public string client;
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
            TextBox1.Text = "";
            GetData();

            UnitTest test = new UnitTest();
            test.ClassInitialize();

            switch (DropDownList1.SelectedIndex)
            {
                case 0: RunMethod(test, test.VerifyWebGoogleBlocked, ref error,
                                 "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 1: RunMethod(test, test.VerifyAllWebSiteAttributes, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 2: RunMethod(test, test.VerifyWebSiteAttributes, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 3: RunMethod(test, test.VerifyWebSiteWebAttributes, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 4: RunMethod(test, test.VerifyWebSiteDate, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 5: RunMethod(test, test.VerifyLocalFilesDate, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                case 6: RunMethod(test, test.VerifyP2C, ref error,
                                "\n" + DropDownList1.SelectedItem.ToString() + "\n");
                        break;
                default: RunAlertScript("Ошибка во время определения метода для тестирования.");
                        break;
            }

            test.ClassClean();

            var finish = DateTime.Now;
            var delta = finish - start;
            error += "\nВремя выполнения: " + delta;

            TextBox1.Text = error;
        }

        private void GetData()
        {
            SiteItems.WebSites.Clear();
            SiteItems.WebAttributes.Clear();

            string[] splitS = TextBox2.Text.Split(new Char[] { '\n' });
            string[] splitA = TextBox3.Text.Split(new Char[] { '\n' });

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
            UploadFileToText(FileUpload1, TextBox2, WebItems.uploadDir, WebItems.uploadFiletype, 
                SiteItems.WebSites, "site");
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            UploadFileToText(FileUpload2, TextBox3, WebItems.uploadDir, WebItems.uploadFiletype, 
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

                        if(File.Exists(filefullname))
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
            Response.Write(TextBox1.Text);
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
            Message.Subject = md.subject;
            Message.Body = md.message;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.From = new System.Net.Mail.MailAddress(md.from);
            Message.To.Add(new MailAddress(md.to));
            System.Net.Mail.SmtpClient Smtp = new SmtpClient(md.client);
            Smtp.EnableSsl = true; // актуально для почтовых служб с SSL, например Gmail
            Smtp.Credentials = new System.Net.NetworkCredential(md.from, md.password);
            Smtp.Send(Message);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var md = new MailData();

            md.message = "Ваш результат:\n\n" +
                "Сайты: \n" + TextBox2.Text + "\n\n" +
                "Результат: \n" + TextBox1.Text;
            md.subject = "Результат тестов";
            md.from = "testdp0mail@gmail.com";
            md.client = "smtp.gmail.com";
            md.password = "Qwerty!123";
            md.to = TextBox4.Text;

            SendEmail(md);
        }
    }
}