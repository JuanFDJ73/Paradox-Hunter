using UnityEngine;

public class TimeObject : MonoBehaviour
{
    public GameObject pastVersion;
    public GameObject futureVersion;

    public void ShowPast()
    {
        pastVersion.SetActive(true);
        futureVersion.SetActive(false);
    }

    public void ShowFuture()
    {
        pastVersion.SetActive(false);
        futureVersion.SetActive(true);
    }
}