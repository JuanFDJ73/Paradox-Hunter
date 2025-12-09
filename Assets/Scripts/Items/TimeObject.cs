using UnityEngine;

public class TimeObject : MonoBehaviour
{
    public GameObject pastVersion;
    public GameObject futureVersion;

    // Posiciones registradas
    private Vector3 pastPosition;
    private Vector3 futurePosition;

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
        if (!TimeTraveler.isInFuture)
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
            futureVersion.SetActive(true);
            futureVersion.transform.position = futurePosition;
        }
        else
        {
            pastVersion.SetActive(true);
            futureVersion.SetActive(false);
        }
    }
}