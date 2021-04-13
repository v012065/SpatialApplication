using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadTrigger : MonoBehaviour
{
    //public GameObject scene;
    public SceneSwitcher sceneSwitcher;
    public Text info;
    //public Northumbria.RecordingManager rMan;
    //public bool record;

    // Start is called before the first frame update
    void Start()
    {
        info.text = "";
        //scene.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerProc()
    {
        //other.transform.parent = null;
        //other.gameObject.SetActive(false);

        if (sceneSwitcher != null)
        {
            info.text = "Welcome";
            sceneSwitcher.SetActiveScene(1);
        }

        //if(rMan != null && record)
        //{
        //    rMan.StartRecording();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ARGoggles>())
        {
            Destroy(other.gameObject);

            TriggerProc();
        }
    }
}
