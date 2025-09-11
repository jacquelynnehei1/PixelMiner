using pixel_miner.World.Enums;

namespace pixel_miner.World
{
    public class ResourceDrop
    {
        public ResourceType Type { get; }
        public int Amount { get; }

        public ResourceDrop(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}