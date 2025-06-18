using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ModUtility : MonoBehaviour
{
    [Header("Units")]
    [SerializeField]
    private List<UnitDefinitionWrapper> unitDefinitions = new List<UnitDefinitionWrapper>();

    [Header("Nations")]
    [SerializeField]
    private List<NationDefinitionWrapper> nationDefinitions = new List<NationDefinitionWrapper>();

    [Header("Technologies")]
    [SerializeField]
    private List<TechnologyDefinitionWrapper> technologyDefinitions = new List<TechnologyDefinitionWrapper>();

    [Header("Tile Types")]
    [SerializeField]
    private List<TileTypeWrapper> tileTypeDefinitions = new List<TileTypeWrapper>();

    [Header("Tile Extras")]
    [SerializeField]
    private List<TileExtraDefinitionWrapper> tileExtraDefinitions = new List<TileExtraDefinitionWrapper>();

    [Header("Buildings")]
    [SerializeField]
    private List<BuildingDefinitionWrapper> buildingDefinitions = new List<BuildingDefinitionWrapper>();

    [Header("Government Effects")]
    [SerializeField]
    private List<GovernmentEffectsWrapper> governmentEffects = new List<GovernmentEffectsWrapper>();

    // Wrapper classes
    [System.Serializable]
    public class UnitDefinitionWrapper
    {
        public string gameName;
        public UnitDefinition definition;
    }

    [System.Serializable]
    public class NationDefinitionWrapper
    {
        public int nationID;
        public NationDefinition definition;
    }

    [System.Serializable]
    public class TechnologyDefinitionWrapper
    {
        public string techName;
        public TechnologyDefinition definition;
    }

    [System.Serializable]
    public class TileTypeWrapper
    {
        public TileTypeName name;
        public TileType definition;
    }

    [System.Serializable]
    public class TileExtraDefinitionWrapper
    {
        public TileExtraType type;
        public TileExtraDefinition definition;
    }

    [System.Serializable]
    public class BuildingDefinitionWrapper
    {
        public string buildingName;
        public BuildingDefinition definition;
    }

    [System.Serializable]
    public class GovernmentEffectsWrapper
    {
        public GovernmentType type;
        public GovernmentEffects effects;
    }

    // Method to push changes from the Inspector to staticHolder
    public void ApplyModChanges()
    {
        Debug.Log("Applying Mod Changes...");

        // Clear existing staticHolder data
        staticHolder.unitDefinitions.Clear();
        staticHolder.nationDefinitions.Clear();
        staticHolder.technologyDefinitions.Clear();
        staticHolder.tileTypeDefinitions.Clear();
        staticHolder.tileExtraDefinitions.Clear();
        staticHolder.buildingDefinitions.Clear();
        staticHolder.governmentEffects.Clear();

        // Populate staticHolder dictionaries from the wrapper lists
        foreach (var wrapper in unitDefinitions)
        {
            if (!string.IsNullOrEmpty(wrapper.gameName))
            {
                staticHolder.unitDefinitions[wrapper.gameName] = wrapper.definition;
            }
        }

        foreach (var wrapper in nationDefinitions)
        {
            staticHolder.nationDefinitions[wrapper.nationID] = wrapper.definition;
        }

        foreach (var wrapper in technologyDefinitions)
        {
            if (!string.IsNullOrEmpty(wrapper.techName))
            {
                staticHolder.technologyDefinitions[wrapper.techName] = wrapper.definition;
            }
        }

        foreach (var wrapper in tileTypeDefinitions)
        {
            staticHolder.tileTypeDefinitions[wrapper.name] = wrapper.definition;
        }

        foreach (var wrapper in tileExtraDefinitions)
        {
            staticHolder.tileExtraDefinitions[wrapper.type] = wrapper.definition;
        }

        foreach (var wrapper in buildingDefinitions)
        {
            if (!string.IsNullOrEmpty(wrapper.buildingName))
            {
                staticHolder.buildingDefinitions[wrapper.buildingName] = wrapper.definition;
            }
        }

        foreach (var wrapper in governmentEffects)
        {
            staticHolder.governmentEffects[wrapper.type] = wrapper.effects;
        }

        Debug.Log("Mod Changes Applied.");
    }

    // Method to pull data from staticHolder to the Inspector (useful after initial setup or loading)
    public void LoadStaticDataIntoInspector()
    {
        Debug.Log("Loading Static Data into Inspector...");

        // Clear existing wrapper lists
        unitDefinitions.Clear();
        nationDefinitions.Clear();
        technologyDefinitions.Clear();
        tileTypeDefinitions.Clear();
        tileExtraDefinitions.Clear();
        buildingDefinitions.Clear();
        governmentEffects.Clear();

        // Populate wrapper lists from staticHolder dictionaries
        foreach (var entry in staticHolder.unitDefinitions)
        {
            unitDefinitions.Add(new UnitDefinitionWrapper { gameName = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.nationDefinitions)
        {
            nationDefinitions.Add(new NationDefinitionWrapper { nationID = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.technologyDefinitions)
        {
            technologyDefinitions.Add(new TechnologyDefinitionWrapper { techName = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.tileTypeDefinitions)
        {
            tileTypeDefinitions.Add(new TileTypeWrapper { name = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.tileExtraDefinitions)
        {
            tileExtraDefinitions.Add(new TileExtraDefinitionWrapper { type = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.buildingDefinitions)
        {
            buildingDefinitions.Add(new BuildingDefinitionWrapper { buildingName = entry.Key, definition = entry.Value });
        }

        foreach (var entry in staticHolder.governmentEffects)
        {
            governmentEffects.Add(new GovernmentEffectsWrapper { type = entry.Key, effects = entry.Value });
        }

        Debug.Log("Static Data Loaded into Inspector.");
    }

    // New methods for saving and loading mod data using JsonUtility
    public void SaveModDataToFile(string filePath)
    {
        ModData modData = new ModData
        {
            unitDefinitions = this.unitDefinitions,
            nationDefinitions = this.nationDefinitions,
            technologyDefinitions = this.technologyDefinitions,
            tileTypeDefinitions = this.tileTypeDefinitions,
            tileExtraDefinitions = this.tileExtraDefinitions,
            buildingDefinitions = this.buildingDefinitions,
            governmentEffects = this.governmentEffects
        };

        string jsonData = JsonUtility.ToJson(modData, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"Mod data saved to {filePath}");
    }

    public void LoadModDataFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string jsonData = File.ReadAllText(filePath);
        ModData modData = JsonUtility.FromJson<ModData>(jsonData);

        if (modData != null)
        {
            this.unitDefinitions = modData.unitDefinitions;
            this.nationDefinitions = modData.nationDefinitions;
            this.technologyDefinitions = modData.technologyDefinitions;
            this.tileTypeDefinitions = modData.tileTypeDefinitions;
            this.tileExtraDefinitions = modData.tileExtraDefinitions;
            this.buildingDefinitions = modData.buildingDefinitions;
            this.governmentEffects = modData.governmentEffects;

            Debug.Log($"Mod data loaded from {filePath}");
        }
        else
        {
            Debug.LogError($"Failed to load mod data from {filePath}");
        }
    }
}

[System.Serializable]
public class ModData
{
    public List<ModUtility.UnitDefinitionWrapper> unitDefinitions = new List<ModUtility.UnitDefinitionWrapper>();
    public List<ModUtility.NationDefinitionWrapper> nationDefinitions = new List<ModUtility.NationDefinitionWrapper>();
    public List<ModUtility.TechnologyDefinitionWrapper> technologyDefinitions = new List<ModUtility.TechnologyDefinitionWrapper>();
    public List<ModUtility.TileTypeWrapper> tileTypeDefinitions = new List<ModUtility.TileTypeWrapper>();
    public List<ModUtility.TileExtraDefinitionWrapper> tileExtraDefinitions = new List<ModUtility.TileExtraDefinitionWrapper>();
    public List<ModUtility.BuildingDefinitionWrapper> buildingDefinitions = new List<ModUtility.BuildingDefinitionWrapper>();
    public List<ModUtility.GovernmentEffectsWrapper> governmentEffects = new List<ModUtility.GovernmentEffectsWrapper>();
}