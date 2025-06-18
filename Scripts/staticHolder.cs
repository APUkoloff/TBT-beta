using UnityEngine;
using System.Collections.Generic;
using System; // Added for Guid
using System.Linq; // Added for LINQ

public enum SuperType
{
    Ground,
    Air,
    Naval
}

public enum AttackType
{
    Melee,
    Ranged,
    Combined,
    Kamikaze,
    AntiAir // New attack type for Flak
}

public enum UnitRole
{
    Infantry,
    Cavalry,
    Tank,
    Artillery,
    Flak,
    Bomber,
    Fighter,
    GroundStrikeFighter,
    TransportHelicopter,
    AttackHelicopter,
    TransportShip,
    Destroyer,
    Cruiser,
    Battleship,
    Worker,
    Engineer,
    Diplomat,
    Spy,
    Missile,
    SAM
}

public enum GovernmentType
{
    Anarchy,
    Tribal,
    Despotism,
    Monarchy,
    Theocracy,
    Democracy,
    Socialism,
    Nationalism,
    HiveMind, // weird
    NomadicFederation
}

public enum DiplomacyState
{
    NeverMet,
    War,
    Trade,
    Ceasefire,
    Armistice,
    Peace,
    Alliance
}

public enum TileTypeName
{
    Plain,
    Forest,
    Hills,
    Mountain,
    Desert,
    Lake,
    Sea,
    Ocean,
    Swamp,
    Tundra,
    Arctic,
    Grassland
}

public enum TileExtraType
{
    None,
    Forest,
    Hills,
    Mountain,
    Swamp,
    River,
    Resource,
    Airfield
    // Add more tile extras as needed
}

// Класс для хранения префабов тайлов
[System.Serializable]
public class TilePrefabEntry
{
    public TileTypeName type;
    public GameObject prefab;
}

public class TilePrefabsContainer : MonoBehaviour
{
    [SerializeField]
    public List<TilePrefabEntry> tilePrefabEntries = new List<TilePrefabEntry>();

    void Awake()
    {
        Dictionary<TileTypeName, GameObject> tilePrefabs = new Dictionary<TileTypeName, GameObject>();
        foreach (var entry in tilePrefabEntries)
        {
            if (entry.prefab != null)
            {
                tilePrefabs[entry.type] = entry.prefab;
            }
        }
        staticHolder.InitializeTilePrefabs(tilePrefabs);
    }
}

[System.Serializable]
public class TileType // Example structure for TileType definitions
{
    public TileTypeName name;
    public int food;
    public int production;
    public int trade;
    public Dictionary<SuperType, int> moveCosts;
    public Dictionary<TileTypeName, int> transformations; // Cost to transform to another tile type

    public TileType(TileTypeName name, int food, int production, int trade, Dictionary<SuperType, int> moveCosts, Dictionary<TileTypeName, int> transformations)
    {
        this.name = name;
        this.food = food;
        this.production = production;
        this.trade = trade;
        this.moveCosts = moveCosts;
        this.transformations = transformations;
    }
}

[System.Serializable]
public class TileExtra // Example structure for TileExtra instances on a tile
{
    public TileExtraType type;
    public Dictionary<string, float> effects; // Effects on tile output, movement cost, defense, etc.
    public List<TileTypeName> exclusions; // Tile types this extra cannot be on
    public List<TileExtraType> extraExclusions; // Other extras this extra cannot be with

    public TileExtra(TileExtraType type, Dictionary<string, float> effects, List<TileTypeName> exclusions, List<TileExtraType> extraExclusions)
    {
        this.type = type;
        this.effects = effects;
        this.exclusions = exclusions;
        this.extraExclusions = extraExclusions;
    }
}

