using UnityEngine;

public class DamageItem : MonoBehaviour
{
    public enum DamageType
    {
        Fixed,      // Daño fijo
        InstantKill // Matar al jugador instantáneamente
    }

    [Header("Configuración de Daño")]
    [SerializeField] private DamageType damageType = DamageType.Fixed;
    [SerializeField] private int damageAmount = 2;  // Solo se usa si damageType es Fixed

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        Vector2 sourcePos = transform.position; // Punto desde donde viene el daño

        // Aplicar daño según el tipo
        if (damageType == DamageType.InstantKill)
        {
            player.TakeDamage(sourcePos, player.MaxHealth); // Mata al jugador
        }
        else
        {
            player.TakeDamage(sourcePos, damageAmount);     // Daño normal
        }

        // Sonido al recibir daño
        PlayerSoundController soundController = other.GetComponent<PlayerSoundController>();
        if (soundController != null)
            soundController.PlayHurtSound();

        // Notificar al TimeObject (si existe) para lógica temporal
        TimeObject.NotifyCollected(gameObject);

        // Destruir el objeto dañino
        Destroy(gameObject);
    }
}