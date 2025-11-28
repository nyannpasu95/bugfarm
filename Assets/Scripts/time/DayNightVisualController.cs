using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DayNightEnvironmentManager : MonoBehaviour
{
    [Header("时间系统引用")]
    public GameTimeWithDayNight timeSystem;

    [Header("Image 切换设置")]
    public Image dayImage;    // 白天显示的 Image
    public Image nightImage;  // 晚上显示的 Image

    [Header("动画控制设置")]
    public Animator mainAnimator;
    public string isDayParam = "isDay";

    [Header("夜晚显示对象列表")]
    [Tooltip("白天隐藏，晚上显示")]
    public List<GameObject> nightObjects = new List<GameObject>();

    private bool lastIsDay;

    void Start()
    {
        if (timeSystem == null)
        {
            Debug.LogWarning("未绑定时间系统！");
            return;
        }

        lastIsDay = timeSystem.IsDay();

        // 初始化 Image 显示
        if (dayImage != null) dayImage.gameObject.SetActive(lastIsDay);
        if (nightImage != null) nightImage.gameObject.SetActive(!lastIsDay);

        // 初始化 Animator
        if (mainAnimator != null)
            mainAnimator.SetBool(isDayParam, lastIsDay);

        // 初始化夜晚对象显示状态
        UpdateNightObjects(lastIsDay);
    }

    void Update()
    {
        if (timeSystem == null) return;

        bool isDay = timeSystem.IsDay();
        if (isDay != lastIsDay)
        {
            // 切换 Image / Animator
            if (dayImage != null) dayImage.gameObject.SetActive(isDay);
            if (nightImage != null) nightImage.gameObject.SetActive(!isDay);

            if (mainAnimator != null)
                mainAnimator.SetBool(isDayParam, isDay);

            // 切换夜晚对象显示
            UpdateNightObjects(isDay);

            lastIsDay = isDay;

            Debug.Log($"环境切换为 {(isDay ? "白天" : "夜晚")}");
        }
    }

    private void UpdateNightObjects(bool isDay)
    {
        bool active = !isDay;
        foreach (var obj in nightObjects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}
