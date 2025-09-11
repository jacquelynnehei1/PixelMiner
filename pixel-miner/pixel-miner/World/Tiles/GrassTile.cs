using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixel_miner.World.Tiles
{
    public class GrassTile : Tile
    {
        public GrassTile(GridPosition position) : base(position) { }

        public override int FuelCost => 0;
    }
}