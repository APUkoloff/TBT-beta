using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissileUI : MonoBehaviour
{
    public GameObject missilePanel;
    public Button cruiseMissileButton;
    public Button ballisticMissileButton;
    public Button cruiseMissileNukeButton;
    public Button ballisticMissileNukeButton;
    public Button exitButton;
    public Button cancelButton;

    public GameObject explosionPrefab;
    public GameObject mushroomCloudPrefab;
    public GameObject smoke;

    private Force selectedForce;
    private Vector3 targetPosition;
    private UIManager uiManager;
    private List<GameObject> activeEffects = new List<GameObject>();

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found!");
            return;
        }

        cruiseMissileButton.onClick.AddListener(() => OnMissileSelected("CruiseMissile"));
        ballisticMissileButton.onClick.AddListener(() => OnMissileSelected("BallisticMissile"));
        cruiseMissileNukeButton.onClick.AddListener(() => OnMissileSelected("CruiseMissileNuke"));
        ballisticMissileNukeButton.onClick.AddListener(() => OnMissileSelected("BallisticMissileNuke"));
        exitButton.onClick.AddListener(OnExit);
        cancelButton.onClick.AddListener(OnCancel);

        missilePanel.SetActive(false);
    }

    public void ShowMissilePanel(Force force, Vector3 targetPos)
    {
        selectedForce = force;
        targetPosition = targetPos;
        missilePanel.SetActive(true);
    }

    public void HideMissilePanel()
    {
        missilePanel.SetActive(false);
    }

    private void OnMissileSelected(string missileType)
    {
        uiManager.LaunchMissile(selectedForce, missileType, targetPosition);
        HideMissilePanel();
    }

    private void OnExit()
    {
        HideMissilePanel();
        Debug.Log("Exit missile selection. Please select a new target.");
    }

    private void OnCancel()
    {
        HideMissilePanel();
        Debug.Log("Missile launch cancelled.");
    }

    public void CreateMissileHitEffect(string missileType, Vector3 position)
    {
        GameObject effectPrefab = null;
        switch (missileType)
        {
            case "CruiseMissile":
            case "BallisticMissile":
                effectPrefab = explosionPrefab;
                break;
            case "CruiseMissileNuke":
            case "BallisticMissileNuke":
                effectPrefab = mushroomCloudPrefab;
                break;
            default:
                effectPrefab = explosionPrefab;
                break;
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            DestroyEffect destroyScript = effect.AddComponent<DestroyEffect>();
            destroyScript.fuseTime = 10f;

            //GameManager gameManager = FindObjectOfType<GameManager>();
            //if (gameManager != null)
            //{
                //gameManager.RegisterEffect(effect);
            //}
            //activeEffects.Add(effect);
        }

        if (smoke != null)
        {
            GameObject smokeEffect = Instantiate(smoke, position, Quaternion.identity);
            DestroyEffect destroyScript = smokeEffect.AddComponent<DestroyEffect>();
            destroyScript.fuseTime = 10f;

            //GameManager gameManager = FindObjectOfType<GameManager>();
            //if (gameManager != null)
            //{
                //gameManager.RegisterEffect(smokeEffect);
            //}
            //activeEffects.Add(smokeEffect);
        }
    }

    public void CreateCombatEffect(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(explosionPrefab, position, Quaternion.identity);
            DestroyEffect destroyScript = effect.AddComponent<DestroyEffect>();
            destroyScript.fuseTime = 10f;

            //GameManager gameManager = FindObjectOfType<GameManager>();
            //if (gameManager != null)
            //{
                //gameManager.RegisterEffect(effect);
            //}
            //activeEffects.Add(effect);
        }

        if (smoke != null)
        {
            GameObject smokeEffect = Instantiate(smoke, position, Quaternion.identity);
            DestroyEffect destroyScript = smokeEffect.AddComponent<DestroyEffect>();
            destroyScript.fuseTime = 10f;

            //GameManager gameManager = FindObjectOfType<GameManager>();
            //if (gameManager != null)
            //{
                //gameManager.RegisterEffect(smokeEffect);
            //}
            //activeEffects.Add(smokeEffect);
        }
    }

    public void ClearOldEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }
}

public class DestroyEffect : MonoBehaviour
{
    public float fuseTime = 10f;

    void Start()
    {
        Destroy(gameObject, fuseTime);
    }
}