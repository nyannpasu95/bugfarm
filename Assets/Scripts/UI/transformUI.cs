using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerUIController : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject uiPanel;

    [Header("Text List - Front Area")]
    public List<TMP_Text> frontTexts = new List<TMP_Text>();

    [Header("Text List - Back Area")]
    public List<TMP_Text> backTexts = new List<TMP_Text>();

    [Header("Player Transform")]
    public Transform player;

    [Header("Teleport Target A (when Y < 30)")]
    public Transform teleportTargetA;

    [Header("Teleport Target B (when Y > 30)")]
    public Transform teleportTargetB;

    [Header("UI Buttons")]
    public Button teleportButton;
    public Button closeButton;

    [Header("Camera Follow Script")]
    public CameraFollow cameraFollow;

    [Header("New Camera Bounds (used when teleporting to A/B)")]
    public float newMinX, newMaxX, newMinY, newMaxY;

    [Header("Original Camera Bounds (saved at start)")]
    private float originalMinX, originalMaxX, originalMinY, originalMaxY;

    private void Awake()
    {
        // Initialize UI state
        uiPanel?.SetActive(false);

        // Initialize texts
        foreach (var t in frontTexts) t.gameObject.SetActive(true);
        foreach (var t in backTexts) t.gameObject.SetActive(false);

        // Setup buttons
        if (teleportButton != null)
            teleportButton.onClick.AddListener(OnTeleportClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        // Save original camera bounds
        if (cameraFollow != null)
        {
            originalMinX = cameraFollow.minX;
            originalMaxX = cameraFollow.maxX;
            originalMinY = cameraFollow.minY;
            originalMaxY = cameraFollow.maxY;
        }
    }

    /// <summary>
    /// Teleport button handler
    /// </summary>
    private void OnTeleportClicked()
    {
        if (player == null) return;

        // Check player's current Y position
        bool isUpper = player.position.y > 30f;

        Transform target = isUpper ? teleportTargetB : teleportTargetA;
        if (target == null) return;

        // Teleport
        player.position = target.position;
        // When teleporting to A, apply new bounds; when to B, restore original bounds
        if (cameraFollow != null)
        {
            if (target == teleportTargetA)
            {
                cameraFollow.minX = newMinX;
                cameraFollow.maxX = newMaxX;
                cameraFollow.minY = newMinY;
                cameraFollow.maxY = newMaxY;
            }
            else // target == teleportTargetB
            {
                cameraFollow.minX = originalMinX;
                cameraFollow.maxX = originalMaxX;
                cameraFollow.minY = originalMinY;
                cameraFollow.maxY = originalMaxY;
            }

            // Close UI
            uiPanel?.SetActive(false);
        }
    }

    /// <summary>
    /// Close UI
    /// </summary>
    private void CloseUI()
    {
        uiPanel?.SetActive(false);
    }

    /// <summary>
    /// Show UI when player enters trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform != player) return;

        uiPanel?.SetActive(true);

        // Show appropriate text based on player's Y position
        bool isUpper = player.position.y > 30f;

        foreach (var t in frontTexts)
            t.gameObject.SetActive(!isUpper);
        foreach (var t in backTexts)
            t.gameObject.SetActive(isUpper);
    }
}
