using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public bool active;

    // Start is called before the first frame update
    virtual public void Start()
    {

    }

    // Update is called once per frame
    virtual public void Update()
    {

    }

    abstract public void Action(GameObject obj);

    private void OnCollisionStay(Collision collision)
    {
        HandState hand = collision.collider.GetComponent<HandState>();
        if (hand)
        {
            if(hand.isGrabbing && !active)
            {
                Action(collision.gameObject);
                active = true;
            }
            else if(!hand.isGrabbing)
            {
                active = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}
