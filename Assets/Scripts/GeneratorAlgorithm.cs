using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Generators
{
    public interface IAlgorithm
    {
        void GenerateTerrain(TerrainInfo terrain);
    }

    public class PerimeterWall : IAlgorithm
    {
        public void GenerateTerrain(TerrainInfo terrain)
        {
            foreach (var tile in terrain.tiles.Where(t => terrain.IsEdge(t)))
            {
                tile.walkable = false;
            }
        }
    }

    public class RandomWalkableTiles : IAlgorithm
    {
        public float walkableChance = 0.5f;

        public void GenerateTerrain(TerrainInfo terrain)
        {
            foreach (var tile in terrain.tiles)
            {
                if (UnityEngine.Random.value > walkableChance)
                {
                    tile.walkable = true;
                }
            }
        }
    }

    // Mark cells as walkable (alive) or not walkable (dead) based on the status of their neighbors.
    // Loosely based on https://gamedevelopment.tutsplus.com/tutorials/generate-random-cave-levels-using-cellular-automata--gamedev-9664
    public class GameOfLifeAutomata : IAlgorithm
    {
        // Kill living cells with too few living neighbors.
        public int starvationLimit = 2;

        // Kill living cells with too many living neighbors.
        public int crowdingLimit = 3;

        // Resurrect dead cells with enough living neighbors.
        public int birthLimit = 3;

        // Repeat the check this many times.
        public int iterations = 10;

        public void GenerateTerrain(TerrainInfo terrain)
        {
            for (int i = 0; i < iterations; i++)
            {
                GenerateTerrainInternal(terrain);
            }
        }

        private void GenerateTerrainInternal(TerrainInfo original)
        {
            // Apply changes from this iteration to a copy first to prevent the algorithm from seeing a mix of old and updated cells.
            TerrainInfo clone = original.Clone();
            foreach (var tile in clone.tiles)
            {
                List<TerrainInfo.Tile> neighbors = original.GetNeighbors(tile);
                int numLivingNeighbors = neighbors.Where(t => t.walkable).Count();
                if (tile.walkable && (numLivingNeighbors < starvationLimit || numLivingNeighbors > crowdingLimit))
                {
                    tile.walkable = false;
                }
                else if (!tile.walkable && numLivingNeighbors >= birthLimit)
                {
                    tile.walkable = true;
                }
            }
            original.tiles = clone.tiles;
        }
    }

    // Remove diagonals between walkable cells by doing one of the following:
    //   * Open one mutually adjacent unwalkable neighbor.
    //   * Make one of the cells unwalkable.
    // Note: This generator makes more sense when logical tiles are the same as graphical tiles.
    public class RemoveDiagonals : IAlgorithm
    {
        public int iterations = 1;

        public void GenerateTerrain(TerrainInfo terrain)
        {
            for (int i = 0; i < iterations; i++)
            {
                GenerateTerrainInternal(terrain);
            }
        }

        private bool IsOpenUnreachableDiagonal(TerrainInfo terrain, TerrainInfo.Tile tile, TerrainInfo.Tile other)
        {
            // Ignore unwalkable cells
            if (!other.walkable) return false;

            // Ignore adjacent cells
            if (other.X == tile.X || other.Y == tile.Y) return false;

            // Ignore diagonal cells that can be reached via a mutually adjacent cell
            return !terrain[tile.X, other.Y].walkable && !terrain[other.X, tile.Y].walkable;
        }

        private void GenerateTerrainInternal(TerrainInfo terrain)
        {
            foreach (var tile in terrain.tiles.Where(t => t.walkable))
            {
                List<TerrainInfo.Tile> neighbors = terrain.GetNeighbors(tile);
                var diagonals = neighbors.Where(t => IsOpenUnreachableDiagonal(terrain, tile, t)).ToList();
                if (diagonals.Count == 0) continue;

                // Make the tile unwalkable if it isn't reachable from any of its neighbors
                if (neighbors.Count(n => n.walkable) == diagonals.Count)
                {
                    tile.walkable = false;
                    continue;
                }

                foreach (var diagonal in diagonals)
                {
                    // We already established that neither of these tiles are walkable
                    var adjacent1 = terrain[tile.X, diagonal.Y];
                    var adjacent2 = terrain[diagonal.X, tile.Y];

                    // Open the tile with the most walkable neighbors
                    if (terrain.GetNeighbors(adjacent1).Where(n => n.walkable).Count() > terrain.GetNeighbors(adjacent2).Where(n => n.walkable).Count())
                    {
                        adjacent1.walkable = true;
                    }
                    else
                    {
                        adjacent2.walkable = true;
                    }
                }
            }

        }
    }

    public class PruneUnreachableAreas : IAlgorithm
    {
        public void GenerateTerrain(TerrainInfo terrain)
        {
            var playerSpawn = terrain.GetExit();
            var reachable = terrain.GetReachable(playerSpawn);
            foreach (var tile in terrain.tiles.Except(reachable).Where(t => t.walkable))
            {
                tile.walkable = false;
            }
        }
    }

    public class CreatePlayerSpawn : IAlgorithm
    {
        // Maximum number of times to try placing the player.
        public int maxTries;

        public void GenerateTerrain(TerrainInfo terrain)
        {
            var walkableTiles = terrain.tiles.Where(t => t.walkable);

            var potentialSpawns = new Dictionary<TerrainInfo.Tile, int>(maxTries);
            for (int i = 0; i < maxTries; i++)
            {
                var spawnPoint = walkableTiles.ElementAt(UnityEngine.Random.Range(0, walkableTiles.Count()));

                var reachable = terrain.GetReachable(spawnPoint);
                potentialSpawns.Add(spawnPoint, reachable.Count);

                walkableTiles = walkableTiles.Except(reachable);
                if (walkableTiles.Count() == 0) break;
            }

            // Select the potential spawn point with access to the most walkable tiles.
            potentialSpawns.Aggregate((a, b) => a.Value > b.Value ? a : b).Key.feature = TerrainInfo.Tile.Feature.PlayerSpawn;
        }
    }

    public class SpawnFeatures : IAlgorithm
    {
        public int quantity = 1;
        public TerrainInfo.Tile.Feature feature;

        public void GenerateTerrain(TerrainInfo terrain)
        {
            var openTiles = terrain.tiles.Where(t => t.walkable && t.feature == TerrainInfo.Tile.Feature.None);
            for (int i = 0; i < quantity; i++)
            {
                var tile = openTiles.ElementAt(Random.Range(0, openTiles.Count()));
                tile.feature = feature;
                openTiles = openTiles.Except(new List<TerrainInfo.Tile>{ tile });
            }
        }
    }

    public class Scale : IAlgorithm
    {
        public Vector2Int scale = new Vector2Int(2, 2);

        public void GenerateTerrain(TerrainInfo terrain)
        {
            System.Diagnostics.Debug.Assert(scale.x > 0 && scale.y > 0);
            System.Diagnostics.Debug.Assert(scale.x > 1 || scale.y > 1);

            var clone = new TerrainInfo(new Vector2Int(terrain.Size.x * scale.x, terrain.Size.y * scale.y));
            foreach (var tile in terrain.tiles)
            {
                Vector2Int basePos = tile.position;
                basePos.Scale(scale);

                for (int x = 0; x < scale.x; x++)
                {
                    for (int y = 0; y < scale.y; y++)
                    {
                        var newPos = basePos + new Vector2Int(x, y);
                        clone[newPos] = new TerrainInfo.Tile
                        {
                            walkable = tile.walkable,
                            position = newPos
                        };
                    }
                }
            }

            terrain.Apply(clone);
        }
    }
}
