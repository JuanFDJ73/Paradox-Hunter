using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Salto")]
    public float jumpForce = 10f;              // Fuerza del salto en tierra
    public float airJumpForce = 7f;            // Fuerza del salto en el aire

    [Header("Coyote Time")]
    public float coyoteTime = 0.25f;           // Tiempo de gracia después de dejar el suelo

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    [Tooltip("Activar para ver mensajes de debug en consola")]
    public bool debugMode = false;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    // Estado del salto
    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteTimeCounter;
    private int jumpsRemaining;
    private const int maxJumps = 2;
    private bool hasUsedGroundJump;
    private bool justJumped;  // Evita reset inmediato al saltar

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            TryJump();
        }
    }
    
    private void Update()
    {
        CheckGrounded();
        UpdateCoyoteTime();
        
        if (debugMode)
        {
            Debug.Log($"Grounded: {isGrounded} | Jumps: {jumpsRemaining}");
        }
    }

    // Verifica si el jugador está en el suelo
    private void CheckGrounded()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;

        // Solo resetear si está en el suelo Y no acaba de saltar
        if (isGrounded && !justJumped)
        {
            jumpsRemaining = maxJumps;
            hasUsedGroundJump = false;
            coyoteTimeCounter = coyoteTime;
        }
        
        // Si ya no está en el suelo, permitir reset en el próximo aterrizaje
        if (!isGrounded)
        {
            justJumped = false;
        }
        else if (wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
    }

    // Actualiza el contador de coyote time
    private void UpdateCoyoteTime()
    {
        if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void TryJump()
    {
        bool canCoyoteJump = coyoteTimeCounter > 0;
        bool canGroundJump = isGrounded || canCoyoteJump;

        if (canGroundJump && !hasUsedGroundJump)
        {
            // Salto de tierra
            hasUsedGroundJump = true;
            justJumped = true;  // Evitar que se reseteen los saltos inmediatamente
            jumpsRemaining = 1; // Solo queda 1 salto (el del aire)
            coyoteTimeCounter = 0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (jumpsRemaining > 0)
        {
            // Salto en el aire
            jumpsRemaining--;
            rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
        }
    }

    private void FixedUpdate()
    {
        // Movimiento horizontal
        float moveX = moveInput.x;
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        animator.SetBool("isRunning", moveX != 0);

        if (moveX != 0)
        {
            transform.localScale = new Vector3(moveX > 0 ? 1 : -1, 1, 1);
        }
    }

    // Visualización del ground check en el editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * groundCheckRadius, 0.05f);
        }
    }
}