using pixel_miner.World.Enums;

namespace pixel_miner.World.Tiles
{
    public class PreciousGemTile : MinableTile
    {
        public PreciousGemTile(GridPosition position) : base(position) {}

        public override int FuelCost => 15;
        public override float MiningTime => 2.0f;

        public override ResourceDrop Mine()
        {
            return new ResourceDrop(ResourceType.Gems, 1);
        }
    }
}