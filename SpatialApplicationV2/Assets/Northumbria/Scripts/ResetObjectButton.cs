using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjectButton : MonoBehaviour
{
    public Transform obj;
    public Vector3 pos;
    public Vector3 rot;
    public bool makeActive;
    public bool pressed;

    // Start is called before the first frame update
    void Start()
    {
        pos = obj.position;
        rot = obj.rotation.eulerAngles;
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
            if(!obj.gameObject.activeSelf && makeActive)
            {
                obj.gameObject.SetActive(true);
            }

            if(obj.gameObject.activeSelf)
            {
                obj.position = pos;
                obj.rotation = Quaternion.Euler(rot);
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
