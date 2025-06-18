using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ForceCompositionEditorWrapper : MonoBehaviour
{
    [System.Serializable]
    public class EditableUnitCount
    {
        // The game name of the unit (must match UnitDefinition.gameName in staticHolder)
        // We still store the string name, but the editor will help select it.
        public string unitGameName;
        public int count;
    }

    // This list will be editable in the Unity Inspector
    public List<EditableUnitCount> editableUnitCounts = new List<EditableUnitCount>();

    // The ForceComposition object that will be populated
    private ForceComposition _forceComposition;

    // Call this method from your Force script or another script to get the ForceComposition
    public ForceComposition GetForceComposition()
    {
        if (_forceComposition == null)
        {
            _forceComposition = new ForceComposition();
            PopulateForceComposition();
        }
        return _forceComposition;
    }

    // Populates the internal ForceComposition dictionary from the editable list
    private void PopulateForceComposition()
    {
        _forceComposition.unitCounts.Clear(); // Clear existing data

        // Ensure staticHolder data is initialized before accessing unitDefinitions
        if (staticHolder.unitDefinitions == null || staticHolder.unitDefinitions.Count == 0)
        {
            Debug.LogError("staticHolder.unitDefinitions is not initialized! Call staticHolder.InitializeRulesetData() first.");
            return;
        }

        foreach (var editableCount in editableUnitCounts)
        {
            // Validate that the unitGameName exists in staticHolder
            if (!string.IsNullOrEmpty(editableCount.unitGameName) && staticHolder.unitDefinitions.ContainsKey(editableCount.unitGameName) && editableCount.count >= 0)
            {
                // Check if the key already exists before adding
                if (_forceComposition.unitCounts.ContainsKey(editableCount.unitGameName))
                {
                    Debug.LogWarning($"Duplicate unit game name '{editableCount.unitGameName}' found in Force Composition. Overwriting count.");
                    _forceComposition.unitCounts[editableCount.unitGameName] = editableCount.count;
                }
                else
                {
                    _forceComposition.unitCounts.Add(editableCount.unitGameName, editableCount.count);
                }
            }
            else
            {
                // Provide more specific warnings
                if (string.IsNullOrEmpty(editableCount.unitGameName))
                {
                    Debug.LogWarning("Invalid entry in Force Composition: Unit Game Name is empty. Skipping.");
                }
                else if (!staticHolder.unitDefinitions.ContainsKey(editableCount.unitGameName))
                {
                    Debug.LogWarning($"Invalid entry in Force Composition: Unit Game Name '{editableCount.unitGameName}' not found in staticHolder.unitDefinitions. Skipping.");
                }
                else if (editableCount.count < 0)
                {
                    Debug.LogWarning($"Invalid entry in Force Composition: Count for unit '{editableCount.unitGameName}' is negative. Skipping.");
                }
            }
        }
    }

    // it's generally better to call GetForceComposition() when needed.
    // void Start()
    // {
    //     GetForceComposition();
    // }

    // call PopulateForceComposition in OnValidate for editor feedback,
    // but ensure staticHolder is initialized first.
    // private void OnValidate()
    // {
    //     // This is called in the editor when values change.
    //     // We need to be careful about accessing staticHolder here,
    //     // as it might not be initialized yet in all editor scenarios.
    //     // A more robust approach is to rely on GetForceComposition()
    //     // being called at runtime.
    //
    //     // OnValidate checks:
    //     if (staticHolder.unitDefinitions != null && staticHolder.unitDefinitions.Count > 0)
    //     {
    //         if (_forceComposition == null)
    //         {
    //             _forceComposition = new ForceComposition();
    //         }
    //         PopulateForceComposition();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("staticHolder.unitDefinitions not initialized in OnValidate. Cannot update Force Composition.");
    //     }
    // }
}
