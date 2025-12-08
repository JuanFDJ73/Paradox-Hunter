using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region === VARIABLES PÚBLICAS (Inspector) ===
    
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb2D;

    [Header("Estadísticas")]
    [SerializeField] private int damage = 2;

    [Header("Movimiento")]
    [SerializeField] private float speed = 3f;
    
    [Header("Patrulla")]
    [SerializeField] private float walkTime = 2f;
    [SerializeField] private float pauseTime = 0.5f;
    
    #endregion

    #region === VARIABLES PRIVADAS ===
    
    private float walkTimer;
    private float pauseTimer;
    private int direction = 1;  // 1 = derecha, -1 = izquierda
    private bool isPaused;
    
    #endregion

    #region === PROPIEDADES PÚBLICAS ===
    
    public int Damage => damage;
    
    #endregion

    #region === INICIALIZACIÓN ===
    
    private void Start()
    {
        walkTimer = walkTime;
    }
    
    #endregion

    #region === UPDATE LOOPS ===
    
    private void Update()
    {
        if (isPaused)
        {
            UpdatePauseState();
        }
        else
        {
            UpdateWalkState();
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }
    
    #endregion

    #region === ESTADOS DE PATRULLA ===
    
    private void UpdateWalkState()
    {
        walkTimer -= Time.deltaTime;
        
        if (walkTimer <= 0)
        {
            StartPause();
        }
    }

    private void UpdatePauseState()
    {
        pauseTimer -= Time.deltaTime;
        
        if (pauseTimer <= 0)
        {
            EndPauseAndTurn();
        }
    }

    private void StartPause()
    {
        isPaused = true;
        pauseTimer = pauseTime;
    }

    private void EndPauseAndTurn()
    {
        // Cambiar dirección
        direction *= -1;
        FlipSprite();
        
        // Reiniciar caminata
        isPaused = false;
        walkTimer = walkTime;
    }
    
    #endregion

    #region === MOVIMIENTO ===
    
    private void ApplyMovement()
    {
        float velocityX = isPaused ? 0 : speed * direction;
        rb2D.velocity = new Vector2(velocityX, rb2D.velocity.y);
    }

    private void FlipSprite()
    {
        transform.localScale = new Vector3(direction, 1, 1);
    }
    
    #endregion
}
