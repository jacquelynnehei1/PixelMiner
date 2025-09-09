using System.Collections.Generic;
using pixel_miner.Utils.Enums;
using pixel_miner.World;

namespace pixel_miner.Utils
{
    public static class Pathfinder
    {
        /// <summary>
        /// Calculate a path between two grid positions using only cardinal directions
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
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

        private static Queue<GridPosition> CalculateManhattanHorizontalFirst(GridPosition from, GridPosition to)
        {
            var path = new Queue<GridPosition>();
            var current = from;

            current = HandleHorizontalMoves(current, to, ref path);
            current =HandleVerticalMoves(current, to, ref path);

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

        /// <summary>
        /// Handle all horizontal moves and add them to the path
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handle all vertical moves and add them to the path
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculate Manhattan distance between two positions
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int CalculateDistance(GridPosition from, GridPosition to)
        {
            return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        }

        /// <summary>
        /// Check if two positions are adjacent (one move apart)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool AreAdjacent(GridPosition a, GridPosition b)
        {
            return CalculateDistance(a, b) == 1;
        }
    }
}