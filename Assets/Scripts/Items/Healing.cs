using UnityEngine;

public class Healing : MonoBehaviour
{
    public enum HealType
    {
        Fixed,      // Cura una cantidad fija
        FullHealth  // Cura toda la vida
    }

    [Header("Configuración de Curación")]
    [SerializeField] private HealType healType = HealType.Fixed;
    [SerializeField] private int healAmount = 4;  // Solo se usa si healType es Fixed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Aplicar curación según el tipo
        if (healType == HealType.FullHealth)
        {
            player.Heal(player.MaxHealth);  // Cura toda la vida
        }
        else
        {
            player.Heal(healAmount);  // Cura cantidad fija
        }

        // Sonido de curación
        PlayerSoundController soundController = other.GetComponent<PlayerSoundController>();
        if (soundController != null)
            soundController.PlayHealthPickupSound();

        // Notificar al TimeObject (si existe) para lógica temporal
        TimeObject.NotifyCollected(gameObject);

        // Destruir el item
        Destroy(gameObject);
    }
}
