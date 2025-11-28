using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemItemDataList_SO;
        public InventoryBag_SO playerBag;
        public GameObject bagUI;
        private bool bagOpened = false;

        // Find itemDetails by ID, returns ID
        public ItemDetails GetItemDetails(int id)
        {
            return itemItemDataList_SO.itemDetailsList.Find(i => i.itemID == id);
        }

        /// <summary>
        /// Query total amount of an item in bag
        /// </summary>
        public int GetItemAmount(int itemID)
        {
            int total = 0;
            if (playerBag == null || playerBag.itemList == null) return 0;

            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == itemID)
                    total += playerBag.itemList[i].itemAmount;
            }

            return total;
        }

        /// <summary>
        /// Remove specified amount of item from bag
        /// </summary>
        public bool RemoveItem(int itemID, int amount)
        {
            if (amount <= 0) return true;

            if (playerBag == null || playerBag.itemList == null) return false;

            int remaining = amount;

            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == itemID)
                {
                    int slotAmount = playerBag.itemList[i].itemAmount;
                    if (slotAmount > remaining)
                    {
                        playerBag.itemList[i] = new InventoryItem
                        {
                            itemID = itemID,
                            itemAmount = slotAmount - remaining
                        };
                        remaining = 0;
                        break;
                    }
                    else
                    {
                        remaining -= slotAmount;
                        playerBag.itemList[i] = new InventoryItem();
                    }
                }
            }

            // Update UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);

            return remaining == 0;
        }

        /// <summary>
        /// Add item to player bag
        /// </summary>
        public void AddItem(Item item, bool toDestory, int amount)
        {
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Check if bag has empty slots
        /// </summary>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find if item exists in bag by ID
        /// </summary>
        public int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }
            return -1;
        }

        public void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Swap items within player bag
        /// </summary>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        /// <summary>
        /// Add items directly to bag by item ID, without Item script instance
        /// </summary>
        public void AddItemByID(int itemID, int amount)
        {
            if (amount <= 0) return;

            int remaining = amount;
            while (remaining > 0)
            {
                int index = GetItemIndexInBag(itemID); // Find existing item
                if (index == -1) // Not found, find empty slot
                {
                    bool hasSpace = false;
                    for (int i = 0; i < playerBag.itemList.Count; i++)
                    {
                        if (playerBag.itemList[i].itemID == 0)
                        {
                            playerBag.itemList[i] = new InventoryItem { itemID = itemID, itemAmount = 1 };
                            hasSpace = true;
                            break;
                        }
                    }
                    if (!hasSpace)
                    {
                        Debug.LogWarning("Bag is full, cannot add more items");
                        break; // No space, stop
                    }
                }
                else
                {
                    InventoryItem existing = playerBag.itemList[index]; // Get copy
                    existing.itemAmount += 1;                          // Modify copy
                    playerBag.itemList[index] = existing;              // Write back to list
                }

                remaining--;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}
