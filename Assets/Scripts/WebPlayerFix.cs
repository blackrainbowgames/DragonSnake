using UnityEngine;

public class WebPlayerFix : MonoBehaviour
{
    public GameObject[] WebPlayerContent;
    public GameObject[] WebPlayerExcludeContent;

    public void Awake()
    {
        #if UNITY_WEBPLAYER

        foreach (var content in WebPlayerContent)
        {
            content.SetActive(true);
        }

        foreach (var content in WebPlayerExcludeContent)
        {
            content.SetActive(false);
        }

        #endif
    }
}