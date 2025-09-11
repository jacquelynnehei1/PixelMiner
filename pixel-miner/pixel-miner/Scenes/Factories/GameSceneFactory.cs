using pixel_miner.Components.Gameplay;
using pixel_miner.Components.Input;
using pixel_miner.Components.Movement;
using pixel_miner.Components.Rendering;
using pixel_miner.Components.Rendering.Cameras;
using pixel_miner.Core;
using pixel_miner.Core.Enums;
using pixel_miner.World;
using SFML.System;
using SFML.Graphics;

namespace pixel_miner.Scenes.Factories
{
    public class GameSceneFactory : SceneFactoryBase
    {
        public override string SceneName => "GameScene";

        public override Scene CreateScene(RenderWindow window)
        {
            var scene = new Scene(SceneName);
            
            // Create camera that follows player
            var camera = CreatePlayerCamera(window, null);
            var mainCamera = camera.GetComponent<MainGameCamera>();

            var boardObject = new GameObject("Board");
            var board = boardObject.AddComponent<Board>();

            var boardVisualizer = boardObject.AddComponent<BoardVisualizer>();
            boardVisualizer.DefaultTileColor = new Color(139, 119, 101);
            boardVisualizer.SurfaceTileColor = new Color(144, 238, 144);
            boardVisualizer.DeepTileColor = new Color(101, 67, 33);
            boardVisualizer.TilePadding = 2f;

            board.InitializeGrid(25);
            scene.AddGameObject(boardObject);

            var spawnPosition = new GridPosition(0, board.GetTopRowIndex());

            var player = CreatePlayer();
            var playerComponent = player.GetComponent<Player>()!;
            var movementSystem = player.GetComponent<MovementSystem>()!;

            var initialPosition = board.GridToWorldPosition(playerComponent.GridPosition);
            player.Transform.Position = initialPosition;

            playerComponent.RespawnPosition = playerComponent.GridPosition;

            if (mainCamera != null)
            {
                mainCamera.Transform.Position = player.Transform.Position;
            }

            var inputController = player.GetComponent<PlayerInputController>()!;

            playerComponent.Initialize(board, maxFuel: 100, spawnPosition);
            inputController.Initialize(playerComponent);
            movementSystem.Initialize(board, playerComponent);

            GameManager.Instance.OnGameOver += (reason) =>
            {
                Console.WriteLine($"Game Over: {reason}. \nPress R to Restart!");
            };

            if (mainCamera != null)
            {
                mainCamera.SetTarget(player);
            }

            scene.AddGameObject(camera);
            scene.AddGameObject(player);

            // Add some test reference objects
            var ref1 = CreateTestObject("Ref1", 200, 200, Color.Green);
            var ref2 = CreateTestObject("Ref2", -200, -200, Color.Blue);

            scene.AddGameObject(ref1);
            scene.AddGameObject(ref2);

            var hud = CreateHUDCamera();
            scene.AddGameObject(hud);

            return scene;
        }

        private GameObject CreatePlayer()
        {
            var player = new GameObject("Player");

            player.AddComponent<PlayerMover>();
            player.AddComponent<Player>();
            player.AddComponent<PlayerInputController>();
            player.AddComponent<MovementSystem>();

            var playerSpriteRenderer = player.AddComponent<SpriteRenderer>();
            playerSpriteRenderer.SetColor(Color.Red);
            playerSpriteRenderer.SetSize(new Vector2f(32, 32));

            return player;
        }

        private GameObject CreatePlayerCamera(RenderWindow window, GameObject? player = null)
        {
            var cameraObject = new GameObject("PlayerCamera", 0, 0);
            var playerCamera = cameraObject.AddComponent<MainGameCamera>();

            if (player != null)
            {
                playerCamera.SetTarget(player);
            }

            playerCamera.SetName(cameraObject.Name);
            playerCamera.SetViewport(0.25f, 0f, 0.5f, 1f);
            playerCamera.SetViewSize(new Vector2f(
                window.Size.X * playerCamera.Viewport.Width,
                window.Size.Y * playerCamera.Viewport.Height)
            );
            
            playerCamera.FollowX = false;

            playerCamera.FollowSpeed = 5f;

            var background = cameraObject.AddComponent<SpriteRenderer>();
            background.RenderLayer = RenderLayer.World;
            background.SetColor(new Color(20, 30, 50));
            background.SetSize(new Vector2f(window.Size.X, window.Size.Y));
            background.SortingOrder = -2;

            CameraManager.AddCamera(playerCamera, isMainCamera: true);

            return cameraObject;
        }

        private GameObject CreateHUDCamera()
        {
            var gameObject = new GameObject("HUD", 0, 0);
            var hudCamera = gameObject.AddComponent<HUDCamera>();
            hudCamera.SetName(gameObject.Name);

            CameraManager.AddCamera(hudCamera);

            return gameObject;
        }
    }
}