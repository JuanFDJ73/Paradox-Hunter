using UnityEngine;

// Gestiona la persistencia del estado del tiempo entre escenas
public static class TimeManager
{
    private const string TIME_STATE_KEY = "IsInFuture";
    
    // Guarda el estado actual del tiempo
    public static void SaveTimeState()
    {
        PlayerPrefs.SetInt(TIME_STATE_KEY, TimeTraveler.isInFuture ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // Carga el estado del tiempo guardado (pasado por default si no hay guardado)
    public static void LoadTimeState()
    {
        if (PlayerPrefs.HasKey(TIME_STATE_KEY))
        {
            TimeTraveler.isInFuture = PlayerPrefs.GetInt(TIME_STATE_KEY) == 1;
        }
        else
        {
            // Por default es PASADO
            TimeTraveler.isInFuture = false;
        }
    }
    
    // Resetea el estado del tiempo al pasado
    public static void ResetTimeState()
    {
        TimeTraveler.isInFuture = false;
        PlayerPrefs.DeleteKey(TIME_STATE_KEY);
    }
}
