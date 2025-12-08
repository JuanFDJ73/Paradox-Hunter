using UnityEngine;
using UnityEngine.InputSystem;

public class TimeTraveler : MonoBehaviour
{
    public static bool isInFuture = false;

    // Método llamado por el New Input System (L o Button East)
    public void OnTime(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleTime();
        }
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