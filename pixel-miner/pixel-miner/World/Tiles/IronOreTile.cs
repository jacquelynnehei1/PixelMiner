using pixel_miner.World.Enums;

namespace pixel_miner.World.Tiles
{
    public class IronOreTile : MinableTile
    {
        public IronOreTile(GridPosition position) : base(position) {}

        public override int FuelCost => 8;
        public override float MiningTime => 1.0f;

        public override ResourceDrop Mine()
        {
            return new ResourceDrop(ResourceType.Iron, 2);
        }
    }
}