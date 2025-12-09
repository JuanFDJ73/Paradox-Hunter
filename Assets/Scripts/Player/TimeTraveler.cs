using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TimeTraveler : MonoBehaviour
{
    public static bool isInFuture = false;
    private bool canTravel = true;

    [Header("Efecto Visual")]
    public TimeRingEffect ringEffect;

    // Método llamado por el New Input System (L o Button East)
    public void OnTime(InputValue value)
    {
        if (value.isPressed && canTravel)
        {
            StartCoroutine(Travel());
        }
    }

    IEnumerator Travel()
    {
        canTravel = false;

        if (ringEffect != null)
        {
            // Reproducir animación del círculo
            yield return StartCoroutine(ringEffect.Play(() =>
            {
                // Cambio temporal ocurre cuando el círculo cubre la pantalla
                ToggleTime();
            }));
        }
        else
        {
            // Sin efecto visual, cambiar tiempo inmediatamente
            ToggleTime();
        }

        canTravel = true;
    }

    void ToggleTime()
    {
        isInFuture = !isInFuture;

        // Buscar todos los objetos temporales
        TimeObject[] objs = FindObjectsOfType<TimeObject>();

        foreach (var obj in objs)
        {
            obj.UpdateVisual(isInFuture);
        }
    }
}