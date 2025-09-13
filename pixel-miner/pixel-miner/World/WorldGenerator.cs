using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pixel_miner.World.Tiles;

namespace pixel_miner.World
{
    public static class WorldGenerator
    {
        private static Random random = new Random();

        public class TileConfig
        {
            public Type TileType { get; set; }
            public int PreferredDepthStart { get; set; } = 0;
            public int PreferredDepthEnd { get; set; } = int.MaxValue;
            public float PeakProbability { get; set; } = 50f;
            public float MinProbability { get; set; } = 0f;
            public float MaxProbability { get; set; } = 100f;
        }

        private static readonly List<TileConfig> TileConfigs = new List<TileConfig>
        {
            new TileConfig
            {
                TileType = typeof(GrassTile),
                PreferredDepthStart = 0,
                PreferredDepthEnd = 0,
                PeakProbability = 100f,
                MinProbability = 0f,
            },
            new TileConfig
            {
                TileType = typeof(DirtTile),
                PreferredDepthStart = 1,
                PreferredDepthEnd = 8,
                PeakProbability = 70f,
                MinProbability = 5f
            },
            new TileConfig
            {
                TileType = typeof(StoneTile),
                PreferredDepthStart = 3,
                PreferredDepthEnd = 20,
                PeakProbability = 60f,
                MinProbability = 2f
            },
            new TileConfig
            {
                TileType = typeof(IronOreTile),
                PreferredDepthStart = 8,
                PreferredDepthEnd = 25,
                PeakProbability = 25f,
                MinProbability = 1f
            },
            new TileConfig
            {
                TileType = typeof(PreciousGemTile),
                PreferredDepthStart = 15,
                PreferredDepthEnd = int.MaxValue,
                PeakProbability = 20f,
                MinProbability = 0.5f
            },
            new TileConfig
            {
                TileType = typeof(FuelCrystalTile),
                PreferredDepthStart = 20,
                PreferredDepthEnd = int.MaxValue,
                PeakProbability = 8f,
                MinProbability = 0.1f
            }
        };

        public static Tile GenerateTile(GridPosition position, int surfaceRow)
        {
            int depthFromSurface = position.Y - surfaceRow;

            if (depthFromSurface <= 0)
            {
                return new GrassTile(position);
            }

            var probabilities = CalculateTileProbabilities(depthFromSurface);

            return CreateTileFromProbabilities(position, probabilities);
        }

        private static Dictionary<Type, float> CalculateTileProbabilities(int depth)
        {
            var probabilities = new Dictionary<Type, float>();

            foreach (var config in TileConfigs)
            {
                if (config.TileType == typeof(GrassTile) && depth > 0)
                {
                    continue;
                }

                float probability = CalculateProbabilityForDepth(config, depth);
                if (probability > 0)
                {
                    probabilities[config.TileType] = probability;
                }
            }

            return probabilities;
        }

        private static float CalculateProbabilityForDepth(TileConfig config, int depth)
        {
            if (depth >= config.PreferredDepthStart && depth <= config.PreferredDepthEnd)
            {
                return config.PeakProbability;
            }

            int distanceFromRange;
            if (depth < config.PreferredDepthStart)
            {
                distanceFromRange = config.PreferredDepthStart - depth;
            }
            else
            {
                distanceFromRange = depth - config.PreferredDepthEnd;
            }

            float fallOffRate = 0.7f;
            float probability = config.PeakProbability * (float)Math.Pow(fallOffRate, distanceFromRange);

            return Math.Max(probability, config.MinProbability);
        }

        private static Tile CreateTileFromProbabilities(GridPosition position, Dictionary<Type, float> probabilities)
        {
            if (!probabilities.Any())
            {
                return new DirtTile(position);
            }

            float totalWeight = probabilities.Values.Sum();
            float randomValue = (float)random.NextDouble() * totalWeight;
            float currentWeight = 0f;

            foreach (var probability in probabilities)
            {
                currentWeight += probability.Value;
                if (randomValue <= currentWeight)
                {
                    return CreateTileOfType(probability.Key, position);
                }
            }

            return new DirtTile(position);
        }

        private static Tile CreateTileOfType(Type tileType, GridPosition position)
        {
            return tileType.Name switch
            {
                nameof(DirtTile) => new DirtTile(position),
                nameof(StoneTile) => new StoneTile(position),
                nameof(IronOreTile) => new IronOreTile(position),
                nameof(PreciousGemTile) => new PreciousGemTile(position),
                nameof(FuelCrystalTile) => new FuelCrystalTile(position),
                nameof(GrassTile) => new GrassTile(position),
                _ => new DirtTile(position)
            };
        }

        public static void RegisterTileType(TileConfig config)
        {
            TileConfigs.Add(config);
        }

        public static string GetDepthInfo(int depthFromSurface)
        {
            var probabilities = CalculateTileProbabilities(depthFromSurface);
            var info = $"Depth {depthFromSurface}";

            foreach (var probability in probabilities.OrderByDescending(x => x.Value))
            {
                info += $"{probability.Key.Name}: {probability.Value:F1}%";
            }

            return info;
        }

        public static void SetSeed(int seed)
        {
            random = new Random(seed);
        }
    }
}