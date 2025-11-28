using System.Collections;
using MFarm.Inventory;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("动画控制器")]
    public Animator animator;

    [Header("当前阶段（动画序号从1开始）")]
    public int currentStage = 1;

    [Header("总阶段数")]
    public int totalStages = 5;

    [Header("每个阶段的等比缩放")]
    public float[] stageScales;

    [Header("每个阶段的Y轴偏移量")]
    public float[] stageYOffset;

    [Header("淡出淡入时间")]
    public float fadeDuration = 0.5f;

    [Header("玩家对象")]
    public Transform player;

    [Header("自动进入下一阶段距离（仅第三阶段）")]
    public float autoAdvanceDistance = 1f;

    [Header("任务数据源")]
    public TaskData_SO taskDataSO;

    [Header("最后阶段完成UI")]
    public GameObject finalStageUI; 


    private bool isTransitioning = false;

    private void Start()
    {
        PlayStage(currentStage);
    }

    private void Update()
    {
        // 第三阶段靠近玩家自动触发下一阶段
        if (currentStage == 3 && !isTransitioning && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= autoAdvanceDistance)
            {
                StartCoroutine(SwitchToNextStage());
            }
        }
    }

    public void OnTreeClicked()
    {
        if (isTransitioning) return;

        // 阶段三靠近玩家自动触发，不允许点击
        if (currentStage == 3) return;

        // 判断是否是最后阶段
        if (currentStage == totalStages)
        {
            if (finalStageUI != null)
                finalStageUI.SetActive(true); // 显示最后阶段UI
            return; // 阻止其他逻辑
        }
        // 尝试找到当前阶段任务
        TaskDate task = null;
        if (taskDataSO != null)
            task = taskDataSO.TaskDateList.Find(t => t.stage == currentStage);

        // 有任务且需要 UI 弹面板
        if (task != null && task.hasUI)
        {
            TaskSubmitUI.Instance?.OpenUI(task, this);
            return;
        }

        // 没任务或者任务不弹 UI，直接切换下一阶段
        StartCoroutine(SwitchToNextStage());
    }

    public IEnumerator SwitchToNextStage()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // 保存当前完成阶段任务（奖励用）
        int completedStage = currentStage;
        TaskDate completedTask = null;
        if (taskDataSO != null)
            completedTask = taskDataSO.TaskDateList.Find(t => t.stage == completedStage);


        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("未找到子物体的 SpriteRenderer！");
            yield break;
        }

        Color originalColor = sr.color;

        // 渐隐
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }


        // 切换阶段
        currentStage++;
        if (currentStage > totalStages) currentStage = 1;

        PlayStage(currentStage);

        // 渐显
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        sr.color = originalColor;
        isTransitioning = false;

        // ===== 发放奖励 =====
        if (completedTask != null && completedTask.rewards != null)
        {
            foreach (var reward in completedTask.rewards)
            {
                if (reward != null && reward.itemID > 0 && reward.amount > 0)
                {
                    InventoryManager.Instance.AddItemByID(reward.itemID, reward.amount);
                    Debug.Log($"奖励发放：ItemID {reward.itemID} × {reward.amount}");
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);
        }

    }

    private void PlayStage(int stage)
    {
        if (animator != null)
        {
            string stateName = $"tree_{stage:D2}";
            animator.Play(stateName);
        }

        if (stageScales != null && stageScales.Length >= stage)
            transform.localScale = Vector3.one * stageScales[stage - 1];

        if (stageYOffset != null && stageYOffset.Length >= stage)
        {
            Vector3 pos = transform.position;
            pos.y = stageYOffset[stage - 1];
            transform.position = pos;
        }
    }
}
