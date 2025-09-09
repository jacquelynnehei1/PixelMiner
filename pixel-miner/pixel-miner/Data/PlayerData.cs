using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.World;

namespace pixel_miner.Data
{
    public class PlayerData
    {
        public GridPosition GridPosition { get; set; }
        public int CurrentFuel { get; set; }
        public int MaxFuel { get; set; }

        public event Action<int>? OnFuelChanged;
        public event Action<GridPosition>? OnPositionChanged;

        public PlayerData(int maxFuel = 100)
        {
            GridPosition = new GridPosition(0, 0);
            MaxFuel = maxFuel;
            CurrentFuel = MaxFuel;
        }

        public PlayerData(GridPosition startPosition, int maxFuel = 100)
        {
            GridPosition = startPosition;
            MaxFuel = maxFuel;
            CurrentFuel = MaxFuel;
        }

        public bool TryConsumeFuel(int amount)
        {
            if (CurrentFuel < amount) return false;

            CurrentFuel = Math.Max(0, CurrentFuel - amount);
            Console.WriteLine($"Fuel Used: {amount}, Current Fuel: {CurrentFuel}");
            OnFuelChanged?.Invoke(CurrentFuel);
            return true;
        }

        public void AddFuel(int amount)
        {
            int oldFuel = CurrentFuel;
            CurrentFuel = Math.Min(MaxFuel, CurrentFuel + amount);

            if (CurrentFuel != oldFuel)
            {
                OnFuelChanged?.Invoke(CurrentFuel);
            }
        }

        public void SetMaxFuel(int maxFuel)
        {
            MaxFuel = maxFuel;

            int oldFuel = CurrentFuel;
            CurrentFuel = Math.Min(CurrentFuel, MaxFuel);

            if (CurrentFuel != oldFuel)
            {
                OnFuelChanged?.Invoke(CurrentFuel);
            }
        }

        public void SetPosition(GridPosition newPosition)
        {
            if (!GridPosition.Equals(newPosition))
            {
                GridPosition = newPosition;
                OnPositionChanged?.Invoke(GridPosition);
            }
        }

        public bool HasFuel(int amount)
        {
            return CurrentFuel >= amount;
        }

        public float FuelPercentage => MaxFuel > 0 ? (float)CurrentFuel / MaxFuel : 0f;
    }
}