using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;

namespace pixel_miner.Scenes.Factories
{
    public interface ISceneFactory
    {
        abstract Scene CreateScene();
        abstract string SceneName { get; }
    }
}