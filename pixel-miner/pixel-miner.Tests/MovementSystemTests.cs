using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Data;
using pixel_miner.Systems;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class MovementSystemTests
    {
        [Fact]
        public void RequestMove_WithValidMoveAndSufficientFuel_ShouldMovePlayerAndConsumeFuel()
        {
            // Arrange
            var board = new Board();
            var playerData = new PlayerData();
            var movementSystem = new MovementSystem(board, playerData);
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
            Assert.Equal(new GridPosition(1, 0), playerData.GridPosition);
            Assert.Equal(99, playerData.CurrentFuel);
        }

        [Fact]
        public void RequestMove_WithInsufficientFuel_ShouldNotMoveAndFireOutOfFuelEvent()
        {
            // Arrange
            var board = new Board();

            var playerData = new PlayerData();
            playerData.TryConsumeFuel(100);

            var movementSystem = new MovementSystem(board, playerData);

            bool outOfFuelEventFired = false;
            bool moveEventFired = false;

            movementSystem.OnOutOfFuel += () => outOfFuelEventFired = true;
            movementSystem.OnPlayerMoved += (from, to) => moveEventFired = true;

            // Act
            movementSystem.RequestMove(new GridPosition(1, 0));

            // Assert
            Assert.True(outOfFuelEventFired);
            Assert.False(moveEventFired);
            Assert.Equal(new GridPosition(0, 0), playerData.GridPosition);
        }

        [Fact]
        public void RequestMove_ToInvalidPosition_ShouldNotMoveAndFireBlockedEvent()
        {
            // Arrange
            var board = new Board();
            var playerData = new PlayerData(new GridPosition(0, 0), 100);
            var movementSystem = new MovementSystem(board, playerData);

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
            Assert.Equal(new GridPosition(0, 0), playerData.GridPosition);
        }

        [Fact]
        public void CanPlayerMove_WithValidMoveAndFuel_ShouldReturnTrue()
        {
            // Arrange
            var board = new Board();
            var playerData = new PlayerData();
            var movementSystem = new MovementSystem(board, playerData);

            // Act
            bool canMove = movementSystem.CanPlayerMove(new GridPosition(1, 0));

            // Assert
            Assert.True(canMove);
        }
    }
}