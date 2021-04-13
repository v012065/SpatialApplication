using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public Text text;
    public bool pressed;
    public bool triggered;
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pressed = OVRInput.Get(OVRInput.Button.One);
    }

    private void FixedUpdate()
    {
        if (pressed)
        {
            if (!triggered && !active)
            {
                RaycastHit hit;
                Transform cameraTrans = Camera.main.transform;
                Ray forwardRay = new Ray(cameraTrans.position, cameraTrans.forward);

                if (Physics.Raycast(forwardRay, out hit, Camera.main.farClipPlane))
                {
                    if (hit.collider.tag == "Moveable" || hit.collider.tag == "Interactible")
                    {
                        InfoAttach info = hit.collider.GetComponent<InfoAttach>();
                        if (info) text.text = info.info;
                        active = true;
                    }
                    else
                    {
                        text.text = "";
                        active = false;
                    }
                }

                triggered = true;
            }
            else if(!triggered && active)
            {
                active = false;
            }
        }
        else
        {
            triggered = false;
        }
    }
}
