using NeuralNetworks;
using System;

namespace OPSPredicter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)

        {
            // TODO fix neural network (no change in results from inputs)
            // TODO fix dynamic content on old games (ex: 245094)

            /* 
             * 244519 to 526517
             * v gets data for 2017 season
             * OPSPredict.GetDataThrough(526517, 490102);
             * OPSPredict.TrainModelWithData(net, 10);
             * NeuralNetwork.LoadModel("test", path)
             * OPSPredict.TestModel(NeuralNetwork.LoadModel("test", path), 526516);
             * OPSPredict.TestModel(net, 526517);
             */

            //OPSPredict.GetDataThrough(492523, 490102);


            //NeuralNetwork net = OPSPredict.CreateModel("Model01");
            //OPSPredict.TrainModelWithData(net, 10);

            NeuralNetwork net = OPSPredict.LoadModel("Model01");
            OPSPredict.TestModel(net, 526514);



            Console.ReadLine();
        }

        private static void Train(int min, int max, int epochs)
        {
            NeuralNetwork net = OPSPredict.CreateModel("test");
            OPSPredict.TrainModelThrough(net, min, max, epochs);
            Console.WriteLine("=============Finished Training=============");
        }
    }
}
