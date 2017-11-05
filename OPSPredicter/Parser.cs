using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;

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
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            Debug.WriteLine(doc.ToString());
            

            return null;
        }
    }
}
