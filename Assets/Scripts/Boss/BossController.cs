using UnityEngine;

public enum BossState { Idle, Run, Attack, Hit, Death }

/// Controlador principal del Boss - Máquina de estados
public class BossController : MonoBehaviour
{
    [Header("Estado")]
    public BossState currentState = BossState.Idle;
    
    [Header("Detección")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 2f;
    
    [Header("Tiempos")]
    [SerializeField] private float idleTime = 1.5f;
    [SerializeField] private float hitStunTime = 0.3f;
    
    [Header("Referencias")]
    [SerializeField] private GameObject attackHitBox;
    
    // Componentes
    private BossStats stats;
    private BossMovement movement;
    private BossAnimationHandler animHandler;
    private Transform player;
    
    // Timers
    private float stateTimer;
    private bool isDead = false;

    private void Awake()
    {
        stats = GetComponent<BossStats>();
        movement = GetComponent<BossMovement>();
        animHandler = GetComponent<BossAnimationHandler>();
    }

    private void Start()
    {
        // Buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // Desactivar hitbox al inicio
        if (attackHitBox != null)
            attackHitBox.SetActive(false);
        
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        if (isDead) return;
        
        switch (currentState)
        {
            case BossState.Idle:
                IdleBehaviour();
                break;
            case BossState.Run:
                RunBehaviour();
                break;
            case BossState.Attack:
                AttackBehaviour();
                break;
            case BossState.Hit:
                HitBehaviour();
                break;
            case BossState.Death:
                DeathBehaviour();
                break;
        }
    }

    #region State Behaviours

    private void IdleBehaviour()
    {
        movement.Stop();
        
        // Si el jugador está en rango de detección, perseguir
        if (player != null && GetDistanceToPlayer() <= detectionRange)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                ChangeState(BossState.Run);
            }
        }
    }

    private void RunBehaviour()
    {
        if (player == null) return;
        
        float distance = GetDistanceToPlayer();
        
        // Si está en rango de ataque, atacar
        if (distance <= attackRange)
        {
            ChangeState(BossState.Attack);
            return;
        }
        
        // Si está fuera de rango de detección, volver a Idle
        if (distance > detectionRange)
        {
            ChangeState(BossState.Idle);
            return;
        }
        
        // Moverse hacia el jugador
        movement.MoveTowards(player.position);
    }

    private void AttackBehaviour()
    {
        movement.Stop();
        // La animación controla el ataque mediante eventos
        // El método EndAttack() es llamado por evento de animación
    }

    private void HitBehaviour()
    {
        movement.Stop();
        
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            // Después del hit, decidir siguiente acción
            if (GetDistanceToPlayer() <= attackRange)
                ChangeState(BossState.Attack);
            else
                ChangeState(BossState.Run);
        }
    }

    private void DeathBehaviour()
    {
        movement.Stop();
        // La animación de muerte maneja la destrucción
    }

    #endregion

    #region State Management

    public void ChangeState(BossState newState)
    {
        if (isDead && newState != BossState.Death) return;
        
        currentState = newState;
        
        switch (newState)
        {
            case BossState.Idle:
                stateTimer = idleTime;
                animHandler?.PlayIdle();
                Debug.Log("Cambiando estado a: " + newState);
                break;
            case BossState.Run:
                animHandler?.PlayRun();
                Debug.Log("Cambiando estado a: " + newState);
                break;
            case BossState.Attack:
                animHandler?.PlayAttack();
                Debug.Log("Cambiando estado a: " + newState);
                break;
            case BossState.Hit:
                stateTimer = hitStunTime;
                animHandler?.PlayHit();
                Debug.Log("Cambiando estado a: " + newState);
                break;
            case BossState.Death:
                isDead = true;
                animHandler?.PlayDeath();
                Debug.Log("Cambiando estado a: " + newState);
                break;
        }
    }

    #endregion

    #region Public Methods (Llamados por otros scripts)

    /// Recibir daño - llamado por el jugador u otras fuentes
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        stats.TakeDamage(damage);
        
        if (stats.CurrentHealth <= 0)
        {
            ChangeState(BossState.Death);
        }
        else
        {
            ChangeState(BossState.Hit);
        }
    }

    #endregion

    #region Animation Events (Llamados desde Animator)

    public void EnableAttackHitBox()
    {
        if (attackHitBox != null)
            attackHitBox.SetActive(true);
    }

    public void DisableAttackHitBox()
    {
        if (attackHitBox != null)
            attackHitBox.SetActive(false);
    }

    public void EndAttack()
    {
        DisableAttackHitBox();
        
        // Decidir siguiente estado
        if (player != null && GetDistanceToPlayer() <= attackRange)
            ChangeState(BossState.Attack);  // Atacar de nuevo
        else if (player != null && GetDistanceToPlayer() <= detectionRange)
            ChangeState(BossState.Run);
        else
            ChangeState(BossState.Idle);
    }

    public void EndDeath()
    {
        // Puedes agregar efectos, drops, etc.
        Destroy(gameObject);
    }

    #endregion

    #region Helpers

    private float GetDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.position);
    }

    private void OnDrawGizmosSelected()
    {
        // Rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rango de ataque (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
}