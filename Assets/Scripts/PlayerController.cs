using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        /*
        // Only allow one direction at a time
        if (moveY != 0)
        {
            moveX = 0; // Prioritize vertical movement
        }
        */

        // Create movement vector and normalize to prevent faster diagonal movement
        Vector2 movement = new Vector2(moveX, moveY);
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Apply movement to the Rigidbody2D (use velocity)
        rb.linearVelocity = movement * moveSpeed;

        // Update animator 'IsRunning' flag if Animator exists
        bool isMoving = (moveX != 0f) || (moveY != 0f);
        if (animator != null)
        {
            animator.SetBool("IsRunning", isMoving);
        }

        // Flip sprite horizontally when moving left/right
        if (spriteRenderer != null && moveX != 0f)
        {
            spriteRenderer.flipX = moveX < 0f;
        }
    }
}
