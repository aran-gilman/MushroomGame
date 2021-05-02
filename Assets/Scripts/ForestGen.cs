using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestGen : MonoBehaviour
{
    public int randomSeed = -1;

    public Vector2Int baseSize = new Vector2Int(10, 10);
    public Vector2Int scale = new Vector2Int(2, 2);

    public static bool isLuckStoneActive;

    [Serializable]
    public class Style
    {
        public Tile wall;
        public Tile ground;
    }
    public Style style;

    [Serializable]
    public class SpawnInfo
    {
        public GameObject prefab;
        public int weight = 1;
        public bool requiresLuckStone = false;
    }

    [Serializable]
    public class Options
    {
        public float walkableChance = 0.35f;
        public int starvationLimit = 2;
        public int crowdingLimit = 5;
        public int birthLimit = 3;
        public int cellularAutomataIterations = 1;
        public int maxPlayerPlacementTries = 5;
        public int numMushroomsSpawned = 10;
        public List<SpawnInfo> mushrooms = new List<SpawnInfo>();
        public int numItemsSpawned = 2;
        public List<SpawnInfo> items = new List<SpawnInfo>();
    }
    public Options options;

    public class TilemapHolder
    {
        public Tilemap background;
        public Tilemap collision;
    }
    private TilemapHolder tilemaps;
    
    private GameObject player;
    private GameObject teleporter;

    private Grid grid;

    public void SetupLevel()
    {
        if (randomSeed != -1)
        {
            UnityEngine.Random.InitState(randomSeed);
        }
        var terrainInfo = new TerrainInfo(baseSize);

        var generators = new List<Generators.IAlgorithm>()
        {
            new Generators.RandomWalkableTiles {
                walkableChance = options.walkableChance
            },
            new Generators.GameOfLifeAutomata {
                starvationLimit = options.starvationLimit,
                crowdingLimit = options.crowdingLimit,
                birthLimit = options.birthLimit,
                iterations = options.cellularAutomataIterations
            },
            new Generators.RemoveDiagonals(),
            new Generators.PerimeterWall(),
            new Generators.Scale
            {
                scale = scale
            },
            new Generators.CreatePlayerSpawn {
                maxTries = options.maxPlayerPlacementTries
            },
            new Generators.PruneUnreachableAreas(),
            new Generators.SpawnFeatures
            {
                quantity = options.numMushroomsSpawned,
                feature = TerrainInfo.Tile.Feature.MushroomSpawn
            },
            new Generators.SpawnFeatures
            {
                quantity = options.numItemsSpawned,
                feature = TerrainInfo.Tile.Feature.ItemSpawn
            }
        };

        foreach (var gen in generators)
        {
            gen.GenerateTerrain(terrainInfo);
        }

        tilemaps.background.ClearAllTiles();
        tilemaps.collision.ClearAllTiles();

        tilemaps.background.size = new Vector3Int(baseSize.x * scale.x, baseSize.y * scale.y, 0);
        tilemaps.collision.size = tilemaps.background.size;

        foreach (var tile in terrainInfo.tiles)
        {
            var position = LogicalToGraphical(tile.position);
            tilemaps.background.SetTile(position, style.ground);
            if (!tile.walkable)
            {
                tilemaps.collision.SetTile(position, style.wall);
            }

            switch (tile.feature)
            {
                case TerrainInfo.Tile.Feature.None:
                    break;
                case TerrainInfo.Tile.Feature.PlayerSpawn:
                    Vector3 pos = CellToWorld(tile);
                    player.transform.position = pos;
                    teleporter.transform.position = pos;
                    break;
                case TerrainInfo.Tile.Feature.MushroomSpawn:
                    List<SpawnInfo> mushrooms = options.mushrooms
                        .Where(m => m.requiresLuckStone == isLuckStoneActive)
                        .ToList();
                    GameObject mushroom = Instantiate(
                        SelectWeightedRandom(
                            mushrooms.ToDictionary(e => e.prefab, e => e.weight)));
                    mushroom.transform.position = CellToWorld(tile);
                    isLuckStoneActive = false;
                    break;
                case TerrainInfo.Tile.Feature.ItemSpawn:
                    GameObject item = Instantiate(
                        SelectWeightedRandom(
                            options.items.ToDictionary(e => e.prefab, e => e.weight)));
                    item.transform.position = CellToWorld(tile);
                    break;
            }
        }
    }

    private Vector3Int LogicalToGraphical(Vector2Int position)
    {
        return new Vector3Int(position.x, position.y, 0);
    }

    private Vector3 CellToWorld(TerrainInfo.Tile tile)
    {
        return tilemaps.background.CellToWorld(LogicalToGraphical(tile.position)) + new Vector3(grid.cellSize.x * 0.5f, grid.cellSize.y * 0.5f, 0);
    }
    
    private void Start()
    {
        tilemaps = new TilemapHolder
        {
            background = transform.Find("Ground").gameObject.GetComponent<Tilemap>(),
            collision = transform.Find("Walls").gameObject.GetComponent<Tilemap>()
        };

        grid = GetComponent<Grid>();
        
        player = GameObject.FindWithTag("Player");
        teleporter = GameObject.FindWithTag("Teleporter");

        SetupLevel();
    }
    
    private static T SelectWeightedRandom<T>(IDictionary<T, int> options)
    {
        float val = UnityEngine.Random.Range(0, options.Values.Max() - 1);
        return options
            .OrderByDescending(kv => kv.Value)
            .TakeWhile(kv => kv.Value > val)
            .Last().Key;
    }
}
