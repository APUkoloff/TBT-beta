using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager
{
    public void SaveGame(string fileName, GameState gameState)
    {
        string jsonData = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", jsonData);
    }

    public GameState LoadGame(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameState>(jsonData);
        }
        return null;
    }
}

[System.Serializable]
public class GameState
{
    public List<CityData> cities = new List<CityData>();
    public List<TileData> tiles = new List<TileData>();
    public List<ForceData> forces = new List<ForceData>();
    public Dictionary<int, int> nationTreasuries = new Dictionary<int, int>();
    public Dictionary<int, Dictionary<int, int>> relations = new Dictionary<int, Dictionary<int, int>>();
}

[System.Serializable]
public class CityData
{
    public string name;
    public int population;
    public int conscripts;
    public int production;
    public int gold;
    public int scienceOutput;
    public List<ProductionItemData> productionQueue = new List<ProductionItemData>();
}

[System.Serializable]
public class ProductionItemData
{
    public string type;
    public string name;
    public int cost;
    public int progress;
}

[System.Serializable]
public class TileData
{
    public Vector2Int gridPosition;
    public TileTypeName terrainType;
}

[System.Serializable]
public class ForceData
{
    public Vector2Int position;
    public List<UnitData> units = new List<UnitData>();
}

[System.Serializable]
public class UnitData
{
    public string gameName;
    public int health;
}
