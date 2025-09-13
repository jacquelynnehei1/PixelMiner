using pixel_miner.Components.Gameplay;
using pixel_miner.Components.UI.Enums;
using pixel_miner.Core;
using pixel_miner.World.Enums;
using SFML.Graphics;

namespace pixel_miner.Components.UI.Display
{
    public class ResourceDisplayContainer : UIContainer
    {
        private UIText? headerText;
        private List<ResourceDisplayItem> resourceItems = new List<ResourceDisplayItem>();
        private Inventory? inventory;

        public ResourceDisplayContainer() { }

        public void Initialize(Inventory playerInventory)
        {
            inventory = playerInventory;

            SetupPanel();
            CreateHeader();
            CreateResourceDisplays();
            PositionAllElements();
        }

        private void SetupPanel()
        {
            SetAnchor(AnchorPreset.StretchHorizontal);
            SetSize(0, 150);
            SetBackground(new Color(0, 50, 100, 100));
        }

        private void CreateHeader()
        {
            var headerObject = new GameObject($"{GameObject.Name}_Header");
            headerText = headerObject.AddComponent<UIText>();
            headerText.SetText("RESOURCE INVENTORY");
            headerText.SetFontSize(14);
            headerText.SetColor(new Color(0, 255, 255));
            headerText.SetFont("Assets/Fonts/Default.otf");
            AddChild(headerText);
        }

        private void CreateResourceDisplays()
        {
            if (inventory == null) return;

            foreach (ResourceType resourceType in Enum.GetValues<ResourceType>())
            {
                if (resourceType == ResourceType.Fuel) continue;

                var resourceItemObject = new GameObject($"{GameObject.Name}_Resource_{resourceType}");
                var resourceDisplayItem = resourceItemObject.AddComponent<ResourceDisplayItem>();
                resourceDisplayItem.Initialize(resourceType, inventory);

                resourceItems.Add(resourceDisplayItem);
                AddChild(resourceDisplayItem);
            }

            Console.WriteLine($"Created {resourceItems.Count} resource display items");
        }

        private void PositionAllElements()
        {
            if (headerText != null)
            {
                headerText.SetPosition(10, 5);
            }

            float yOffset = 35;
            foreach (var item in resourceItems)
            {
                item.SetPosition(10, yOffset);
                yOffset += 25f;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Destroy()
        {
            resourceItems.Clear();
            base.Destroy();
        }
    }
}