using System.Collections.Generic;
using MFarm.Inventory;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [Header("Shop UI References")]
    public FunctionShopUI functionShopUI;
    public ShopUI shopUI;

    [Header("Build Button UI")]
    public GameObject buildUIButton;

    [Tooltip("Player can trigger shop UI on collision")]
    public bool enableCollisionTrigger = true;

    [Header("Building Requirements List")]
    public List<ItemRequirement> buildRequirements;

    [Header("Building Objects")]
    public GameObject buildingObject_Ruins;       // Ruins state
    public GameObject buildingObject_Completed;   // Completed state

    [Header("Use Function Shop UI")]
    public bool useFunctionShop = true;


    private void Start()
    {
        // Initial state
        if (buildingObject_Ruins != null) buildingObject_Ruins.SetActive(true);
        if (buildingObject_Completed != null) buildingObject_Completed.SetActive(false);

        // UI
        if (buildUIButton != null) buildUIButton.SetActive(true);
    }

    /// <summary>
    /// Build button click handler
    /// </summary>
    public void OnBuildButtonClicked()
    {
        // Use BuildTaskUI to pass current building and requirements
        if (BuildTaskUI.Instance != null)
        {
            BuildTaskUI.Instance.OpenForBuilding(this, buildRequirements);
        }
        else
        {
            Debug.LogWarning("BuildTaskUI instance not found");
        }
    }

    /// <summary>
    /// Called when building is complete
    /// </summary>
    public void OnBuildComplete()
    {
        // Hide build button
        if (buildUIButton != null)
            buildUIButton.SetActive(false);

        // Switch building state
        if (buildingObject_Ruins != null) buildingObject_Ruins.SetActive(false);
        if (buildingObject_Completed != null) buildingObject_Completed.SetActive(true);

        Debug.Log($"{name} building complete, collision trigger activated");
    }

    // Trigger shop UI on collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && functionShopUI != null)
        {
            // Example materials and rewards list
            List<ItemRequirement> materials = new List<ItemRequirement>()
            {
                new ItemRequirement { itemID = 101, amount = 3 },
                new ItemRequirement { itemID = 102, amount = 2 }
            };

            List<ItemRequirement> rewards = new List<ItemRequirement>()
            {
                new ItemRequirement { itemID = 201, amount = 1 }
            };

            functionShopUI.Open();
        }
        else if (!useFunctionShop && shopUI != null)
        {
            shopUI.OpenShop();
            Debug.Log($"{name} opened ShopUI");
        }
        else
        {
            Debug.LogWarning($"{name} no available UI to open");
        }
    }
}


