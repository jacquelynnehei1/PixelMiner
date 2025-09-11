using pixel_miner.Core;
using pixel_miner.World;

namespace pixel_miner.Tests
{
    public class BoardTests
    {
        private Board CreateTestBoard()
        {
            var gameObject = new GameObject("TestBoard");
            var board = gameObject.AddComponent<Board>();
            board.InitializeGrid(20);
            return board;
        }

        [Fact]
        public void GetTile_WithValidPosition_ShouldReturnTile()
        {
            // Arrange
            var board = CreateTestBoard();
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
            var board = CreateTestBoard();
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
            var board = CreateTestBoard();
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
            var board = CreateTestBoard();
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
            var board = CreateTestBoard();
            var gridPos = new GridPosition(2, 3);
            
            // Act
            var worldPos = board.GridToWorldPosition(gridPos);
            
            // Assert
            Assert.Equal(2 * board.TileSize, worldPos.X);
            Assert.Equal(3 * board.TileSize, worldPos.Y);
        }

        [Fact]
        public void HasTile_WithValidPosition_ShouldReturnTrue()
        {
            // Arrange
            var board = CreateTestBoard();
            var position = new GridPosition(0, 0);

            // Act
            bool hasTile = board.HasTile(position);

            // Assert
            Assert.True(hasTile);
        }

        [Fact]
        public void HasTile_WithInvalidPosition_ShouldReturnFalse()
        {
            // Arrange
            var board = CreateTestBoard();
            var invalidPosition = new GridPosition(100, 100);

            // Act
            bool hasTile = board.HasTile(invalidPosition);

            // Assert
            Assert.False(hasTile);
        }
    }
}