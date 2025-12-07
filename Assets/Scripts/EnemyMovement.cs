using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb2D;

    [Header("Movimiento Horizontal")]
    [SerializeField] private float speed = 3f;
    
    [Header("Patrulla")]
    [SerializeField] private float changeDirectionTime = 2f;  // Tiempo entre cambios de dirección
    [SerializeField] private float pauseTime = 0.5f;          // Tiempo de pausa al cambiar dirección
    
    private float directionTimer;
    private float pauseTimer;
    private int direction = 1;  // 1 = derecha, -1 = izquierda
    private bool isPaused = false;

    private void Start()
    {
        directionTimer = changeDirectionTime;
    }

    private void Update()
    {
        if (isPaused)
        {
            // Esperando durante la pausa
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0)
            {
                // Al terminar la pausa, ahora sí voltea y camina
                direction *= -1;
                transform.localScale = new Vector3(direction, 1, 1);
                isPaused = false;
                directionTimer = changeDirectionTime;
            }
        }
        else
        {
            // Contando para cambiar de dirección
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0)
            {
                StartPause();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            rb2D.velocity = new Vector2(speed * direction, rb2D.velocity.y);
        }
        else
        {
            rb2D.velocity = new Vector2(0, rb2D.velocity.y);
        }
    }

    private void StartPause()
    {
        // Solo inicia la pausa, NO cambia dirección todavía
        isPaused = true;
        pauseTimer = pauseTime;
    }
}
