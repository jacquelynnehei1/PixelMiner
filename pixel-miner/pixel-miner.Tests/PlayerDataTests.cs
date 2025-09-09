using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Data;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class PlayerDataTests
    {
        [Fact]
        public void TryConsumeFuel_WithSufficientFuel_ShouldReturnTrueAndDecreaseFuel()
        {
            // Arrange
            var playerData = new PlayerData();
            int initialFuel = playerData.CurrentFuel;

            // Act
            bool result = playerData.TryConsumeFuel(10);

            // Assert
            Assert.True(result);
            Assert.Equal(initialFuel - 10, playerData.CurrentFuel);
        }

        [Fact]
        public void TryConsumeFuel_WithInsufficientFuel_ShouldReturnFalseAndNotChangeFuel()
        {
            // Arrange
            var playerData = new PlayerData();
            int initialFuel = playerData.CurrentFuel;

            // Act
            bool result = playerData.TryConsumeFuel(initialFuel + 10);

            // Assert
            Assert.False(result);
            Assert.Equal(initialFuel, playerData.CurrentFuel);
        }

        [Fact]
        public void AddFuel_ShouldNotExceedMaxFuel()
        {
            // Arrange
            var playerData = new PlayerData();
            playerData.TryConsumeFuel(50);

            // Act
            playerData.AddFuel(200);

            // Assert
            Assert.Equal(playerData.MaxFuel, playerData.CurrentFuel);
        }

        [Fact]
        public void SetMaxFuel_ShouldClampCurrentFuelIfExceedsNewMax()
        {
            // Arrange
            var playerData = new PlayerData();

            // Act
            playerData.SetMaxFuel(50);

            // Assert
            Assert.Equal(50, playerData.MaxFuel);
            Assert.Equal(50, playerData.CurrentFuel);
        }

        [Fact]
        public void SetPosition_ShouldUpdatePosition()
        {
            // Arrange
            var playerData = new PlayerData();
            var newPosition = new GridPosition(5, 10);

            // Act
            playerData.SetPosition(newPosition);

            // Assert
            Assert.Equal(newPosition, playerData.GridPosition);
        }
    }
}