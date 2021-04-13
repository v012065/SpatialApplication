using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpatialDocument : MonoBehaviour
{
    public AudioSource emitter;
    public AudioClip sound;
    public Text documentText;
    public Color originalColour;
    public EmailLoader spawner;
    public bool interacted;

    // Start is called before the first frame update
    void Start()
    {
        originalColour = transform.Find("Sides").GetComponentInChildren<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<OnlyPhysicsOnDesk>().onDesk)
        {
            Transform doc = transform.Find("Document");
            if (doc)
            {
                doc.localRotation = Quaternion.identity;
            }
        }

        if(transform.position.y < 0.8 && !GetComponent<Rigidbody>().isKinematic)
        {
            transform.position = new Vector3(transform.position.x, 0.82f, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandState hand = collision.collider.GetComponent<HandState>();

        if (hand != null)
        {
            GameObject doc = transform.Find("Document").gameObject;
            doc.SetActive(true);

            if (!interacted)
            {
                interacted = true;
                spawner.CheckAllInteracted();
            }

            if (emitter && sound)
            {
                if (emitter.enabled)
                {
                    emitter.PlayOneShot(sound);
                }
            }
        }

        //if (collision.collider.tag == "Brush")
        //{
        //    ColourCube(collision.collider.GetComponent<Renderer>().material.color);
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        HandState hand = collision.collider.GetComponent<HandState>();

        if (hand != null)
        {
            GameObject doc = transform.Find("Document").gameObject;
            doc.SetActive(false);
        }
    }

    //public void ColourCube(Color colour)
    //{
    //    Transform sides = transform.Find("Sides");

    //    foreach(Transform side in sides)
    //    {
    //        side.GetComponent<Renderer>().material.color = colour;
    //    }
    //}

    //public void ResetColour()
    //{
    //    ColourCube(originalColour);
    //}

    //public Color GetColour()
    //{
    //    Transform sides = transform.Find("Sides");

    //    foreach (Transform side in sides)
    //    {
    //        return side.GetComponent<Renderer>().material.color;
    //    }

    //    return originalColour;
    //}
}
