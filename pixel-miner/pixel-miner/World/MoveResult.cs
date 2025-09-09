using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixel_miner.World
{
    public struct MoveResult
    {
        public bool IsValid { get; set; }
        public GridPosition TargetPosition { get; set; }
        public int FuelCost { get; set; }
        public string? ErrorMessage { get; set; }

        public static MoveResult Valid(GridPosition target, int fuelCost)
        {
            return new MoveResult
            {
                IsValid = true,
                TargetPosition = target,
                FuelCost = fuelCost
            };
        }

        public static MoveResult Invalid(string errorMessage)
        {
            return new MoveResult
            {
                IsValid = false,
                ErrorMessage = errorMessage
            };
        }
    }
}