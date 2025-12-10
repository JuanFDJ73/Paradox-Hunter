using UnityEngine;

// Administrador de spawn del jugador.
// Busca el SpawnPoint correcto basándose en la última puerta usada.
public class SpawnManager : MonoBehaviour
{
    // ID del spawn point donde debe aparecer el jugador
    private static string targetSpawnID = "";

    [Header("Referencias")]
    [SerializeField] private GameObject player;

    private void Start()
    {
        // Buscar al jugador si no está asignado
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Si hay un spawn point objetivo, mover al jugador ahí
        if (!string.IsNullOrEmpty(targetSpawnID))
        {
            MovePlayerToSpawn();
        }
        else
        {
            Debug.Log("[SpawnManager] No hay spawn objetivo, jugador en posición por defecto.");
        }
    }

    private void MovePlayerToSpawn()
    {
        if (player == null)
        {
            Debug.LogError("[SpawnManager] No se encontró el jugador!");
            return;
        }

        // Buscar todos los spawn points en la escena
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        
        Debug.Log($"[SpawnManager] Buscando SpawnPoint con ID: '{targetSpawnID}'. Encontrados: {spawnPoints.Length} spawn points.");

        foreach (SpawnPoint sp in spawnPoints)
        {
            Debug.Log($"[SpawnManager] SpawnPoint encontrado: '{sp.SpawnID}'");
            
            if (sp.SpawnID == targetSpawnID)
            {
                player.transform.position = sp.transform.position;
                Debug.Log($"[SpawnManager] ¡Jugador movido a spawn '{targetSpawnID}' en posición {sp.transform.position}!");
                
                // Limpiar el ID después de usarlo
                targetSpawnID = "";
                return;
            }
        }

        Debug.LogWarning($"[SpawnManager] No se encontró SpawnPoint con ID: '{targetSpawnID}'");
        targetSpawnID = "";
    }

    // Llamado por las puertas antes de cambiar de escena
    public static void SetTargetSpawn(string spawnID)
    {
        targetSpawnID = spawnID;
        Debug.Log($"[SpawnManager] Spawn objetivo establecido: '{spawnID}'");
    }
}
