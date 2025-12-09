using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TimeTraveler : MonoBehaviour
{
    public static bool isInFuture = false;
    public static bool canUseTimePower = true;  // Permite bloquear el poder en ciertas zonas

    private bool canTravel = true;

    [Header("Efecto Visual")]
    public TimeRingEffect ringEffect;

    [Header("Sonido")]
    public PlayerSoundController soundController;

    // Resetear variables static cuando se inicia el juego (Editor o Build)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        isInFuture = false;
        canUseTimePower = true;
    }

    private void Start()
    {
        // Actualizar los objetos temporales al estado correcto
        // (El estado ya fue cargado por SceneTimeSettings en Awake)
        UpdateAllTimeObjects();
    }

    // Método llamado por el New Input System (L o Button East)
    public void OnTime(InputValue value)
    {
        if (value.isPressed && canTravel && canUseTimePower)
        {
            StartCoroutine(Travel());
        }
    }

    IEnumerator Travel()
    {
        canTravel = false;

        if (ringEffect != null)
        {
            // Sonido de viaje en el tiempo
            if (soundController != null)
                soundController.PlayTimeTravelSound();

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
        UpdateAllTimeObjects();
    }

    void UpdateAllTimeObjects()
    {
        // Buscar todos los objetos temporales
        TimeObject[] objs = FindObjectsOfType<TimeObject>();

        foreach (var obj in objs)
        {
            obj.UpdateVisual(isInFuture);
        }
    }

    // Métodos estáticos para bloquear/desbloquear el poder
    public static void EnableTimePower()
    {
        canUseTimePower = true;
    }

    public static void DisableTimePower()
    {
        canUseTimePower = false;
    }
}