using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    public Material originalMat;
    public string colour;

    // Start is called before the first frame update
    void Start()
    {
        originalMat = GetComponent<Renderer>().material;
        colour = "White";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Colour")
        {
            Renderer thisRen = GetComponent<Renderer>();
            Renderer thatRen = collision.collider.GetComponent<Renderer>();

            thisRen.material = thatRen.material;

            colour = collision.collider.name;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Colour")
        {
            Renderer thisRen = GetComponent<Renderer>();
            Renderer thatRen = other.gameObject.GetComponent<Renderer>();

            thisRen.material = thatRen.material;

            colour = other.name;
        }
    }
}
