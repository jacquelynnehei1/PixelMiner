using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixel_miner.World
{
    public class Tile
    {
        public GridPosition Position { get; set; }
        public int FuelCost { get; set; } = 1;
        public bool CanMoveTo { get; set; } = true;

        public Tile(GridPosition position, int fuelCost = 1, bool canMoveTo = true)
        {
            Position = position;
            FuelCost = fuelCost;
            CanMoveTo = canMoveTo;
        }
    }
}