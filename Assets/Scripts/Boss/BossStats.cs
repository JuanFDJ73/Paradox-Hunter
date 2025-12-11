using UnityEngine;

// Estadísticas del Boss: vida, daño, velocidad
public class BossStats : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Daño")]
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;

    // Propiedades públicas (solo lectura)
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackDamage => attackDamage;
    public float KnockbackForce => knockbackForce;
    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"[BossStats] Daño recibido: {damage}. Vida actual: {currentHealth}/{maxHealth}");
    }

    // Curar vida
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    // Obtener porcentaje de vida (para UI)
    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    // Resetear stats (para reiniciar pelea)
    public void ResetStats()
    {
        currentHealth = maxHealth;
    }
}
