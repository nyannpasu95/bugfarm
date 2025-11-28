using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGamePanelUI : MonoBehaviour
{
    [Header("UI Panel (Default Closed)")]
    public GameObject panel;

    [Header("Exit Button")]
    public Button exitButton;

    [Header("Confirm Button")]
    public Button confirmButton;

    [Header("Game Scene Name to Load")]
    public string gameSceneName = "Farm";

    // Called when start game button is clicked
    private void Start()
    {
        // Ensure UI panel is closed when entering scene
        if (panel != null)
            panel.SetActive(false);
    }
    public void OpenPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);

            // Setup button events
            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(ClosePanel);
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(LoadGameScene);
            }
        }
    }

    // Close UI panel
    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // Load game scene
    public void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("Please set the scene name to load in Inspector");
        }
    }
}
