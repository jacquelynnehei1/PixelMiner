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

        public override void Start()
        {
            Console.WriteLine("BoardVisualizer starting...");

            var boardObject = GameObject.FindObjectOfType<Board>();
            if (boardObject != null)
            {
                board = boardObject.GetComponent<Board>();
                if (board != null)
                {
                    Console.WriteLine("BoardVisualizer connected to board");

                    board.OnTilesAdded += OnTilesAdded;
                    board.OnTilesRemoved += OnTilesRemoved;

                    CreateVisualsForAllTiles();
                }
                else
                {
                    Console.WriteLine("BoardVisualizer: Found Board GameObject but no Board component!");
                }
            }
            else
            {
                Console.WriteLine("BoardVisualize: Could not find Board GameObject!");
            }
        }

        private void CreateVisualsForAllTiles()
        {
            if (board == null) return;

            var allTilePositions = board.GetAllTilePositions().ToList();
            Console.WriteLine($"Creating visuals for all {allTilePositions.Count} tiles");

            foreach (var tilePosition in allTilePositions)
            {
                CreatTileVisual(tilePosition);
            }

            Console.WriteLine($"BoardVisualizer created {tileVisuals.Count} tile visuals");
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
            Console.WriteLine($"BoardVisualizer: Adding visuals for {newTiles.Count} new tiles");

            foreach (var tilePosition in newTiles)
            {
                CreatTileVisual(tilePosition);
            }
        }

        private void OnTilesRemoved(IEnumerable<GridPosition> removedTilePositions)
        {
            var removedTiles = removedTilePositions.ToList();
            Console.WriteLine($"BoardVisualizer: Removing visuals for {removedTiles.Count}");

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
            Console.WriteLine("BoardVisualizer destroyed and cleaned up all tile visuals");
        }
    }
}