using UnityEngine;
using System.Collections;

public class TimeRingEffect : MonoBehaviour
{
    public SpriteRenderer ring;
    public float expandSpeed = 10f;
    public float fadeSpeed = 5f;

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
        // 1. Activamos alpha (fade in)
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            ring.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // 2. Crecemos el círculo
        while (transform.localScale.x < 20f)
        {
            transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
            yield return null;
        }

        // 3. Aquí hacemos el cambio de tiempo
        onMidEffect?.Invoke();

        // 4. Desvanecemos el círculo
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            ring.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // 5. Reseteamos para la próxima vez
        transform.localScale = startScale;
    }
}