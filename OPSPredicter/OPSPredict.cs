using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace OPSPredicter
{
    static class OPSPredict
    {
        public static NeuralNetwork CreateModel(string modelName)
        {
            NeuralNetwork net = new NeuralNetwork(new int[] { 18, 29, 29, 2 }, modelName);
            return net;
        }

        public static NeuralNetwork LoadModel(string modelName)
        {
            string path = Directory.GetCurrentDirectory() + @"\..\..\Model";
            return NeuralNetwork.LoadModel(modelName, path);
        }

        public static void TrainModelAt(NeuralNetwork net, int gameNumber)
        {
            Console.WriteLine($"Training at {gameNumber}");
            string path = Directory.GetCurrentDirectory() + @"\..\..\Model";
            string dataPath = Directory.GetCurrentDirectory() + @"\..\..\Data";

            //Check if data is already saved
            //if not null then parse
            OPSData data = OPSData.GetData(dataPath, "data");
            OPSGame game;

            //Check if not empty game and not already added to data
            if (data.GetGame(gameNumber) == null && !data.emptyGames.Contains(gameNumber))
            {
                game = Parser.ParseUrl(gameNumber);
                if (game != null)
                    data.AddGame(game);
                else
                    data.AddEmptyGame(gameNumber);
            }
            else
                game = data.GetGame(gameNumber);

            if (game == null)
            {
                Console.WriteLine(gameNumber + " was empty");
                return;
            }

            net.FeedForward(ToFloatArray(game.PlayerOPS));
            net.BackProp(ToFloatArray(game.TeamScores));

            net.SaveModel(path);
            data.SaveData(dataPath, "data");

            Console.WriteLine("Train sucessful!");
        }

        public static void TrainModelThrough(NeuralNetwork net, int minGameNumber, int maxGameNumber, int epochs)
        {
            // If min and max is fliped
            if (minGameNumber > maxGameNumber)
            {
                int temp = maxGameNumber;
                maxGameNumber = minGameNumber;
                minGameNumber = temp;
            }

            for (int i = 0; i < epochs; i++)
            {
                Console.WriteLine($"=====>{epochs}<=====");
                for (int j = minGameNumber; j <= maxGameNumber; j++)
                {
                    TrainModelAt(net, j);
                }
            }
        }

        public static void TrainModelWithData(NeuralNetwork net, int epochs)
        {
            string path = Directory.GetCurrentDirectory() + @"\..\..\Model";
            string dataPath = Directory.GetCurrentDirectory() + @"\..\..\Data";
            OPSData data = OPSData.GetData(dataPath, "data");
            var games = data.games;

            for (int j = 0; j < epochs; j++)
            {
                foreach (OPSGame value in games.Values)
                {
                    Console.WriteLine($"Training at {value.GameNumber}");
                    net.FeedForward(ToFloatArray(value.PlayerOPS));
                    net.BackProp(ToFloatArray(value.TeamScores));

                    net.SaveModel(path);
                    data.SaveData(dataPath, "data");

                    Console.WriteLine("Train sucessful!");
                }
            }
        }


        public static void TestModel(NeuralNetwork net, double[] ops)
        {
            float[] array = ToFloatArray(ops);

            Console.WriteLine(net.FeedForward(array)[0] + ", " + net.FeedForward(array)[1]);
        }

        public static void TestModel(NeuralNetwork net, int gameNumber)
        {
            OPSGame game = Parser.ParseUrl(gameNumber);
            if (game == null)
            {
                Console.WriteLine("Empty Page");
                return;
            }

            Console.WriteLine("Expected outcome:");
            Console.WriteLine(game.TeamScores[0] + ", " + game.TeamScores[1]);

            Console.WriteLine("Perdicted outcome:");
            Console.WriteLine(net.FeedForward(ToFloatArray(game.PlayerOPS))[0] + ", " + net.FeedForward(ToFloatArray(game.PlayerOPS))[1]);
        }

        public static void GetDataThrough(int minGameNumber, int maxGameNumber)
        {
            string dataPath = Directory.GetCurrentDirectory() + @"\..\..\Data";

            // If min and max is fliped
            if (minGameNumber > maxGameNumber)
            {
                int temp = maxGameNumber;
                maxGameNumber = minGameNumber;
                minGameNumber = temp;
            }

            Console.WriteLine("=========>Starting to collect data<=========");
            for (int i = maxGameNumber; i >= minGameNumber; i--)
            {
                Console.WriteLine($"Getting game data for {i}...");
                OPSData data = OPSData.GetData(dataPath, "data");

                // Check if there is no game data and is not empty game
                if (data.GetGame(i) == null && !data.emptyGames.Contains(i))
                {
                    // Parse Game and update it to the data
                    OPSGame game = Parser.ParseUrl(i);

                    if (game != null)
                    {
                        data.AddGame(game);
                        Console.WriteLine(" > Adding game stats.");
                    }
                    else
                    {
                        data.AddEmptyGame(i);
                        Console.WriteLine(" > Adding empty game");
                    }

                    data.SaveData(dataPath, "data");
                }
                Console.WriteLine($"Data updated for {i}...");
            }
        }

        static float[] ToFloatArray(double[] arr)
        {
            if (arr == null)
                return null;
            int n = arr.Length;
            float[] ret = new float[n];
            for (int i = 0; i < n; i++)
            {
                ret[i] = (float)arr[i];
            }
            return ret;
        }

        static float[] ToFloatArray(int[] arr)
        {
            if (arr == null)
                return null;
            int n = arr.Length;
            float[] ret = new float[n];
            for (int i = 0; i < n; i++)
            {
                ret[i] = (float)arr[i];
            }
            return ret;
        }
    }
}
