using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourableTexture : MonoBehaviour
{
    public Color originalColour;

    // Start is called before the first frame update
    void Start()
    {
        originalColour = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Brush")
        {
            GetComponent<Renderer>().material.color = collision.collider.GetComponent<Renderer>().material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Brush")
        {
            GetComponent<Renderer>().material.color = other.gameObject.GetComponent<Renderer>().material.color;
        }
    }
}
