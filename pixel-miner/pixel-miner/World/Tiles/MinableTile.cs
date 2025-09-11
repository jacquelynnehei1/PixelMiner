using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixel_miner.World.Tiles
{
    public abstract class MinableTile : Tile
    {
        public MinableTile(GridPosition position) : base(position) { }

        public override bool CanMoveTo => false;

        public virtual bool IsMinable => true;
        public abstract float MiningTime { get; }

        public abstract ResourceDrop Mine();
    }
}