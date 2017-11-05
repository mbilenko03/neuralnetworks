using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace OPSPredicter
{
    class Parser
    {
        public static OPSGame ParseUrl(string gameNumber)
        {
            string url = ("https://www.mlb.com/gameday/" + gameNumber + "?#game_state=final,game_tab=box,game=" + gameNumber);

            string content = getContent(url);



            return null;
        }

        public static string getContent(string url) 
        {
            // HtmlWeb web = new HtmlWeb();
            // HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            // Debug.WriteLine(doc.DocumentNode.InnerHtml);
            LoadHtmlWithBrowser(url);

            return null;
        }

        private static void LoadHtmlWithBrowser(String url)
        {
            WebBrowser webBrowser1 = new WebBrowser();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(url);

            waitTillLoad(webBrowser1);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
            StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
            doc.Load(sr);
            Debug.WriteLine(doc.DocumentNode.OuterHtml);
        }

        private static void waitTillLoad(WebBrowser webBrControl)
        {
            WebBrowserReadyState loadStatus;
            int waittime = 100000;
            int counter = 0;
            while (true)
            {
                loadStatus = webBrControl.ReadyState;
                Application.DoEvents();
                if ((counter > waittime) || (loadStatus == WebBrowserReadyState.Uninitialized) || (loadStatus == WebBrowserReadyState.Loading) || (loadStatus == WebBrowserReadyState.Interactive))
                {
                    break;
                }
                counter++;
            }

            counter = 0;
            while (true)
            {
                loadStatus = webBrControl.ReadyState;
                Application.DoEvents();
                if (loadStatus == WebBrowserReadyState.Complete && webBrControl.IsBusy != true)
                {
                    break;
                }
                counter++;
            }
        }
    }
}
