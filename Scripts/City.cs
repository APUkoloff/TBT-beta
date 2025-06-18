using UnityEngine;
using System.Collections.Generic;

public class City : MonoBehaviour
{
    public string cityName;
    public int population;
    public int conscripts;
    public int production;
    public int gold;
    public int scienceOutput;
    public List<ProductionItem> productionQueue = new List<ProductionItem>();
    public List<Building> buildings = new List<Building>();

    public City(string name, int pop)
    {
        cityName = name;
        population = pop;
        conscripts = (int)(pop * 0.1f);
        production = 0;
        gold = 0;
        scienceOutput = 0;
    }

    void Start()
    {
        // При старте города применяем бонусы от тайла
        ApplyTileBonuses();
    }

    public void ManufacterPerTurn()
    {
        if (productionQueue.Count > 0)
        {
            ProductionItem currentItem = productionQueue[0];
            currentItem.progress += production;
            if (currentItem.progress >= currentItem.cost)
            {
                productionQueue.RemoveAt(0);
                // Создать юнит или здание
            }
        }
    }

    // Метод для получения текущего тайла
    public Tile GetCurrentTile()
    {
        Vector2Int gridPos = GridManager.Instance.GetGridPosition(transform.position);
        return GridManager.Instance.GetTileAt(gridPos);
    }

    // Метод для получения тайла с использованием Raycast
    public Tile GetCurrentTileWithRaycast()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 10, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null;
    }

    public void AddToProductionQueue(ProductionItem item)
    {
        productionQueue.Add(item);
    }

    public Missile CreateMissile(string missileType, int range, int damage)
    {
        Tile currentTile = GetCurrentTile();
        if (currentTile == null)
        {
            Debug.LogWarning("Current tile not found for city!");
            return null;
        }

        Missile missile = new Missile(missileType, range, damage);
        Force forceOnTile = FindForceAtPosition(transform.position);
        if (forceOnTile != null)
        {
            forceOnTile.AddMissile(missile);
            Debug.Log($"Missile {missileType} added to force {forceOnTile.forceName}.");
            return null;
        }
        else
        {
            Debug.Log($"Missile {missileType} created in city {cityName}.");
            return missile;
        }
    }

    // Метод для поиска группы по позиции
    private Force FindForceAtPosition(Vector3 position)
    {
        Force[] forces = FindObjectsOfType<Force>();
        foreach (Force force in forces)
        {
            if (Vector3.Distance(force.transform.position, position) < 1f)
            {
                return force;
            }
        }
        return null;
    }

    // Метод для применения бонусов от тайла
    public void ApplyTileBonuses()
    {
        Tile currentTile = GetCurrentTile();
        if (currentTile != null)
        {
            // Применяем бонусы от тайла к городу
            production = currentTile.baseProduction;
            gold = currentTile.baseTrade;
            Debug.Log($"Applied resource factor for {cityName}: Production +{production}, Gold +{gold}");
        }
    }

    // Метод для улучшения текущего тайла (STUB)
    public void ImproveCurrentTile()
    {
        Tile currentTile = GetCurrentTile();
        if (currentTile != null)
        {
            currentTile.baseProduction += 1;
            Debug.Log($"Improved tile at {currentTile.gridPosition}. New production bonus: {currentTile.baseProduction}");
        }
    }

    // Метод для изменения типа текущего тайла (STUB)
    public void ChangeTileType(TileTypeName newType)
    {
        Tile currentTile = GetCurrentTile();
        if (currentTile != null)
        {
            Vector2Int gridPos = currentTile.gridPosition;
            GridManager.Instance.ChangeTileType(gridPos, newType);
        }
    }
}

public class ProductionItem
{
    public string type; // "Unit" или "Building"
    public string name;
    public int cost;
    public int progress;
}

public class Building
{
    public string name;
    public int productionBonus;
    public int goldBonus;
    public int scienceBonus;
    public int defenseBonus;
}