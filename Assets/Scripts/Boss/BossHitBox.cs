using UnityEngine;

// Hitbox de ataque del Boss - Detecta colisión con el jugador
// Colocar este script en el GameObject hijo que es el hitbox
public class BossHitBox : MonoBehaviour
{
    private BossStats stats;
    
    [Header("Configuración")]
    [SerializeField] private bool useParentStats = true;
    [SerializeField] private int customDamage = 1;

    private void Awake()
    {
        // Obtener stats del padre (Boss)
        stats = GetComponentInParent<BossStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo dañar al jugador
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Obtener valor de daño
        int damage = useParentStats && stats != null ? stats.AttackDamage : customDamage;

        // Aplicar daño al jugador (posición del boss para calcular knockback)
        player.TakeDamage(transform.position, damage);
        
        Debug.Log($"[BossHitBox] Golpeó al jugador! Daño: {damage}");
    }
}