[System.Serializable]
public class UnitDefinition // Represents a type of unit (Infantry, Tank, etc.)
{
    public string unitName; // Internal name (e.g., "Infantry_Musketeer")
    public string displayedName; // Name shown to the player (e.g., "Musketeers")
    public UnitRole role; // The role of this unit type
    public SuperType superType; // Ground, Air, Naval
    public int attack; // Individual attack value
    public int defense; // Individual defense value
    public int cost; // Production cost per unit
    public int upkeepProd; // Upkeep per unit in production terms
    public int upkeepGold; // Upkeep per unit in gold terms
    public int payload; // Total number of rockets/missiles which can be carried by unit
    public string payloadType1; // Type of munition. Used for hardcoded events
    public string payloadType2; // Second type of munition
    public int payloadLaunchRange1; // Max launch distance for type 1
    public int payloadLaunchRange2; // Max launch distance for type 2
    public int baseMovePoints; // Base move points for this unit type
    public int cargoCapacity; // How much cargo this unit type can carry (e.g., for transports)
    public float cargoWeight; // Weight of one unit of this type (for cargo capacity calculations)
    public int antiAirDefense; // For Flak and other anti-air units
    public int antiGroundAttack; // For Bombers, CAS, Attack Helicopters
    public int antiAirAttack; // For Fighters, Attack Helicopters, Flak
}

[System.Serializable]
public class ForceComposition // Represents the number of units of each type in a Force
{
    public Dictionary<string, int> unitCounts = new Dictionary<string, int>(); // Map UnitDefinition.gameName to count

    // Helper to check if the composition has any units
    public bool HasUnits()
    {
        return unitCounts.Sum(entry => entry.Value) > 0;
    }
}

[System.Serializable]
public class GovernmentEffects // Effects associated with a government type
{
    public GovernmentType type;
    public Dictionary<string, float> effects; // Effects on production, science, happiness, unit stats, etc.
    public List<string> prerequisites; // Required technologies to adopt this government

    public GovernmentEffects(GovernmentType type, Dictionary<string, float> effects, List<string> prerequisites)
    {
        this.type = type;
        this.effects = effects;
        this.prerequisites = prerequisites;
    }
}

[System.Serializable]
public class NationDefinition
{
    public int nationID;
    public string civNameInternal;
    public string civNameDisplayed; //cosmetic feature
    public Sprite flag; // another cosmetic feature
    public string rulerName;
    public bool femaleRuler;
    public GovernmentType governmentType;
    public string aiType;
    public List<string> startingTechnologies; 
    public int startingGold; 
    public List<string> startingUnits;
    public int startingUnitCount;
}

[System.Serializable]
public class TechnologyDefinition
{
    public string techName;
    public string description;
    public int scienceCost;
    public List<string> unlocks; // What this technology unlocks (units, buildings, etc.)
    public List<string> prerequisites; // Required technologies
}

[System.Serializable]
public class TileExtraDefinition
{
    public TileExtraType type;
    public int cost;
    public Dictionary<string, float> effects; // Effects on tile output, movement cost, defense, etc.
    public List<TileTypeName> exclusions;
    public List<TileExtraType> extraExclusions;
}

[System.Serializable]
public class BuildingDefinition
{
    public string buildingName;
    public string description;
    public int productionCost;
    public Dictionary<string, float> effects; // Effects on city output, happiness, defense, etc.
    public List<string> prerequisites; // Required technologies or other buildings
}


