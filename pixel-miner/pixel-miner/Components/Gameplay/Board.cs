using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.Core;
using SFML.System;

namespace pixel_miner.World
{
    public class Board : Component
    {
        public int TileSize { get; set; } = 32;

        private Dictionary<GridPosition, Tile> tiles = new Dictionary<GridPosition, Tile>();

        public Board(){}

        public override void Start()
        {
            InitializeGrid();
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

        private void InitializeGrid()
        {
            for (int x = -10; x < 10; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    var position = new GridPosition(x, y);
                    tiles[position] = new Tile(position, fuelCost: 1, canMoveTo: true);
                }
            }
        }

        public IEnumerable<GridPosition> GetAllTilePositions()
        {
            return tiles.Keys;
        }

        public bool HasTile(GridPosition position)
        {
            return tiles.ContainsKey(position);
        }
    }
}