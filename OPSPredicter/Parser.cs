using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Threading;

namespace OPSPredicter
{
    static class Parser
    {
        static private WebBrowser webBrowser1 = new WebBrowser { ScriptErrorsSuppressed = true };

        public static OPSGame ParseUrl(int gameNumber)
        {
            try
            {
                bool contentIsDynamic = false;

                // Get content based on gameNumber
                string url = ("https://www.mlb.com/gameday/" + gameNumber + "?#game_state=final,game_tab=box,game=" + gameNumber);
                string content = GetContent(url); ;

                // Ensure content is dynamic
                do
                {
                    contentIsDynamic = CheckIfDynamic(content, gameNumber);
                    content = GetContent(url);
                } while (!contentIsDynamic);

                // Check if page is blank
                if (content.Contains("We did not find the page you were looking for. Did you type the link correctly?"))
                    return null;

                // What to extract from content
                double[] playerOPS = new double[18];
                int[] teamScores = new int[2];

                // Method to get scores
                int position = content.IndexOf("data-game-pk=\"" + gameNumber + "\">");
                if (position == -1)
                    position = 0;
                int maxIterations = content.Length - position;

                int count = 0;
                for (int i = 0; i < maxIterations; i++)
                {
                    if (count > 1)
                        break;

                    // Get score (works for all games)
                    position = content.IndexOf("score\">", position) + 7;
                    if (content[position] == 'R')
                        position = content.IndexOf("score\">", position + 1) + 7;

                    int score = ExtractScoreContent(content, position);

                    teamScores[count] = score;
                    count++;
                }

                // Method to get OPS
                position = content.IndexOf("batting stats");
                maxIterations = content.Length - position;

                bool isBattingOrderOdd = true;
                count = 0;
                for (int i = 0; i < maxIterations; i++)
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

                    double OPS;

                    try
                    {
                        OPS = ExtractOPSContent(content, position);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }

                    playerOPS[count] = OPS;
                    count++;
                    isBattingOrderOdd = !isBattingOrderOdd;

                    if (count == 9)
                    {
                        position = content.IndexOf("batting stats is-batting");
                        isBattingOrderOdd = true;
                    }
                }

                /*
                 * DEBUG
                 */
                Debug.Print($"Debugging {gameNumber}...");

                //Debug Scores
                Debug.Print(teamScores[0].ToString() + "-" + teamScores[1].ToString());

                //Debug OPS
                for (int i = 0; i < 18; i++)
                {
                    Debug.Print((i + 1).ToString() + " " + playerOPS[i].ToString());
                }
                /*
                 * DEBUG
                 */

                return new OPSGame(gameNumber, playerOPS, teamScores);
            }
            catch
            {
                return null;
            }
        }

        private static int ExtractScoreContent(string content, int position)
        {
            int endPosition = content.IndexOf('<', position);
            if (endPosition < 0)
                throw new ApplicationException("Score has invalid format");

            string value = content.Substring(position, endPosition - position);

            if (!Int32.TryParse(value, out int score))
                throw new ApplicationException($"Score not number {value}");

            return score;
        }

        private static double ExtractOPSContent(string content, int position)
        {
            int endPosition = content.IndexOf('<', position);
            if (endPosition < 0)
                throw new ApplicationException("OPS has invalid format");
            string value = content.Substring(position, endPosition - position);
            if (value[0] == '.')
                value = "0" + value;

            if (!Double.TryParse(value, out double ops))
                throw new ApplicationException($"OPS not number {value}");

            return ops;
        }

        private static string GetContent(string url)
        {
            webBrowser1.Navigate(url);

            var sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Getting stats from: " + url);

           WaitTillLoad(webBrowser1);

            sw.Stop();

            Debug.WriteLine($"{url} loaded in {sw.ElapsedMilliseconds} mls");

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
            StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
            doc.Load(sr);

            //Debug
            System.IO.File.WriteAllText(@"C:\Users\mbile\Desktop\example.txt", doc.DocumentNode.OuterHtml);

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
                //Application.DoEvents();
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
