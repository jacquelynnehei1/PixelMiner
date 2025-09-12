using pixel_miner.Core;
using pixel_miner.World.Tiles;
using SFML.System;

namespace pixel_miner.World
{
    public class Board : Component
    {
        public float TileSize { get; set; } = 32;
        public int SurfaceDepth { get; private set; }

        // Dynamic loading configuration
        private int loadAheadDistance = 15;
        private int rowsToLoadAtOnce = 15;

        //Grid bounds tracking
        private int currentTopRow;
        private int currentBottomRow;
        private int gridColumns;

        public event Action<IEnumerable<GridPosition>>? OnTilesAdded;
        public event Action<IEnumerable<GridPosition>>? OnTilesRemoved;

        private Dictionary<GridPosition, Tile> tiles = new Dictionary<GridPosition, Tile>();

        public Board() { }

        public void InitializeGrid(int columns, int surfaceRows = 5, int undergroundRows = 15)
        {
            ClearBoard();
            gridColumns = columns;

            var mainCamera = CameraManager.GetMainCamera();
            if (mainCamera != null)
            {
                var viewportWidth = mainCamera.ViewSize.X;
                var viewportHeight = mainCamera.ViewSize.Y;

                TileSize = viewportWidth / columns;

                float gridStartY = -(viewportHeight / 4f);
                currentTopRow = (int)(gridStartY / TileSize);
                currentBottomRow = currentTopRow + surfaceRows + undergroundRows;

                SurfaceDepth = surfaceRows;

                Console.WriteLine($"Initial grid: Top row {currentTopRow}, Bottom row {currentBottomRow}");
            }

            GenerateRowRange(currentTopRow, currentBottomRow);
        }

        private void GenerateRowRange(int startRow, int endRow)
        {
            var newTiles = new List<GridPosition>();

            for (int col = -(gridColumns / 2); col <= (gridColumns / 2); col++)
            {
                for (int row = startRow; row <= endRow; row++)
                {
                    var gridPosition = new GridPosition(col, row);

                    if (!tiles.ContainsKey(gridPosition))
                    {
                        var tile = CreateTileForPosition(gridPosition);
                        tiles.Add(gridPosition, tile);
                        newTiles.Add(gridPosition);
                    }
                }
            }

            Console.WriteLine($"Generated rows {startRow} to {endRow}. Total tiles: {tiles.Count}");

            if (newTiles.Count > 0)
            {
                OnTilesAdded?.Invoke(newTiles);
            }

        }

        private Tile CreateTileForPosition(GridPosition position)
        {
            int depth = position.Y - currentTopRow;

            if (depth < SurfaceDepth)
            {
                return new GrassTile(position);
            }

            return new DirtTile(position);
        }

        public void CheckAndExpandGrid(GridPosition position)
        {
            int distanceFromBottom = currentBottomRow - position.Y;

            if (distanceFromBottom <= loadAheadDistance)
            {
                ExpandGrid();
            }
        }

        private void ExpandGrid()
        {
            int newBottomRow = currentBottomRow + rowsToLoadAtOnce;
            Console.WriteLine($"Expanding grid down from row {currentBottomRow} to {newBottomRow}");

            GenerateRowRange(currentBottomRow, newBottomRow);
            currentBottomRow = newBottomRow;
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

        public int GetTopRowIndex()
        {
            if (tiles.Count == 0) return 0;

            return tiles.Keys.Min(pos => pos.Y);
        }

        public int GetBottomRowIndex()
        {
            if (tiles.Count == 0) return 0;

            return tiles.Keys.Max(pos => pos.Y);
        }

        public bool TryMineTile(GridPosition targetPosition, out ResourceDrop? resource)
        {
            resource = null;

            var tile = GetTile(targetPosition);
            if (tile == null || tile is not MinableTile minableTile)
            {
                return false;
            }

            resource = minableTile.Mine();

            SetTile(targetPosition, new EmptyTile(targetPosition));

            return true;
        }
    }
}