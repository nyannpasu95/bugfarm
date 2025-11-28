using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        bool isMoving = moveInput.magnitude > 0;

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            animator.SetBool("IsMoving", moveInput.magnitude > 0);

            if (moveInput.x > 0)
                spriteRenderer.flipX = true; // 右走翻转
            else if (moveInput.x < 0)
                spriteRenderer.flipX = false; // 左走保持原向


        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
