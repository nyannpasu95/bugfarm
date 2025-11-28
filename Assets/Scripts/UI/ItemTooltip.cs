using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI valueText;  // 改成 TextMeshProUGUI
    [SerializeField] private GameObject bottomPart;

    public void SetupTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        if (itemDetails == null) return;

        // 填充基本信息
        nameText.text = itemDetails.name;
        typeText.text = itemDetails.itemType.ToString();
        descriptionText.text = itemDetails.itemDescription;

        // -------------------------------
        // 显示所有物品价格
        // -------------------------------
        if (bottomPart != null && valueText != null)
        {
            bottomPart.SetActive(true);
            valueText.text = itemDetails.itemPrice.ToString();
        }

        // 防止布局渲染延迟
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
