using UnityEngine;
using System.Collections;

public class TimeRingEffect : MonoBehaviour
{
    public SpriteRenderer ring;
    
    [Header("Duración")]
    public float fadeInDuration = 0.1f;   // Tiempo para aparecer
    public float spinDuration = 0.6f;     // Tiempo de giro inicial
    public float expandDuration = 0.4f;   // Tiempo de expansión
    public float fadeOutDuration = 0.1f;  // Tiempo para desaparecer

    [Header("Escala")]
    public float spinScale = 2f;          // Tamaño durante el giro
    public float maxScale = 20f;          // Tamaño máximo

    [Header("Giro")]
    public float spinSpeed = 320f;        // Grados por segundo

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
        Color c = ring.color;
        c.a = 0;
        ring.color = c;
    }

    public IEnumerator Play(System.Action onMidEffect)
    {
        // Resetear rotación
        transform.rotation = Quaternion.identity;
        
        // 1. Fade In (aparecer rápido)
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            ring.color = new Color(1, 1, 1, t);
            
            // Crecer hasta spinScale durante el fade in
            float scale = Mathf.Lerp(startScale.x, spinScale, t);
            transform.localScale = new Vector3(scale, scale, 1f);
            
            yield return null;
        }
        ring.color = new Color(1, 1, 1, 1);
        transform.localScale = new Vector3(spinScale, spinScale, 1f);

        // 2. Girar en spinScale durante spinDuration
        elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. Expandir el círculo (con giro continuo)
        elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / expandDuration;
            float scale = Mathf.Lerp(spinScale, maxScale, t);
            transform.localScale = new Vector3(scale, scale, 1f);
            
            // Seguir girando mientras expande
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            
            yield return null;
        }

        // 4. Cambio de tiempo
        onMidEffect?.Invoke();

        // 5. Fade Out (desaparecer rápido)
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            ring.color = new Color(1, 1, 1, 1f - t);
            
            // Seguir girando
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            
            yield return null;
        }

        // 6. Resetear
        transform.localScale = startScale;
        transform.rotation = Quaternion.identity;
        ring.color = new Color(1, 1, 1, 0);
    }
}