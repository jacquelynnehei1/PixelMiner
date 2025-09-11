using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;

namespace pixel_miner.World.Tiles
{
    public abstract class Tile
    {
        public GridPosition Position { get; set; }
        public virtual int FuelCost => 1;
        public virtual bool CanMoveTo => true;

        public Tile(GridPosition position)
        {
            Position = position;
        }
    }
}