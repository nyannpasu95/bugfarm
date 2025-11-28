using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Component References")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Image slotHighlight;
        [SerializeField] private Button button;

        [Header("Slot Type (Bag/Shop/Action/Warehouse)")]
        public SlotType slotType;

        public bool isSelected;
        public ItemDetails itemDetails;
        public int itemAmount;

        [Header("Current Slot Index")]
        public int slotIndex;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        // Start is called before the first frame update
        void Start()
        {
            // Initially not selected
            isSelected = false;
            UpdateHighlight(); // Ensure no highlight at start

            if (itemDetails == null || itemDetails.itemID == 0)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// Update slot UI info
        /// </summary>
        /// <param name="item">itemDetails</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            slotImage.enabled = true; // Show image
            itemAmount = amount;
            amountText.text = amount.ToString();
            button.interactable = true;
        }

        /// <summary>
        /// Set as empty slot, cannot be selected or interacted
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
            itemDetails = null;
            itemAmount = 0;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize(); // Prevent image distortion during drag
                isSelected = true;
                inventoryUI.UpdateSlotHighlight(slotIndex);

                // Record dragged item
                inventoryUI.SetDraggedItem(itemDetails, itemAmount);
            }
        }


        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {

            inventoryUI.dragItem.enabled = false;

            // Clear dragged item
            inventoryUI.SetDraggedItem(null, 0);


            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null) return;

                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                // Within player bag range, swap via index through manager
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }

                inventoryUI.UpdateSlotHighlight(-1);
            }
            else // Dropped outside, spawn in scene
            {
                if (itemDetails.canDropped)
                {
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
                    EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;

            // Pass own index to inventory UI for highlight toggle
            inventoryUI.UpdateSlotHighlight(slotIndex);
        }

        // Called from InventoryUI to control highlight display
        public void SetHighlight(bool active)
        {
            isSelected = active;
            UpdateHighlight();
        }

        private void UpdateHighlight()
        {
            if (slotHighlight != null)
            {
                slotHighlight.gameObject.SetActive(isSelected);
            }
        }
    }
}