public static class staticHolder
{
    // Section 1: Ruleset Data (Cached)
    // This data defines the rules and properties of game elements.
    // Typically loaded once at the start of the game.
    public static Dictionary<string, UnitDefinition> unitDefinitions = new Dictionary<string, UnitDefinition>();
    public static Dictionary<int, NationDefinition> nationDefinitions = new Dictionary<int, NationDefinition>(); // Using int for Nation ID
    public static Dictionary<string, TechnologyDefinition> technologyDefinitions = new Dictionary<string, TechnologyDefinition>();
    public static Dictionary<string, List<string>> civilizationCityNameLists = new Dictionary<string, List<string>>(); // Map Civ Name to list of city names
    public static Dictionary<TileTypeName, TileType> tileTypeDefinitions = new Dictionary<TileTypeName, TileType>(); // Definitions for TileTypes
    public static Dictionary<TileExtraType, TileExtraDefinition> tileExtraDefinitions = new Dictionary<TileExtraType, TileExtraDefinition>(); // Definitions for TileExtras
    public static Dictionary<string, BuildingDefinition> buildingDefinitions = new Dictionary<string, BuildingDefinition>(); // Definitions for Buildings
    public static Dictionary<GovernmentType, GovernmentEffects> governmentEffects = new Dictionary<GovernmentType, GovernmentEffects>(); // Government effects

    // Section 2: Game Data (Dynamic)
    // This data changes during gameplay and represents the current state.
    // Should be initialized at the start of a new game.
    public static Dictionary<int, int> nationTreasuries = new Dictionary<int, int>(); // Map Nation ID to Gold Treasury
    public static Dictionary<int, Dictionary<int, DiplomacyState>> diplomacyStates = new Dictionary<int, Dictionary<int, DiplomacyState>>(); // Map Nation ID to another Dictionary of Nation ID and DiplomacyState
    public static Dictionary<int, List<string>> nationKnownTechnologies = new Dictionary<int, List<string>>(); // Map Nation ID to list of known technology names
    public static Dictionary<int, string> nationAccessCodes = new Dictionary<int, string>(); // Nation ID to Access Code (for multiplayer/player control)

     public static Dictionary<TileTypeName, GameObject> tilePrefabs = new Dictionary<TileTypeName, GameObject>();

    // Метод для инициализации префабов тайлов
    public static void InitializeTilePrefabs(Dictionary<TileTypeName, GameObject> prefabs)
    {
        tilePrefabs = prefabs;
    }

    // Метод для получения префаба тайла по типу местности
    public static GameObject GetTilePrefab(TileTypeName type)
    {
        if (tilePrefabs.ContainsKey(type))
        {
            return tilePrefabs[type];
        }
        Debug.LogWarning($"Tile prefab not found for type: {type}");
        return null;
    }

