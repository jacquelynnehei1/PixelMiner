using pixel_miner.Utils;
using SFML.System;

namespace pixel_miner.Components.Movement
{
    public class PlayerMover : Mover
    {
        private Vector2f startPosition;
        private Vector2f targetPosition;
        private float progress = 0f;

        public override void Start()
        {
            MoveSpeed = 200f;
        }

        public override void MoveTo(Vector2f target)
        {
            if (Transform == null) return;

            startPosition = Transform.Position;
            targetPosition = target;
            progress = 0f;
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
                return;
            }

            Vector2f direction = (targetPosition - currentPosition).Normalized();
            Vector2f movement = direction * MoveSpeed * deltaTime;

            Transform.Position = currentPosition + movement;
        }
    }
}