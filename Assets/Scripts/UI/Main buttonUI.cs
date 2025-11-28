using UnityEngine;
using UnityEngine.UI;

public class UIPanelToggle : MonoBehaviour
{
    [Header("UI Panel to Open")]
    public GameObject panel;

    [Header("Exit Button Reference (Optional)")]
    public Button exitButton;

    // Called when button OnClick() is triggered
    public void OpenPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);

            // If exit button exists, bind close event
            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(ClosePanel);
            }
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
