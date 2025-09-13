using pixel_miner.Components.Gameplay;
using pixel_miner.Core;
using pixel_miner.World.Enums;
using SFML.Graphics;

namespace pixel_miner.Components.UI.Display
{
    public class ResourceDisplayItem : UIContainer
    {
        private UISprite? iconSprite;
        private UIText? nameText;
        private UIText? valueText;
        private Inventory? inventory;

        public ResourceType ResourceType { get; private set; }

        private static readonly Dictionary<ResourceType, Color> ResourceColors = new()
        {
            { ResourceType.Stone, new Color(139, 119, 101)},
            { ResourceType.Iron, new Color(64, 64, 64)},
            { ResourceType.Gems, new Color(128, 0, 128)}
        };

        public ResourceDisplayItem() { }

        public void Initialize(ResourceType resourceType, Inventory playerInventory)
        {
            ResourceType = resourceType;
            inventory = playerInventory;

            SetSize(250, 20);

            CreateChildComponents();
            PositionChildren();
            UpdateDisplay();

            if (inventory != null)
            {
                inventory.OnResourceAdded += OnInventoryChanged;
                inventory.OnResourceRemoved += OnInventoryChanged;
            }
        }

        private void CreateChildComponents()
        {
            var iconObject = new GameObject($"{GameObject.Name}_Icon");
            iconSprite = iconObject.AddComponent<UISprite>();
            iconSprite.SetSize(16, 16);
            iconSprite.SetColor(ResourceColors.GetValueOrDefault(ResourceType, Color.White));
            AddChild(iconSprite);

            var nameObject = new GameObject($"{GameObject.Name}_NameText");
            nameText = nameObject.AddComponent<UIText>();
            nameText.SetText($"{ResourceType}:");
            nameText.SetFontSize(12);
            nameText.SetColor(Color.White);
            nameText.SetFont("Assets/Fonts/Default.otf");
            AddChild(nameText);

            var valueObject = new GameObject($"{GameObject.Name}_ValueText");
            valueText = valueObject.AddComponent<UIText>();
            valueText.SetFontSize(12);
            valueText.SetColor(new Color(255, 255, 0));
            valueText.SetFont("Assets/Fonts/Default.otf");
            AddChild(valueText);
        }

        private void PositionChildren()
        {
            if (iconSprite == null || nameText == null || valueText == null) return;

            iconSprite.SetPosition(0, 2);
            nameText.SetPosition(20, 0);
            valueText.SetPosition(120, 0);
        }

        private void OnInventoryChanged(ResourceType changedResourceType, int amount)
        {
            if (changedResourceType == ResourceType)
            {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (inventory == null || valueText == null) return;

            int quantity = inventory.GetResourceCount(ResourceType);

            valueText.SetText($"{quantity} units");

            if (quantity > 0)
            {
                valueText.SetColor(new Color(255, 255, 0));
                return;
            }

            valueText.SetColor(new Color(150, 150, 150));
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Destroy()
        {
            if (inventory != null)
            {
                inventory.OnResourceAdded -= OnInventoryChanged;
                inventory.OnResourceRemoved -= OnInventoryChanged;
            }

            base.Destroy();
        }
    }
}