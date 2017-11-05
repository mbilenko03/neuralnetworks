using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace OPSPredicter
{
    class Parser
    {
        public static OPSGame ParseUrl(int gameNumber)
        {
            bool contentIsDynamic = false;

            // Get content based on gameNumber
            string url = ("https://www.mlb.com/gameday/" + gameNumber + "?#game_state=final,game_tab=box,game=" + gameNumber);
            string content = GetContent(url); ;

            do
            {
                contentIsDynamic = CheckIfDynamic(content, gameNumber);
                content = GetContent(url);
            } while (!contentIsDynamic);

            //What to extract from content
            double[] playerOPS = new double[18];
            int[] teamScores = new int[2];

            // Get Scores
            string findGameScore = "data-game-pk=\"" + gameNumber + "\">";
            int gameScoreScope = content.IndexOf(findGameScore);

            int count = 0;

            for (int i = 0; i < (content.Length - gameScoreScope); i++)
            {
                if (count > 1)
                    break;

                int place = gameScoreScope + i;

                char lastChar = content[place];

                if (lastChar == '>' && content[place - 1] == '"' && content[place - 2] == 't' && content[place - 3] == 'x' && content[place - 4] == 'e'
                    && content[place - 5] == 'T' && content[place - 6] == 'e' && content[place - 7] == 'm' && content[place - 8] == 'a' && content[place - 9] == 'g'
                    && content[place - 10] == ' ' && content[place - 11] == 's')
                {
                    teamScores[count] = Int32.Parse(content[place + 1].ToString());
                    count++;
                }
            }

            // Get OPS
            string findBattingStats = "batting stats";
            int battingStatsScope = content.IndexOf(findBattingStats);

            bool isBattingOrderOdd = true;
            count = 0;
            // Check if count over 17
            // Check what batting order is up
            // Find the OPS and add to array
            // Change Batting order
            // Increase count
            // if count 10 revert change in batting order
            int position = battingStatsScope;

            for (int i = 0; i < (content.Length - battingStatsScope); i++)
            {
                if (count > 17)
                    break;

                if (isBattingOrderOdd)
                {
                    position = content.IndexOf("bat-order-odd", position);
                }
                else
                {
                    position = content.IndexOf("bat-order-even", position);
                }

                position = content.IndexOf("ops wider\">", position) + 11;

                double OPS = ExtractOPSContent(content, position);

                playerOPS[count] = OPS;
                count++;
                isBattingOrderOdd = !isBattingOrderOdd;

                if (count == 9)
                {
                    position = content.IndexOf("batting stats is-batting");
                    isBattingOrderOdd = true;
                }
            }

            //Debug Scores
            for (int i = 0; i < 2; i++)
            {
                Debug.Print(teamScores[i].ToString());
            }

            //Debug OPS
            for (int i = 0; i < 18; i++)
            {
                Debug.Print(playerOPS[i].ToString() + " " + (i+ 1).ToString());
            }

            return new OPSGame(gameNumber, playerOPS, teamScores);
        }

        private static double ExtractOPSContent(string content, int position)
        {
            int endPosition = content.IndexOf('<', position);
            if (endPosition < 0)
                throw new ApplicationException("OPS has invalid format");
            string value = content.Substring(position, endPosition - position);
            if (value[0] == '.')
                value = "0" + value;

            double ops;
            if (!Double.TryParse(value, out ops))
                throw new ApplicationException($"OPS not number {value}");

            return ops;
        }

        private static string GetContent(string url)
        {
            WebBrowser webBrowser1 = new WebBrowser();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(url);

            WaitTillLoad(webBrowser1);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
            StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
            doc.Load(sr);

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

        private static bool CheckIfDynamic(string content, int gameNumber)
        {
            string findGameScore = "data-game-pk=\"" + gameNumber + "\">";
            int j = content.IndexOf(findGameScore);

            if (j != 1)
                return true;

            return false;
        }
    }
}
