using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskInteractible : Interactible
{
    public Renderer diskFront;
    public Renderer diskBack;
    public Material originalFront;
    public Material originalBack;

    // Start is called before the first frame update
    public override void Start()
    {
        originalFront = diskFront.material;
        originalBack = diskBack.material;
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Action(GameObject obj)
    {

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.tag == "Brush")
    //    {
    //        diskFront.material = collision.collider.GetComponent<Renderer>().material;
    //        diskBack.material = collision.collider.GetComponent<Renderer>().material;
    //    }
    //}

    //public void ResetMaterials()
    //{
    //    diskFront.material = originalFront;
    //    diskBack.material = originalBack;
    //}

    //public void SetColour(Color c)
    //{
    //    diskFront.material.color = c;
    //    diskBack.material.color = c;
    //}

    //public Color GetColour()
    //{
    //    return diskFront.material.color;
    //}
}