    // Methods to initialize static data (Call this at the start of game)
    // This method should load data from some source (JSON, ScriptableObjects, etc.)
    public static void InitializeRulesetData()
    {
        Debug.Log("Initializing Ruleset Data...");

        // Clear existing data before loading (important for MODS utility or re-initialization)
        unitDefinitions.Clear();
        nationDefinitions.Clear();
        technologyDefinitions.Clear();
        civilizationCityNameLists.Clear();
        tileTypeDefinitions.Clear();
        tileExtraDefinitions.Clear();
        buildingDefinitions.Clear();
        governmentEffects.Clear();

        // --- Populate dictionaries with actual game data ---
        // Unit Definitions
        unitDefinitions.Add("Infantry_Musketeer", new UnitDefinition
        {
            unitName = "Infantry_Musketeer",
            displayedName = "Musketeers",
            role = UnitRole.Infantry,
            superType = SuperType.Ground,
            attack = 2,
            defense = 2,
            cost = 5,
            upkeepProd = 0,
            upkeepGold = 1,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 2,
            cargoCapacity = 0,
            cargoWeight = 1f,
            antiAirDefense = 0,
            antiGroundAttack = 0,
            antiAirAttack = 0
        });

        unitDefinitions.Add("Infantry_Modern", new UnitDefinition
        {
            unitName = "Infantry_Modern",
            displayedName = "Modern Infantry",
            role = UnitRole.Infantry,
            superType = SuperType.Ground,
            attack = 4,
            defense = 4,
            cost = 15,
            upkeepProd = 0,
            upkeepGold = 1,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 2,
            cargoCapacity = 0,
            cargoWeight = 1f,
            antiAirDefense = 0,
            antiGroundAttack = 0,
            antiAirAttack = 0
        });

        unitDefinitions.Add("Tank_WW1", new UnitDefinition
        {
            unitName = "Tank_WW1",
            displayedName = "WW1 Tank",
            role = UnitRole.Tank,
            superType = SuperType.Ground,
            attack = 8,
            defense = 10,
            cost = 50,
            upkeepProd = 2,
            upkeepGold = 2,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 3,
            cargoCapacity = 0,
            cargoWeight = 5f,
            antiAirDefense = 0,
            antiGroundAttack = 0,
            antiAirAttack = 0
        });

        unitDefinitions.Add("Artillery_Cannon", new UnitDefinition
        {
            unitName = "Artillery_Cannon",
            displayedName = "Cannon",
            role = UnitRole.Artillery,
            superType = SuperType.Ground,
            attack = 15,
            defense = 1,
            cost = 30,
            upkeepProd = 1,
            upkeepGold = 1,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 1,
            cargoCapacity = 0,
            cargoWeight = 3f,
            antiAirDefense = 0,
            antiGroundAttack = 0,
            antiAirAttack = 0
        });

        unitDefinitions.Add("FlakGun_WW2", new UnitDefinition
        {
            unitName = "FlakGun_WW2",
            displayedName = "WW2 Flak Gun",
            role = UnitRole.Flak,
            superType = SuperType.Ground,
            attack = 3,
            defense = 5,
            cost = 40,
            upkeepProd = 1,
            upkeepGold = 1,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 1,
            cargoCapacity = 0,
            cargoWeight = 2f,
            antiAirDefense = 10,
            antiGroundAttack = 0,
            antiAirAttack = 8
        });

        unitDefinitions.Add("TransportShip", new UnitDefinition
        {
            unitName = "TransportShip",
            displayedName = "Transport Ship",
            role = UnitRole.TransportShip,
            superType = SuperType.Naval,
            attack = 0,
            defense = 5,
            cost = 80,
            upkeepProd = 1,
            upkeepGold = 2,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 5,
            cargoCapacity = 50000,
            cargoWeight = 10f,
            antiAirDefense = 0,
            antiGroundAttack = 0,
            antiAirAttack = 0
        });

        unitDefinitions.Add("Fighter_WW2", new UnitDefinition
        {
            unitName = "Fighter_WW2",
            displayedName = "WW2 Fighter",
            role = UnitRole.Fighter,
            superType = SuperType.Air,
            attack = 5,
            defense = 3,
            cost = 60,
            upkeepProd = 2,
            upkeepGold = 3,
            payload = 0,
            payloadType1 = "",
            payloadType2 = "",
            payloadLaunchRange1 = 0,
            payloadLaunchRange2 = 0,
            baseMovePoints = 8,
            cargoCapacity = 0,
            cargoWeight = 0f,
            antiAirDefense = 8,
            antiGroundAttack = 5,
            antiAirAttack = 12
        });

        unitDefinitions.Add("MissileLauncher_Modern", new UnitDefinition
        {
            unitName = "MissileLauncher_Modern",
            displayedName = "Ракетный комплекс",
            role = UnitRole.Artillery,
            superType = SuperType.Ground,
            attack = 20,
            defense = 5,
            cost = 100,
            upkeepProd = 3,
            upkeepGold = 5,
            payload = 24,
            payloadType1 = "Missile",
            payloadType2 = "",
            payloadLaunchRange1 = 10,
            payloadLaunchRange2 = 0,
            baseMovePoints = 2,
            cargoCapacity = 0,
            cargoWeight = 5f,
            antiAirDefense = 0,
            antiGroundAttack = 20,
            antiAirAttack = 5
        });

        // TileType Definitions
        tileTypeDefinitions.Add(TileTypeName.Plain, new TileType(
            TileTypeName.Plain,
            food: 2,
            production: 1,
            trade: 0,
            moveCosts: new Dictionary<SuperType, int> { { SuperType.Ground, 1 }, { SuperType.Air, 1 }, { SuperType.Naval, int.MaxValue } },
            transformations: new Dictionary<TileTypeName, int>()
        ));

        // TileExtra Definitions
        //tileExtraDefinitions.Add(TileExtraType.Hills, new TileExtraDefinition(
        //    TileExtraType.Hills,
        //    cost: 0,
        //    effects: new Dictionary<string, float> { { "production", 1 }, { "MoveCost", 1 }, { "defence", 2 } },
        //    exclusions: new List<TileTypeName>(),
        //    extraExclusions: new List<TileExtraType>()
        //));

        // Building Definitions
        buildingDefinitions.Add("Barracks", new BuildingDefinition
        {
            buildingName = "Barracks",
            description = "Increases unit veterancy.",
            productionCost = 50,
            effects = new Dictionary<string, float>(),
            prerequisites = new List<string>()
        });

        // Technology Definitions
        technologyDefinitions.Add("Writing", new TechnologyDefinition
        {
            techName = "Writing",
            description = "Enables basic research and communication.",
            scienceCost = 50,
            unlocks = new List<string> { "ScribeBuilding", "Government_Despotism" },
            prerequisites = new List<string>()
        });

         technologyDefinitions.Add("CodeOfLaws", new TechnologyDefinition
        {
            techName = "Code of Laws",
            description = "Foundation for organized societies.",
            scienceCost = 75,
            unlocks = new List<string> { "CourthouseBuilding", "Government_Monarchy" },
            prerequisites = new List<string> { "Writing" }
        });

         technologyDefinitions.Add("Philosophy", new TechnologyDefinition
        {
            techName = "Philosophy",
            description = "Understanding of different societal structures.",
            scienceCost = 150,
            unlocks = new List<string> { "UniversityBuilding", "Government_Democracy", "Government_Communism" },
            prerequisites = new List<string> { "Writing", "CodeOfLaws" }
        });

        // Government Effects
        governmentEffects.Add(GovernmentType.Anarchy, new GovernmentEffects(
            GovernmentType.Anarchy,
            new Dictionary<string, float> {
                { "productionModifier", -0.2f },
                { "scienceModifier", -0.3f },
                { "happinessModifier", 0f },
                { "militaryStrengthBonus", -0.15f }
            },
            new List<string>()
        ));

        governmentEffects.Add(GovernmentType.Tribal, new GovernmentEffects(
            GovernmentType.Tribal,
            new Dictionary<string, float> {
                { "productionModifier", -0.1f },
                { "scienceModifier", -0.2f },
                { "happinessModifier", 0.05f },
                { "foodModifier", 0.05f }
            },
            new List<string>()
        ));


        governmentEffects.Add(GovernmentType.Despotism, new GovernmentEffects(
            GovernmentType.Despotism,
            new Dictionary<string, float> {
                { "productionModifier", 0.1f },
                { "scienceModifier", -0.1f },
                { "happinessModifier", -0.15f }
            },
            new List<string> { "Writing" }
        ));

        governmentEffects.Add(GovernmentType.Monarchy, new GovernmentEffects(
            GovernmentType.Monarchy,
            new Dictionary<string, float> {
                { "productionModifier", 0.05f },
                { "scienceModifier", 0f },
                { "happinessModifier", 0.1f },
                { "goldModifier", 0.05f }
            },
            new List<string> { "CodeOfLaws" }
        ));

         governmentEffects.Add(GovernmentType.Theocracy, new GovernmentEffects(
            GovernmentType.Theocracy,
            new Dictionary<string, float> {
                { "productionModifier", 0f },
                { "scienceModifier", -0.05f },
                { "happinessModifier", 0.15f },
                { "cultureModifier", 0.1f },
                { "unitDefenseBonus", 0.05f }
            },
            new List<string> { "Philosophy" } // Example: Requires Philosophy (or a religion-related tech)
        ));


        governmentEffects.Add(GovernmentType.Democracy, new GovernmentEffects(
            GovernmentType.Democracy,
            new Dictionary<string, float> {
                { "productionModifier", -0.1f },
                { "scienceModifier", 0.15f },
                { "happinessModifier", 0.2f },
                { "tradeModifier", 0.1f }
            },
            new List<string> { "Philosophy" }
        ));

         governmentEffects.Add(GovernmentType.Socialism, new GovernmentEffects(
            GovernmentType.Socialism,
            new Dictionary<string, float> {
                { "productionModifier", 0.2f },
                { "scienceModifier", 0.1f },
                { "happinessModifier", -0.1f },
                { "militaryStrengthBonus", 0.1f },
                { "unitCostModifier", -0.1f }
            },
            new List<string> { "Socialism" }
        ));

         governmentEffects.Add(GovernmentType.Nationalism, new GovernmentEffects(
            GovernmentType.Nationalism,
            new Dictionary<string, float> {
                { "productionModifier", 0.15f },
                { "scienceModifier", -0.1f },
                { "happinessModifier", -0.1f },
                { "militaryStrengthBonus", 0.2f },
                { "unitAttackBonus", 0.05f }
            },
            new List<string> { "Socialism" } // Example: Requires Industrialization
        ));


        // Weird Government Types with Prerequisites
        governmentEffects.Add(GovernmentType.HiveMind, new GovernmentEffects(
            GovernmentType.HiveMind,
            new Dictionary<string, float> {
                { "productionModifier", 0.2f },
                { "scienceModifier", 0.25f },
                { "happinessModifier", -0.5f },
                { "unitUpkeepModifier", -0.2f },
                { "unitDefenseBonus", 0.15f }
            },
            new List<string> { "Robotics", "AdvancedAI" }
        ));

        governmentEffects.Add(GovernmentType.NomadicFederation, new GovernmentEffects(
            GovernmentType.NomadicFederation,
            new Dictionary<string, float> {
                { "productionModifier", -0.2f },
                { "foodModifier", 0.15f },
                { "unitMovePointsModifier", 0.25f },
                { "unitCostModifier", -0.1f },
                { "cityGrowthRateModifier", -0.3f }
            },
            new List<string> { "HorsebackRiding" }
        ));

        // --- End of Example Data ---

        Debug.Log("Ruleset Data Initialized.");
    }

