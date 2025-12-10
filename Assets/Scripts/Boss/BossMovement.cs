using UnityEngine;

// Maneja el movimiento del Boss
public class BossMovement : MonoBehaviour
{
    private BossStats stats;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    private bool facingRight = true;
    private bool canMove = true;

    private void Awake()
    {
        stats = GetComponent<BossStats>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Si el sprite está en un hijo
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Mover hacia una posición objetivo
    public void MoveTowards(Vector3 targetPosition)
    {
        if (!canMove) return;
        
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        
        // Aplicar velocidad
        rb.velocity = new Vector2(direction * stats.MoveSpeed, rb.velocity.y);
        
        // Voltear sprite según dirección
        FlipTowards(direction > 0);
    }

    // Detener movimiento
    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    // Voltear el sprite hacia una dirección
    public void FlipTowards(bool right)
    {
        if (facingRight != right)
        {
            facingRight = right;
            
            // Método 1: Flip del SpriteRenderer
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !right;
            }
            
        }
    }

    // Voltear hacia el jugador
    public void FacePlayer(Transform player)
    {
        if (player == null) return;
        FlipTowards(player.position.x > transform.position.x);
    }

    // Habilitar/deshabilitar movimiento
    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove) Stop();
    }

    // Aplicar knockback (cuando el boss es golpeado)
    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public bool IsFacingRight => facingRight;
}
