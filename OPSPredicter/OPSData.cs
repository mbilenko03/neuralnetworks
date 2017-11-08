using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPSPredicter
{
    class OPSData
    {
        public Dictionary<int, OPSGame> games = new Dictionary<int, OPSGame>();

        public void AddGame(OPSGame game)
        {
            if (game != null && (games == null || !games.ContainsValue(game)))
                games.Add(game.GameNumber, game);
        }

        public OPSGame GetGame(int gameNumber)
        {
            if (games == null || !games.ContainsKey(gameNumber))
                return null;
            else
                return games[gameNumber];
        }

        public void SaveData(string path, string name)
        {
            string fileName = Path.Combine(path, name + ".json");
            using (var file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
                file.Close();
            }
        }

        public static OPSData GetData(string path, string name)
        {
            string fileName = Path.Combine(path, name + ".json");

            try
            {
                using (var file = File.OpenText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (OPSData)serializer.Deserialize(file, typeof(OPSData));
                }
            }
            catch (FileNotFoundException)
            {
                return new OPSData();
            }
        }
    }
}