    // Methods to initialize dynamic game data (Call this when a new game starts)
    public static void InitializeGameData(List<Nation> gameNations)
    {
        Debug.Log("Initializing Game Data...");

        nationTreasuries.Clear();
        diplomacyStates.Clear();
        nationKnownTechnologies.Clear();
        nationAccessCodes.Clear(); // Clear access codes for a new game

        foreach (var nation in gameNations)
        {
            // Initialize treasury based on definition or a default value
            if (nationDefinitions.ContainsKey(nation.nationID))
            {
                nationTreasuries[nation.nationID] = nationDefinitions[nation.nationID].startingGold;
            }
            else
            {
                nationTreasuries[nation.nationID] = 100; // Default starting gold
            }

            // Initialize diplomacy states (all NeverMet initially)
            diplomacyStates[nation.nationID] = new Dictionary<int, DiplomacyState>();
            foreach (var otherNation in gameNations)
            {
                if (nation.nationID != otherNation.nationID)
                {
                    diplomacyStates[nation.nationID][otherNation.nationID] = DiplomacyState.NeverMet;
                }
            }

            // Initialize known technologies based on definition or empty list
            if (nationDefinitions.ContainsKey(nation.nationID))
            {
                nationKnownTechnologies[nation.nationID] = new List<string>(nationDefinitions[nation.nationID].startingTechnologies ?? new List<string>()); // Handle null startingTechologies
            }
            else
            {
                nationKnownTechnologies[nation.nationID] = new List<string>();
            }

             // Generate a basic access code for each nation (can be improved)
             nationAccessCodes[nation.nationID] = Guid.NewGuid().ToString("N").Substring(0, 8);
             Debug.Log($"Generated access code for Nation {nation.nationID} ({nation.civNameDisplayed}): {nationAccessCodes[nation.nationID]}");
        }

         Debug.Log("Game Data Initialized.");
    }

