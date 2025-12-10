using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public enum DoorTime
    {
        Past,
        Future
    }

    [Header("Configuración de Escena")]
    [SerializeField] private string sceneName;  // Nombre de la escena (asignado por el Editor)
    
    [Header("Spawn Point")]
    [Tooltip("ID del SpawnPoint donde aparecerá el jugador en la escena destino")]
    [SerializeField] private string spawnPointID;
    
    [Header("Configuración de Tiempo")]
    [SerializeField] private DoorTime doorTime = DoorTime.Past;  // En qué tiempo funciona esta puerta
    
    #if UNITY_EDITOR
    // Este campo solo existe en el Editor para drag & drop
    [HideInInspector] public UnityEditor.SceneAsset sceneAsset;
    #endif
    
    [Header("Configuración de Input")]
    [SerializeField] private float inputThreshold = 0.5f;  // Umbral para detectar "arriba"

    private bool playerInDoor = false;

    private void Update()
    {
        if (!playerInDoor) return;
        
        // Solo funciona si el tiempo actual coincide con el tiempo de la puerta
        bool correctTime = (doorTime == DoorTime.Past && !TimeTraveler.isInFuture) ||
                          (doorTime == DoorTime.Future && TimeTraveler.isInFuture);
        
        if (!correctTime) return;

        // Detectar input hacia arriba (W, flecha arriba, o joystick)
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput > inputThreshold)
        {
            EnterDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInDoor = false;
        }
    }

    private void EnterDoor()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Sonido de puerta
            PlayerSoundController soundController = FindObjectOfType<PlayerSoundController>();
            if (soundController != null)
            {
                Debug.Log("[Door] Playing door open sound");
                soundController.PlayDoorOpenSound();
            }
            
            // Establecer el spawn point de destino
            if (!string.IsNullOrEmpty(spawnPointID))
            {
                SpawnManager.SetTargetSpawn(spawnPointID);
            }
            
            // Guardar el estado del tiempo antes de cambiar de escena
            TimeManager.SaveTimeState();
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Door: No se ha asignado una escena!");
        }
    }
    
    // Propiedad para que el Editor pueda asignar el nombre
    public string SceneName
    {
        get => sceneName;
        set => sceneName = value;
    }
}
