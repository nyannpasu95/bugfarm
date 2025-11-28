using MFarm.Inventory;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SellSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI组件")]
    public Image itemImage;
    public Button sellButton;

    private ItemDetails currentItem;
    private int currentAmount;

    private InventoryUI inventoryUI => InventoryUI.Instance;

    private void Awake()
    {
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnSellClicked);
        }

        ClearSlot();
    }

    #region 拖入槽
    public void OnDrop(PointerEventData eventData)
    {
        var (draggedItem, draggedAmount) = inventoryUI.GetDraggedItem();
        if (draggedItem == null || draggedAmount <= 0) return;

        // 如果槽里已有物品，先返还到背包
        if (currentItem != null)
        {
            InventoryManager.Instance.AddItemByID(currentItem.itemID, currentAmount);
        }

        // 接收新物品
        currentItem = draggedItem;
        currentAmount = draggedAmount;
        UpdateSlotVisual();

        InventoryManager.Instance.RemoveItem(currentItem.itemID, currentAmount);

        // 清空拖拽状态
        inventoryUI.SetDraggedItem(null, 0);
        inventoryUI.dragItem.enabled = false;
    }
    #endregion

    #region 卖出功能
    private void OnSellClicked()
    {
        if (currentItem == null || currentAmount <= 0) return;

        int totalPrice = currentItem.itemPrice * currentAmount;
        PlayerMoney.Instance.AddMoney(totalPrice);

        ClearSlot();
    }
    #endregion

    #region 拖拽槽内物品回背包
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null || currentAmount <= 0) return;

        // 设置拖拽物
        inventoryUI.SetDraggedItem(currentItem, currentAmount);
        inventoryUI.dragItem.sprite = currentItem.itemIcon;
        inventoryUI.dragItem.enabled = true;
        inventoryUI.dragItem.SetNativeSize();

        // 清空槽内
        ClearSlot();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventoryUI.dragItem.enabled)
            inventoryUI.dragItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryUI.dragItem.enabled)
        {
            var (draggedItem, draggedAmount) = inventoryUI.GetDraggedItem();
            if (draggedItem != null && draggedAmount > 0)
            {
                InventoryManager.Instance.AddItemByID(draggedItem.itemID, draggedAmount);
            }

            inventoryUI.SetDraggedItem(null, 0);
            inventoryUI.dragItem.enabled = false;
        }
    }
    #endregion

    #region 辅助方法
    public void ClearSlot()
    {
        currentItem = null;
        currentAmount = 0;

        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.color = Color.clear;
            itemImage.enabled = true;
        }
    }

    private void UpdateSlotVisual()
    {
        if (itemImage != null && currentItem != null)
        {
            itemImage.sprite = currentItem.itemIcon;
            itemImage.color = Color.white;
        }
    }
    #endregion
}
