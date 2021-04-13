using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    //public enum Type
    //{
    //    EndOnly,
    //    Full
    //}

    public bool pressed;
    public AudioSource sound;
    public Northumbria.RecordingManager rMan;
    //public Type type;
    //public GameObject info;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerProc()
    {
        if (rMan != null)
        {
            //if (type == Type.EndOnly)
            //{
            //    rMan.EndRecording();
            //    gameObject.SetActive(false);
            //}
            //else if (type == Type.Full)
            //{
            //    info.SetActive(true);
            //}

            rMan.EndRecording();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && !pressed)
        {
            if (sound != null)
            {
                if (sound.clip != null)
                {
                    sound.PlayOneShot(sound.clip);
                }
            }

            TriggerProc();

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
