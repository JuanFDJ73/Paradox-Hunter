using UnityEngine;
using UnityEngine.SceneManagement;

// Controlador del menú principal.
// Maneja los botones y paneles del menú.
public class MainMenuController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelComoJugar;
    [SerializeField] private GameObject panelCreditos;

    [Header("Escena de Inicio")]
    [SerializeField] private string sceneToLoad = "Start";

    private void Start()
    {
        // Asegurar que los paneles estén cerrados al inicio
        if (panelComoJugar != null)
            panelComoJugar.SetActive(false);
        
        if (panelCreditos != null)
            panelCreditos.SetActive(false);
    }

    // === BOTONES PRINCIPALES ===

    public void Comenzar()
    {
        Debug.Log("[MainMenu] Comenzar juego");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void AbrirComoJugar()
    {
        Debug.Log("[MainMenu] Abrir Como Jugar");
        if (panelComoJugar != null)
            panelComoJugar.SetActive(true);
    }

    public void AbrirCreditos()
    {
        Debug.Log("[MainMenu] Abrir Créditos");
        if (panelCreditos != null)
            panelCreditos.SetActive(true);
    }

    public void Salir()
    {
        Debug.Log("[MainMenu] Salir del juego");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // === BOTONES DE CERRAR ===

    public void CerrarComoJugar()
    {
        Debug.Log("[MainMenu] Cerrar Como Jugar");
        if (panelComoJugar != null)
            panelComoJugar.SetActive(false);
    }

    public void CerrarCreditos()
    {
        Debug.Log("[MainMenu] Cerrar Créditos");
        if (panelCreditos != null)
            panelCreditos.SetActive(false);
    }

    // Método genérico para cerrar cualquier panel
    public void CerrarPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
