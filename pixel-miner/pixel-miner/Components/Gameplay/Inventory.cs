using pixel_miner.Core;
using pixel_miner.World.Enums;

namespace pixel_miner.Components.Gameplay
{
    public class Inventory : Component
    {
        private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
        private Dictionary<ResourceType, int> maxCapacity = new Dictionary<ResourceType, int>();

        public event Action<ResourceType, int>? OnResourceAdded;
        public event Action<ResourceType, int>? OnResourceRemoved;
        public event Action<ResourceType>? OnInventoryFull;

        public override void Start()
        {
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            foreach (ResourceType resourceType in Enum.GetValues<ResourceType>())
            {
                if (resourceType == ResourceType.Fuel) continue;

                resources[resourceType] = 0;
                maxCapacity[resourceType] = GetDefaultCapacity(resourceType);
            }
        }

        private int GetDefaultCapacity(ResourceType type)
        {
            return type switch
            {
                ResourceType.Stone => 50,
                ResourceType.Iron => 20,
                ResourceType.Gems => 10,
                _ => 10
            };
        }

        public int TryAddResource(ResourceType resourceType, int amount)
        {
            if (amount <= 0) return 0;

            int currentAmount = GetResourceCount(resourceType);
            int maxAmount = GetMaxCapacity(resourceType);
            int availableSpace = maxAmount - currentAmount;
            int amountToAdd = Math.Min(amount, availableSpace);

            if (amountToAdd > 0)
            {
                resources[resourceType] = currentAmount + amountToAdd;
                OnResourceAdded?.Invoke(resourceType, amountToAdd);

                Console.WriteLine($"Added {amountToAdd} {resourceType} to inventory. Total: {resources[resourceType]}/{maxAmount}");
            }

            if (amountToAdd < amount)
            {
                OnInventoryFull?.Invoke(resourceType);
                Console.WriteLine($"Inventory full! Could not add {amount - amountToAdd} {resourceType}");
            }

            return amountToAdd;
        }

        public int TryRemoveResource(ResourceType resourceType, int amount)
        {
            if (amount <= 0) return 0;

            int currentAmount = GetResourceCount(resourceType);
            int amountToRemove = Math.Min(amount, currentAmount);

            if (amountToRemove > 0)
            {
                resources[resourceType] = currentAmount - amountToRemove;
                OnResourceRemoved?.Invoke(resourceType, amountToRemove);

                Console.WriteLine($"Removed {amountToRemove} {resourceType} from inventory. Remaining: {resources[resourceType]}");
            }

            return amountToRemove;
        }

        public bool CanAddResource(ResourceType resourceType, int amount)
        {
            int currentAmount = GetResourceCount(resourceType);
            int maxAmount = GetMaxCapacity(resourceType);
            return (currentAmount + amount) <= maxAmount;
        }

        public int GetMaxCapacity(ResourceType resourceType)
        {
            return maxCapacity.ContainsKey(resourceType) ? maxCapacity[resourceType] : 0;
        }

        public void SetMaxCapacity(ResourceType resourceType, int newCapacity)
        {
            maxCapacity[resourceType] = Math.Max(0, newCapacity);

            int currentAmount = GetResourceCount(resourceType);
            if (currentAmount > newCapacity)
            {
                int excess = currentAmount - newCapacity;
                TryRemoveResource(resourceType, excess);
            }
        }

        public int GetResourceCount(ResourceType resourceType)
        {
            return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
        }

        public Dictionary<ResourceType, int> GetAllResources()
        {
            return new Dictionary<ResourceType, int>(resources);
        }

        public int GetAvailableSpace(ResourceType resourceType)
        {
            return GetMaxCapacity(resourceType) - GetResourceCount(resourceType);
        }

        public bool IsInventoryFull()
        {
            foreach (var kvp in resources)
            {
                if (kvp.Value < GetMaxCapacity(kvp.Key))
                {
                    return false;
                }
            }

            return true;
        }

        public void ClearInventory()
        {
            foreach (var resourceType in resources.Keys)
            {
                resources[resourceType] = 0;
            }

            Console.WriteLine("Inventory cleared");
        }

        public int GetTotalItemCount()
        {
            return resources.Values.Sum();
        }
    }
}