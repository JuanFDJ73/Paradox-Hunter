using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image lifeFill;
    
    private PlayerController playerController;
    private float maxHealth;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        maxHealth = playerController.MaxHealth;
    }

    void Update()
    {   
        lifeFill.fillAmount = (float)playerController.CurrentHealth / maxHealth;
    }
}
