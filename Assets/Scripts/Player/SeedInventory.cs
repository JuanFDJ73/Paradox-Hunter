using UnityEngine;
using UnityEngine.InputSystem;

// Inventario de semillas del jugador.
// Permite recoger, almacenar y plantar semillas.
public class SeedInventory : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxSeeds = 1;  // Máximo de semillas que puede cargar
    [SerializeField] private float plantRange = 1.5f;  // Rango para plantar

    [Header("UI (Opcional)")]
    [SerializeField] private GameObject seedIndicator;  // Indicador visual de que tiene semilla

    // Semillas en inventario
    private Seed currentSeed;
    private int seedCount = 0;
    
    // Maceta cercana
    private Flowerpot nearbyPot;

    public bool HasSeed => seedCount > 0;
    public int SeedCount => seedCount;

    private void Update()
    {
        // Actualizar indicador visual
        if (seedIndicator != null)
        {
            seedIndicator.SetActive(HasSeed);
        }
    }

    // Input para plantar semilla (llamado desde PlayerInput)
    public void OnPlant(InputValue value)
    {
        if (value.isPressed)
        {
            TryPlantSeed();
        }
    }

    // Método alternativo para plantar (llamar desde código)
    public void TryPlantSeed()
    {
        if (!HasSeed)
        {
            Debug.Log("[SeedInventory] No tienes semillas!");
            return;
        }

        // Buscar maceta cercana
        Flowerpot pot = FindNearbyFlowerpot();
        
        if (pot == null)
        {
            Debug.Log("[SeedInventory] No hay maceta cerca!");
            return;
        }

        if (!pot.IsEmpty)
        {
            Debug.Log("[SeedInventory] La maceta ya tiene una planta!");
            return;
        }

        // Plantar la semilla
        PlantSeed(pot);
    }

    // Buscar maceta cercana
    private Flowerpot FindNearbyFlowerpot()
    {
        // Primero verificar si hay una registrada
        if (nearbyPot != null && Vector2.Distance(transform.position, nearbyPot.transform.position) <= plantRange)
        {
            return nearbyPot;
        }

        // Buscar macetas en rango
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, plantRange);
        
        foreach (Collider2D col in colliders)
        {
            Flowerpot pot = col.GetComponent<Flowerpot>();
            if (pot != null && pot.IsEmpty)
            {
                return pot;
            }
        }

        return null;
    }

    // Recoger una semilla
    public void CollectSeed(Seed seed)
    {
        if (seedCount >= maxSeeds)
        {
            Debug.Log("[SeedInventory] Inventario lleno!");
            MessageUI.Show("¡Inventario de semillas lleno!");
            return;
        }

        currentSeed = seed;
        seedCount++;
        
        // Sonido de recoger
        PlayerSoundController soundController = GetComponent<PlayerSoundController>();
        if (soundController != null)
        {
            soundController.PlayCollectItemSound();
        }
        
        // Mostrar mensaje en pantalla
        MessageUI.Show($"¡Recogiste una semilla! ({seedCount}/{maxSeeds})");
        
        Debug.Log($"[SeedInventory] Semilla recogida! Total: {seedCount}");
    }

    // Plantar semilla en maceta
    private void PlantSeed(Flowerpot pot)
    {
        if (currentSeed == null || !HasSeed) return;

        // Plantar
        pot.PlantSeed(currentSeed);
        currentSeed.PlantInPot(pot);
        
        // Limpiar inventario
        currentSeed = null;
        seedCount--;
        
        // Mostrar mensaje
        MessageUI.Show("¡Semilla plantada!");
        
        Debug.Log($"[SeedInventory] Semilla plantada! Restantes: {seedCount}");
    }

    public bool CanCollectSeed()
    {
        return seedCount < maxSeeds;
    }

    // Detectar macetas cercanas
    private void OnTriggerEnter2D(Collider2D other)
    {
        Flowerpot pot = other.GetComponent<Flowerpot>();
        if (pot != null)
        {
            nearbyPot = pot;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Flowerpot pot = other.GetComponent<Flowerpot>();
        if (pot != null && pot == nearbyPot)
        {
            nearbyPot = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Mostrar rango de plantado
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, plantRange);
    }
}
