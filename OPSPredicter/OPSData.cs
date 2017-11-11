using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OPSPredicter
{
    class OPSData
    {
        public Dictionary<int, OPSGame> games = new Dictionary<int, OPSGame>();
        public List<int> emptyGames = new List<int>();

        public void AddGame(OPSGame game)
        {
            if (game != null && (games == null || !games.ContainsValue(game)))
                games.Add(game.GameNumber, game);
        }

        public void AddEmptyGame(int gameNumber)
        {
            if (!games.ContainsKey(gameNumber) || games[gameNumber] == null)
                emptyGames.Add(gameNumber);
        }

        public bool IsGameEmpty(int gameNumber)
        {
            return emptyGames.Contains(gameNumber);  
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
