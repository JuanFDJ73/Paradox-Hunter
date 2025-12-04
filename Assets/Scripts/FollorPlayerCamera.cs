using UnityEngine;
using Cinemachine;

public class FollowPlayerCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();

        if (player == null)
        {
            Debug.LogWarning("No se encontró el jugador en la escena");
            return;
        }

        if (virtualCamera == null)
        {
            Debug.LogWarning("No se encontró CinemachineVirtualCamera");
            return;
        }

        virtualCamera.Follow = player.transform;
    }
}
