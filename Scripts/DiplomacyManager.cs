using UnityEngine;
using System.Collections.Generic;

public class DiplomacyManager
{
    public Dictionary<int, Dictionary<int, int>> relations = new Dictionary<int, Dictionary<int, int>>();

    public void Initialize(List<Nation> nations)
    {
        foreach (var nation in nations)
        {
            relations[nation.nationID] = new Dictionary<int, int>();
            foreach (var otherNation in nations)
            {
                if (nation.nationID != otherNation.nationID)
                {
                    relations[nation.nationID][otherNation.nationID] = 0; // Нейтральные отношения
                }
            }
        }
    }

    public void ChangeRelations(int nation1Id, int nation2Id, int change)
    {
        if (relations.ContainsKey(nation1Id) && relations[nation1Id].ContainsKey(nation2Id))
        {
            relations[nation1Id][nation2Id] += change;
        }
    }

    public void PerformDiplomaticAction(string actionType, int actorId, int targetId)
    {
        switch (actionType)
        {
            case "investigate":
                ChangeRelations(actorId, targetId, -5);
                break;
            case "sabotage":
                ChangeRelations(actorId, targetId, -25);
                break;
            case "incite":
                ChangeRelations(actorId, targetId, -25);
                break;
        }
    }
}
