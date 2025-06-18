using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;
    void Start()
    {
        // Инициализация данных правил
        staticHolder.InitializeRulesetData();

        // Создание списка наций для игры
        List<Nation> gameNations = new List<Nation>
        {
            new Nation { nationID = 1, civNameDisplayed = "Nation 1" },
            new Nation { nationID = 2, civNameDisplayed = "Nation 2" }
        };

        // Инициализация игровых данных
        staticHolder.InitializeGameData(gameNations);

        // Инициализация игрового поля
        GridManager gridManager = gameObject.AddComponent(typeof(GridManager)) as GridManager;
        gridManager.InitializeGrid(200, 200);

        // Инициализация пользовательского интерфейса
        uiManager = GetComponent<UIManager>();
        if (uiManager == null)
        {
            Debug.Log("Forgot to add UI controller in that version");
            //uiManager = gameObject.AddComponent<UIManager>();
        }
    }
    void Update()
    {
        
    }
}