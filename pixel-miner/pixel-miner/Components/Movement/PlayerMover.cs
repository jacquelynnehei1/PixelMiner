using System.Xml.Serialization;
using pixel_miner.Components.Gameplay;
using pixel_miner.Core;
using pixel_miner.Utils;
using pixel_miner.World;
using SFML.System;

namespace pixel_miner.Components.Movement
{
    public class PlayerMover : Mover
    {
        private Board? board;
        private Queue<Vector2f> moveQueue = new Queue<Vector2f>();
        private Vector2f targetPosition;

        public override void Start()
        {
            MoveSpeed = 200f;
        }

        public void QueueMove(Vector2f target)
        {
            moveQueue.Enqueue(target);

            if (!IsMoving && moveQueue.Count > 0)
            {
                ProcessNextQueuedMove();
            }
        }

        public override void MoveTo(Vector2f target)
        {
            if (Transform == null) return;

            targetPosition = target;
            IsMoving = true;
        }
 
        public override void Update(float deltaTime)
        {
            if (!IsMoving || Transform == null) return;

            Vector2f currentPosition = Transform.Position;
            float distanceToTarget = currentPosition.Distance(targetPosition);

            if (distanceToTarget < 5f)
            {
                Transform.Position = targetPosition;
                IsMoving = false;

                if (moveQueue.Count > 0)
                {
                    ProcessNextQueuedMove();
                }

                return;
            }

            Vector2f direction = (targetPosition - currentPosition).Normalized();
            Vector2f movement = direction * MoveSpeed * deltaTime;

            Transform.Position = currentPosition + movement;
        }

        private void ProcessNextQueuedMove()
        {
            if (moveQueue.Count == 0)
            {
                return;
            }

            var nextMove = moveQueue.Dequeue();

            MoveTo(nextMove);
        }

        public void SetBoard(Board gameBoard)
        {
            board = gameBoard;
        }
    }
}