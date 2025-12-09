using UnityEngine;

public class Climb : MonoBehaviour
{
    [Header("Configuración de Escalada")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float climbSoundInterval = 0.3f;  // Intervalo entre sonidos

    private bool playerInZone = false;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private PlayerSoundController playerSoundController;
    private float originalGravity;
    private float climbSoundTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerRb = other.GetComponent<Rigidbody2D>();
        playerController = other.GetComponent<PlayerController>();
        playerSoundController = other.GetComponent<PlayerSoundController>();
        
        if (playerRb != null)
        {
            playerInZone = true;
            originalGravity = playerRb.gravityScale;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (playerRb != null)
        {
            playerRb.gravityScale = originalGravity;
        }
        
        playerInZone = false;
        playerRb = null;
        playerController = null;
        playerSoundController = null;
    }

    private void Update()
    {
        if (!playerInZone || playerRb == null) return;

        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0)
        {
            // Escalando: desactivar gravedad y mover verticalmente
            playerRb.gravityScale = 0f;
            playerRb.velocity = new Vector2(playerRb.velocity.x, verticalInput * climbSpeed);
            
            // Sonido de escalada con intervalo
            climbSoundTimer -= Time.deltaTime;
            if (climbSoundTimer <= 0f && playerSoundController != null)
            {
                playerSoundController.PlayClimbSound();
                climbSoundTimer = climbSoundInterval;
            }
        }
        else
        {
            // Quieto en la escalera: mantener sin gravedad para no caer
            playerRb.gravityScale = 0f;
            playerRb.velocity = new Vector2(playerRb.velocity.x, 0f);
            climbSoundTimer = 0f;  // Resetear timer cuando no escala
        }
    }
}
