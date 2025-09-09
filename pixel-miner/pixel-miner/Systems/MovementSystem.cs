using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Data;
using pixel_miner.World;

namespace pixel_miner.Systems
{
    public class MovementSystem
    {
        private Board board;
        private PlayerData playerData;

        public event Action<GridPosition, GridPosition>? OnPlayerMoved;
        public event Action<string>? OnMoveBlocked;
        public event Action? OnOutOfFuel;

        public MovementSystem(Board gameBoard, PlayerData player)
        {
            board = gameBoard;
            playerData = player;
        }

        public void RequestMove(GridPosition direction, bool bypassFuelCheck = false)
        {
            var moveResult = board.ValidateMove(playerData.GridPosition, direction);

            if (!moveResult.IsValid)
            {
                OnMoveBlocked?.Invoke(moveResult.ErrorMessage ?? "Invalid move");
                return;
            }

            if (!bypassFuelCheck && !playerData.HasFuel(moveResult.FuelCost))
            {
                OnOutOfFuel?.Invoke();
                return;
            }

            var oldPosition = playerData.GridPosition;

            if (!bypassFuelCheck)
            {
                playerData.TryConsumeFuel(moveResult.FuelCost);
            }

            playerData.SetPosition(moveResult.TargetPosition);
            OnPlayerMoved?.Invoke(oldPosition, moveResult.TargetPosition);
        }

        public bool CanPlayerMove(GridPosition direction)
        {
            var moveResult = board.ValidateMove(playerData.GridPosition, direction);
            return moveResult.IsValid && playerData.HasFuel(moveResult.FuelCost);
        }

        public GridPosition GetPlayerPosition()
        {
            return playerData.GridPosition;
        }

        public int GetPlayerFuel()
        {
            return playerData.CurrentFuel;
        }
    }
}