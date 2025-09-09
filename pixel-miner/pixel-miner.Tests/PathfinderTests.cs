using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Utils;
using pixel_miner.Utils.Enums;
using pixel_miner.World;
using Xunit;

namespace pixel_miner.Tests
{
    public class PathfinderTests
    {
        [Fact]
        public void CalculatePath_SamePosition_ShouldReturnEmptyQueue()
        {
            // Arrange
            var from = new GridPosition(5, 5);
            var to = new GridPosition(5, 5);

            // Act
            var path = Pathfinder.CalculatePath(from, to);

            // Assert
            Assert.Empty(path);
        }

        [Fact]
        public void CalculatePath_AdjecentHorizontal_ShouldReturnSingleMove()
        {
            // Arrange
            var from = new GridPosition(0, 0);
            var to = new GridPosition(1, 0);

            // Act
            var path = Pathfinder.CalculatePath(from, to);

            // Assert
            Assert.Single(path);
            Assert.Equal(new GridPosition(1, 0), path.Dequeue());
        }

        [Fact]
        public void CalculatePath_AdjacentVertical_ShouldReturnSingleMove()
        {
            // Arrange
            var from = new GridPosition(0, 0);
            var to = new GridPosition(0, 1);

            // Act
            var path = Pathfinder.CalculatePath(from, to);

            // Assert
            Assert.Single(path);
            Assert.Equal(new GridPosition(0, 1), path.Dequeue());
        }

        [Fact]
        public void CalculatePath_HorizontalFirst_ShouldMoveHorizontallyThenVertically()
        {
            // Arrange
            var from = new GridPosition(0, 0);
            var to = new GridPosition(3, 2);

            // Act
            var path = Pathfinder.CalculatePath(from, to, PathfindingStrategy.ManhattanHorizontalFirst);

            // Assert
            Assert.Equal(5, path.Count);

            // Should move horizontally first: (1, 0), (2, 0), (3, 0)
            Assert.Equal(new GridPosition(1, 0), path.Dequeue());
            Assert.Equal(new GridPosition(2, 0), path.Dequeue());
            Assert.Equal(new GridPosition(3, 0), path.Dequeue());

            // Then vertically: (3, 1), (3, 2)
            Assert.Equal(new GridPosition(3, 1), path.Dequeue());
            Assert.Equal(new GridPosition(3, 2), path.Dequeue());
        }

        [Fact]
        public void CalculatePath_VerticalFirst_ShouldMoveVerticallyThenHorizontally()
        {
            // Arrange
            var from = new GridPosition(0, 0);
            var to = new GridPosition(3, 2);

            // Act
            var path = Pathfinder.CalculatePath(from, to, PathfindingStrategy.ManhattanVerticalFirst);

            // Assert
            Assert.Equal(5, path.Count);

            // Should move vertically first (0, 1), (0, 2)
            Assert.Equal(new GridPosition(0, 1), path.Dequeue());
            Assert.Equal(new GridPosition(0, 2), path.Dequeue());

            // Then horizontally (1, 2), (2, 2), (3, 2)
            Assert.Equal(new GridPosition(1, 2), path.Dequeue());
            Assert.Equal(new GridPosition(2, 2), path.Dequeue());
            Assert.Equal(new GridPosition(3, 2), path.Dequeue());
        }

        [Fact]
        public void CalculatePath_NegativeCoordinates_ShouldWork()
        {
            // Arrange
            var from = new GridPosition(-2, -1);
            var to = new GridPosition(1, 1);

            // Act
            var path = Pathfinder.CalculatePath(from, to);

            // Assert
            Assert.Equal(5, path.Count); // 3 horizontal + 2 vertical moves

            // First step should be adjacent to starting position
            var firstStep = path.Dequeue();
            var distance = Pathfinder.CalculateDistance(from, firstStep);
            Assert.Equal(1, distance);
        }

        [Fact]
        public void CalculatePath_AllMovesAreAdjacent()
        {
            // Arrange
            var from = new GridPosition(5, 3);
            var to = new GridPosition(-2, -4);

            // Act
            var path = Pathfinder.CalculatePath(from, to);

            // Assert
            var current = from;
            while (path.Count > 0)
            {
                var next = path.Dequeue();
                var distance = Pathfinder.CalculateDistance(current, next);
                Assert.Equal(1, distance);
                current = next;
            }

            // Should end at target
            Assert.Equal(to, current);
        }

        [Fact]
        public void CalculateDistance_ShouldReturnManhattanDistance()
        {
            // Arrange
            var pos1 = new GridPosition(0, 0);
            var pos2 = new GridPosition(3, 4);

            // Act
            var distance = Pathfinder.CalculateDistance(pos1, pos2);

            // Assert
            Assert.Equal(7, distance); // |3-0| + |4-0| = 7
        }
        
        [Fact]
        public void CalculateDistance_WithNegativeCoordinates_ShouldWork()
        {
            // Arrange
            var pos1 = new GridPosition(-2, -3);
            var pos2 = new GridPosition(1, 2);

            // Act
            var distance = Pathfinder.CalculateDistance(pos1, pos2);

            // Assert
            Assert.Equal(8, distance); // |1-(-2)| + |2-(-3)| = 3 + 5 = 8
        }
    }
}