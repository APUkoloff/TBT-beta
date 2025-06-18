using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileTypeName terrainType;
    public int movementCost;
    public int baseProduction;
    public int baseTrade;

    public void Initialize(Vector2Int gridPos, TileTypeName type, int moveCost, int prod, int trade)
    {
        gridPosition = gridPos;
        terrainType = type;
        movementCost = moveCost;
        baseProduction = prod;
        baseTrade = trade;
    }
}