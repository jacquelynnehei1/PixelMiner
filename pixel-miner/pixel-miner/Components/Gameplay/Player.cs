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

        private PlayerMover? mover;
        private MovementSystem movementSystem = null!;
        private Board? board = null!;

        public override void Start()
        {
            if (Transform == null || board == null) return;

            var initialWorldPosition = board.GridToWorldPosition(GridPosition);
            Transform.Position = initialWorldPosition;

            mover = GameObject.GetComponent<PlayerMover>();
        }

        public void Initialize(Board gameBoard, int maxFuel, GridPosition gridPosition)
        {
            board = gameBoard;
            movementSystem = GameObject.GetComponent<MovementSystem>()!;

            RenderLayer = RenderLayer.World;

            MaxFuel = maxFuel;
            GridPosition = gridPosition;
            CurrentFuel = maxFuel;

            movementSystem.OnPlayerMoved += OnPlayerMoved;
            movementSystem.OnOutOfFuel += HandleOutOfFuel;
            movementSystem.OnMoveBlocked += HandleMoveBlocked;

            GameManager.Instance.OnGameRestart += ResetPlayer;
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

        private void ResetPlayer()
        {
            if (mover != null)
            {
                mover.ClearMoveQueue();
            }

            movementSystem.ResetMovement();

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
            if (board != null && mover != null)
            {
                var worldPosition = board.GridToWorldPosition(to);
                mover.QueueMove(worldPosition);
            }
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