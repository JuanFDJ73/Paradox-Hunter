using UnityEngine;

/// Configura el estado inicial del tiempo en una escena.
[DefaultExecutionOrder(-100)]  // Ejecutar antes que otros scripts
public class SceneTimeSettings : MonoBehaviour
{
    public enum DefaultTime
    {
        KeepCurrent,  // Mantiene el tiempo guardado (default)
        ForcePast,    // Fuerza pasado (ignora estado guardado)
        ForceFuture   // Fuerza futuro (ignora estado guardado)
    }

    [Header("Configuración de Tiempo")]
    [Tooltip("KeepCurrent: Usa el tiempo guardado (pasado por default).\nForcePast/Future: Ignora el estado guardado.")]
    [SerializeField] private DefaultTime defaultTime = DefaultTime.KeepCurrent;
    
    [Header("Bloqueo de Poder")]
    [SerializeField] private bool blockTimePower = false;  // Bloquear poder en toda la escena

    [Header("Debug")]
    [SerializeField] private bool showCurrentTime = true;

    // Variable estática para saber si es la primera vez que se carga el juego
    private static bool isFirstLoad = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetFirstLoad()
    {
        isFirstLoad = true;
    }

    private void Awake()
    {
        // Si es la primera vez que se carga el juego, resetear a pasado
        if (isFirstLoad)
        {
            isFirstLoad = false;
            TimeManager.ResetTimeState();
            
            if (showCurrentTime)
                Debug.Log("[SceneTimeSettings] Primera carga - Reseteando a PASADO");
        }
        
        // Configurar el tiempo según la opción seleccionada
        switch (defaultTime)
        {
            case DefaultTime.KeepCurrent:
                // Cargar estado guardado
                TimeManager.LoadTimeState();
                break;
            case DefaultTime.ForcePast:
                TimeTraveler.isInFuture = false;
                break;
            case DefaultTime.ForceFuture:
                TimeTraveler.isInFuture = true;
                break;
        }

        // Configurar bloqueo de poder
        if (blockTimePower)
        {
            TimeTraveler.DisableTimePower();
        }
        else
        {
            TimeTraveler.EnableTimePower();
        }
        
        if (showCurrentTime)
        {
            Debug.Log($"[SceneTimeSettings] Awake - Tiempo: {(TimeTraveler.isInFuture ? "FUTURO" : "PASADO")}");
        }
    }

    private void Start()
    {
        if (showCurrentTime)
        {
            Debug.Log($"[SceneTimeSettings] Start - Tiempo: {(TimeTraveler.isInFuture ? "FUTURO" : "PASADO")}");
            Debug.Log($"[SceneTimeSettings] Poder de tiempo: {(TimeTraveler.canUseTimePower ? "HABILITADO" : "BLOQUEADO")}");
        }
    }
}