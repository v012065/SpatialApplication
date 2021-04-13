using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandState : MonoBehaviour
{
    public OVRInput.Controller controller;
    public bool isRight;
    public bool onDesk;
    public bool isGrabbing;
    public bool isPointing;
    public bool isThumbsUp;
    public GameObject poke;

    // Start is called before the first frame update
    void Start()
    {
        if((controller == OVRInput.Controller.RHand) || (controller == OVRInput.Controller.RTouch))
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isGrabbing = isRight ? OVRInput.Get(OVRInput.RawButton.RIndexTrigger) : OVRInput.Get(OVRInput.RawButton.LIndexTrigger);
        isGrabbing = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, controller);
        isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, controller);

        if(isPointing)
        {
            poke.SetActive(true);
        }
        else
        {
            poke.SetActive(false);
        }
    }
}
