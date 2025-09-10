using pixel_miner.Core.Enums;

namespace pixel_miner.Components.Rendering.Cameras
{
    public class MainGameCamera : FollowCamera
    {
        public MainGameCamera() {}

        public override void Start()
        {
            RenderLayer = RenderLayer.World;
        }
    }
}