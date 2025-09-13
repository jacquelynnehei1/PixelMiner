using pixel_miner.Components.Gameplay;
using pixel_miner.Core;
using pixel_miner.Utils;
using pixel_miner.World;
using pixel_miner.World.Tiles;
using pixel_miner.World.Enums;

namespace pixel_miner.Components.Movement
{
    public class MovementSystem : Component
    {
        private Board board = null!;
        private Player playerData = null!;

        public event Action<GridPosition, GridPosition>? OnPlayerMoved;
        public event Action<string>? OnMoveBlocked;
        public event Action? OnOutOfFuel;
        public event Action<string>? OnMiningFailed;
        public event Action<GridPosition, ResourceDrop?>? OnResourceMined;

        public void Initialize(Board gameBoard, Player player)
        {
            board = gameBoard;
            playerData = player;
        }

        public void RequestMove(GridPosition direction, bool bypassFuelCheck = false)
        {
            var playerMover = playerData.GameObject.GetComponent<PlayerMover>();
            if (playerMover != null)
            {
                playerMover.SetFacingDirection(direction);
            }

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
            board.CheckAndExpandGrid(moveResult.TargetPosition);
            OnPlayerMoved?.Invoke(oldPosition, moveResult.TargetPosition);
        }

        public void RequestMine(GridPosition targetPosition)
        {
            var targetTile = board.GetTile(targetPosition);

            if (targetTile == null)
            {
                OnMiningFailed?.Invoke("No tile to mine");
                return;
            }

            if (targetTile is not MinableTile minableTile)
            {
                OnMiningFailed?.Invoke("Tile cannot be mined");
                return;
            }

            if (!playerData.HasFuel(targetTile.FuelCost))
            {
                OnMiningFailed?.Invoke("Not enough fuel to mine");
                return;
            }

            // TODO: Add mining time delay here later
            // For now, mine instantly

            playerData.TryConsumeFuel(targetTile.FuelCost);

            if (board.TryMineTile(targetPosition, out ResourceDrop? resource) && resource != null)
            {
                bool resourceCollected = playerData.TryCollectResource(resource);

                if (resourceCollected)
                {
                    OnResourceMined?.Invoke(targetPosition, resource);
                    Console.WriteLine($"Mined and collected {resource.Amount} {resource.Type} at {targetPosition}");
                }
                else
                {
                    OnResourceMined?.Invoke(targetPosition, resource);
                    Console.WriteLine($"Mined {resource.Amount} {resource.Type} at {targetPosition} but could not collect (inventory full)");
                }
            }

            OnMiningFailed?.Invoke("Mining failed");
        }

        public bool CanPlayerMove(GridPosition direction)
        {
            var moveResult = board.ValidateMove(playerData.GridPosition, direction);
            return moveResult.IsValid && playerData.HasFuel(moveResult.FuelCost);
        }

        public void ResetMovement()
        {
            var currentPosition = playerData.GridPosition;
            var startPosition = playerData.RespawnPosition;

            Console.WriteLine($"Attempting return from {currentPosition} to {startPosition}");

            if (!currentPosition.Equals(startPosition))
            {
                // Use the empty tiles network pathfinding
                var path = Pathfinder.FindPathThroughEmptyTiles(currentPosition, startPosition, board);
                
                if (path.Count == 0)
                {
                    Console.WriteLine("ERROR: No path found through mined tunnels back to surface!");
                    Console.WriteLine("This should not happen unless there's a bug in the pathfinding.");
                    return;
                }

                Console.WriteLine($"Found path through empty tiles with {path.Count} steps");
                
                // Execute the path step by step
                while (path.Count > 0)
                {
                    var nextStep = path.Dequeue();
                    var direction = nextStep - playerData.GridPosition;
                    
                    Console.WriteLine($"Return step: moving to {nextStep}, direction: {direction}");
                    RequestMove(direction, bypassFuelCheck: true);
                }
                
                // Verify we reached the target
                if (playerData.GridPosition.Equals(startPosition))
                {
                    Console.WriteLine("Successfully returned to surface via mined tunnels");
                }
                else
                {
                    Console.WriteLine($"WARNING: Return incomplete. Expected {startPosition}, got {playerData.GridPosition}");
                }
            }
            else
            {
                Console.WriteLine("Already at starting position");
            }
        }
    }
}