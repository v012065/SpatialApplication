using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextMenuButton : MonoBehaviour
{
    public GameObject screen;
    public BackButton back;
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
    //        screen.SetActive(true);
    //        back.prevMenu = transform.parent.gameObject;
    //        transform.parent.gameObject.SetActive(false);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && !pressed)
        {
            screen.SetActive(true);
            back.prevMenu = transform.parent.gameObject;
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
