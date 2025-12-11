using UnityEngine;

public class BossAnimationHandler : MonoBehaviour
{
    [Header("Referencia (dejar vacío para buscar automáticamente)")]
    [SerializeField] private Animator animator;

    private readonly string RUN = "isRunning";
    private readonly string ATTACK = "isAttacking";
    private readonly string HIT = "isHitting";
    private readonly string DIE = "isDying";

    private void Awake()
    {
        // Si no está asignado manualmente, buscar en hijos (excluyendo este objeto)
        if (animator == null)
        {
            // Buscar en todos los hijos
            Animator[] animators = GetComponentsInChildren<Animator>();
            foreach (Animator anim in animators)
            {
                // Tomar el que NO esté en este mismo objeto
                if (anim.gameObject != gameObject)
                {
                    animator = anim;
                    break;
                }
            }
        }
        
        if (animator == null)
        {
            Debug.LogError("[BossAnimationHandler] No se encontró Animator en los hijos!");
        }
        else
        {
            Debug.Log("[BossAnimationHandler] Animator encontrado en: " + animator.gameObject.name);
        }
    }

    private void ResetBools()
    {
        animator.SetBool(RUN, false);
        animator.SetBool(ATTACK, false);
        animator.SetBool(HIT, false);
        animator.SetBool(DIE, false);
    }

    public void PlayIdle()
    {
        ResetBools();
        // Idle es estado base sin bools
    }

    public void PlayRun()
    {
        ResetBools();
        animator.SetBool(RUN, true);
    }

    public void PlayAttack()
    {
        ResetBools();
        animator.SetBool(ATTACK, true);
    }

    public void PlayHit()
    {
        ResetBools();
        animator.SetBool(HIT, true);
    }

    public void PlayDeath()
    {
        ResetBools();
        animator.SetBool(DIE, true);
    }
}