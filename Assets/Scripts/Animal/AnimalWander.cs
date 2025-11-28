using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AnimalWander2D : MonoBehaviour
{
    [Header("活动边界 (世界范围限制)")]
    public Vector2 worldMin = new Vector2(-10f, -5f);
    public Vector2 worldMax = new Vector2(10f, 5f);

    [Header("移动参数")]
    public float moveSpeed = 1.5f;
    public Vector2 idleTimeRange = new Vector2(1f, 3f);
    [Tooltip("每次随机移动的最大距离")]
    public float maxMoveDistance = 3f;

    [Header("卡住检测")]
    public float stuckTimeThreshold = 3f;   // 秒数
    public float minMoveDistance = 0.5f;    // 小于这个认为没动

    private Vector2 targetPos;
    private bool isMoving = false;
    private float idleTimer = 0f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 新增卡住检测
    private Vector2 lastPos;
    private float stuckTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChooseNewTarget();
        lastPos = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            // 移动
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 到达目标
            if (Vector2.Distance(transform.position, targetPos) < 0.001f)
            {
                isMoving = false;
                idleTimer = Random.Range(idleTimeRange.x, idleTimeRange.y);
                animator.SetBool("Is Moving", false);
                stuckTimer = 0f; // 重置卡住计时器
            }

            // 卡住检测
            float movedDistance = Vector2.Distance(transform.position, lastPos);
            if (movedDistance < minMoveDistance)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckTimeThreshold)
                {
                    ChooseNewTarget(); // 选新目标
                    stuckTimer = 0f;  // 重置计时
                }
            }
            else
            {
                stuckTimer = 0f; // 有移动就重置计时
            }

            lastPos = transform.position;
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
                ChooseNewTarget();
        }
    }

    private void ChooseNewTarget()
    {
        Vector2 currentPos = transform.position;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector2 candidate = currentPos + randomDir * Random.Range(1f, maxMoveDistance);

        candidate.x = Mathf.Clamp(candidate.x, worldMin.x, worldMax.x);
        candidate.y = Mathf.Clamp(candidate.y, worldMin.y, worldMax.y);

        targetPos = candidate;
        isMoving = true;
        animator.SetBool("Is Moving", true);

        Vector3 dir = (targetPos - currentPos).normalized;
        animator.SetFloat("Move X", dir.x);
        animator.SetFloat("Move Y", dir.y);

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            spriteRenderer.flipX = (dir.x > 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (worldMin + worldMax) / 2f;
        Vector2 size = worldMax - worldMin;
        Gizmos.DrawWireCube(center, size);

        if (Application.isPlaying)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, maxMoveDistance);
        }
    }
}
