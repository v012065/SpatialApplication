using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadButton : MonoBehaviour
{
    public enum Type
    {
        Accept,
        Decline
    }

    public Type type;
    public bool pressed;
    public Northumbria.RecordingManager rMan;

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
                case Type.Accept:
                    rMan.UpdateServerRecording();
                    break;
                case Type.Decline:
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
