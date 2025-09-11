using pixel_miner.Components.Gameplay;
using pixel_miner.Utils;
using pixel_miner.World;

namespace pixel_miner.Core
{
    public class GameSession
    {
        // Core game systems
        public Board Board { get; set; }
        public Player PlayerData { get; set; }

        // Game State
        public bool IsGameOver { get; set; }
        public string? GameOverReason { get; set; }

        // Events for UI/rendering systems
        public event Action<int>? OnFuelChanged;
        public event Action? OnClearMoveQueue;

        public GameSession(Player player)
        {
            Board = new Board();
            PlayerData = player;
        }

        public void RestartGame()
        {
            OnClearMoveQueue?.Invoke();

            var currentPosition = PlayerData.GridPosition;
            var startPosition = new GridPosition(0, 0);

            Console.WriteLine($"Restarting from {currentPosition} to {startPosition}");

            if (!currentPosition.Equals(startPosition))
            {
                var path = Pathfinder.CalculatePath(currentPosition, startPosition);
                Console.WriteLine("Raw path from pathfinder: ");
                foreach (var step in path)
                {
                    Console.WriteLine($"Step: {step}");
                }

                var currentStep = currentPosition;
                foreach (var nextStep in path)
                {
                    var direction = nextStep - currentStep;
                    Console.WriteLine($"Moving from {currentStep} to {nextStep}, direction: {direction}");
                    PlayerData.HandleMoveRequest(direction, bypassFuelCheck: true);
                    currentStep = nextStep;
                }        
            }

            // Reset player data
            PlayerData.SetMaxFuel(PlayerData.MaxFuel);
            PlayerData.AddFuel(PlayerData.MaxFuel);

            // Reset game state
            IsGameOver = false;
            GameOverReason = null;

            Console.WriteLine("Game state restart - fuel restored, ready to play!");
        }
    }
}