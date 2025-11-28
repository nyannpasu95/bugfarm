using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class GameTimeWithDayNight : MonoBehaviour
{
    [Header("时间设置")]
    public float dayDuration = 600f;   // 白天现实时间（秒）
    public float nightDuration = 600f; // 夜晚现实时间（秒）

    [Header("UI显示")]
    public TextMeshProUGUI timeText;

    [Header("Volume设置")]
    public Volume nightVolume;
    [Tooltip("入夜前现实时间渐变持续时间（秒）")]
    public float nightTransitionDuration = 60f; // 1分钟渐变

    // 游戏时间
    private float day;
    private float currentTime = 6f; // 游戏小时
    private float gameMinute = 0f;

    private bool isDay = true; // 白天状态
    private bool isTransitioning = false;

    private float minutesPerSecond;

    void Start()
    {
        SetMinutesPerSecond();
        if (nightVolume != null)
            nightVolume.weight = 0f;
        UpdateTimeText();
    }

    void Update()
    {
        gameMinute += minutesPerSecond * Time.deltaTime;

        if (gameMinute >= 60f)
        {
            gameMinute -= 60f;
            currentTime += 1f;
            if (currentTime >= 24f)
                currentTime = 0f;
        }

        float displayMinute = Mathf.Floor(gameMinute / 10f) * 10f;
        UpdateTimeText((int)displayMinute);

        // 白天/夜晚切换
        if (isDay && currentTime >= 18f)
        {
            isDay = false;
            SetMinutesPerSecond();
            isTransitioning = false;
        }
        else if (!isDay && currentTime >= 6f && currentTime < 18f)
        {
            isDay = true;
            SetMinutesPerSecond();
            isTransitioning = false;
        }
        EventHandler.CallAdvanceGameDayEvent();

        // 入夜渐变
        if (isDay && currentTime == 17 && !isTransitioning)
        {
            StartCoroutine(FadeVolume(nightVolume.weight, 1f, nightTransitionDuration));
            isTransitioning = true;
        }

        // 夜晚结束渐变
        if (!isDay && currentTime == 5 && !isTransitioning)
        {
            StartCoroutine(FadeVolume(nightVolume.weight, 0f, nightTransitionDuration));
            isTransitioning = true;
        }
    }

    void SetMinutesPerSecond()
    {
        minutesPerSecond = isDay ? 720f / dayDuration : 720f / nightDuration;
    }

    void UpdateTimeText(int minute = 0)
    {
        if (timeText != null)
            timeText.text = $"{Mathf.FloorToInt(currentTime):00}:{minute:00}";
    }

    private System.Collections.IEnumerator FadeVolume(float from, float to, float duration)
    {
        if (nightVolume == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            nightVolume.weight = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        nightVolume.weight = to;
    }

    // ✅ 对外公开方法，让其他脚本可以判断昼夜
    public bool IsDay()
    {
        return isDay;
    }
}
