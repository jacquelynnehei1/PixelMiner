using pixel_miner.Components.Rendering;
using pixel_miner.Core.Enums;
using pixel_miner.Core;
using pixel_miner.World;
using pixel_miner.Utils.Extensions;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.Components.Gameplay
{
    public class BoardVisualizer : Component
    {
        private Board? board;
        private Dictionary<GridPosition, GameObject> tileVisuals = new Dictionary<GridPosition, GameObject>();

        // Visual configuration
        public Color DefaultTileColor { get; set; } = Color.Yellow;
        public Color SurfaceTileColor { get; set; } = new Color(144, 238, 144);
        public Color DeepTileColor { get; set; } = new Color(101, 67, 33);
        public float TilePadding { get; set; } = 2f;

        private bool hasCreatedInitialTiles = false;

        public override void Start()
        {
            var boardObject = GameObject.FindObjectOfType<Board>();
            if (boardObject != null)
            {
                board = boardObject.GetComponent<Board>();
                if (board != null)
                {
                    board.OnTilesAdded += OnTilesAdded;
                    board.OnTilesRemoved += OnTilesRemoved;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            if (!hasCreatedInitialTiles && board != null)
            {
                CreateVisualsForAllTiles();
                hasCreatedInitialTiles = true;
            }
        }

        private void CreateVisualsForAllTiles()
        {
            if (board == null) return;

            var allTilePositions = board.GetAllTilePositions().ToList();
            foreach (var tilePosition in allTilePositions)
            {
                CreatTileVisual(tilePosition);
            }
        }

        private void CreatTileVisual(GridPosition tilePosition)
        {
            if (board == null) return;
            if (tileVisuals.ContainsKey(tilePosition)) return;

            var tile = board.GetTile(tilePosition);
            if (tile == null) return;

            var worldPosition = board.GridToWorldPosition(tilePosition);

            var tileObject = new GameObject($"Tile_{tilePosition.X}_{tilePosition.Y}", worldPosition.X, worldPosition.Y);

            var spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

            var tileSize = board.TileSize - TilePadding;
            spriteRenderer.SetColor(GetTileColor(tilePosition, tile));
            spriteRenderer.SetSize(new Vector2f(tileSize, tileSize));

            spriteRenderer.RenderLayer = RenderLayer.World;
            spriteRenderer.SortingOrder = -1;

            tileVisuals[tilePosition] = tileObject;

            GameObject.Create(tileObject);
        }

        private void RemoveTileVisual(GridPosition tilePosition)
        {
            if (!tileVisuals.ContainsKey(tilePosition)) return;

            var tileObject = tileVisuals[tilePosition];
            tileVisuals.Remove(tilePosition);

            GameObject.Destroy(tileObject);
        }

        private Color GetTileColor(GridPosition position, Tile tile)
        {
            if (board == null) return DefaultTileColor;

            var topRow = board.GetTopRowIndex();
            int depth = position.Y - topRow;

            if (depth <= 2)
            {
                return SurfaceTileColor;
            }
            else if (depth <= 10)
            {
                return DefaultTileColor;
            }
            else
            {
                float depthFactor = Math.Min(1f, (depth - 10) / 20f);
                return DefaultTileColor.Lerp(DeepTileColor, depthFactor);
            }
        }

        private void OnTilesAdded(IEnumerable<GridPosition> newTilePositions)
        {
            var newTiles = newTilePositions.ToList();

            foreach (var tilePosition in newTiles)
            {
                CreatTileVisual(tilePosition);
            }
        }

        private void OnTilesRemoved(IEnumerable<GridPosition> removedTilePositions)
        {
            var removedTiles = removedTilePositions.ToList();

            foreach (var tilePosition in removedTiles)
            {
                RemoveTileVisual(tilePosition);
            }
        }

        public override void Destroy()
        {
            if (board != null)
            {
                board.OnTilesAdded -= OnTilesAdded;
                board.OnTilesRemoved -= OnTilesRemoved;
            }

            var currentScene = SceneManager.GetCurrentScene();
            foreach (var tile in tileVisuals.Values)
            {
                currentScene?.RemoveGameObject(tile);
            }

            tileVisuals.Clear();
        }
    }
}