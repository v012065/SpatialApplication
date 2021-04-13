using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);

        HandState hand = other.gameObject.GetComponent<HandState>();
        if(hand)
        {
            hand.onDesk = true;
            return;
        }

        if (other.gameObject.tag == "Hand")
        {
            other.gameObject.GetComponent<HandInteract>().onDesk = true;
            return;
        }

        OnlyPhysicsOnDesk doc = other.gameObject.GetComponent<OnlyPhysicsOnDesk>();
        if(doc)
        {
            doc.onDesk = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.gameObject);

        HandState hand = other.gameObject.GetComponent<HandState>();
        if (hand)
        {
            hand.onDesk = false;
            return;
        }

        if (other.gameObject.tag == "Hand")
        {
            other.gameObject.GetComponent<HandInteract>().onDesk = false;
            return;
        }

        OnlyPhysicsOnDesk doc = other.gameObject.GetComponent<OnlyPhysicsOnDesk>();
        if (doc)
        {
            doc.onDesk = false;
        }
    }
}
