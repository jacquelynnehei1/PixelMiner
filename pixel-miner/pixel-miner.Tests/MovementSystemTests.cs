using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Components.Gameplay;
using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    [Collection("GameManager Collection")]
    public class MovementSystemTests
    {
        private (GameObject playerObject, Player player, MovementSystem movementSystem, Board board) CreateTestSetup()
        {
            var board = new Board();
            board.InitializeGrid(20);

            var playerObject = new GameObject("TestPlayer");
            var player = playerObject.AddComponent<Player>();
            var movementSystem = playerObject.AddComponent<MovementSystem>();

            player.Initialize(board, maxFuel: 100, new GridPosition(0, 0));
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

            // Act
            movementSystem.RequestMove(direction);

            // Assert
            Assert.True(moveEventFired);
            Assert.Equal(new GridPosition(0, 0), fromPos);
            Assert.Equal(new GridPosition(1, 0), toPos);
            Assert.Equal(new GridPosition(1, 0), player.GridPosition);
            Assert.Equal(99, player.CurrentFuel);
        }

        [Fact]
        public void RequestMove_WithInsufficientFuel_ShouldNotMoveAndFireOutOfFuelEvent()
        {
            // Arrange
            var (playerObject, player, movementSystem, board) = CreateTestSetup();
            
            // Consume all fuel
            player.TryConsumeFuel(100);

            bool outOfFuelEventFired = false;
            bool moveEventFired = false;

            movementSystem.OnOutOfFuel += () => outOfFuelEventFired = true;
            movementSystem.OnPlayerMoved += (from, to) => moveEventFired = true;

            // Act
            movementSystem.RequestMove(new GridPosition(1, 0));

            // Assert
            Assert.True(outOfFuelEventFired);
            Assert.False(moveEventFired);
            Assert.Equal(new GridPosition(0, 0), player.GridPosition);
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

            // Act
            movementSystem.RequestMove(new GridPosition(100, 100));

            // Assert
            Assert.True(blockedEventFired);
            Assert.False(moveEventFired);
            Assert.NotNull(blockedMessage);
            Assert.Equal(new GridPosition(0, 0), player.GridPosition);
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