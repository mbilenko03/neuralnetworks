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
            

            //OPSPredict.TestModel(NeuralNetwork.LoadModel("test", path), 347830);

           

            //OPSPredict.GetDataThrough(347740, 526517);
            // Fix around 347869 
            //getting empty when there is data
            //figure out exceptions stuff
            // make sure empty data is updated when you fix parsing

            OPSPredict.GetDataThrough(449037, 449037);

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
