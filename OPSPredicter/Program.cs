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

            //Train(244519, 526517, 10);

            //OPSPredict.TestModel(NeuralNetwork.LoadModel("test", path), 244519);


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
