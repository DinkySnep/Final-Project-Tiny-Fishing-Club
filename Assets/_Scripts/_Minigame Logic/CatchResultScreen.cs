using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CatchResultScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text continueText;

    [Header("3D Preview System")]
    [SerializeField] private Transform previewSpawnPoint;
    [SerializeField] private Camera previewCamera;
    [SerializeField] private RawImage previewImage;
    [SerializeField] private float rotationSpeed = 15f;

    private FishInstance currentFish;
    private GameObject spawnedModel;

    private void Awake()
    {
        // bind render texture to UI image automatically
        if (previewCamera != null && previewImage != null)
        {
            previewImage.texture = previewCamera.targetTexture;
        }
    }

    public void ShowResult(FishInstance fish)
    {
        currentFish = fish;

        // clear old model
        if (spawnedModel != null)
            Destroy(spawnedModel);

        SpawnFishModel(fish);
        UpdateUI(fish);
    }

    private void SpawnFishModel(FishInstance fish)
    {
        if (fish.data.fishPrefab == null || previewSpawnPoint == null) return;

        // spawn in world space preview scene
        spawnedModel = Instantiate(
            fish.data.fishPrefab,
            previewSpawnPoint.position,
            Quaternion.Euler(-90f, -90f, 0f)
        );

        // assign correct layer for preview camera rendering
        SetLayerRecursively(spawnedModel, LayerMask.NameToLayer("FishPreview"));

        // apply fish color to all renderers
        Renderer[] renderers = spawnedModel.GetComponentsInChildren<Renderer>();

        foreach (var r in renderers)
        {
            if (r.material != null)
            {
                r.material.color = fish.data.fishColor;
            }
        }
    }

    private void UpdateUI(FishInstance fish)
    {
        if (fish == null || fish.data == null) return;

        fishNameText.text = fish.data.fishName;

        float roundedWeight = Mathf.Round(fish.weight * 10f) / 10f;
        weightText.text = roundedWeight.ToString("0.0") + " kg";

        rarityText.text = fish.data.rarity.ToString();
        rarityText.color = GetRarityColor(fish.data.rarity);

        continueText.text = "Press SPACE to continue";
    }

    private Color GetRarityColor(FishRarity rarity)
    {
        return rarity switch
        {
            FishRarity.Common => Color.white,
            FishRarity.Uncommon => Color.green,
            FishRarity.Rare => Color.cyan,
            FishRarity.Epic => new Color(0.8f, 0.3f, 1f),
            _ => Color.white
        };
    }

    private void Update()
    {
        // rotation should only happen when model exists
        HandleFishRotation();

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.CatchResult)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Continuing after catch...");

            if (spawnedModel != null)
                Destroy(spawnedModel);

            gameObject.SetActive(false);
            GameManager.Instance.SetState(GameState.Idle);
        }
    }

    // recursively set layer so preview camera ONLY renders fish
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void HandleFishRotation()
    {
        if (spawnedModel == null) return;

        spawnedModel.transform.Rotate(
            Vector3.up * rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}