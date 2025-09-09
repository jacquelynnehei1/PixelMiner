using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class BoardTests
    {
        [Fact]
        public void GetTile_WithValidPosition_ShouldReturnTile()
        {
            // Arrange
            var board = new Board();
            var position = new GridPosition(0, 0);

            // Act
            var tile = board.GetTile(position);

            // Assert
            Assert.NotNull(tile);
            Assert.Equal(position, tile.Position);
        }

        [Fact]
        public void GetTile_WithInvalidPosition_ShouldReturnNull()
        {
            // Arrange
            var board = new Board();
            var invalidPosition = new GridPosition(100, 100); // Outside grid bounds

            // Act
            var tile = board.GetTile(invalidPosition);

            // Assert
            Assert.Null(tile);
        }

        [Fact]
        public void ValidateMove_WithValidMove_ShouldReturnValidResult()
        {
            // Arrange
            var board = new Board();
            var from = new GridPosition(0, 0);
            var direction = new GridPosition(1, 0);

            // Act
            var result = board.ValidateMove(from, direction);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(new GridPosition(1, 0), result.TargetPosition);
            Assert.True(result.FuelCost > 0);
        }

        [Fact]
        public void ValidateMove_ToInvalidPosition_ShouldReturnInvalidResult()
        {
            // Arrange
            var board = new Board();
            var from = new GridPosition(9, 9);
            var direction = new GridPosition(10, 10); // Would go outside grid

            // Act
            var result = board.ValidateMove(from, direction);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessage);
        }
        
        [Fact]
        public void GridToWorldPosition_ShouldConvertCorrectly()
        {
            // Arrange
            var board = new Board();
            var gridPos = new GridPosition(2, 3);
            
            // Act
            var worldPos = board.GridToWorldPosition(gridPos);
            
            // Assert
            Assert.Equal(2 * board.TileSize, worldPos.X);
            Assert.Equal(3 * board.TileSize, worldPos.Y);
        }
    }
}