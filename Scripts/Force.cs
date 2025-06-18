using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Force : MonoBehaviour
{
    public string forceName;
    public List<Unit> units = new List<Unit>();
    public List<Missile> missiles = new List<Missile>();
    public int movePoints;
    public Vector3 position;

    void Start()
    {
        UpdateMovePoints();
    }

    // Метод для добавления юнита в группу
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        UpdateMovePoints();
    }

    // Метод для добавления ракеты в группу
    public void AddMissile(Missile missile)
    {
        missiles.Add(missile);
    }

    // Метод для получения тайла, на котором находится группа
    public Tile GetCurrentTile()
    {
        // Получаем позицию группы в сетке
        Vector2Int gridPos = GridManager.Instance.GetGridPosition(transform.position);
        // Получаем тайл по позиции в сетке
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

    // Метод для перемещения группы в заданном направлении
    public void MoveForce(Vector3 direction)
    {
        if (movePoints > 0)
        {
            // Получаем текущий тайл
            Tile currentTile = GetCurrentTile();
            if (currentTile == null)
            {
                Debug.LogWarning("Current tile not found!");
                return;
            }

            // Вычисляем новую позицию
            Vector3 newPosition = transform.position + direction.normalized * GridManager.Instance.tileSize;

            // Получаем новый тайл
            Vector2Int newGridPos = GridManager.Instance.GetGridPosition(newPosition);
            Tile newTile = GridManager.Instance.GetTileAt(newGridPos);

            if (newTile != null)
            {
                // Следует добавить код для определения фактического SuperType для всей толпы. Сейчас КАК ЕСТЬ.
                SuperType unitSuperType = units.Count > 0 ? units[0].definition.superType : SuperType.Ground;
                int moveCost = newTile.movementCost;
                if (unitSuperType == SuperType.Air)
                {
                    moveCost = 1; // Для воздушных юнитов стоимость движения всегда 1
                }
                else if (unitSuperType == SuperType.Naval)
                {
                    moveCost = newTile.terrainType == TileTypeName.Ocean || newTile.terrainType == TileTypeName.Sea ? 1 : int.MaxValue;
                }

                if (movePoints >= moveCost)
                {
                    // Перемещаем группу
                    transform.position = newPosition;
                    movePoints -= moveCost;

                    // Обновляем очки движения для всех юнитов в группе
                    foreach (Unit unit in units)
                    {
                        unit.currentMovePoints -= moveCost;
                    }

                    Debug.Log($"Force {forceName} moved in direction: {direction}");
                }
                else
                {
                    Debug.Log($"Not enough move points to move to tile {newTile.gridPosition} with cost {moveCost}.");
                }
            }
            else
            {
                Debug.LogWarning("New tile not found!");
            }
        }
    }

    // Метод для обновления очков движения группы
    public void UpdateMovePoints()
    {
        if (units.Count > 0)
        {
            // Очки движения группы равны минимальным очкам движения юнитов в группе
            movePoints = units.Min(u => u.currentMovePoints);
        }
        else
        {
            movePoints = 0;
        }
    }

    // Метод для проверки, можно ли удалить группу
    public bool ShouldDeleteForce()
    {
        // Группа должна быть удалена, если она пустая или содержит только ракеты
        // Исключение: группа находится на клетке города
        bool isOnCityTile = IsOnCityTile();

        if (units.Count == 0 && missiles.Count > 0)
        {
            return !isOnCityTile; // Удаляем, если не на клетке города
        }
        else if (units.Count == 0 && missiles.Count == 0)
        {
            return true; // Удаляем пустую группу
        }
        return false; // Не удаляем группу в других случаях
    }

    // Метод для проверки, находится ли группа на клетке города
    private bool IsOnCityTile()
    {
        // Здесь должна быть логика для проверки, находится ли группа на клетке города
        // Например, проверка коллизии с городом или проверка позиции на карте
        // Для простоты предположим, что группа находится на клетке города, если ее позиция совпадает с позицией города
        City[] cities = FindObjectsOfType<City>();
        foreach (City city in cities)
        {
            if (Vector3.Distance(transform.position, city.transform.position) < 1f)
            {
                return true;
            }
        }
        return false;
    }

    // Метод для разделения группы
    public Force SplitForce(List<Unit> unitsToSplit, List<Missile> missilesToSplit)
    {
        // Создаем новую группу
        GameObject newForceObj = new GameObject("NewForce");
        Force newForce = newForceObj.AddComponent<Force>();
        newForce.forceName = forceName + "_Split";
        newForce.position = position;
        newForce.transform.position = transform.position;

        // Перемещаем выбранные юниты и ракеты в новую группу
        foreach (Unit unit in unitsToSplit)
        {
            units.Remove(unit);
            newForce.AddUnit(unit);
        }

        foreach (Missile missile in missilesToSplit)
        {
            missiles.Remove(missile);
            newForce.AddMissile(missile);
        }

        // Обновляем очки движения для исходной группы
        UpdateMovePoints();

        return newForce;
    }

    // Метод для объединения групп
    public void MergeForce(Force otherForce)
    {
        if (otherForce == null) return;

        // Перемещаем юниты и ракеты из другой группы в эту
        foreach (Unit unit in otherForce.units)
        {
            units.Add(unit);
        }

        foreach (Missile missile in otherForce.missiles)
        {
            missiles.Add(missile);
        }

        // Обновляем очки движения
        UpdateMovePoints();

        // Уничтожаем другую группу
        Destroy(otherForce.gameObject);
    }

    // Метод для проверки, находится ли цель в пределах дистанции пуска
    public bool IsWithinLaunchRange(Vector3 targetPosition, Missile missile)
    {
        // Используем расстояние Манхэттена для проверки дистанции
        int distance = CalculateManhattanDistance(transform.position, targetPosition);
        return distance <= missile.range;
    }

    // Метод для расчета расстояния Манхэттена между двумя точками
    private int CalculateManhattanDistance(Vector3 start, Vector3 end)
    {
        int dx = Mathf.Abs(Mathf.RoundToInt(start.x) - Mathf.RoundToInt(end.x));
        int dz = Mathf.Abs(Mathf.RoundToInt(start.z) - Mathf.RoundToInt(end.z));
        return dx + dz;
    }

    // Метод для запуска ракеты
    public void LaunchMissile(Missile missile, Vector3 targetPosition)
    {
        if (missile == null) return;

        // Проверяем дистанцию
        if (!IsWithinLaunchRange(targetPosition, missile))
        {
            Debug.Log("Target is out of range.");
            return;
        }

        // Ищем цель на указанной позиции
        Force targetForce = FindForceAtPosition(targetPosition);
        if (targetForce == null)
        {
            Debug.Log("No target force found at the specified position.");
            return;
        }

        // Вычисляем шанс перехвата
        int interception = targetForce.GetMaxInterception();
        int roll = Random.Range(1, 101); // Бросок d100

        if (roll > interception)
        {
            // Ракета достигает цели
            Debug.Log($"Missile hit the target! Interception: {interception}, Roll: {roll}");
            // Урон по армии-цели.
            targetForce.TakeDamage(missile.damage);
        }
        else
        {
            Debug.Log($"Missile intercepted! Interception: {interception}, Roll: {roll}");
        }

        // Удаляем ракету из группы
        missiles.Remove(missile);
    }

    // Метод для получения максимального значения Interception среди юнитов в группе
    private int GetMaxInterception()
    {
        int maxInterception = 0;
        foreach (Unit unit in units)
        {
            if (unit.interception > maxInterception)
            {
                maxInterception = unit.interception;
            }
        }
        return maxInterception;
    }

    private void TakeDamage(int damage)
    {
        // Логика для нанесения урона всей армии
        foreach (Unit unit in units)
        {

            unit.health -= damage;
            if (unit.health <= 0)
            {
                // Уничтожаем юнит, если его здоровье <= 0
                units.Remove(unit);
            }
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


}