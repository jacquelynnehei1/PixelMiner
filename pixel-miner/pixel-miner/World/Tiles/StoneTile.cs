using pixel_miner.World.Enums;

namespace pixel_miner.World.Tiles
{
    public class StoneTile : MinableTile
    {
        public StoneTile(GridPosition position) : base(position) {}

        public override int FuelCost => 5;
        public override float MiningTime => 0.5f;

        public override ResourceDrop Mine()
        {
            return new ResourceDrop(ResourceType.Stone, 3);
        }
    }
}