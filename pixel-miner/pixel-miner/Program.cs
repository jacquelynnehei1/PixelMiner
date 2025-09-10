using SFML.Graphics;
using SFML.Window;
using pixel_miner.Core;
using pixel_miner.Scenes.Factories;

namespace pixel_miner
{
    class Program
    {
        static void Main(string[] args)
        {
            var desktopMode = VideoMode.DesktopMode;
            var window = new RenderWindow(desktopMode, "Pixel Miner");
            window.SetFramerateLimit(60);
            
            window.Closed += (sender, e) => window.Close();
            window.KeyPressed += OnKeyPressed;

            // Create game scene using factory
            var gameSceneFactory = new GameSceneFactory();
            var gameScene = gameSceneFactory.CreateScene(window);
            
            // Set up scene manager
            SceneManager.AddScene(gameScene);
            SceneManager.LoadScene(gameSceneFactory.SceneName);

            var lastLogTime = 0f;

            while (window.IsOpen)
            {
                Time.Update();

                if ((int)Time.TotalTime % 10 == 0 && Time.TotalTime > lastLogTime + 9f)
                {
                    Time.LogPerformance();
                    lastLogTime = Time.TotalTime;
                }

                window.DispatchEvents();

                SceneManager.Update(Time.DeltaTime);

                window.Clear(Color.Black);

                CameraManager.ApplyCameraToWindow(window);
                SceneManager.Render(window);

                window.Display();
            }

            SceneManager.Destroy();
        }
        
        static void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            var window = (RenderWindow)sender!;
            if (e.Code == Keyboard.Key.Escape)
                window.Close();
        }
    }
}