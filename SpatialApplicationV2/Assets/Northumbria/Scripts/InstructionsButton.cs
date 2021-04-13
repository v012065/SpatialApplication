using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsButton : MonoBehaviour
{
    public enum Type
    {
        Back,
        Next,
    }

    public Type type;
    public ScreenMessage screen;
    public bool pressed;

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
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && !pressed)
        {
            switch (type)
            {
                case Type.Next:
                    screen.Next();
                    break;
                case Type.Back:
                    screen.Back();
                    break;
                default: break;
            }

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
