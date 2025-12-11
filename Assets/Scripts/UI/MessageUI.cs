using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// Muestra mensajes temporales en pantalla.
/// Colocar en un Canvas con un Panel hijo.
public class MessageUI : MonoBehaviour
{
    public static MessageUI Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject messagePanel;

    [Header("Configuración")]
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeTime = 0.5f;

    private Coroutine currentMessage;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ocultar al inicio
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    /// <summary>
    /// Mostrar un mensaje en pantalla
    /// </summary>
    public static void Show(string message)
    {
        if (Instance != null)
        {
            Instance.ShowMessage(message);
        }
        else
        {
            Debug.Log($"[Message] {message}");
        }
    }

    public void ShowMessage(string message)
    {
        if (currentMessage != null)
            StopCoroutine(currentMessage);

        currentMessage = StartCoroutine(DisplayMessage(message));
    }

    private IEnumerator DisplayMessage(string message)
    {
        // Mostrar
        if (messageText != null)
            messageText.text = message;
        
        if (messagePanel != null)
            messagePanel.SetActive(true);

        // Esperar
        yield return new WaitForSeconds(displayTime);

        // Ocultar
        if (messagePanel != null)
            messagePanel.SetActive(false);

        currentMessage = null;
    }
}
