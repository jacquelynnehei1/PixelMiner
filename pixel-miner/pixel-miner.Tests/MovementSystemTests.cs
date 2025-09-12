using pixel_miner.Components.Gameplay;
using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.World;
using pixel_miner.World.Tiles;
using Xunit;

namespace pixel_miner.Tests
{
    [Collection("GameManager Collection")]
    public class MovementSystemTests : IDisposable
    {
        private GameObject? testCamera;

        public MovementSystemTests()
        {
            // Set up camera for each test
            testCamera = TestCameraSetup.CreateTestCamera();
        }

        public void Dispose()
        {
            // Clean up camera after each test
            TestCameraSetup.CleanupCameras();
            if (testCamera != null)
            {
                testCamera.Destroy();
            }
        }

        private (GameObject playerObject, Player player, MovementSystem movementSystem, Board board) CreateTestSetup()
        {
            var board = new Board();
            board.InitializeGrid(20);

            var playerObject = new GameObject("TestPlayer");
            var player = playerObject.AddComponent<Player>();
            var movementSystem = playerObject.AddComponent<MovementSystem>();

            // Use the board's actual top row index for spawn position
            var spawnPosition = new GridPosition(0, board.GetTopRowIndex());
            player.Initialize(board, maxFuel: 100, spawnPosition);
            movementSystem.Initialize(board, player);

            playerObject.Start();

            return (playerObject, player, movementSystem, board);
        }

        [Fact]
        public void RequestMove_WithValidMoveAndSufficientFuel_ShouldMovePlayerAndConsumeFuel()
        {
            // Arrange
            var (playerObject, player, movementSystem, board) = CreateTestSetup();
            var direction = new GridPosition(1, 0);

            bool moveEventFired = false;
            GridPosition fromPos = default;
            GridPosition toPos = default;

            movementSystem.OnPlayerMoved += (from, to) =>
            {
                moveEventFired = true;
                fromPos = from;
                toPos = to;
            };

            var initialPosition = player.GridPosition;
            var expectedTargetPosition = initialPosition + direction;
            var initialFuel = player.CurrentFuel;

            // Act
            movementSystem.RequestMove(direction);

            // Assert
            Assert.True(moveEventFired);
            Assert.Equal(initialPosition, fromPos);
            Assert.Equal(expectedTargetPosition, toPos);
            Assert.Equal(expectedTargetPosition, player.GridPosition);
            // Grass tiles have 0 fuel cost, so fuel shouldn't change
            Assert.Equal(initialFuel, player.CurrentFuel);
        }

        [Fact]
        public void RequestMove_WithInsufficientFuel_ShouldNotMoveAndFireOutOfFuelEvent()
        {
            // Arrange
            var (playerObject, player, movementSystem, board) = CreateTestSetup();
            
            // Create a custom scenario: Place player on a surface tile, then manually create 
            // an empty tile next to them that costs fuel (simulate a cleared underground area)
            var surfacePosition = new GridPosition(0, board.GetTopRowIndex());
            player.SetPosition(surfacePosition);
            
            var targetDirection = new GridPosition(1, 0);
            var targetPosition = surfacePosition + targetDirection;
            
            // Replace the target tile with an EmptyTile that costs fuel to simulate
            // moving through a cleared underground area that still costs fuel
            var emptyTileWithFuelCost = new EmptyTileWithFuelCost(targetPosition);
            board.SetTile(targetPosition, emptyTileWithFuelCost);
            
            // Verify the target tile setup
            var targetTile = board.GetTile(targetPosition);
            Assert.NotNull(targetTile);
            Assert.True(targetTile.CanMoveTo, "Target tile should allow movement");
            Assert.True(targetTile.FuelCost > 0, $"Target tile should cost fuel but costs {targetTile.FuelCost}");
            
            // Verify move validation would succeed
            var moveResult = board.ValidateMove(surfacePosition, targetDirection);
            Assert.True(moveResult.IsValid, $"Move should be valid but got error: {moveResult.ErrorMessage}");
            Assert.True(moveResult.FuelCost > 0, $"Move should cost fuel but costs {moveResult.FuelCost}");
            
            // Consume all fuel
            player.TryConsumeFuel(100);
            Assert.Equal(0, player.CurrentFuel);

            bool outOfFuelEventFired = false;
            bool moveEventFired = false;

            movementSystem.OnOutOfFuel += () => outOfFuelEventFired = true;
            movementSystem.OnPlayerMoved += (from, to) => moveEventFired = true;

            var initialPosition = player.GridPosition;

            // Act
            movementSystem.RequestMove(targetDirection);

            // Assert
            Assert.True(outOfFuelEventFired, "OnOutOfFuel event should have fired when trying to move with 0 fuel to a tile that costs fuel");
            Assert.False(moveEventFired);
            Assert.Equal(initialPosition, player.GridPosition);
        }

        // Helper class for testing - an empty tile that costs fuel to move through
        private class EmptyTileWithFuelCost : Tile
        {
            public EmptyTileWithFuelCost(GridPosition position) : base(position) { }
            public override int FuelCost => 5; // Costs fuel to move through
            public override bool CanMoveTo => true; // But allows movement
        }

        [Fact]
        public void RequestMove_ToInvalidPosition_ShouldNotMoveAndFireBlockedEvent()
        {
            // Arrange
            var (playerObject, player, movementSystem, board) = CreateTestSetup();

            bool blockedEventFired = false;
            bool moveEventFired = false;
            string? blockedMessage = null;

            movementSystem.OnMoveBlocked += (message) =>
            {
                blockedEventFired = true;
                blockedMessage = message;
            };

            movementSystem.OnPlayerMoved += (from, to) => moveEventFired = true;

            var initialPosition = player.GridPosition;

            // Act - try to move way outside the grid
            movementSystem.RequestMove(new GridPosition(100, 100));

            // Assert
            Assert.True(blockedEventFired);
            Assert.False(moveEventFired);
            Assert.NotNull(blockedMessage);
            Assert.Equal(initialPosition, player.GridPosition);
        }

        [Fact]
        public void CanPlayerMove_WithValidMoveAndFuel_ShouldReturnTrue()
        {
            // Arrange
            var (playerObject, player, movementSystem, board) = CreateTestSetup();

            // Act
            bool canMove = movementSystem.CanPlayerMove(new GridPosition(1, 0));

            // Assert
            Assert.True(canMove);
        }
    }
}