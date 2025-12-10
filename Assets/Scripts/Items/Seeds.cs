using UnityEngine;

// Semilla que el jugador puede recoger y plantar en macetas.
// En el pasado es semilla (recolectable), en el futuro es planta (trepable).
// NOTA: No usar TimeObject junto con este script - Seed maneja sus propios visuals.
[DefaultExecutionOrder(100)]  // Ejecutar DESPUÉS de TimeObject y TimeTraveler
public class Seed : MonoBehaviour
{
    [Header("Estado")]
    [SerializeField] private bool isPlanted = false;  // Si está plantada en una maceta
    
    [Header("Referencias")]
    [SerializeField] private GameObject seedVisual;    // Visual de semilla (PastVersion)
    [SerializeField] private GameObject plantVisual;   // Visual de planta (FutureVersion)
    [SerializeField] private Collider2D seedCollider;  // Collider para recoger la semilla
    
    private Flowerpot currentPot;  // Maceta donde está plantada

    private void Awake()
    {
        // Auto-buscar collider si no está asignado
        if (seedCollider == null)
            seedCollider = GetComponentInChildren<Collider2D>();
            
        // Auto-buscar visuals si no están asignados
        if (seedVisual == null || plantVisual == null)
        {
            Transform past = transform.Find("PastVersion");
            Transform future = transform.Find("FutureVersion");
            if (past != null) seedVisual = past.gameObject;
            if (future != null) plantVisual = future.gameObject;
        }
    }

    private void Start()
    {
        Debug.Log($"[Seed] Start - isPlanted: {isPlanted}, isInFuture: {TimeTraveler.isInFuture}");
        
        // Forzar activación correcta al inicio
        ForceUpdateVisual();
    }
    
    private void ForceUpdateVisual()
    {
        if (seedVisual == null || plantVisual == null)
        {
            Debug.LogError($"[Seed] Referencias NULL! seedVisual: {seedVisual}, plantVisual: {plantVisual}");
            return;
        }
        
        // Siempre forzar el estado correcto
        bool showPlant = TimeTraveler.isInFuture && isPlanted;
        
        seedVisual.SetActive(!showPlant);
        plantVisual.SetActive(showPlant);
        
        Debug.Log($"[Seed] ForceUpdateVisual - seedVisual activo: {seedVisual.activeSelf}, plantVisual activo: {plantVisual.activeSelf}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Seed] Trigger con: {other.name}, tag: {other.tag}");
        HandlePlayerContact(other);
    }
    
    // También detectar desde los hijos
    public void OnChildTriggerEnter(Collider2D other)
    {
        Debug.Log($"[Seed] Child Trigger con: {other.name}, tag: {other.tag}");
        HandlePlayerContact(other);
    }
    
    private void HandlePlayerContact(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Solo se puede recoger si estamos en el pasado (es semilla)
        if (!TimeTraveler.isInFuture && CanBeCollected())
        {
            TryCollect(other.gameObject);
        }
    }

    private bool CanBeCollected()
    {
        // Puede recogerse si estamos en el pasado
        return !TimeTraveler.isInFuture;
    }

    private void TryCollect(GameObject playerObj)
    {
        SeedInventory inventory = playerObj.GetComponent<SeedInventory>();
        if (inventory != null && inventory.CanCollectSeed())
        {
            // Recoger semilla
            inventory.CollectSeed(this);
            
            // Si estaba plantada, liberar la maceta
            if (isPlanted && currentPot != null)
            {
                currentPot.RemoveSeed();
                currentPot = null;
                isPlanted = false;
            }
            
            // Desactivar la semilla completa
            gameObject.SetActive(false);
            
            Debug.Log("[Seed] Semilla recogida!");
        }
    }

    // Plantar la semilla en una maceta
    public void PlantInPot(Flowerpot pot)
    {
        currentPot = pot;
        isPlanted = true;
        
        // Posicionar en la maceta
        transform.position = pot.GetPlantPosition();
        transform.SetParent(pot.transform);
        
        gameObject.SetActive(true);
        UpdateVisual();
        
        Debug.Log("[Seed] Semilla plantada en maceta!");
    }

    // Actualiza el visual según el tiempo actual
    public void UpdateVisual()
    {
        ForceUpdateVisual();
    }

    // Llamado cuando cambia el tiempo
    public void OnTimeChanged()
    {
        ForceUpdateVisual();
    }

    public bool IsPlanted => isPlanted;
}
