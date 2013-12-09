using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Windows.Forms;

namespace UnitSite
{
    public static class SiteItems
    {
        public static List<string> Sites = new List<string>()
        {
            "http://kpi.ua/",
            "http://cad.edu.kpi.ua/tsoorin/"
        };

        public static HashSet<string> WebSites = new HashSet<string>();

        public static HashSet<string> WebAttributes = new HashSet<string>();

        public static HashSet<string> Attributes = new HashSet<string>()
        {
                "abstract",
                "author",
                "content-language",
                "content-style-type",
                "content-type",
                "copyright",
                "description",
                "designer",
                "document-state",
                "expires",
                "generator",
                "google",
                "google-site-verification",
                "imagetoolbar",
                "keywords",
                "language",
                "msnbot",
                "mssmarttagspreventparsing",
                "pics-label",
                "pragma",
                "publisher",
                "rating",
                "refresh",
                "reply-to",
                "resource-type",
                "revisit",
                "revisit-after",
                "robots",
                "set-cookie",
                "subject",
                "title",
                "url",
                "vw96.objecttype",
                "window-target"
        };
    }
}

