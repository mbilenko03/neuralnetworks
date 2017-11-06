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
            NeuralNetwork net = new NeuralNetwork(new int[] {18, 29, 29, 2}, modelName);
            return net;
        }

        public static void TrainModelAt(NeuralNetwork net, int gameNumber)
        {
            string path = Directory.GetCurrentDirectory() + "/Model";

            OPSGame game = Parser.ParseUrl(gameNumber);
            net.FeedForward(ToFloatArray(game.PlayerOPS));
            net.BackProp(ToFloatArray(game.TeamScores));
            
            net.SaveModel(path);
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
                for (int j = minGameNumber; j <= maxGameNumber; j++)
                {
                    TrainModelAt(net, j);
                }
            }
        }

        public static void TestModel(NeuralNetwork net, double[] ops)
        {
            float[] array = ToFloatArray(ops);
            
            Console.WriteLine(net.FeedForward(array)[0] + ", " + net.FeedForward(array)[1]);
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