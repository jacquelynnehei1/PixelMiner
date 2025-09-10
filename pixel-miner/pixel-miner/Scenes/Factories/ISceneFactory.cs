using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.Graphics;

namespace pixel_miner.Scenes.Factories
{
    public interface ISceneFactory
    {
        abstract Scene CreateScene(RenderWindow window);
        abstract string SceneName { get; }
    }
}