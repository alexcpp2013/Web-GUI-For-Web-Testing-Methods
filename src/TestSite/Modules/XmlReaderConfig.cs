using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace UnitSite
{
    public class XmlReaderConfig
    {
        string fileheader = "/config";

        public void GetParameters(string file, HashSet<string> list, string tag)
        {
            list.Clear();
            try
            {
                XmlDocument rdr = new XmlDocument();
                rdr.Load(@file); // Загрузка XML

                /*XmlNode xmlData = rdr.SelectSingleNode(fileheader);*/
                XmlNodeList xnList = rdr.SelectNodes(fileheader + "/"+ tag);
                foreach (XmlNode xn in xnList)
                {
                    list.Add(xn.InnerText);
                }

                /*foreach (var n in xmlData)
                {
                    list.Add(xmlData[tag].InnerText);
                }*/
            }
            catch (Exception ex)
            {
                throw;
            }
         }
    }
}


