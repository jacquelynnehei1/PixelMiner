using pixel_miner.Components.Gameplay;
using pixel_miner.Components.Input;
using pixel_miner.Components.Movement;
using pixel_miner.Components.Rendering;
using pixel_miner.Components.UI;
using pixel_miner.Core;
using SFML.System;
using SFML.Graphics;

namespace pixel_miner.Scenes.Factories
{
    public class GameSceneFactory : SceneFactoryBase
    {
        public override string SceneName => "GameScene";

        public override Scene CreateScene()
        {
            var scene = new Scene(SceneName);

            var gameSession = new GameSession(maxFuel: 20);

            gameSession.OnGameOver += (reason) =>
            {
                Console.WriteLine($"Game Over: {reason}. \nPress R to Restart!");
            };

            var player = CreatePlayer(ref gameSession);
            scene.AddGameObject(player);

            // Create camera that follows player
            var camera = CreateBasicCamera(player);
            var cameraComponent = camera.GetComponent<Camera>();
            scene.AddGameObject(camera);

            // Add some test reference objects
            var ref1 = CreateTestObject("Ref1", 200, 200, Color.Green);
            var ref2 = CreateTestObject("Ref2", -200, -200, Color.Blue);

            scene.AddGameObject(ref1);
            scene.AddGameObject(ref2);

            // Add a percentage bar to the UI
            var percentageBar = new GameObject("TestPercentageBar");
            var percentageBarUI = percentageBar.AddComponent<PercentageBarUI>();

            percentageBarUI.Configure(
                position: cameraComponent != null ? cameraComponent.ScreenToWorld(new Vector2f(20f, 20f)) : new Vector2f(20f, 20f),
                size: new Vector2f(200f, 20f),
                backgroundColor: new Color(255, 255, 255),
                fillColor: new Color(237, 161, 47),
                textColor: Color.White
            );

            percentageBarUI.SetPercentage(0.75f);

            scene.AddGameObject(percentageBar);

            return scene;
        }

        private GameObject CreatePlayer(ref GameSession session)
        {
            var player = new GameObject("Player");

            var initialWorldPosition = session.Board.GridToWorldPosition(session.PlayerData.GridPosition);
            player.Transform.Position = initialWorldPosition;

            var playerMover = player.AddComponent<PlayerMover>();
            var playerComponent = player.AddComponent<Player>();
            var inputController = player.AddComponent<PlayerInputController>();

            var playerSpriteRenderer = player.AddComponent<SpriteRenderer>();
            playerSpriteRenderer.SetColor(SFML.Graphics.Color.Red);
            playerSpriteRenderer.SetSize(new Vector2f(32, 32));

            playerComponent.Initialize(session);
            inputController.Initialize(session);

            return player;
        }
    }
}