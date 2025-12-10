using UnityEngine;

// Llave que el jugador puede recoger.
// Solo se puede llevar una llave a la vez.
public class Key : MonoBehaviour
{
    [Header("Identificador")]
    [Tooltip("ID único para esta llave (debe coincidir con la puerta)")]
    [SerializeField] private string keyID = "key_default";
    
    [Header("Visual")]
    [SerializeField] private string keyName = "Llave";

    public string KeyID => keyID;
    public string KeyName => keyName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        KeyInventory inventory = other.GetComponent<KeyInventory>();
        if (inventory != null)
        {
            inventory.TryCollectKey(this);
        }
    }

    // Desactivar la llave cuando se recoge
    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
