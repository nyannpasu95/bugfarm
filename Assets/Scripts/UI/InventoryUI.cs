using MFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MFarm.Inventory
{

    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance;

        [SerializeField] private SlotUI[] playerSlots;
        [Header("Dragged Item Icon")]
        public Image dragItem;
        public ItemTooltip itemTooltip;

        // Track currently highlighted slot index
        private int currentHighlightedIndex = -1;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInevntoryUI;
        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInevntoryUI;
        }
        /// <summary>
        /// ����UI��Ҫ���Ĳ��������ݶ�Ӧ��Slot�����Ʒ����������Ϣ
        /// /// </summary>
        /// <param name="location"></param>
        /// <param name="list"></param>
        private void OnUpdateInevntoryUI(InventoryLocation loc, List<InventoryItem> list)
        {
            switch (loc)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            if (item != null)
                            {
                                playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                            }
                            else
                            {
                                Debug.LogWarning($"ItemID {list[i].itemID} not found in ItemDataList_SO");
                                playerSlots[i].UpdateEmptySlot();
                            }
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }

        // Update slot highlight, supports toggling by clicking again
        public void UpdateSlotHighlight(int index)
        {
            if (index < 0 || index >= playerSlots.Length)
            {
                // Clear all highlights
                for (int i = 0; i < playerSlots.Length; i++)
                    playerSlots[i].SetHighlight(false);

                currentHighlightedIndex = -1;
                return;
            }

            if (currentHighlightedIndex == index)
            {
                // Clicking same slot again - toggle off
                playerSlots[index].SetHighlight(false);
                currentHighlightedIndex = -1;
            }
            else
            {
                // Highlight new slot, clear old highlights
                for (int i = 0; i < playerSlots.Length; i++)
                {
                    playerSlots[i].SetHighlight(i == index);
                }
                currentHighlightedIndex = index;
            }
        }
        // Currently dragged item (for external queries)
        private ItemDetails draggedItemDetails = null;
        private int draggedItemAmount = 0;

        /// <summary>
        /// Set currently dragged item
        /// </summary>
        public void SetDraggedItem(ItemDetails item, int amount)
        {
            draggedItemDetails = item;
            draggedItemAmount = amount;
        }

        /// <summary>
        /// Get currently dragged item
        /// </summary>
        public (ItemDetails, int) GetDraggedItem()
        {
            return (draggedItemDetails, draggedItemAmount);
        }

    }




}