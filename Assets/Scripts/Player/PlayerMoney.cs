using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney Instance;

    [Header("初始金币数量")]
    public int startMoney = 100;

    [Header("UI显示")]
    [SerializeField] private TMP_Text moneyText;

    private int currentMoney;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentMoney = startMoney;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.Log("金币不足，购买失败！");
            return false;
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public int GetCurrentMoney() => currentMoney;

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $" {currentMoney}";
    }
}
