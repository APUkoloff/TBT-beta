using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    public float tileSize = 1f;

    private static GridManager instance;
    public static GridManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeGrid(int width, int height)
    {
        if (staticHolder.tilePrefabs.Count == 0)
        {
            Debug.LogError("Tile prefabs are not initialized!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                TileTypeName terrainType = GetRandomTileType();
                TileType tileTypeDef = staticHolder.GetTileTypeDefinition(terrainType);

                GameObject tilePrefab = staticHolder.GetTilePrefab(terrainType);
                if (tilePrefab == null)
                {
                    Debug.LogWarning($"No prefab found for tile type: {terrainType}. Using default prefab.");
                    continue;
                }

                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
                tileObj.name = $"Tile_{x}_{y}_{terrainType}";

                Tile tileComponent = tileObj.GetComponent<Tile>();
                if (tileComponent == null)
                {
                    tileComponent = tileObj.AddComponent<Tile>();
                }
                tileComponent.Initialize(
                    gridPos,
                    terrainType,
                    tileTypeDef.moveCosts[SuperType.Ground],
                    tileTypeDef.production,
                    tileTypeDef.trade
                );

                BoxCollider boxCollider = tileObj.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    boxCollider = tileObj.AddComponent<BoxCollider>();
                }
                boxCollider.size = new Vector3(tileSize, 0.1f, tileSize);
                boxCollider.center = new Vector3(tileSize / 2, -0.05f, tileSize / 2);

                grid.Add(gridPos, tileComponent);
            }
        }
    }

    private TileTypeName GetRandomTileType()
    {
        var tileTypes = System.Enum.GetValues(typeof(TileTypeName));
        TileTypeName randomTileType = (TileTypeName)tileTypes.GetValue(Random.Range(0, tileTypes.Length));
        return randomTileType;
    }

    public Tile GetTileAt(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
        {
            return grid[gridPos];
        }
        return null;
    }

    public Tile GetTileAtWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / tileSize);
        int z = Mathf.FloorToInt(worldPos.z / tileSize);
        Vector2Int gridPos = new Vector2Int(x, z);

        return GetTileAt(gridPos);
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / tileSize);
        int z = Mathf.FloorToInt(worldPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    public void ChangeTileType(Vector2Int gridPos, TileTypeName newType)
    {
        if (grid.ContainsKey(gridPos))
        {
            Tile tile = grid[gridPos];

            TileType newTileDef = staticHolder.GetTileTypeDefinition(newType);
            if (newTileDef == null)
            {
                Debug.LogError($"TileTypeDefinition not found for type: {newType}");
                return;
            }

            TileTypeName oldType = tile.terrainType;
            tile.terrainType = newType;
            tile.movementCost = newTileDef.moveCosts[SuperType.Ground];
            tile.baseProduction = newTileDef.production;
            tile.baseTrade = newTileDef.trade;

            GameObject newTilePrefab = staticHolder.GetTilePrefab(newType);
            if (newTilePrefab != null)
            {
                GameObject tileObj = tile.gameObject;
                Vector3 position = tileObj.transform.position;
                Quaternion rotation = tileObj.transform.rotation;

                Destroy(tileObj);

                GameObject newTileObj = Instantiate(newTilePrefab, position, rotation);
                newTileObj.name = $"Tile_{gridPos.x}_{gridPos.y}_{newType}";

                Tile newTileComponent = newTileObj.GetComponent<Tile>();
                if (newTileComponent == null)
                {
                    newTileComponent = newTileObj.AddComponent<Tile>();
                }
                newTileComponent.Initialize(
                    gridPos,
                    newType,
                    newTileDef.moveCosts[SuperType.Ground],
                    newTileDef.production,
                    newTileDef.trade
                );

                grid[gridPos] = newTileComponent;
            }
            else
            {
                Debug.LogWarning($"No prefab found for new tile type: {newType}");
            }

            Debug.Log($"Changed tile type at {gridPos} from {oldType} to {newType}");
        }
        else
        {
            Debug.LogWarning($"No tile found at grid position: {gridPos}");
        }
    }

    public void ImproveTile(Vector2Int gridPos, int productionBonus, int tradeBonus)
    {
        if (grid.ContainsKey(gridPos))
        {
            Tile tile = grid[gridPos];
            tile.baseProduction += productionBonus;
            tile.baseTrade += tradeBonus;
            Debug.Log($"Improved tile at {gridPos}. New production: {tile.baseProduction}, New trade: {tile.baseTrade}");
        }
        else
        {
            Debug.LogWarning($"No tile found at grid position: {gridPos}");
        }
    }

    public Dictionary<Vector2Int, Tile> GetAllTiles()
    {
        return grid;
    }

    public bool IsPositionInGrid(Vector2Int gridPos)
    {
        return grid.ContainsKey(gridPos);
    }

    public List<Tile> GetNeighboringTiles(Vector2Int gridPos)
    {
        List<Tile> neighbors = new List<Tile>();
        int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighborPos = new Vector2Int(gridPos.x + dx[i], gridPos.y + dy[i]);
            Tile neighborTile = GetTileAt(neighborPos);
            if (neighborTile != null)
            {
                neighbors.Add(neighborTile);
            }
        }

        return neighbors;
    }
}