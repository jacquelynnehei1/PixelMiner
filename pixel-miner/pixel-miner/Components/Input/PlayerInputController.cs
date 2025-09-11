using pixel_miner.Components.Gameplay;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.World;
using SFML.Window;

namespace pixel_miner.Components.Input
{    public class PlayerInputController : Component
    {
        private struct KeyMapping
        {
            public Keyboard.Key PrimaryKey;
            public Keyboard.Key AlternateKey;
            public GridPosition Direction;
            public bool WasPressed;
        }

        private KeyMapping[] keyMappings = new KeyMapping[]
        {
            new KeyMapping { PrimaryKey = Keyboard.Key.W, AlternateKey = Keyboard.Key.Up, Direction = new GridPosition(0, -1) },
            new KeyMapping { PrimaryKey = Keyboard.Key.A, AlternateKey = Keyboard.Key.Left, Direction = new GridPosition(-1, 0) },
            new KeyMapping { PrimaryKey = Keyboard.Key.S, AlternateKey = Keyboard.Key.Down, Direction = new GridPosition(0, 1) },
            new KeyMapping { PrimaryKey = Keyboard.Key.D, AlternateKey = Keyboard.Key.Right, Direction = new GridPosition(1, 0) },
        };

        private Player? player;
        private bool restartPressed;

        public void Initialize(Player player)
        {
            this.player = player;
        }

        public override void Update(float deltaTime)
        {
            CheckMovementInput();
            CheckRestartInput();
        }

        private void CheckMovementInput()
        {
            if (GameManager.Instance.IsGameOver) return;

            for (int i = 0; i < keyMappings.Length; i++)
            {
                bool currentlyPressed = Keyboard.IsKeyPressed(keyMappings[i].PrimaryKey) || Keyboard.IsKeyPressed(keyMappings[i].AlternateKey);

                if (currentlyPressed && !keyMappings[i].WasPressed)
                {
                    if (player != null)
                    {
                        player.HandleMoveRequest(keyMappings[i].Direction);
                    }
                }

                keyMappings[i].WasPressed = currentlyPressed;
            }
        }

        private void CheckRestartInput()
        {
            if (GameManager.Instance.IsGameOver)
            {
                bool restartCurrently = Keyboard.IsKeyPressed(Keyboard.Key.R);

                if (restartCurrently && !restartPressed)
                {
                    GameManager.Instance.RestartGame();
                }

                restartPressed = restartCurrently;
            }
        }
    }
}