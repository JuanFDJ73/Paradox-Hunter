using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region ==================== VARIABLES DE CONFIGURACIÓN ====================
    
    [Header("=== MOVIMIENTO ===")]
    [SerializeField] private float speed = 5f;

    [Header("=== SALTO ===")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float airJumpForce = 7f;
    [SerializeField] private float coyoteTime = 0.25f;

    [Header("=== DETECCIÓN DE SUELO ===")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("=== VIDA ===")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent<int> onHealthChanged;

    [Header("=== DAÑO E INMUNIDAD ===")]
    [SerializeField] private float knockbackForceX = 5f;
    [SerializeField] private float knockbackForceY = 3f;
    [SerializeField] private float hurtDuration = 0.5f;
    [SerializeField] private float immunityDuration = 1.5f;

    [Header("=== DEBUG ===")]
    [SerializeField] private bool debugMode = false;
    
    #endregion

    #region ==================== VARIABLES PRIVADAS ====================
    
    // Componentes
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Input
    private Vector2 moveInput;

    // Salto
    private bool isGrounded;
    private bool wasGrounded;
    private float coyoteTimeCounter;
    private int jumpsRemaining;
    private const int MAX_JUMPS = 2;
    private bool hasUsedGroundJump;
    private bool justJumped;

    // Vida
    private int currentHealth;

    // Daño e Inmunidad
    private bool isHurt;
    private bool isImmune;
    private float hurtTimer;
    private float immunityTimer;
    private int originalLayer;
    private int immuneLayer;
    
    #endregion

    #region ==================== PROPIEDADES PÚBLICAS ====================
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsHurt => isHurt;
    public bool IsImmune => isImmune;
    public bool IsGrounded => isGrounded;
    
    #endregion

    #region ==================== INICIALIZACIÓN ====================
    
    private void Awake()
    {
        InitializeComponents();
        InitializeLayers();
        InitializeHealth();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void InitializeLayers()
    {
        originalLayer = gameObject.layer;
        immuneLayer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
    }
    
    #endregion

    #region ==================== INPUT SYSTEM ====================
    
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (isHurt) return;
        
        if (value.isPressed)
        {
            TryJump();
        }
    }
    
    #endregion

    #region ==================== GAME LOOPS ====================
    
    private void Update()
    {
        UpdateGroundCheck();
        UpdateCoyoteTime();
        UpdateHurtState();
        UpdateImmunityState();
        
        ShowDebugInfo();
    }

    private void FixedUpdate()
    {
        if (isHurt)
        {
            animator.SetBool("isRunning", false);
            return;
        }

        ApplyHorizontalMovement();
    }
    
    #endregion

    #region ==================== MOVIMIENTO ====================
    
    private void ApplyHorizontalMovement()
    {
        float moveX = moveInput.x;
        
        // Aplicar velocidad
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        // Animación de correr
        animator.SetBool("isRunning", moveX != 0);

        // Voltear sprite
        if (moveX != 0)
        {
            FlipSprite(moveX > 0);
        }
    }

    private void FlipSprite(bool facingRight)
    {
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }
    
    #endregion

    #region ==================== SALTO ====================
    
    private void UpdateGroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !justJumped)
        {
            ResetJumps();
        }
        
        if (!isGrounded)
        {
            justJumped = false;
        }
        else if (wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
    }

    private void ResetJumps()
    {
        jumpsRemaining = MAX_JUMPS;
        hasUsedGroundJump = false;
        coyoteTimeCounter = coyoteTime;
    }

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
            PerformGroundJump();
        }
        else if (jumpsRemaining > 0)
        {
            PerformAirJump();
        }
    }

    private void PerformGroundJump()
    {
        hasUsedGroundJump = true;
        justJumped = true;
        jumpsRemaining = 1;
        coyoteTimeCounter = 0f;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void PerformAirJump()
    {
        jumpsRemaining--;
        rb.velocity = new Vector2(rb.velocity.x, airJumpForce);
    }
    
    #endregion

    #region ==================== VIDA ====================
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth);
    }

    public void SetMaxHealth(int newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        onHealthChanged?.Invoke(currentHealth);
    }

    private void LoseHealth(int damage)
    {
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("¡El jugador ha muerto!");
        onDeath?.Invoke();
        // Agregar: reiniciar nivel, mostrar pantalla de game over, etc.
    }
    
    #endregion

    #region ==================== DAÑO E INMUNIDAD ====================
    
    public void TakeDamage(Vector2 damageSourcePosition, int damage = 1)
    {
        if (isImmune) return;

        LoseHealth(damage);
        
        if (currentHealth > 0)
        {
            ActivateHurtState();
            ActivateImmunity();
            ApplyKnockback(damageSourcePosition);
        }
    }

    private void ActivateHurtState()
    {
        isHurt = true;
        hurtTimer = hurtDuration;
        animator.SetBool("isHurt", true);
    }

    private void ActivateImmunity()
    {
        isImmune = true;
        immunityTimer = immunityDuration;
        gameObject.layer = immuneLayer;
    }

    private void ApplyKnockback(Vector2 damageSourcePosition)
    {
        float direction = transform.position.x > damageSourcePosition.x ? 1f : -1f;
        rb.velocity = new Vector2(knockbackForceX * direction, knockbackForceY);
    }

    private void UpdateHurtState()
    {
        if (!isHurt) return;

        hurtTimer -= Time.deltaTime;
        
        if (hurtTimer <= 0)
        {
            EndHurtState();
        }
    }

    private void EndHurtState()
    {
        isHurt = false;
        animator.SetBool("isHurt", false);
    }

    private void UpdateImmunityState()
    {
        if (!isImmune) return;

        immunityTimer -= Time.deltaTime;
        
        // Efecto de parpadeo
        UpdateBlinkEffect();
        
        if (immunityTimer <= 0)
        {
            EndImmunity();
        }
    }

    private void UpdateBlinkEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = Mathf.FloorToInt(immunityTimer * 10) % 2 == 0;
        }
    }

    private void EndImmunity()
    {
        isImmune = false;
        gameObject.layer = originalLayer;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }
    
    #endregion

    #region ==================== COLISIONES ====================
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyContact(other.gameObject, other.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnemyContact(collision.gameObject, collision.transform.position);
    }

    private void HandleEnemyContact(GameObject obj, Vector2 position)
    {
        if (!obj.CompareTag("Enemy") || isImmune) return;

        int damageAmount = 1; // Valor por defecto
        
        // Obtener el daño del enemigo
        EnemyController enemy = obj.GetComponent<EnemyController>();
        if (enemy != null)
        {
            damageAmount = enemy.Damage;
        }

        TakeDamage(position, damageAmount);
    }
    
    #endregion

    #region ==================== DEBUG ====================
    
    private void ShowDebugInfo()
    {
        if (!debugMode) return;
        
        Debug.Log($"HP: {currentHealth}/{maxHealth} | Grounded: {isGrounded} | Jumps: {jumpsRemaining} | Hurt: {isHurt} | Immune: {isImmune}");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        // Línea del raycast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);
        
        // Punto de detección
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * groundCheckRadius, 0.05f);
    }
    
    #endregion
}