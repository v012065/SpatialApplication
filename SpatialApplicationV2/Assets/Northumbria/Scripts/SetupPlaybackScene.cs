using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlaybackScene : MonoBehaviour
{
    public GameObject[] scenes;
    public GameObject activeScene;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject scene in scenes)
        {
            scene.SetActive(false);
        }

        activeScene.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
