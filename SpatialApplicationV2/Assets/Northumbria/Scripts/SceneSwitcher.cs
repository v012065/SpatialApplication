using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public int startScene;
    public List<GameObject> scenes;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveScene(startScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveScene(int value)
    {
        if (scenes != null)
        {
          foreach (GameObject scene in scenes)
         {
             scene.SetActive(false);
            }

            scenes[value].SetActive(true);
        }
    }
}
