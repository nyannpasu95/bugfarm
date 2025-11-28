using System.Collections.Generic;
using MFarm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildTaskUI : MonoBehaviour
{
    public static BuildTaskUI Instance;

    [Header("提交按钮")]
    public Button submitButton;

    [Header("任务描述文本")]
    public TMP_Text descriptionText;

    [Header("Slot格子预制体父物体")]
    public Transform slotsParent;

    [Header("Slot格子预制体")]
    public GameObject slotPrefab;

    [Header("退出按钮")]
    public Button exitButton;


    private List<RequirementSlotUI> requirementSlots = new List<RequirementSlotUI>();
    private BuildingController currentBuilding;
    private List<ItemRequirement> currentRequirements;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);


        gameObject.SetActive(false);
    }

    public void OpenForBuilding(BuildingController building, List<ItemRequirement> requirements)
    {
        currentBuilding = building;
        currentRequirements = requirements;

        gameObject.SetActive(true);
        if (descriptionText != null)

            // 🧹【新增】彻底清空旧的 Slot 对象
            foreach (Transform child in slotsParent)
                Destroy(child.gameObject);


        // 清空旧slot
        foreach (var slot in requirementSlots)
            Destroy(slot.gameObject);
        requirementSlots.Clear();

        // 创建新slot
        foreach (var req in requirements)
        {
            GameObject go = Instantiate(slotPrefab, slotsParent);
            var slotUI = go.GetComponent<RequirementSlotUI>();
            slotUI.SetRequirement(req);
            requirementSlots.Add(slotUI);
        }
    }

    private void OnSubmitClicked()
    {
        // 检查是否全部完成
        foreach (var slot in requirementSlots)
        {
            if (!slot.IsCompleted())
            {
                Debug.Log("材料不足！");
                return;
            }
        }

        
        // 通知建筑完成
        currentBuilding.OnBuildComplete();

        // 关闭面板
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 点击退出按钮
    /// </summary>
    private void OnExitClicked()
    {
        // 返还已提交材料
        foreach (var slot in requirementSlots)
        {
            // 判断该格子是否有已提交的物品
            int usedAmount = slot.UsedAmount;
            int itemID = slot.RequiredItemID;

            if (usedAmount > 0 && itemID > 0)
            {
              InventoryManager.Instance.AddItemByID(itemID, usedAmount);
                
            }


            // 重置Slot显示
            slot.ResetSlot();
        }

        // 更新一次背包UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);

        // 最后关闭建造面板
        gameObject.SetActive(false);
    }

}
