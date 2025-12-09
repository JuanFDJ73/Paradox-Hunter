using UnityEngine;

public class TimeObject : MonoBehaviour
{
    public GameObject pastVersion;
    public GameObject futureVersion;

    // Posiciones registradas
    private Vector3 pastPosition;
    private Vector3 futurePosition;

    // Estado de recolección/destrucción
    private bool pastCollected = false;
    private bool futureCollected = false;

    void Start()
    {
        // Inicialmente la posición del futuro es igual a la del pasado
        pastPosition = pastVersion.transform.position;
        futurePosition = pastPosition;

        // Mostrar según el estado ACTUAL del tiempo (no siempre pasado)
        UpdateVisual(TimeTraveler.isInFuture);
    }

    void Update()
    {
        // Si estamos en el pasado, actualizamos la posición guardada
        if (!TimeTraveler.isInFuture && pastVersion.activeInHierarchy)
        {
            pastPosition = pastVersion.transform.position;
            futurePosition = pastPosition;
        }
    }

    public void UpdateVisual(bool showFuture)
    {
        if (showFuture)
        {
            pastVersion.SetActive(false);
            
            // Si fue recolectado en el pasado, tampoco existe en el futuro
            // O si fue recolectado en el futuro
            if (pastCollected || futureCollected)
            {
                futureVersion.SetActive(false);
            }
            else
            {
                futureVersion.SetActive(true);
                futureVersion.transform.position = futurePosition;
            }
        }
        else
        {
            futureVersion.SetActive(false);
            
            // Solo mostrar si no fue recolectado en el pasado
            if (!pastCollected)
            {
                pastVersion.SetActive(true);
            }
            else
            {
                pastVersion.SetActive(false);
            }
        }
    }

    // Llamar cuando el objeto es recolectado/destruido.
    // Si se recolecta en el pasado, también desaparece del futuro.
    // Si se recolecta en el futuro, el del pasado sigue disponible.
    public void OnCollected()
    {
        if (TimeTraveler.isInFuture)
        {
            // Recolectado en el futuro - solo afecta al futuro
            futureCollected = true;
            futureVersion.SetActive(false);
            Debug.Log($"[TimeObject] {gameObject.name} recolectado en el FUTURO");
        }
        else
        {
            // Recolectado en el pasado - afecta pasado Y futuro
            pastCollected = true;
            pastVersion.SetActive(false);
            Debug.Log($"[TimeObject] {gameObject.name} recolectado en el PASADO - también desaparece del futuro");
        }
    }

    // Método alternativo para llamar desde los hijos (GoodVersion/BadVersion)
    public static void NotifyCollected(GameObject collectedObject)
    {
        // Buscar el TimeObject padre
        TimeObject timeObject = collectedObject.GetComponentInParent<TimeObject>();
        if (timeObject != null)
        {
            timeObject.OnCollected();
        }
    }
}