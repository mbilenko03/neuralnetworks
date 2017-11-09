using NeuralNetworks;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;

using System.Threading.Tasks;

namespace OPSPredicter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + @"\..\..\Model";

            // TODO fix neural network (no change in results from inputs)
            // TODO fix dynamic content on old games (ex: 245094)

            /* 
             * 244519 to 526517
             * OPSPredict.GetDataThrough(526517, 244519);
             * OPSPredict.TrainModelWithData(net, 10);
             * NeuralNetwork.LoadModel("test", path)
             * OPSPredict.TestModel(NeuralNetwork.LoadModel("test", path), 526516);
             */

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
