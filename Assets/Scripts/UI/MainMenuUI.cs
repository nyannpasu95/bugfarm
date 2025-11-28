using UnityEngine;
using UnityEngine.SceneManagement; // 用于场景切换
using UnityEngine.UI;              // 用于按钮交互

public class MainMenuUI : MonoBehaviour
{
    [Header("按钮引用")]
    public Button newGameButton;
    public Button quitButton;

    [Header("主场景名称")]
    public string mainSceneName = "GameScene";

    private void Start()
    {
        // 注册按钮点击事件
        newGameButton.onClick.AddListener(StartNewGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartNewGame()
    {
        Debug.Log("开始新游戏...");
        SceneManager.LoadScene(mainSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("退出游戏...");
#if UNITY_EDITOR
        // 如果在 Unity 编辑器中运行
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 如果是打包后的游戏
        Application.Quit();
#endif
    }
}
