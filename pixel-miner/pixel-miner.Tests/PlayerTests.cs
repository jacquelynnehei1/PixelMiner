using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Components.Gameplay;
using pixel_miner.Core;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class PlayerTests
    {
        private (GameObject playerObject, Player player, Board board) CreateTestPlayer()
        {
            var board = new Board();
            board.InitializeGrid(20);

            var playerObject = new GameObject("TestPlayer");
            var player = playerObject.AddComponent<Player>();
            
            // Note: We need to add other required components for full functionality
            playerObject.AddComponent<pixel_miner.Components.Movement.MovementSystem>();
            playerObject.AddComponent<pixel_miner.Components.Movement.PlayerMover>();

            player.Initialize(board, maxFuel: 100, new GridPosition(0, 0));
            playerObject.Start();

            return (playerObject, player, board);
        }

        [Fact]
        public void TryConsumeFuel_WithSufficientFuel_ShouldReturnTrueAndDecreaseFuel()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();
            int initialFuel = player.CurrentFuel;

            // Act
            bool result = player.TryConsumeFuel(10);

            // Assert
            Assert.True(result);
            Assert.Equal(initialFuel - 10, player.CurrentFuel);
        }

        [Fact]
        public void TryConsumeFuel_WithInsufficientFuel_ShouldReturnFalseAndNotChangeFuel()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();
            int initialFuel = player.CurrentFuel;

            // Act
            bool result = player.TryConsumeFuel(initialFuel + 10);

            // Assert
            Assert.False(result);
            Assert.Equal(initialFuel, player.CurrentFuel);
        }

        [Fact]
        public void AddFuel_ShouldNotExceedMaxFuel()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();
            player.TryConsumeFuel(50);

            // Act
            player.AddFuel(200);

            // Assert
            Assert.Equal(player.MaxFuel, player.CurrentFuel);
        }

        [Fact]
        public void SetMaxFuel_ShouldClampCurrentFuelIfExceedsNewMax()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();

            // Act
            player.SetMaxFuel(50);

            // Assert
            Assert.Equal(50, player.MaxFuel);
            Assert.Equal(50, player.CurrentFuel);
        }

        [Fact]
        public void SetPosition_ShouldUpdatePosition()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();
            var newPosition = new GridPosition(5, 10);

            // Act
            player.SetPosition(newPosition);

            // Assert
            Assert.Equal(newPosition, player.GridPosition);
        }

        [Fact]
        public void HasFuel_WithSufficientFuel_ShouldReturnTrue()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();

            // Act
            bool hasFuel = player.HasFuel(50);

            // Assert
            Assert.True(hasFuel);
        }

        [Fact]
        public void HasFuel_WithInsufficientFuel_ShouldReturnFalse()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();

            // Act
            bool hasFuel = player.HasFuel(150);

            // Assert
            Assert.False(hasFuel);
        }

        [Fact]
        public void FuelPercentage_ShouldCalculateCorrectly()
        {
            // Arrange
            var (playerObject, player, board) = CreateTestPlayer();
            player.TryConsumeFuel(25); // Should have 75/100 fuel

            // Act
            float percentage = player.FuelPercentage;

            // Assert
            Assert.Equal(0.75f, percentage, precision: 2);
        }
    }
}