using pixel_miner.Components.Rendering.Cameras;
using pixel_miner.Core;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Tests
{
    // Collection fixture that initializes GameManager once for all tests that use it
    public class GameManagerFixture : IDisposable
    {
        public GameManagerFixture()
        {
            // Initialize GameManager once when the fixture is created
            GameManager.Initialize();
            Console.WriteLine("GameManager initialized for test collection");
        }

        public void Dispose()
        {
            // Clean up GameManager when all tests in the collection are done
            GameManager.Destroy();
            Console.WriteLine("GameManager destroyed after test collection");
        }
    }

    // Define the test collection that will share the GameManagerFixture
    [CollectionDefinition("GameManager Collection")]
    public class GameManagerCollection : ICollectionFixture<GameManagerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}