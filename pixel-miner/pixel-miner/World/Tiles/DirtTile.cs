using pixel_miner.World.Enums;

namespace pixel_miner.World.Tiles
{
    public class DirtTile : MinableTile
    {
        public DirtTile(GridPosition position) : base(position) { }

        public override int FuelCost => 2;
        public override float MiningTime => 0.1f;

        public override ResourceDrop Mine()
        {
            return new ResourceDrop(ResourceType.Stone, 1);
        }
    }
}