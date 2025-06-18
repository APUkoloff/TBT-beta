using UnityEngine;
using System.Collections.Generic;

public class TilePrefabLoader : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject plainPrefab;
    public GameObject forestPrefab;
    public GameObject hillsPrefab;
    public GameObject mountainPrefab;
    public GameObject desertPrefab;
    public GameObject lakePrefab;
    public GameObject seaPrefab;
    public GameObject oceanPrefab;
    public GameObject swampPrefab;
    public GameObject tundraPrefab;
    public GameObject arcticPrefab;
    public GameObject grasslandPrefab;

    void Awake()
    {
        Dictionary<TileTypeName, GameObject> tilePrefabs = new Dictionary<TileTypeName, GameObject>();

        if (plainPrefab != null) tilePrefabs[TileTypeName.Plain] = plainPrefab;
        if (forestPrefab != null) tilePrefabs[TileTypeName.Forest] = forestPrefab;
        if (hillsPrefab != null) tilePrefabs[TileTypeName.Hills] = hillsPrefab;
        if (mountainPrefab != null) tilePrefabs[TileTypeName.Mountain] = mountainPrefab;
        if (desertPrefab != null) tilePrefabs[TileTypeName.Desert] = desertPrefab;
        if (lakePrefab != null) tilePrefabs[TileTypeName.Lake] = lakePrefab;
        if (seaPrefab != null) tilePrefabs[TileTypeName.Sea] = seaPrefab;
        if (oceanPrefab != null) tilePrefabs[TileTypeName.Ocean] = oceanPrefab;
        if (swampPrefab != null) tilePrefabs[TileTypeName.Swamp] = swampPrefab;
        if (tundraPrefab != null) tilePrefabs[TileTypeName.Tundra] = tundraPrefab;
        if (arcticPrefab != null) tilePrefabs[TileTypeName.Arctic] = arcticPrefab;
        if (grasslandPrefab != null) tilePrefabs[TileTypeName.Grassland] = grasslandPrefab;

        staticHolder.InitializeTilePrefabs(tilePrefabs);
        Debug.Log("Tile prefabs loaded into staticHolder.");
    }
}
