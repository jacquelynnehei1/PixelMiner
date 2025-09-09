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
        }

        public override void Start()
        {
            if (gameSession == null || Transform == null) return;

            var initialWorldPosition = gameSession.Board.GridToWorldPosition(gameSession.PlayerData.GridPosition);
            Transform.Position = initialWorldPosition;

            mover = GameObject.GetComponent<PlayerMover>();

            if (mover == null) return;

            mover.SetBoard(gameSession.Board);
        }

        private void OnPlayerMoved(GridPosition from, GridPosition to)
        {
            if (gameSession == null) return;

            if (gameSession.Board != null && mover != null)
            {
                var worldPosition = gameSession.Board.GridToWorldPosition(to);
                mover.QueueMove(worldPosition);
            }
        }

        public override void Destroy()
        {
            if (gameSession != null)
            {
                gameSession.OnPlayerMoved -= OnPlayerMoved;
            }
        }
    }
}