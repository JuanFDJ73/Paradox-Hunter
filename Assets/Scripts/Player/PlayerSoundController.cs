using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip climbSound;
    public AudioClip timeTravelSound;
    public AudioClip footstep1Sound;
    public AudioClip footstep2Sound;
    public AudioClip jumpSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip collectItemSound;
    public AudioClip powerUpSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    public AudioClip healthPickupSound;
    private bool playFirstFootstep = true; // Alternar entre los sonidos de paso

    public void PlayClimbSound()
    {
        audioSource.PlayOneShot(climbSound);
    }
    public void PlayTimeTravelSound()


    {
        audioSource.PlayOneShot(timeTravelSound);
    }

    public void PlayFootstepSound()
    {
        if (playFirstFootstep)
            audioSource.PlayOneShot(footstep1Sound);
        else
            audioSource.PlayOneShot(footstep2Sound);

        playFirstFootstep = !playFirstFootstep; // Cambia al siguiente paso
    }

    public void PlayFootstep1Sound()
    {
        audioSource.PlayOneShot(footstep1Sound);
    }
    public void PlayFootstep2Sound()
    {
        audioSource.PlayOneShot(footstep2Sound);
    }
    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtSound);
    }
    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
    public void PlayCollectItemSound()
    {
        audioSource.PlayOneShot(collectItemSound);
    }
    public void PlayPowerUpSound()
    {
        audioSource.PlayOneShot(powerUpSound);
    }
    public void PlayDoorOpenSound()
    {
        audioSource.PlayOneShot(doorOpenSound);
    }
    public void PlayDoorCloseSound()
    {
        audioSource.PlayOneShot(doorCloseSound);
    }
    public void PlayHealthPickupSound()
    {
        audioSource.PlayOneShot(healthPickupSound);
    }
}
