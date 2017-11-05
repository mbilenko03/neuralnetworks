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
        public static OPSGame ParseUrl(int gameNumber)
        {
            string url = ("https://www.mlb.com/gameday/" + gameNumber + "?#game_state=final,game_tab=box,game=" + gameNumber);

            string content = GetContent(url);



            return null;
        }

        public static string GetContent(string url) 
        {
            WebBrowser webBrowser1 = new WebBrowser();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(url);

            WaitTillLoad(webBrowser1);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
            StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
            doc.Load(sr);

            // Debug
            Debug.WriteLine(doc.DocumentNode.OuterHtml);
            
            return doc.DocumentNode.OuterHtml;
        }

        private static void WaitTillLoad(WebBrowser webBrControl)
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
