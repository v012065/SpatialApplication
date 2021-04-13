using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentInteractible : MonoBehaviour
{
    public GameObject marker;
    public bool faceUser;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandState hand = transform.parent.parent.GetComponent<HandState>();
        if (hand != null)
        {
            if (faceUser && hand.isGrabbing)
            {
                transform.LookAt(Camera.main.transform);
                //Quaternion rot = Quaternion.Euler(transform.localRotation.eulerAngles.x, 0.0f, transform.localRotation.eulerAngles.z);
                //transform.localRotation = rot;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Brush")
        {
            marker.GetComponent<Renderer>().material = collision.collider.GetComponent<Renderer>().material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Brush")
        {
            marker.GetComponent<Renderer>().material = other.gameObject.GetComponent<Renderer>().material;
        }
    }
}
