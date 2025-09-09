using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.World;
using SFML.System;
using Xunit;

namespace pixel_miner.Tests
{
    public class PlayerMoverTests
    {
        private PlayerMover CreatePlayerMover()
        {
            var gameObject = new GameObject("TestPlayer");
            var mover = gameObject.AddComponent<PlayerMover>();
            gameObject.Start();
            return mover;
        }

        private Board CreateTestBoard()
        {
            return new Board();
        }

        [Fact]
        public void QueueMove_WhenNotMoving_ShouldStartMovingImmediately()
        {
            // Arrange
            var mover = CreatePlayerMover();
            var targetPosition = new Vector2f(32f, 0f);

            // Act
            mover.QueueMove(targetPosition);

            // Assert
            Assert.True(mover.IsMoving);
        }

        [Fact]
        public void QueueMove_WhenMoving_ShouldNotStartUntilCurrentMoveCompletes()
        {
            // Arrange
            var mover = CreatePlayerMover();
            var firstTarget = new Vector2f(32f, 0f);
            var secondTarget = new Vector2f(64f, 0f);

            // Act
            mover.MoveTo(firstTarget); // Start first move
            mover.QueueMove(secondTarget); // Queue second move

            // Assert
            Assert.True(mover.IsMoving);
            Assert.True(mover.HasQueuedMoves());
        }

        [Fact]
        public void MoveTo_ShouldSetTargetAndStartMoving()
        {
            // Arrange
            var mover = CreatePlayerMover();
            var targetPosition = new Vector2f(100f, 50f);

            // Act
            mover.MoveTo(targetPosition);

            // Assert
            Assert.True(mover.IsMoving);
        }

        [Fact]
        public void Update_WhenReachingTarget_ShouldStopMovingAndProcessQueue()
        {
            // Arrange
            var mover = CreatePlayerMover();
            var nearbyTarget = new Vector2f(2f, 0f);
            var queuedTarget = new Vector2f(4f, 0f);

            mover.MoveTo(nearbyTarget);
            mover.QueueMove(queuedTarget);

            // Act - simulate reaching the target
            mover.Update(1f);

            // Assert
            Assert.True(mover.IsMoving);
        }

        [Fact]
        public void QueueMove_MultipleSequentialMoves_ShouldProcessInOrder()
        {
            // Arrange
            var mover = CreatePlayerMover();
            var move1 = new Vector2f(32f, 0f);
            var move2 = new Vector2f(64f, 0f);
            var move3 = new Vector2f(96f, 0f);

            // Act
            mover.QueueMove(move1);
            mover.QueueMove(move2);
            mover.QueueMove(move3);

            // Assert
            Assert.True(mover.IsMoving);
            Assert.True(mover.HasQueuedMoves());
        }

        [Fact]
        public void ClearQueue_ShouldRemoveAllQueuedMoves()
        {
            // Arrange
            var mover = CreatePlayerMover();
            mover.QueueMove(new Vector2f(32f, 0f));
            mover.QueueMove(new Vector2f(64f, 0f));
            mover.QueueMove(new Vector2f(96f, 0f));

            // Act
            mover.ClearMoveQueue();

            // Assert
            Assert.False(mover.HasQueuedMoves());
        }

        [Fact]
        public void HasQueuedMoves_WhenEmpty_ShouldReturnFalse()
        {
            // Arrange
            var mover = CreatePlayerMover();

            // Act & Assert
            Assert.False(mover.HasQueuedMoves());
        }

        [Fact]
        public void HasQueuedMoves_WhenNotEmpty_ShouldReturnTrue()
        {
            // Arrange
            var mover = CreatePlayerMover();
            mover.MoveTo(new Vector2f(100f, 0));

            // Act
            mover.QueueMove(new Vector2f(32f, 0f));

            // Assert
            Assert.True(mover.HasQueuedMoves());
        }
        
        [Fact]
        public void Stop_ShouldStopMovingAndClearQueue()
        {
            // Arrange
            var mover = CreatePlayerMover();
            mover.MoveTo(new Vector2f(100f, 0f));
            mover.QueueMove(new Vector2f(200f, 0f));

            // Act
            mover.Stop();

            // Assert
            Assert.False(mover.IsMoving);
            Assert.False(mover.HasQueuedMoves());
        }
    }
}