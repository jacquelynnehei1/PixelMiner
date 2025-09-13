using pixel_miner.Components.Movement;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.World;
using pixel_miner.World.Enums;
using SFML.System;

namespace pixel_miner.Components.Gameplay
{
    public class Player : Component
    {
        public GridPosition GridPosition { get; set; }
        public GridPosition RespawnPosition { get; set; }
        public int CurrentFuel { get; set; }
        public int MaxFuel { get; set; }

        public event Action<int>? OnFuelChanged;
        public event Action<GridPosition>? OnPositionChanged;

        private PlayerMover? mover;
        private MovementSystem movementSystem = null!;
        private Board? board = null!;
        private Inventory? inventory = null!;

        public override void Start()
        {
            if (Transform == null || board == null) return;

            var initialWorldPosition = board.GridToWorldPosition(GridPosition);
            Transform.Position = initialWorldPosition;

            RespawnPosition = GridPosition;

            mover = GameObject.GetComponent<PlayerMover>();
        }

        public void Initialize(Board gameBoard, int maxFuel, GridPosition gridPosition)
        {
            board = gameBoard;
            movementSystem = GameObject.GetComponent<MovementSystem>()!;

            inventory = GameObject.GetComponent<Inventory>();
            if (inventory == null)
            {
                inventory = GameObject.AddComponent<Inventory>();
            }

            inventory.OnInventoryFull += HandleInventoryFull;
            inventory.OnResourceAdded += HandleResourceAdded;

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

        public void TryMine()
        {
            if (GameManager.Instance.IsGameOver) return;
            if (mover == null) return;

            var facingDirection = mover.LastFacingDirection;
            var targetPosition = GridPosition + facingDirection;

            movementSystem.RequestMine(targetPosition);
        }

        public bool TryCollectResource(ResourceDrop resourceDrop)
        {
            if (inventory == null) return false;

            if (resourceDrop.Type == ResourceType.Fuel)
            {
                AddFuel(resourceDrop.Amount);
                Console.WriteLine($"Collected {resourceDrop.Amount} fuel! Current fuel: {CurrentFuel}/{MaxFuel}");
                return true;
            }

            int amountAdded = inventory.TryAddResource(resourceDrop.Type, resourceDrop.Amount);
            return amountAdded > 0;
        }

        public Inventory? GetInventory()
        {
            return inventory;
        }

        private void HandleInventoryFull(ResourceType resourceType)
        {
            Console.WriteLine($"Cannot collect {resourceType} - inventory full!");
        }

        private void HandleResourceAdded(ResourceType resourceType, int amount)
        {
            Console.WriteLine($"Collected {amount} {resourceType}");
        }
    }
}