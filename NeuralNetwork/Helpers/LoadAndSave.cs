using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    static class LoadAndSave
    {
        public static void SaveModel(Object item, string name, string path)
        {
            string fileName = Path.Combine(path, name + ".json");
            using (var file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, item);
                file.Close();
            }
        }
        public static Object LoadModel(Object item, string name, string path)
        {
            string fileName = Path.Combine(path, name + ".json");

            using (var file = File.OpenText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (Object)serializer.Deserialize(file, typeof(Object));
            }
        }
    }
}
