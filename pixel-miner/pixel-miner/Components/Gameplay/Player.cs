using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.World;

namespace pixel_miner.Components.Gameplay
{
    public class Player : Component
    {
        private GameSession? gameSession;
        private PlayerMover? mover;

        public void Initialize(GameSession session)
        {
            gameSession = session;
            gameSession.OnPlayerMoved += OnPlayerMoved;
            gameSession.PlayerData.OnPositionChanged += OnPositionChanged;
        }

        public override void Start()
        {
            mover = GameObject.GetComponent<PlayerMover>();

            if (gameSession != null && Transform != null)
            {
                var initialWorldPosition = gameSession.Board.GridToWorldPosition(gameSession.PlayerData.GridPosition);
                Transform.Position = initialWorldPosition;
            }
        }

        private void OnPlayerMoved(GridPosition from, GridPosition to)
        {
            if (gameSession == null) return;

            if (gameSession.Board != null && mover != null)
            {
                var worldPosition = gameSession.Board.GridToWorldPosition(to);
                mover.MoveTo(worldPosition);
            }
        }

        private void OnPositionChanged(GridPosition newGridPosition)
        {
            UpdateWorldPosition(newGridPosition);
        }

        private void UpdateWorldPosition(GridPosition gridPosition)
        {
            if (gameSession?.Board == null || Transform == null) return;

            var worldPosition = gameSession.Board.GridToWorldPosition(gridPosition);

            if (mover == null)
            {
                Transform.Position = worldPosition;
                return;
            }

            mover.MoveTo(worldPosition);
        }

        public override void Destroy()
        {
            if (gameSession != null)
            {
                gameSession.OnPlayerMoved -= OnPlayerMoved;
                gameSession.PlayerData.OnPositionChanged -= OnPositionChanged;
            }
        }
    }
}