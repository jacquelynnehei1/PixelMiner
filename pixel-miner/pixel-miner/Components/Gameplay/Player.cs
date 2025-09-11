using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.World;
using SFML.System;

namespace pixel_miner.Components.Gameplay
{
    public class Player : Component
    {
        public GridPosition GridPosition { get; set; }
        public int CurrentFuel { get; set; }
        public int MaxFuel { get; set; }

        public event Action<int>? OnFuelChanged;
        public event Action<GridPosition>? OnPositionChanged;

        private GameSession? gameSession;
        private PlayerMover? mover;
        private MovementSystem movementSystem;

        public override void Start()
        {
            if (gameSession == null || Transform == null) return;

            var initialWorldPosition = gameSession.Board.GridToWorldPosition(GridPosition);
            Transform.Position = initialWorldPosition;

            mover = GameObject.GetComponent<PlayerMover>();

            if (mover == null) return;

            mover.SetBoard(gameSession.Board);

            movementSystem = GameObject.GetComponent<MovementSystem>()!;
        }

        public void Initialize(GameSession session, int maxFuel, GridPosition gridPosition)
        {
            gameSession = session;
            movementSystem.OnPlayerMoved += OnPlayerMoved;
            movementSystem.OnOutOfFuel += HandleOutOfFuel;
            movementSystem.OnMoveBlocked += HandleMoveBlocked;
            gameSession.OnClearMoveQueue += OnClearMoveQueue;

            RenderLayer = RenderLayer.World;

            MaxFuel = maxFuel;
            GridPosition = gridPosition;
            CurrentFuel = maxFuel;

            GameManager.Instance.OnGameRestart += ResetFuel;
        }

        public bool TryConsumeFuel(int amount)
        {
            if (CurrentFuel < amount) return false;

            CurrentFuel = Math.Max(0, CurrentFuel - amount);
            OnFuelChanged?.Invoke(CurrentFuel);

            if (CurrentFuel <= 0)
            {
                HandleOutOfFuel();
            }

            return true;
        }

        public void AddFuel(int amount)
        {
            int oldFuel = CurrentFuel;
            CurrentFuel = Math.Min(MaxFuel, CurrentFuel + amount);

            if (CurrentFuel != oldFuel)
            {
                OnFuelChanged?.Invoke(CurrentFuel);
            }
        }

        public void SetMaxFuel(int maxFuel)
        {
            MaxFuel = maxFuel;

            int oldFuel = CurrentFuel;
            CurrentFuel = Math.Min(CurrentFuel, MaxFuel);

            if (CurrentFuel != oldFuel)
            {
                OnFuelChanged?.Invoke(CurrentFuel);
            }
        }

        public void SetPosition(GridPosition newPosition)
        {
            if (!GridPosition.Equals(newPosition))
            {
                GridPosition = newPosition;
                OnPositionChanged?.Invoke(GridPosition);
            }
        }

        public bool HasFuel(int amount)
        {
            return CurrentFuel >= amount;
        }

        private void ResetFuel()
        {
            CurrentFuel = MaxFuel;
        }

        public float FuelPercentage => MaxFuel > 0 ? (float)CurrentFuel / MaxFuel : 0f;

        public void HandleMoveRequest(GridPosition direction, bool bypassFuelCheck = false)
        {
            if (GameManager.Instance.IsGameOver && !bypassFuelCheck) return;

            movementSystem.RequestMove(direction, bypassFuelCheck);
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

        private void OnClearMoveQueue()
        {
            if (mover == null) return;

            mover.ClearMoveQueue();
        }

        public override void Destroy()
        {
            if (movementSystem != null)
            {
                movementSystem.OnPlayerMoved -= OnPlayerMoved;
            }
        }

        private void HandleOutOfFuel()
        {
            if (!GameManager.Instance.IsGameOver)
            {
                string message = "Out of fuel!";
                GameManager.Instance.SetGameOver(message);
            }
        }
        
        private void HandleMoveBlocked(string reason)
        {
            Console.WriteLine($"Move blocked: {reason}");
        }
    }
}