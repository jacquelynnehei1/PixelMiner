using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Data;
using pixel_miner.Systems;
using pixel_miner.World;

namespace pixel_miner.Core
{
    public class GameSession
    {
        // Core game systems
        public Board Board { get; set; }
        public PlayerData PlayerData { get; set; }
        public MovementSystem MovementSystem { get; set; }

        // Game State
        public bool IsGameOver { get; set; }
        public string? GameOverReason { get; set; }

        // Events for UI/rendering systems
        public event Action<GridPosition, GridPosition>? OnPlayerMoved;
        public event Action<int>? OnFuelChanged;
        public event Action<string>? OnGameOver;

        public GameSession()
        {
            Board = new Board();
            PlayerData = new PlayerData(new GridPosition(0, 0), maxFuel: 100);
            MovementSystem = new MovementSystem(Board, PlayerData);

            InitializeEvents();
        }

        public GameSession(int maxFuel = 100)
        {
            Board = new Board();
            PlayerData = new PlayerData(new GridPosition(0, 0), maxFuel);
            MovementSystem = new MovementSystem(Board, PlayerData);

            InitializeEvents();
        }

        public GameSession(GridPosition playerPosition, int maxFuel = 100)
        {
            Board = new Board();
            PlayerData = new PlayerData(playerPosition, maxFuel);
            MovementSystem = new MovementSystem(Board, PlayerData);

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            MovementSystem.OnPlayerMoved += HandlePlayerMoved;
            MovementSystem.OnOutOfFuel += HandleOutOfFuel;
            MovementSystem.OnMoveBlocked += HandleMoveBlocked;
            PlayerData.OnFuelChanged += HandleFuelChanged;
        }

        public void HandleMoveRequest(GridPosition direction)
        {
            if (IsGameOver) return;

            MovementSystem.RequestMove(direction);
        }

        public bool CanPlayerMove(GridPosition direction)
        {
            return !IsGameOver && MovementSystem.CanPlayerMove(direction);
        }

        public void RestartGame()
        {
            // Reset player data
            PlayerData.SetPosition(new GridPosition(0, 0));
            PlayerData.SetMaxFuel(PlayerData.MaxFuel);
            PlayerData.AddFuel(PlayerData.MaxFuel);

            // Reset game state
            IsGameOver = false;
            GameOverReason = null;
        }

        private void HandlePlayerMoved(GridPosition from, GridPosition to)
        {
            OnPlayerMoved?.Invoke(from, to);
        }

        private void HandleFuelChanged(int newFuel)
        {
            OnFuelChanged?.Invoke(newFuel);

            if (newFuel <= 0)
            {
                HandleOutOfFuel();
            }
        }

        private void HandleOutOfFuel()
        {
            if (!IsGameOver)
            {
                IsGameOver = true;
                GameOverReason = "Out of fuel!";
                Console.WriteLine(GameOverReason);
                OnGameOver?.Invoke(GameOverReason);
            }
        }

        private void HandleMoveBlocked(string reason)
        {
            Console.WriteLine($"Move blocked: {reason}");
        }

        public void Dispose()
        {
            MovementSystem.OnPlayerMoved -= HandlePlayerMoved;
            MovementSystem.OnOutOfFuel -= HandleOutOfFuel;
            MovementSystem.OnMoveBlocked -= HandleMoveBlocked;
            PlayerData.OnFuelChanged -= HandleFuelChanged;
        }
    }
}