using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // Ссылка на выбранный объект (Force или City)
    private GameObject selectedObject;
    private Force selectedForce;
    private City selectedCity;

    // Ссылка на скрипт управления камерой
    private camera_movement cameraController;
    // Флаг, указывающий, выбран ли объект
    private bool isObjectSelected = false;
    // Флаг для режима перемещения всей группы
    private bool moveWholeForce = false;
    // Флаг для отображения мини-UI для выбора типа боеприпаса
    private bool showMissileUI = false;
    // Ссылка на MissileUI
    public MissileUI missileUI;
    public GameObject cityUI;
    // Целевая позиция для запуска ракеты
    private Vector3 missileTargetPosition;

    void Start()
    {
        // Получаем ссылку на скрипт управления камерой
        cameraController = Camera.main.GetComponent<camera_movement>();
        if (cameraController == null)
        {
            Debug.LogError("Camera movement script not found on the main camera!");
        }

        // Получаем ссылку на MissileUI
        missileUI = FindObjectOfType<MissileUI>();
        if (missileUI == null)
        {
            Debug.LogError("MissileUI not found!");
        }

    }

    void Update()
    {
        // Обработка ввода для движения выбранной группы
        if (isObjectSelected && selectedObject != null && selectedForce != null)
        {
            HandleForceMovement();
        }

        // Обработка кликов мыши для выбора объектов
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            HandleObjectSelection();
        }

        // Обработка нажатия кнопки 'P' для запуска ракеты
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleMissileLaunch();
        }

        // Обработка нажатия кнопки '~' для переключения режима перемещения всей группы
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            moveWholeForce = !moveWholeForce;
            Debug.Log($"Move whole force mode: {(moveWholeForce ? "ON" : "OFF")}");
        }
    }

    // Метод для обработки движения группы
    protected void HandleForceMovement()
    {
        // Получаем входные данные для движения группы
        Vector3 movementDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.Q)) // Северо-запад (NW)
        {
            movementDirection += new Vector3(-1, 0, 1).normalized;
        }
        if (Input.GetKey(KeyCode.W)) // Север (N)
        {
            movementDirection += new Vector3(0, 0, 1).normalized;
        }
        if (Input.GetKey(KeyCode.E)) // Северо-восток (NE)
        {
            movementDirection += new Vector3(1, 0, 1).normalized;
        }
        if (Input.GetKey(KeyCode.A)) // Запад (W)
        {
            movementDirection += new Vector3(-1, 0, 0).normalized;
        }
        if (Input.GetKey(KeyCode.S)) // Юг (S)
        {
            movementDirection += new Vector3(0, 0, -1).normalized;
        }
        if (Input.GetKey(KeyCode.D)) // Восток (E)
        {
            movementDirection += new Vector3(1, 0, 0).normalized;
        }
        if (Input.GetKey(KeyCode.Z)) // Юго-запад (SW)
        {
            movementDirection += new Vector3(-1, 0, -1).normalized;
        }
        if (Input.GetKey(KeyCode.X)) // Юго-восток (SE)
        {
            movementDirection += new Vector3(1, 0, -1).normalized;
        }

        // Если есть направление движения, перемещаем группу
        if (movementDirection != Vector3.zero)
        {
            // Нормализуем направление движения
            movementDirection.Normalize();
            // Перемещаем группу в выбранном направлении
            selectedForce.MoveForce(movementDirection);
        }
    }

    // Метод для обработки выбора объектов
    private void HandleObjectSelection()
    {
        // Создаем луч из камеры в точку клика
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Проверяем, попал ли луч в какой-либо объект
        if (Physics.Raycast(ray, out hit))
        {
            // Получаем объект, на который кликнули
            GameObject clickedObject = hit.collider.gameObject;

            // Проверяем, является ли объект группой или городом
            Force force = clickedObject.GetComponent<Force>();
            City city = clickedObject.GetComponent<City>();

            if (force != null)
            {
                // Выбрана группа
                selectedObject = clickedObject;
                selectedForce = force;
                selectedCity = null;
                isObjectSelected = true;
                Debug.Log($"Selected force: {force.forceName}");
            }
            else if (city != null)
            {
                // Выбран город
                selectedObject = clickedObject;
                selectedForce = null;
                selectedCity = city;
                isObjectSelected = true;
                Debug.Log($"Selected city: {city.cityName}");

                
                //if (Input.GetKey(KeyCode.LeftControl)) // Например, создаем ракету при зажатой Ctrl
                //{
                    //CreateMissileInCity(city);
                //}
            }
        }
    }

    // Метод для создания ракеты в городе
    private void CreateMissileInCity(City city)
    {
        // Здесь должна быть логика для выбора типа ракеты и создания ее в городе
        // Для простоты создадим ракету фиксированного типа
        Missile missile = city.CreateMissile("CruiseMissile", 10, 20);
        if (missile == null)
        {
            Debug.Log($"Created missile in city {city.cityName} and added to force.");
        }
        else
        {
            Debug.Log($"Created missile in city {city.cityName} but no force found to add it to.");
        }
    }

    // Метод для обработки запуска ракеты
    private void HandleMissileLaunch()
    {
        if (!isObjectSelected || selectedForce == null)
        {
            Debug.Log("No force selected or no missiles available.");
            return;
        }

        // Проверяем, содержит ли выбранная группа ракеты
        if (selectedForce.missiles.Count > 0)
        {
            // Запрашиваем у игрока выбор цели
            Debug.Log("Select target position by clicking on the map.");
            // Подписываемся на событие клика мыши для выбора цели
            StartCoroutine(WaitForTargetSelection());
        }
        else
        {
            Debug.Log("Selected force does not have any missiles.");
        }
    }

    private System.Collections.IEnumerator WaitForTargetSelection()
    {
        bool targetSelected = false;
        Vector3 targetPosition = Vector3.zero;

        // Ждем клика мыши для выбора цели
        while (!targetSelected)
        {
            if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
            {
                // Получаем позицию клика на карте
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    targetPosition = hit.point;
                    targetSelected = true;
                }
            }
            yield return null;
        }

        // Отображаем мини-UI для выбора типа боеприпаса
        if (missileUI != null)
        {
            missileUI.ShowMissilePanel(selectedForce, targetPosition);
        }
    }

    // Метод для запуска ракеты
    public void LaunchMissile(Force force, string missileType, Vector3 targetPosition)
    {
        if (force == null) return;

        // Ищем ракету указанного типа в списке ракет группы
        Missile missileToLaunch = force.missiles.Find(m => m.missileType == missileType);
        if (missileToLaunch != null)
        {
            // Проверяем дистанцию до цели
            if (force.IsWithinLaunchRange(targetPosition, missileToLaunch))
            {
                // Запускаем ракету
                force.LaunchMissile(missileToLaunch, targetPosition);
            }
            else
            {
                Debug.Log("Target is out of range for the selected missile.");
            }
        }
        else
        {
            Debug.Log($"No missile of type {missileType} available.");
        }
    }

    // Метод для получения выбранной группы
    public Force GetSelectedForce()
    {
        return selectedForce;
    }

    // Метод для получения выбранного города
    public City GetSelectedCity()
    {
        return selectedCity;
    }

    // Метод для снятия выбора объекта
    public void ClearSelection()
    {
        selectedObject = null;
        selectedForce = null;
        selectedCity = null;
        isObjectSelected = false;
    }
}