﻿using System.Collections.Generic;
using System.Numerics;

namespace PPather.Data
{
    public class LineArgs
    {
        public string Name { get; set; }
        public List<Vector3> Spots { get; set; }
        public int Colour { get; set; }
        public int MapId { get; set; }

        public LineArgs(string name, List<Vector3> spots, int colour, int mapId)
        {
            Name = name;
            Spots = spots;
            Colour = colour;
            MapId = mapId;
        }
    }
}