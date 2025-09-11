using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.Graphics;
using SFML.System;

namespace pixel_miner.World
{
    public class Board : Component
    {
        public float TileSize { get; set; } = 32;

        private Dictionary<GridPosition, Tile> tiles = new Dictionary<GridPosition, Tile>();

        public Board() { }

        public override void Start()
        {
            
        }

        public MoveResult ValidateMove(GridPosition from, GridPosition direction)
        {
            GridPosition target = from + direction;

            Tile? targetTile = GetTile(target);

            if (targetTile == null)
            {
                return MoveResult.Invalid("No tile at target position");
            }

            if (!targetTile.CanMoveTo)
            {
                return MoveResult.Invalid("Cannot move to that tile");
            }

            return MoveResult.Valid(target, targetTile.FuelCost);
        }

        public Tile? GetTile(GridPosition position)
        {
            return tiles.ContainsKey(position) ? tiles[position] : null;
        }

        public void SetTile(GridPosition position, Tile tile)
        {
            tiles[position] = tile;
        }

        public Vector2f GridToWorldPosition(GridPosition gridPosition)
        {
            return new Vector2f(gridPosition.X * TileSize, gridPosition.Y * TileSize);
        }

        public GridPosition WorldToGridPosition(Vector2f worldPosition)
        {
            return new GridPosition(
                (int)(worldPosition.X / TileSize),
                (int)(worldPosition.Y / TileSize)
            );
        }

        public void InitializeGrid(int columns, int rows = 20)
        {
            ClearBoard();

            var mainCamera = CameraManager.GetMainCamera();

            if (mainCamera != null)
            {
                var viewportWidth = mainCamera.ViewSize.X;
                TileSize = viewportWidth / columns;

                Console.WriteLine($"viewport width = {viewportWidth}");
                Console.WriteLine($"Tile Size = {TileSize}");
            }

            Console.WriteLine($"rows / 2 = {rows / 2}");
            Console.WriteLine($"Min = {-(rows / 2)}, Max = {(rows / 2)}");

            Console.WriteLine($"columns / 2 = {columns / 2}");
            Console.WriteLine($"Min = {-(columns / 2)}, Max = {(columns / 2)}");

            for (int col = -(columns / 2); col <= (columns / 2); col++)
            {
                for (int row = -(rows / 2); row <= (rows / 2); row++)
                {
                    var gridPosition = new GridPosition(col, row);
                    var tile = new Tile(gridPosition);

                    tiles.Add(gridPosition, tile);
                }
            }

            Console.WriteLine($"Num tiles = {tiles.Count}");
        }

        public IEnumerable<GridPosition> GetAllTilePositions()
        {
            return tiles.Keys;
        }

        public List<Vector2f> GetAllTileWorldPositions()
        {
            List<Vector2f> worldPositions = new List<Vector2f>();

            foreach (var tile in tiles)
            {
                var gridPosition = tile.Key;
                var worldPosition = GridToWorldPosition(gridPosition);
                worldPositions.Add(worldPosition);
            }

            return worldPositions;
        }

        public bool HasTile(GridPosition position)
        {
            return tiles.ContainsKey(position);
        }

        public void ClearBoard()
        {
            tiles.Clear();
        }
    }
}