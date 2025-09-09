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
        private PlayerMover mover;

        public void Initialize(GameSession session)
        {
            gameSession = session;
            gameSession.OnPlayerMoved += OnPlayerMoved;
        }

        public override void Start()
        {
            mover = GameObject.GetComponent<PlayerMover>();
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
    }
}