using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// Holds basic information about terrain during terrain generation.
//
// We don't directly set the tilemap to separate the logic determining map shape & features from the logic for rendering the map.
public class TerrainInfo
{
    // A logical tile for the purposes of terrain generation.
    // This isn't necessarily the same size as a graphical tile!
    public class Tile
    {
        public enum Feature
        {
            None,
            ItemSpawn,
            MushroomSpawn,
            PlayerSpawn,
        }
        public Feature feature;
        public bool walkable;
        public Vector2Int position;

        public int X { get => position.x; }
        public int Y { get => position.y; }
    }

    public List<Tile> tiles;
    public Vector2Int Size { get; private set; }

    public TerrainInfo(Vector2Int size)
    {
        Size = size;
        tiles = new List<Tile>(Size.x * Size.y);
        for (int column = 0; column < Size.x; column++)
        {
            for (int row = 0; row < size.y; row++)
            {
                tiles.Add(new Tile
                {
                    feature = Tile.Feature.None,
                    walkable = false,
                    position = new Vector2Int(column, row)
                });
            }
        }
    }

    private int IndexOf(int x, int y)
    {
        return x * Size.y + y;
    }

    public Tile this[int x, int y]
    {
        get => tiles[IndexOf(x, y)];
        set => tiles[IndexOf(x, y)] = value;
    }

    public Tile this[Vector2Int position]
    {
        get => this[position.x, position.y];
        set => this[position.x, position.y] = value;
    }

    public bool IsEdge(Tile tile)
    {
        return tile.X == 0 || tile.Y == 0 || tile.X == Size.x - 1 || tile.Y == Size.y - 1;
    }

    // Return all surrounding tiles, adjacent and diagonal.
    public List<Tile> GetNeighbors(Tile tile)
    {
        int minX = Math.Max(tile.X - 1, 0);
        int maxX = Math.Min(tile.X + 2, Size.x);
        int minY = Math.Max(tile.Y - 1, 0);
        int maxY = Math.Min(tile.Y + 2, Size.y);

        var neighbors = new List<Tile>((maxX - minX) * (maxY - minY));
        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                if (x != tile.X || y != tile.Y)
                {
                    neighbors.Add(this[x, y]);
                }
            }
        }
        return neighbors;
    }

    public List<Tile> GetAdjacent(Tile tile)
    {
        return GetNeighbors(tile).Where(n => n.X == tile.X || n.Y == tile.Y).ToList();
    }

    private void GetReachableInternal(Tile tile, List<Tile> examinedTiles, List<Tile> reachableTiles)
    {
        if (examinedTiles.Contains(tile)) return;
        examinedTiles.Add(tile);

        if (!tile.walkable) return;
        reachableTiles.Add(tile);

        foreach (var neighbor in GetAdjacent(tile))
        {
            GetReachableInternal(neighbor, examinedTiles, reachableTiles);
        }
    }

    public List<Tile> GetReachable(Tile tile)
    {
        var examinedTiles = new List<Tile>();
        var reachableTiles = new List<Tile>();
        GetReachableInternal(tile, examinedTiles, reachableTiles);
        return reachableTiles;
    }

    public Tile GetExit()
    {
        return tiles.Where(t => t.feature == Tile.Feature.PlayerSpawn).ElementAt(0);
    }

    public TerrainInfo Clone()
    {
        var clone = (TerrainInfo)MemberwiseClone();
        clone.tiles = new List<Tile>(tiles.Count);
        foreach (var tile in tiles)
        {
            clone.tiles.Add(new Tile
            {
                feature = tile.feature,
                walkable = tile.walkable,
                position = tile.position
            });
        }
        return clone;
    }

    public void Apply(TerrainInfo other)
    {
        tiles = other.tiles;
        Size = other.Size;
    }
}