    // Helper to get UnitDefinition by game name
    public static UnitDefinition GetUnitDefinition(string gameName)
    {
        if (unitDefinitions.ContainsKey(gameName))
        {
            return unitDefinitions[gameName];
        }
        Debug.LogWarning("Unit Definition not found for: " + gameName);
        return null;
    }

    // Helper to get GovernmentEffects by type
    public static GovernmentEffects GetGovernmentEffects(GovernmentType type)
    {
        if (governmentEffects.ContainsKey(type))
        {
            return governmentEffects[type];
        }
        Debug.LogWarning("Government Effects not found for: " + type);
        return null;
    }

     // Helper to get TechnologyDefinition by name
    public static TechnologyDefinition GetTechnologyDefinition(string techName)
    {
        if (technologyDefinitions.ContainsKey(techName))
        {
            return technologyDefinitions[techName];
        }
        Debug.LogWarning("Technology Definition not found for: " + techName);
        return null;
    }

    // Helper to check if a nation knows a technology
    public static bool NationKnowsTechnology(int nationId, string techName)
    {
        if (nationKnownTechnologies.ContainsKey(nationId))
        {
            return nationKnownTechnologies[nationId].Contains(techName);
        }
        return false;
    }

    // Helper to get BuildingDefinition by name
    public static BuildingDefinition GetBuildingDefinition(string buildingName)
    {
        if (buildingDefinitions.ContainsKey(buildingName))
        {
            return buildingDefinitions[buildingName];
        }
        Debug.LogWarning("Building Definition not found for: " + buildingName);
        return null;
    }

