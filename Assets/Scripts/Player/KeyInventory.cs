using UnityEngine;
using System.Collections.Generic;

// Inventario de llaves del jugador.
// Puede llevar hasta 2 llaves.
public class KeyInventory : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxKeys = 2;

    [Header("Estado")]
    [SerializeField] private List<Key> keys = new List<Key>();

    // Puerta cercana que requiere llave
    private DoorKeyRequired nearbyDoor;

    public int KeyCount => keys.Count;
    public bool HasAnyKey => keys.Count > 0;

    private void Update()
    {
        // Usar llave con E
        if (Input.GetKeyDown(KeyCode.E) && HasAnyKey && nearbyDoor != null)
        {
            TryUseKey();
        }
    }

    // Intentar recoger una llave
    public void TryCollectKey(Key key)
    {
        if (keys.Count >= maxKeys)
        {
            MessageUI.Show("¡Solo puedes llevar 2 llaves!");
            Debug.Log("[KeyInventory] Inventario lleno.");
            return;
        }

        keys.Add(key);

        // Desactivar la llave del mundo
        key.Collect();

        // Sonido
        PlayerSoundController soundController = GetComponent<PlayerSoundController>();
        if (soundController != null)
        {
            soundController.PlayCollectItemSound();
        }

        MessageUI.Show($"¡Recogiste {key.KeyName}!");
        Debug.Log($"[KeyInventory] Llave recogida: {key.KeyID}");
    }

    // Intentar usar una llave en la puerta cercana
    private void TryUseKey()
    {
        if (nearbyDoor == null) return;

        // Buscar una llave compatible
        for (int i = 0; i < keys.Count; i++)
        {
            if (nearbyDoor.TryUnlock(keys[i].KeyID))
            {
                Debug.Log($"[KeyInventory] Llave usada: {keys[i].KeyID}");

                // Eliminar solo la llave usada
                keys.RemoveAt(i);

                MessageUI.Show("¡Usaste la llave! La puerta está desbloqueada.");
                return;
            }
        }

        MessageUI.Show("Ninguna de tus llaves funciona aquí...");
        Debug.Log("[KeyInventory] Ninguna llave coincide.");
    }

    // Verificar si tiene una llave específica
    public bool HasKeyWithID(string keyID)
    {
        return keys.Exists(k => k.KeyID == keyID);
    }

    // Detectar puertas cercanas
    private void OnTriggerEnter2D(Collider2D other)
    {
        DoorKeyRequired door = other.GetComponent<DoorKeyRequired>();
        if (door != null)
        {
            nearbyDoor = door;

            if (HasAnyKey && !door.IsUnlocked)
            {
                MessageUI.Show("Presiona E para usar una llave");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        DoorKeyRequired door = other.GetComponent<DoorKeyRequired>();
        if (door != null && door == nearbyDoor)
        {
            nearbyDoor = null;
        }
    }
}