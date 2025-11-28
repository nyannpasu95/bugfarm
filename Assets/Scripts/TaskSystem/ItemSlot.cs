using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public ItemDetails item;
    public int itemAmount;
    public Text amountText;

    public void ClearSlot()
    {
        item = null;
        itemAmount = 0;
        icon.sprite = null;
        icon.enabled = false;
        if (amountText != null) amountText.text = "";
    }

    public void UpdateSlotUI()
    {
        if (item == null || itemAmount <= 0)
        {
            icon.enabled = false;
            if (amountText != null) amountText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = item.itemIcon;
            if (amountText != null) amountText.text = itemAmount.ToString();
        }
    }
}
