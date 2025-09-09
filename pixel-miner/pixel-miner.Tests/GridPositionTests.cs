using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class GridPositionTests
    {
        [Fact]
        public void Addition_ShouldAddCoordinatesCorrectly()
        {
            // Arrange
            var pos1 = new GridPosition(1, 2);
            var pos2 = new GridPosition(3, 4);

            var expectedX = pos1.X + pos2.X;
            var expectedY = pos1.Y + pos2.Y;

            // Act
            var result = pos1 + pos2;

            // Assert
            Assert.Equal(expectedX, result.X);
            Assert.Equal(expectedY, result.Y);
        }

        [Fact]
        public void Equals_ShouldReturnTrueForSameCoordinates()
        {
            // Arrange
            var pos1 = new GridPosition(5, 10);
            var pos2 = new GridPosition(5, 10);

            // Act
            var result = pos1.Equals(pos2);

            // Assert
            Assert.True(result);
            Assert.Equal(pos1, pos2);
        }

        [Fact]
        public void ToString_ShouldReturnExpectedFormat()
        {
            // Arrange
            var pos = new GridPosition(-2, 7);

            // Act
            var result = pos.ToString();

            // Assert
            Assert.Equal("(-2, 7)", result);
        }
    }
}