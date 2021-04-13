using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public Transform originalParent;
    public GameObject collided;
    public bool grabbed;
    public bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        if (transform.parent != null)
        {
            HandState hand = transform.parent.GetComponent<HandState>();
            if (hand != null)
            {
                if (!hand.isGrabbing)
                {
                    transform.parent = originalParent;

                    if (collided)
                    {
                        if (collided.GetComponent<HandState>() != null)
                        {
                            if (body != null) body.isKinematic = true;
                        }
                    }

                    grabbed = false;
                    triggered = false;
                }
                else
                {
                    grabbed = true;
                }
            }
            else
            {
                if (body != null) body.isKinematic = false;
                triggered = false;
            }
        }
        else
        {
            if (body != null) body.isKinematic = false;
            triggered = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        collided = collision.gameObject;

        HandState hand = collided.GetComponent<HandState>();
        if (hand != null && !triggered)
        {
            if (hand.isGrabbing && (hand.transform.childCount == 2))
            {
                Rigidbody body = GetComponent<Rigidbody>();
                if (body != null) body.isKinematic = true;

                ObjectOutline outline = GetComponent<ObjectOutline>();
                if (outline != null) outline.outline.SetActive(false);

                if (transform.parent != null)
                {
                    if (transform.parent.GetComponent<HandState>() == null)
                    {
                        originalParent = transform.parent;
                    }
                    else
                    {
                        return;
                    }
                }

                transform.parent = collision.collider.transform;
                triggered = true;
            }
        }
    }
}
