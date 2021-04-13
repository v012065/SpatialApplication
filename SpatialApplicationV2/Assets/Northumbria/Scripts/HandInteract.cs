using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteract : MonoBehaviour
{
    //public Transform originalParent;
    public bool pressed;
    public bool pressing;
    public Transform interactChild;
    public bool trigger;
    public Material deleteMat;
    public bool isRight;
    public bool onDesk;
    public Transform originalParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pressing = isRight ? OVRInput.Get(OVRInput.RawButton.RIndexTrigger) : OVRInput.Get(OVRInput.RawButton.LIndexTrigger);//OVRInput.Get(OVRInput.Button.One);

        if (!pressing)
        {
            trigger = false;
        }

        if (interactChild)
        {
            if (!pressing)//(!OVRInput.Get(OVRInput.Button.One))
            {
                if (interactChild.parent.GetComponent<HandInteract>() == null)
                {
                    interactChild.parent = originalParent;

                    Rigidbody body = interactChild.GetComponent<Rigidbody>();
                    if (body != null) body.isKinematic = false;
                }

                if (transform.position.y < 0.8)
                {
                    DeleteInteractible(interactChild);
                }

                interactChild = null;
            }

            if (trigger)
            {
                interactChild.parent = transform;
            }

            ObjectOutline objOutline = null;

            if (interactChild)
            {
                objOutline = interactChild.GetComponent<ObjectOutline>();
            }

            if (pressing)//(OVRInput.Get(OVRInput.Button.One))
            {
                if (objOutline)
                {
                    if (transform.position.y < 0.8)
                    {
                        objOutline.outline.GetComponent<Renderer>().material = deleteMat;
                        objOutline.outline.SetActive(true);
                    }
                    else
                    {
                        objOutline.outline.GetComponent<Renderer>().material = objOutline.outlineMat;
                        objOutline.outline.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Moveable" && pressing)//OVRInput.Get(OVRInput.Button.One))
        {
            if (!trigger)
            {
                Rigidbody body = collision.collider.GetComponent<Rigidbody>();
                if (body != null) body.isKinematic = true;

                interactChild = collision.collider.transform;
                if (interactChild.parent.GetComponent<HandInteract>() == null)
                {
                    originalParent = interactChild.parent;
                }
                trigger = true;
            }
        }
        else if (collision.collider.tag == "Interactible" && pressing)//OVRInput.Get(OVRInput.Button.One))
        {
            if (!trigger)
            {
                Interactible inter = collision.collider.GetComponent<Interactible>();
                inter.Action(gameObject);
                trigger = true;
            }
        }
    }

    void DeleteInteractible(Transform interactible)
    {
        //Destroy(interactible.gameObject);
    }
}
