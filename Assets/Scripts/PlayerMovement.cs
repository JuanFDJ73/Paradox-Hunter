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

    [Header("Daño")]
    public float knockbackForceX = 5f;         // Fuerza del empuje horizontal
    public float knockbackForceY = 3f;         // Fuerza del empuje vertical
    public float hurtDuration = 0.5f;          // Duración de la animación de daño
    public float immunityDuration = 1.5f;      // Duración de la inmunidad

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

    // Estado de daño
    private bool isHurt = false;
    private bool isImmune = false;
    private float hurtTimer;
    private float immunityTimer;
    private SpriteRenderer spriteRenderer;
    private int originalLayer;
    private int immuneLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Guardar el layer original (Player) y obtener el layer para inmunidad
        originalLayer = gameObject.layer;
        immuneLayer = LayerMask.NameToLayer("Ignore Raycast"); // Usa un layer que no colisione con Enemies
    }

    public void OnMove(InputValue value)
    {
        // Siempre capturar el input (para que funcione al terminar el daño)
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        // Solo saltar si no está herido
        if (isHurt) return;
        
        if (value.isPressed)
        {
            TryJump();
        }
    }
    
    private void Update()
    {
        CheckGrounded();
        UpdateCoyoteTime();
        UpdateHurtState();
        UpdateImmunity();
        
        if (debugMode)
        {
            Debug.Log($"Grounded: {isGrounded} | Jumps: {jumpsRemaining} | Hurt: {isHurt} | Immune: {isImmune}");
        }
    }

    private void UpdateHurtState()
    {
        if (isHurt)
        {
            hurtTimer -= Time.deltaTime;
            
            if (hurtTimer <= 0)
            {
                isHurt = false;
                animator.SetBool("isHurt", false);
            }
        }
    }

    private void UpdateImmunity()
    {
        if (isImmune)
        {
            immunityTimer -= Time.deltaTime;
            
            // Parpadeo durante inmunidad
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = Mathf.FloorToInt(immunityTimer * 10) % 2 == 0;
            }
            
            if (immunityTimer <= 0)
            {
                isImmune = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }
                // Volver al layer original para poder recibir daño
                gameObject.layer = originalLayer;
            }
        }
    }

    public void TakeDamage(Vector2 damageSourcePosition)
    {
        if (isImmune) return;  // No recibir daño si es inmune

        // Activar estado de daño
        isHurt = true;
        hurtTimer = hurtDuration;
        animator.SetBool("isHurt", true);

        // Activar inmunidad
        isImmune = true;
        immunityTimer = immunityDuration;
        
        // Cambiar a layer inmune para atravesar enemigos
        gameObject.layer = immuneLayer;

        // Calcular dirección del knockback (opuesta al enemigo)
        float knockbackDirection = transform.position.x > damageSourcePosition.x ? 1f : -1f;
        
        // Aplicar empuje
        rb.velocity = new Vector2(knockbackForceX * knockbackDirection, knockbackForceY);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isImmune)
        {
            TakeDamage(other.transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isImmune)
        {
            TakeDamage(collision.transform.position);
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
        // No permitir movimiento si está herido
        if (isHurt)
        {
            // Mantener solo el knockback, sin input del jugador
            animator.SetBool("isRunning", false);
            return;
        }

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