using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject prevMenu;
    public bool pressed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    HandState hand = other.gameObject.GetComponent<HandState>();
    //    Poke poke = other.gameObject.GetComponent<Poke>();

    //    if (hand != null || poke != null)
    //    {
    //        prevMenu.SetActive(true);
    //        transform.parent.gameObject.SetActive(false);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && !pressed)
        {
            prevMenu.SetActive(true);
            transform.parent.gameObject.SetActive(false);

            pressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && pressed)
        {
            pressed = false;
        }
    }
}
