using UnityEngine;

// Zona que bloquea el poder de viaje en el tiempo
public class NoTimePowerZone : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool showDebugMessage = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TimeTraveler.DisableTimePower();
            
            if (showDebugMessage)
                Debug.Log("Poder de tiempo BLOQUEADO en esta zona");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TimeTraveler.EnableTimePower();
            
            if (showDebugMessage)
                Debug.Log("Poder de tiempo RESTAURADO");
        }
    }

    // Visualizar la zona en el Editor
    private void OnDrawGizmos()
    {
        if (!showDebugMessage) return;
        
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
        }
        else
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}
