using MFarm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RequirementSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI组件")]
    public Image itemImage;
    public TMP_Text amountText;

    private int requiredItemID;
    public int requiredAmount;
    public int currentAmount = 0;

    private const float emptyAlpha = 0.5f;
    private const float filledAlpha = 1f;

    [Header("奖励槽标记")]
    public bool isRewardSlot = false; // 奖励槽专用
    public bool isDraggable = false;  // 是否可以拖拽
    private bool hasReward = false;   // 当前是否有奖励物品

    [Header("可拖拽的物品信息")]
    public ItemDetails itemDetails;
    public int itemAmount;
    private int originalAmount; // 保存奖励槽原始数量
    // 只读属性，用于外部访问
    public int RequiredItemID => requiredItemID;
    public int UsedAmount => currentAmount;

    #region 材料槽逻辑
    public void SetRequirement(ItemRequirement req)
    {
        requiredItemID = req.itemID;
        requiredAmount = req.amount;
        currentAmount = 0;
        amountText.text = $"0/{requiredAmount}";
        itemImage.sprite = InventoryManager.Instance.GetItemDetails(requiredItemID).itemIcon;

        SetImageAlpha(emptyAlpha);
        isDraggable = false;
        isRewardSlot = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isRewardSlot && !isDraggable) return;

        var (draggedItem, amount) = InventoryUI.Instance.GetDraggedItem();
        if (draggedItem == null) return;

        // 材料槽逻辑
        if (!isRewardSlot && draggedItem.itemID == requiredItemID)
        {
            int needed = requiredAmount - currentAmount;
            int toUse = Mathf.Min(amount, needed);

            if (toUse > 0)
            {
                currentAmount += toUse;
                amountText.text = $"{currentAmount}/{requiredAmount}";

                InventoryManager.Instance.RemoveItem(requiredItemID, toUse);
                InventoryUI.Instance.SetDraggedItem(draggedItem, amount - toUse);

                if (currentAmount >= requiredAmount)
                    SetImageAlpha(filledAlpha);
                else
                    SetImageAlpha(emptyAlpha + (currentAmount / (float)requiredAmount) * 0.5f);
            }
        }
    }

    public bool IsCompleted() => currentAmount >= requiredAmount;
    public void ResetSlot()
    {
        if (isRewardSlot)
        {
            // 奖励槽逻辑
            itemAmount = originalAmount;
            amountText.text = originalAmount.ToString(); // 显示原始数量
            SetImageAlpha(emptyAlpha);
            isDraggable = false;
            hasReward = false;
        }
        else
        {
            // 材料槽逻辑
            currentAmount = 0;
            amountText.text = $"0/{requiredAmount}";
            SetImageAlpha(emptyAlpha);
            isDraggable = false;
            hasReward = false;
        }
    }

    #endregion

    #region 奖励槽拖拽逻辑
    public void InitRewardSlot(ItemDetails rewardItem, int amount)
    {
        isRewardSlot = true;
        isDraggable = false;
        hasReward = false;

        itemDetails = rewardItem;
        itemAmount = amount;
        originalAmount = amount;

        itemImage.sprite = rewardItem.itemIcon;
        amountText.text = originalAmount.ToString(); // 显示原始数量

        SetImageAlpha(emptyAlpha);
    }

    public void ActivateRewardSlot()
    {
        if (!isRewardSlot || hasReward) return;

        SetImageAlpha(filledAlpha);
        isDraggable = true;
        hasReward = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isRewardSlot || !isDraggable || itemDetails == null) return;

        InventoryUI.Instance.SetDraggedItem(itemDetails, itemAmount);
        InventoryUI.Instance.dragItem.sprite = itemImage.sprite;
        InventoryUI.Instance.dragItem.SetNativeSize();
        InventoryUI.Instance.dragItem.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isRewardSlot || !isDraggable) return;

        InventoryUI.Instance.dragItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isRewardSlot || !isDraggable) return;

        InventoryUI.Instance.dragItem.enabled = false;
        InventoryUI.Instance.SetDraggedItem(null, 0);

        // 如果拖到Player背包槽上
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            if (targetSlot != null && targetSlot.slotType == SlotType.Bag)
            {
               
                {
                    InventoryManager.Instance.AddItemByID(itemDetails.itemID, itemAmount);


                }
                ResetSlot();
            }
        }
    }
    #endregion

    public void SetImageAlpha(float alpha)
    {
        if (itemImage == null) return;
        Color c = itemImage.color;
        c.a = Mathf.Clamp01(alpha);
        itemImage.color = c;
    }
}
