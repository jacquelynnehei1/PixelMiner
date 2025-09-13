using System.Collections.Generic;
using System.Linq;
using pixel_miner.Utils.Enums;
using pixel_miner.World;
using pixel_miner.World.Tiles;

namespace pixel_miner.Utils
{
    public static class Pathfinder
    {
        /// <summary>
        /// Calculate a path between two grid positions using only cardinal directions
        /// </summary>
        public static Queue<GridPosition> CalculatePath(
            GridPosition from, GridPosition to, PathfindingStrategy strategy = PathfindingStrategy.ManhattanHorizontalFirst)
        {
            return strategy switch
            {
                PathfindingStrategy.ManhattanHorizontalFirst => CalculateManhattanHorizontalFirst(from, to),
                PathfindingStrategy.ManhattanVerticalFirst => CalculateManhattanVerticalFirst(from, to),
                _ => CalculateManhattanHorizontalFirst(from, to),
            };
        }

        /// <summary>
        /// Find a path through the network of empty/passable tiles back to the starting position
        /// </summary>
        public static Queue<GridPosition> FindPathThroughEmptyTiles(GridPosition from, GridPosition to, Board board)
        {
            // Get all passable tiles (empty tiles + grass tiles)
            var passableTiles = GetPassableTiles(board);
            
            // Use A* or BFS to find path through the passable tile network
            return FindPathUsingAStar(from, to, passableTiles);
        }

        private static HashSet<GridPosition> GetPassableTiles(Board board)
        {
            var passableTiles = new HashSet<GridPosition>();
            
            // Get all tile positions from the board
            foreach (var tilePosition in board.GetAllTilePositions())
            {
                var tile = board.GetTile(tilePosition);
                if (tile != null && tile.CanMoveTo)
                {
                    passableTiles.Add(tilePosition);
                }
            }
            
            return passableTiles;
        }

        private static Queue<GridPosition> FindPathUsingAStar(GridPosition start, GridPosition goal, HashSet<GridPosition> passableTiles)
        {
            var openSet = new HashSet<GridPosition> { start };
            var cameFrom = new Dictionary<GridPosition, GridPosition>();
            var gScore = new Dictionary<GridPosition, int> { [start] = 0 };
            var fScore = new Dictionary<GridPosition, int> { [start] = CalculateDistance(start, goal) };

            while (openSet.Count > 0)
            {
                // Get the node with lowest fScore
                var current = openSet.OrderBy(pos => fScore.GetValueOrDefault(pos, int.MaxValue)).First();
                
                if (current.Equals(goal))
                {
                    // Reconstruct path
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);

                // Check all adjacent positions (up, down, left, right)
                var neighbors = GetAdjacentPositions(current);
                
                foreach (var neighbor in neighbors)
                {
                    // Skip if not passable
                    if (!passableTiles.Contains(neighbor))
                        continue;

                    int tentativeGScore = gScore.GetValueOrDefault(current, int.MaxValue) + 1;

                    if (tentativeGScore < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + CalculateDistance(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // No path found
            Console.WriteLine("No path found through empty tiles network!");
            return new Queue<GridPosition>();
        }

        private static List<GridPosition> GetAdjacentPositions(GridPosition position)
        {
            return new List<GridPosition>
            {
                new GridPosition(position.X, position.Y - 1), // Up
                new GridPosition(position.X, position.Y + 1), // Down
                new GridPosition(position.X - 1, position.Y), // Left
                new GridPosition(position.X + 1, position.Y)  // Right
            };
        }

        private static Queue<GridPosition> ReconstructPath(Dictionary<GridPosition, GridPosition> cameFrom, GridPosition current)
        {
            var totalPath = new List<GridPosition>();
            
            while (cameFrom.ContainsKey(current))
            {
                totalPath.Add(current);
                current = cameFrom[current];
            }
            
            // Reverse to get path from start to goal
            totalPath.Reverse();
            
            return new Queue<GridPosition>(totalPath);
        }

        // Keep existing methods for backward compatibility
        private static Queue<GridPosition> CalculateManhattanHorizontalFirst(GridPosition from, GridPosition to)
        {
            var path = new Queue<GridPosition>();
            var current = from;

            current = HandleHorizontalMoves(current, to, ref path);
            current = HandleVerticalMoves(current, to, ref path);

            return path;
        }

        private static Queue<GridPosition> CalculateManhattanVerticalFirst(GridPosition from, GridPosition to)
        {
            var path = new Queue<GridPosition>();
            var current = from;

            current = HandleVerticalMoves(current, to, ref path);
            current = HandleHorizontalMoves(current, to, ref path);

            return path;
        }

        private static GridPosition HandleHorizontalMoves(GridPosition current, GridPosition target, ref Queue<GridPosition> path)
        {
            while (current.X != target.X)
            {
                int deltaX = current.X > target.X ? -1 : 1;
                current = new GridPosition(current.X + deltaX, current.Y);
                path.Enqueue(current);
            }

            return current;
        }

        private static GridPosition HandleVerticalMoves(GridPosition current, GridPosition target, ref Queue<GridPosition> path)
        {
            while (current.Y != target.Y)
            {
                int deltaY = current.Y > target.Y ? -1 : 1;
                current = new GridPosition(current.X, current.Y + deltaY);
                path.Enqueue(current);
            }

            return current;
        }

        public static int CalculateDistance(GridPosition from, GridPosition to)
        {
            return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        }
    }
}