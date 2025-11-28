using UnityEngine;

public class FarmingEmergencyFix : MonoBehaviour
{
    [Header("紧急修复设置")]
    public bool autoFixOnStart = true;
    public int gridSize = 20; // 创建20x20的测试网格

    private void Start()
    {
        if (autoFixOnStart)
        {
            Invoke("RunEmergencyFix", 1f); // 延迟1秒执行，确保其他组件已初始化
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RunEmergencyFix();
        }
    }

    [ContextMenu("运行紧急修复")]
    public void RunEmergencyFix()
    {
        Debug.Log("🚨 开始运行耕种系统紧急修复");

        // 检查关键组件
        CheckGridPropertiesManager();
        CheckPlayerFarming();
        CheckCursorSystem();

        Debug.Log("✅ 紧急修复完成");
    }

    private void CheckGridPropertiesManager()
    {
        Debug.Log("=== 检查GridPropertiesManager ===");

        if (GridPropertiesManager.Instance == null)
        {
            Debug.LogError("❌ GridPropertiesManager实例不存在");
            return;
        }


    }


    private void CheckPlayerFarming()
    {
        Debug.Log("=== Checking PlayerFarming ===");

        PlayerFarming playerFarming = FindFirstObjectByType<PlayerFarming>();
        if (playerFarming == null)
        {
            Debug.LogError("PlayerFarming component not found");
            return;
        }

        // Check component references
        if (playerFarming.mainCamera == null)
        {
            playerFarming.mainCamera = Camera.main;
            Debug.Log("Auto-assigned Main Camera");
        }

        if (playerFarming.grid == null)
        {
            playerFarming.grid = FindFirstObjectByType<Grid>();
            Debug.Log("Auto-assigned Grid component");
        }

        Debug.Log($"PlayerFarming component OK");
    }

    private void CheckCursorSystem()
    {
        Debug.Log("=== Checking Cursor System ===");

        CursorManager cursorManager = FindFirstObjectByType<CursorManager>();
        if (cursorManager == null)
        {
            Debug.LogWarning("CursorManager not found");
        }
        else
        {
            Debug.Log("CursorManager exists");
        }

        GridCursor gridCursor = FindFirstObjectByType<GridCursor>();
        if (gridCursor == null)
        {
            Debug.LogWarning("GridCursor not found");
        }
        else
        {
            Debug.Log("GridCursor exists");
        }
    }
}