    // Helper to get TileTypeDefinition by name
    public static TileType GetTileTypeDefinition(TileTypeName typeName)
    {
        if (tileTypeDefinitions.ContainsKey(typeName))
        {
            return tileTypeDefinitions[typeName];
        }
        Debug.LogWarning("Tile Type Definition not found for: " + typeName);
        return null;
    }

    // Helper to get TileExtraDefinition by type
    public static TileExtraDefinition GetTileExtraDefinition(TileExtraType type)
    {
        if (tileExtraDefinitions.ContainsKey(type))
        {
            return tileExtraDefinitions[type];
        }
        Debug.LogWarning("Tile Extra Definition not found for: " + type);
        return null;
    }

    // Helper to get DiplomacyState between two nations
    public static DiplomacyState GetDiplomacyState(int nation1Id, int nation2Id)
    {
        if (diplomacyStates.ContainsKey(nation1Id) && diplomacyStates[nation1Id].ContainsKey(nation2Id))
        {
            return diplomacyStates[nation1Id][nation2Id];
        }
        // Default to NeverMet if not explicitly set (or handle error)
        return DiplomacyState.NeverMet;
    }

    // Helper to set DiplomacyState between two nations
    public static void SetDiplomacyState(int nation1Id, int nation2Id, DiplomacyState state)
    {
        if (!diplomacyStates.ContainsKey(nation1Id))
        {
            diplomacyStates[nation1Id] = new Dictionary<int, DiplomacyState>();
        }
         if (!diplomacyStates.ContainsKey(nation2Id))
        {
            diplomacyStates[nation2Id] = new Dictionary<int, DiplomacyState>();
        }

        diplomacyStates[nation1Id][nation2Id] = state;
        diplomacyStates[nation2Id][nation1Id] = state; // Diplomacy is usually reciprocal
        Debug.Log($"Diplomacy state changed between Nation {nation1Id} and Nation {nation2Id} to {state}");
    }
}