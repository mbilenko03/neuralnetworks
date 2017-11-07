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
            NeuralNetwork net = OPSPredict.CreateModel("test");
            OPSPredict.TrainModelThrough(net, 526417, 526517, 10);
            Console.WriteLine("Finished Training");
            //OPSPredict.TrainModelAt(NeuralNetwork.LoadModel("test", path), 526484);
            //OPSPredict.TestModel(NeuralNetwork.LoadModel("test", path), 526517);

            //Parser.ParseUrl(526484);

            Console.ReadLine();
        }
    }
}
