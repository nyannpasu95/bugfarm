using System;
using System.Collections.Generic;
using MFarm.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class FunctionShopUI : MonoBehaviour
{
    

    [Header("面板")]
    public GameObject panel;

    [Header("完成按钮")]
    public Button completeButton;

    [Header("退出按钮")]
    public Button exitButton;

    [Header("多任务列表")]
    public List<FunctionTaskData> taskList = new List<FunctionTaskData>();



    private List<RequirementSlotUI> requirementSlots = new List<RequirementSlotUI>();
    private List<RequirementSlotUI> rewardSlots = new List<RequirementSlotUI>();

    [Serializable]
    public class FunctionTaskData
    {
        public string taskName;                   // 可选
        public Transform slotsParent;             // 材料槽父物体
        public GameObject slotPrefab;             // 材料槽Prefab
        public List<ItemRequirement> materialList;

        public Transform rewardSlotParent;        // 奖励槽父物体
        public GameObject rewardSlotPrefab;       // 奖励槽Prefab
        public List<ItemRequirement> rewardList;

        // 运行时使用
        [NonSerialized] public List<RequirementSlotUI> requirementSlots = new List<RequirementSlotUI>();
        [NonSerialized] public List<RequirementSlotUI> rewardSlots = new List<RequirementSlotUI>();
    }

    private void Awake()
    {
        

        if (panel != null) panel.SetActive(false);

        if (completeButton != null)
            completeButton.onClick.AddListener(OnCompleteClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    public void Open()
    {
        panel.SetActive(true);

        requirementSlots.Clear();
        rewardSlots.Clear();

        // 🔹 遍历任务列表
        foreach (var task in taskList)
        {
            foreach (var slot in task.requirementSlots) Destroy(slot.gameObject);
            foreach (var slot in task.rewardSlots) Destroy(slot.gameObject);
            task.requirementSlots.Clear();
            task.rewardSlots.Clear();

            // 材料槽
            foreach (var req in task.materialList)
            {
                GameObject go = Instantiate(task.slotPrefab, task.slotsParent);
                var slotUI = go.GetComponent<RequirementSlotUI>();
                slotUI.SetRequirement(req);
                task.requirementSlots.Add(slotUI);
                requirementSlots.Add(slotUI);
            }

            // 奖励槽
            foreach (var reward in task.rewardList)
            {
                GameObject go = Instantiate(task.rewardSlotPrefab, task.rewardSlotParent);
                var slotUI = go.GetComponent<RequirementSlotUI>();
                slotUI.InitRewardSlot(InventoryManager.Instance.GetItemDetails(reward.itemID), reward.amount);
                task.rewardSlots.Add(slotUI);
                rewardSlots.Add(slotUI);
            }
        }
    }

    private void OnCompleteClicked()
    {
        foreach (var task in taskList)
        {
            bool taskCompleted = true;
            // 检查材料是否全部提交
            foreach (var slot in task.requirementSlots)
            {
                if (!slot.IsCompleted())
                {
                    taskCompleted = false;
                    break;
                }
            }
            if (!taskCompleted)
                continue;

                // ✅ 2. 从玩家背包移除已提交的材料
                foreach (var slot in task.requirementSlots)
            {
                if (slot.itemDetails != null && slot.currentAmount >= slot.requiredAmount)
                {
                    InventoryManager.Instance.RemoveItem(slot.itemDetails.itemID, slot.requiredAmount);
                    slot.ResetSlot(); // 清空UI
                }
            }
            // 激活奖励槽
            foreach (var rewardSlot in task.rewardSlots)
                rewardSlot.ActivateRewardSlot();

            Debug.Log("材料提交完成，奖励槽已激活！");
        }
    }

    // 一键领取奖励到背包
    public void CollectAllRewards()
    {
        foreach (var rewardSlot in rewardSlots)
        {
            if (!rewardSlot.isDraggable || rewardSlot.itemDetails == null)
                continue;

            // 用 InventoryManager 的 AddItemAtIndex 方法直接添加
            int itemID = rewardSlot.itemDetails.itemID;
            int amount = rewardSlot.itemAmount;

            // 找到背包里已有的索引
            int index = InventoryManager.Instance.GetItemIndexInBag(itemID);

            for (int i = 0; i < amount; i++)
            {
                InventoryManager.Instance.AddItemAtIndex(itemID, index, 1);
            }

            rewardSlot.ResetSlot();
        }

        // 刷新背包UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,
            InventoryManager.Instance.playerBag.itemList);
    }
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
                
                {
                    InventoryManager.Instance.AddItemByID(itemID, usedAmount);
                }
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

