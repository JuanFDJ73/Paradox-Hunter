using UnityEngine;

// Maceta donde se pueden plantar semillas.
// La semilla crece en el futuro y se puede trepar.
public class Flowerpot : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform plantPoint;  // Punto donde aparece la planta
    [SerializeField] private Vector3 plantOffset = new Vector3(0, 0.5f, 0);  // Offset si no hay plantPoint

    [Header("Estado")]
    [SerializeField] private bool isEmpty = true;
    
    private Seed plantedSeed;

    public bool IsEmpty => isEmpty;
    public Seed PlantedSeed => plantedSeed;

    // Obtener la posición donde se planta la semilla
    public Vector3 GetPlantPosition()
    {
        if (plantPoint != null)
        {
            return plantPoint.position;
        }
        return transform.position + plantOffset;
    }

    // Plantar una semilla en esta maceta
    public void PlantSeed(Seed seed)
    {
        if (!isEmpty)
        {
            Debug.LogWarning("[Flowerpot] La maceta ya tiene una planta!");
            return;
        }

        plantedSeed = seed;
        isEmpty = false;
        
        Debug.Log("[Flowerpot] Semilla plantada!");
    }

    // Remover la semilla de la maceta (cuando se recoge en el pasado)
    public void RemoveSeed()
    {
        plantedSeed = null;
        isEmpty = true;
        
        Debug.Log("[Flowerpot] Semilla removida de la maceta");
    }

    private void OnDrawGizmos()
    {
        // Mostrar punto de plantado
        Gizmos.color = isEmpty ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(GetPlantPosition(), 0.2f);
    }
}
