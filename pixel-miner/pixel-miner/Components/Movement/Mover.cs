using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.System;

namespace pixel_miner.Components.Movement
{
    public abstract class Mover : Component
    {
        public bool IsMoving { get; protected set; }
        public float MoveSpeed { get; set; }

        public abstract void MoveTo(Vector2f target);
        public virtual void Stop() { IsMoving = false; }
    }
}