using UnityEngine;
using UnityEngine.InputSystem;

public class TimeTraveler : MonoBehaviour
{
    public bool isInFuture = false;

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
            if (isInFuture)
                obj.ShowFuture();
            else
                obj.ShowPast();
        }
    }
}