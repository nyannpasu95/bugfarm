using System.Collections.Generic;
using MFarm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskSubmitUI : MonoBehaviour
{
    public static TaskSubmitUI Instance;

    [Header("提交按钮")]
    public Button submitButton;

    [Header("任务描述文本")]
    public TMP_Text descriptionText;

    [Header("Slot格子预制体父物体")]
    public Transform slotParent;

    [Header("Slot格子预制体")]
    public GameObject slotPrefab;

    [Header("退出按钮")]
    public Button exitButton;

    private List<RequirementSlotUI> currentSlots = new List<RequirementSlotUI>();
    private TaskDate currentTask;
    private TreeController currentTree;

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

    /// <summary>
    /// 打开任务提交界面，由 TreeController 调用
    /// </summary>
    public void OpenUI(TaskDate task, TreeController tree)
    {
        currentTask = task;
        currentTree = tree;

        gameObject.SetActive(true);

        if (descriptionText != null)
            descriptionText.text = task != null ? task.description : "";

        // 清空旧的 slot
        if (slotParent != null)
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);
        }

        currentSlots.Clear();

        // 创建新的 slot（根据 requirements 数量）
        if (task != null && task.requirements != null)
        {
            foreach (var req in task.requirements)
            {
                GameObject go = Instantiate(slotPrefab, slotParent);
                var slotUI = go.GetComponent<RequirementSlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetRequirement(req);
                    currentSlots.Add(slotUI);
                }
            }
        }
    }

    /// <summary>
    /// 点击提交按钮
    /// </summary>
    private void OnSubmitClicked()
    {
        // 检查所有需求是否完成
        foreach (var slot in currentSlots)
        {
            if (!slot.IsCompleted())
            {
                Debug.Log("材料不足！");
                return;
            }
        }
         
        // 执行任务完成逻辑（切换树阶段）
        if (currentTree != null)
            currentTree.StartCoroutine(currentTree.SwitchToNextStage());

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击退出按钮：返还物品并关闭界面
    /// </summary>
    private void OnExitClicked()
    {
        foreach (var slot in currentSlots)
        {
            int usedAmount = slot.UsedAmount;
            int itemID = slot.RequiredItemID;

            if (usedAmount > 0 && itemID > 0)
            {
                InventoryManager.Instance.AddItemByID(itemID, usedAmount);
            }

            slot.ResetSlot();
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);

        gameObject.SetActive(false);
    }
}
