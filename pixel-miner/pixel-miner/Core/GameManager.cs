using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.World;

namespace pixel_miner.Core
{
    public class GameManager
    {
        private static GameManager? instance = null;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("GameManager has not been initialized. Call GameManager.Initialize() first.");
                }

                return instance;
            }
        }

        public bool IsGameOver { get; set; } = false;
        public string? GameOverReason { get; set; } = null;

        public event Action<string>? OnGameOver;
        public event Action? OnGameRestart;

        private GameManager() { }

        public static void Initialize()
        {
            if (instance == null)
            {
                instance = new GameManager();
                Console.WriteLine("GameManager initialized");
            }
        }

        public void SetGameOver(string reason)
        {
            if (!IsGameOver)
            {
                IsGameOver = true;
                GameOverReason = reason;
                OnGameOver?.Invoke(reason);
                Console.WriteLine($"Game Over: {reason}");
            }
        }

        public void RestartGame()
        {
            IsGameOver = false;
            GameOverReason = null;
            OnGameRestart?.Invoke();
            Console.WriteLine($"Game restarted");
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                instance.OnGameOver = null;
                instance.OnGameRestart = null;
                instance = null;
                Console.WriteLine("GameManager destroyed");
            }
        }
    }
}