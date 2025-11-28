using UnityEngine;
using UnityEngine.UI;
using MFarm.Inventory;
using TMPro;

public class ShopUI : MonoBehaviour  
{
    [Header("商店面板")]
    [SerializeField] private GameObject shopPanel;

    [Header("商店按钮列表")]
    public ShopButton[] shopButtons;


    [Header("关闭按钮")]
    [SerializeField] private Button closeButton;

    [System.Serializable]
    public class ShopButton
    {
        public ItemDetails item;   // 对应物品
        public Button button;      // 对应按钮
        [Header("可选UI显示")]
        public Image icon;         // 按钮上的物品图标
        public TMP_Text nameText;  // 按钮上的物品名称
        public TMP_Text priceText; // 按钮上的价格文本
    }

    private void Start()
    {
        // 初始化按钮
        for (int i = 0; i < shopButtons.Length; i++)
        {
            var sb = shopButtons[i];
            if (sb.item != null && sb.button != null)
            {
                ItemDetails currentItem = sb.item;
                sb.button.onClick.RemoveAllListeners(); // 确保没有残留监听
                sb.button.onClick.AddListener(() => BuyItem(currentItem));

            }
        }

        // 绑定关闭按钮
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseShop);
        }

        // 默认隐藏面板
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    // 购买逻辑
    private void BuyItem(ItemDetails item)
    {
        if (item == null) return;

        if (PlayerMoney.Instance.SpendMoney(item.itemPrice))
        {
            InventoryManager.Instance.AddItemByID(item.itemID, 1);
            
        }
        else
        {
            Debug.Log("金币不足，购买失败");
        }
        

    }

    // 打开商店
    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    // 关闭商店
    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    // 切换商店显示状态
    public void ToggleShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(!shopPanel.activeSelf);
    }
}
