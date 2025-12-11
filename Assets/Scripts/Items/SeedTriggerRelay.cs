using UnityEngine;

/// Colocar en PastVersion (hijo de Flower(TO)) para detectar triggers
/// y notificar al Seed padre.
public class SeedTriggerRelay : MonoBehaviour
{
    private Seed parentSeed;

    private void Awake()
    {
        parentSeed = GetComponentInParent<Seed>();
        if (parentSeed == null)
        {
            Debug.LogError("[SeedTriggerRelay] No se encontró Seed en el padre!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentSeed != null)
        {
            parentSeed.OnChildTriggerEnter(other);
        }
    }
}
