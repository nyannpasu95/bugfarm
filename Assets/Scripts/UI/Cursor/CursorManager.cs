using MFarm.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    // 存储当前选中的物品详情
    private ItemDetails _selectedItemDetails;
    private GameObject player;

    private void OnEnable()
    {
        // 订阅物品选中事件
        EventHandler.ItemSelectedEvent += OnItemSelected;
    }

    private void OnDisable()
    {
        // 取消订阅
        EventHandler.ItemSelectedEvent -= OnItemSelected;
    }

    private void OnItemSelected(ItemDetails itemDetails, bool isSelected)
    {
        if (isSelected)
        {
            _selectedItemDetails = itemDetails;
            _selectedItemType = itemDetails.itemType;
        }
        else
        {
            _selectedItemDetails = null;
            _selectedItemType = ItemType.Seed; // 默认类型
        }
    }

    // 临时的HelperMethods替代方案
    private bool GetComponentsAtCursorLocation<T>(out List<T> components, Vector3 cursorPosition) where T : class
    {
        components = new List<T>();
        Collider2D[] colliders = Physics2D.OverlapPointAll(cursorPosition);

        foreach (Collider2D collider in colliders)
        {
            T component = collider.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }
        }

        return components.Count > 0;
    }

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        // Get position for cursor
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        // Set cursor sprite
        SetCursorValidity(cursorWorldPosition, player.transform.position);

        // Get rect transform position for cursor
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();

        // Check use radius corners
        if (
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f)
            )
        {
            SetCursorToInvalid();
            return;
        }

        // Check item use radius is valid
        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius
            || Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // 使用存储的选中物品详情
        if (_selectedItemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // Determine cursor validity based on inventory item selected and what object the cursor is over
        switch (_selectedItemDetails.itemType)
        {
            case ItemType.Tool: // 工具类型
                if (!SetCursorValidityTool(cursorPosition, playerPosition, _selectedItemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.Seed:
            case ItemType.Crop:
            case ItemType.Material:
            case ItemType.Creature:
            case ItemType.Consumable:
            case ItemType.Placeable:
                // 这些类型的物品可能有不同的光标验证逻辑
                // 暂时保持有效状态
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set the cursor to be valid
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;

        if (gridCursor != null)
            gridCursor.DisableCursor();
    }

    /// <summary>
    /// Set the cursor to be invalid
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;

        if (gridCursor != null)
            gridCursor.EnableCursor();
    }

    /// <summary>
    /// Sets the cursor as either valid or invalid for the tool for the target. Returns true if valid or false if invalid
    /// </summary>
    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        // 根据工具的具体用途来判断有效性
        // 这里可以根据itemDetails中的其他属性（如工具效果）来细化判断

        // 临时实现：检查是否有可交互的对象在光标位置
        List<Crop> crops = new List<Crop>();
        if (GetComponentsAtCursorLocation(out crops, cursorPosition))
        {
            // 如果有作物，工具可能有效
            return crops.Count > 0;
        }

        // 默认返回true，需要根据具体工具类型细化
        return true;
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public Vector3 GetWorldPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }
}