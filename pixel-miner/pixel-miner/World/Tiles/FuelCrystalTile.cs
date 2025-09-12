using pixel_miner.World.Enums;

namespace pixel_miner.World.Tiles
{
    public class FuelCrystalTile : MinableTile
    {
        public FuelCrystalTile(GridPosition position) : base(position) {}

        public override int FuelCost => 12;
        public override float MiningTime => 1.5f;

        public override ResourceDrop Mine()
        {
            return new ResourceDrop(ResourceType.Fuel, 10);
        }
    }
}