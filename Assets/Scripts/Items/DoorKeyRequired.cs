using UnityEngine;
using UnityEngine.SceneManagement;

// Puerta que requiere una llave específica y una línea de tiempo para abrirse.
public class DoorKeyRequired : MonoBehaviour
{
    public enum TimeRequirement
    {
        Any,        // Cualquier tiempo
        PastOnly,   // Solo en el pasado
        FutureOnly  // Solo en el futuro
    }

    [Header("Configuración de Llave")]
    [Tooltip("ID de la llave requerida (debe coincidir con Key.keyID)")]
    [SerializeField] private string requiredKeyID = "key_default";
    
    [Header("Configuración de Tiempo")]
    [SerializeField] private TimeRequirement timeRequirement = TimeRequirement.Any;

    [Header("Destino")]
    [SerializeField] private string sceneName;
    
    #if UNITY_EDITOR
    // Este campo solo existe en el Editor para drag & drop
    [HideInInspector] public UnityEditor.SceneAsset sceneAsset;
    #endif

    [Header("Estado")]
    [SerializeField] private bool isUnlocked = false;

    [Header("Visual (Opcional)")]
    [SerializeField] private GameObject lockedVisual;    // Visual de puerta cerrada
    [SerializeField] private GameObject unlockedVisual;  // Visual de puerta abierta

    private bool playerInRange = false;

    public bool IsUnlocked => isUnlocked;

    private void Start()
    {
        UpdateVisual();
    }

    private void Update()
    {
        // Entrar por la puerta con W o flecha arriba
        if (playerInRange && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            TryEnterDoor();
        }
    }

    // Intentar desbloquear la puerta con una llave
    public bool TryUnlock(string keyID)
    {
        // Verificar si la llave es correcta
        if (keyID != requiredKeyID)
        {
            return false;
        }

        // Verificar el tiempo
        if (!IsCorrectTime())
        {
            string timeMsg = timeRequirement == TimeRequirement.PastOnly ? "el pasado" : "el futuro";
            MessageUI.Show($"Esta puerta solo puede abrirse en {timeMsg}");
            return false;
        }

        // Desbloquear
        isUnlocked = true;
        UpdateVisual();
        
        // Sonido de desbloqueo
        PlayerSoundController soundController = FindObjectOfType<PlayerSoundController>();
        if (soundController != null)
        {
            soundController.PlayDoorOpenSound();
        }

        return true;
    }

    // Intentar entrar por la puerta
    private void TryEnterDoor()
    {
        // Verificar si está desbloqueada
        if (!isUnlocked)
        {
            MessageUI.Show("La puerta está cerrada. Necesitas una llave.");
            return;
        }

        // Verificar el tiempo
        if (!IsCorrectTime())
        {
            string timeMsg = timeRequirement == TimeRequirement.PastOnly ? "el pasado" : "el futuro";
            MessageUI.Show($"Esta puerta solo funciona en {timeMsg}");
            return;
        }

        // Entrar
        EnterDoor();
    }

    private void EnterDoor()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Sonido de puerta
            PlayerSoundController soundController = FindObjectOfType<PlayerSoundController>();
            if (soundController != null)
            {
                soundController.PlayDoorOpenSound();
            }

            // Guardar estado del tiempo
            TimeManager.SaveTimeState();
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("[DoorKeyRequired] No se ha asignado una escena!");
        }
    }

    // Verificar si estamos en el tiempo correcto
    private bool IsCorrectTime()
    {
        switch (timeRequirement)
        {
            case TimeRequirement.PastOnly:
                return !TimeTraveler.isInFuture;
            case TimeRequirement.FutureOnly:
                return TimeTraveler.isInFuture;
            case TimeRequirement.Any:
            default:
                return true;
        }
    }

    private void UpdateVisual()
    {
        if (lockedVisual != null)
            lockedVisual.SetActive(!isUnlocked);
        
        if (unlockedVisual != null)
            unlockedVisual.SetActive(isUnlocked);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerInRange = true;

        // Mostrar estado de la puerta
        if (!isUnlocked)
        {
            KeyInventory keyInv = other.GetComponent<KeyInventory>();
            if (keyInv != null && keyInv.HasAnyKey)
            {
                MessageUI.Show("Presiona E para usar la llave");
            }
            else
            {
                MessageUI.Show("Puerta cerrada. Necesitas una llave.");
            }
        }
        else
        {
            if (IsCorrectTime())
            {
                MessageUI.Show("Presiona W para entrar");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Color según estado
        Gizmos.color = isUnlocked ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 2, 0));
    }
    
    // Propiedad para que el Editor pueda asignar el nombre
    public string SceneName
    {
        get => sceneName;
        set => sceneName = value;
    }
}

