using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoSwitcher : MonoBehaviour
{
    public bool pressing;
    public bool triggered;

    public GameObject info;
    public OVRInput.Controller controller = OVRInput.Controller.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pressing = OVRInput.Get(OVRInput.Button.Two, controller);

        if (pressing)
        {
            if (!triggered)
            {
                triggered = true;
                info.SetActive(!info.activeSelf);
            }
        }
        else
        {
            triggered = false;
        }
    }
}
