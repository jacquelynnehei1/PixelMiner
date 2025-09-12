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
using pixel_miner.Components.UI;

namespace pixel_miner.Scenes.Factories
{
    public class GameSceneFactory : SceneFactoryBase
    {
        public override string SceneName => "GameScene";

        public override Scene CreateScene(RenderWindow window)
        {
            var scene = new MainGameScene(SceneName);

            // Create camera that follows player
            var camera = CreatePlayerCamera(window, null);
            var mainCamera = camera.GetComponent<MainGameCamera>();

            var leftCamera = CreateLeftUIPanel(window);
            scene.AddGameObject(leftCamera.GameObject);

            var rightCamera = CreateRightUIPanel(window);
            scene.AddGameObject(rightCamera.GameObject);

            scene.leftCamera = leftCamera;
            scene.centerCamera = mainCamera;
            scene.rightCamera = rightCamera;

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

            var fuelGauge = CreateFuelGauge(playerComponent, leftCamera);
            scene.AddGameObject(fuelGauge);

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
            playerSpriteRenderer.SortingOrder = 5;
            playerSpriteRenderer.SetRenderView(RenderView.CenterPanel);

            return player;
        }

        private HUDCamera CreateLeftUIPanel(RenderWindow window)
        {
            var leftUIPanelObject = new GameObject("LeftUIPanel");
            var leftUIPanel = leftUIPanelObject.AddComponent<HUDCamera>();

            leftUIPanel.SetName(leftUIPanelObject.Name);
            leftUIPanel.SetViewport(0f, 0f, 0.25f, 1f);
            leftUIPanel.SetViewSize(new Vector2f(
                window.Size.X * leftUIPanel.Viewport.Width,
                window.Size.Y * leftUIPanel.Viewport.Height
            ));

            var background = leftUIPanelObject.AddComponent<SpriteRenderer>();
            background.SetRenderView(RenderView.LeftPanel);
            background.SortingOrder = -2;
            background.SetColor(Color.Black);
            background.SetSize(leftUIPanel.ViewSize);

            leftUIPanelObject.Transform.Position = new Vector2f(leftUIPanel.ViewSize.X / 2f, leftUIPanel.ViewSize.Y / 2f);

            CameraManager.AddCamera(leftUIPanel);

            return leftUIPanel;

        }

        private HUDCamera CreateRightUIPanel(RenderWindow window)
        {
            var gameObject = new GameObject("RightPanel");
            var rightCamera = gameObject.AddComponent<HUDCamera>();

            rightCamera.SetName(gameObject.Name);
            rightCamera.SetViewport(0.75f, 0f, 0.25f, 1f);
            rightCamera.SetViewSize(new Vector2f(
                window.Size.X * rightCamera.Viewport.Width,
                window.Size.Y * rightCamera.Viewport.Height
            ));

            var background = gameObject.AddComponent<SpriteRenderer>();
            background.SetRenderView(RenderView.RightPanel);
            background.SortingOrder = -2;
            background.SetColor(Color.Blue);
            background.SetSize(rightCamera.ViewSize);

            gameObject.Transform.Position = new Vector2f(rightCamera.ViewSize.X / 2, rightCamera.ViewSize.Y / 2);

            CameraManager.AddCamera(rightCamera);

            return rightCamera;
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
            background.SetRenderView(RenderView.CenterPanel);
            background.SetColor(new Color(20, 30, 50));
            background.SetSize(new Vector2f(window.Size.X, window.Size.Y));
            background.SortingOrder = -2;

            CameraManager.AddCamera(playerCamera, isMainCamera: true);

            return cameraObject;
        }

        public GameObject CreateFuelGauge(Player player, Camera camera)
        {
            var gameObject = new GameObject("FuelGauge");
            var fuelBar = gameObject.AddComponent<PercentageBarUI>();

            fuelBar.SetRenderView(RenderView.LeftPanel);

            float panelWidth = camera.ViewSize.X;
            float panelHeight = camera.ViewSize.Y;

            fuelBar.Configure(
                size: new Vector2f(150f, 20f),
                backgroundColor: new Color(64, 64, 64),
                fillColor: new Color(0, 255, 0),
                textColor: Color.White,
                showText: true
            );

            fuelBar.SetLabel("FUEL");
            fuelBar.SetLabelOffset(new Vector2f(15f, -20f));

            fuelBar.Transform.Position = new Vector2f(
                camera.Viewport.Left + 20f,
                camera.Viewport.Top + 50f
            );

            fuelBar.SetFont("Assets/Fonts/Default.otf");

            player.OnFuelChanged += (currentFuel) =>
            {
                float percentage = (float)currentFuel / player.MaxFuel;
                fuelBar.SetPercentage(percentage);

                if (percentage > 0.5f)
                {
                    fuelBar.SetFillColor(new Color(0, 255, 0));
                }
                else if (percentage > 0.25f)
                {
                    fuelBar.SetFillColor(new Color(255, 255, 0));
                }
                else
                {
                    fuelBar.SetFillColor(new Color(255, 0, 0));
                }
            };

            float initialPercentage = (float)player.CurrentFuel / player.MaxFuel;
            fuelBar.SetPercentage(initialPercentage);

            return gameObject;
        }
    }
}