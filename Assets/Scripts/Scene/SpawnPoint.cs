using UnityEngine;

// Punto de aparición del jugador.
// Cada puerta debe tener un SpawnPoint asociado con el mismo ID.
public class SpawnPoint : MonoBehaviour
{
    [Header("Identificador")]
    [Tooltip("ID único que debe coincidir con el spawnPointID de la puerta de origen")]
    [SerializeField] private string spawnID;

    public string SpawnID => spawnID;

    private void OnDrawGizmos()
    {
        // Dibujar un icono para visualizar el spawn point
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
    }
}
