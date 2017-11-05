﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPSPredicter
{
    class OPSGame
    {
        public int GameNumber;
        public int[] PlayerOPS = new int[18];
        public int[] TeamScores = new int[2];

        public OPSGame(int gameNumber, int[] playerOPS, int[] teamScores)
        {
            GameNumber = gameNumber;

            if (playerOPS.Length == 18)
                PlayerOPS = playerOPS;

            if (teamScores.Length == 2)
                TeamScores = teamScores;
        }
    }
}