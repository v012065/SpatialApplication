using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOutline : MonoBehaviour
{
    public Material outlineMat;
    public Material deleteMat;
    public GameObject outline;

    // Start is called before the first frame update
    void Start()
    {
        if (!outline) outline = Instantiate(gameObject);
        if (!outline.activeSelf) outline.SetActive(true);
        Destroy(outline.GetComponent<ObjectOutline>());
        Destroy(outline.GetComponent<Collider>());
        foreach (Transform child in outline.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        outline.tag = "Untagged";

        Renderer ren = outline.GetComponent<Renderer>();
        ren.material = outlineMat;
        ren.enabled = true;
        Vector3 temp = outline.transform.localScale;
        outline.transform.localScale = new Vector3(temp.x * 1.1f, temp.y * 1.1f, temp.z * 1.1f);
        outline.name = "Outline";
        outline.transform.parent = transform;
        outline.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.Get(OVRInput.Button.One))
        //{
        //    outline.SetActive(false);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Hand") outline.SetActive(true);
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    HandState handState = collision.collider.GetComponent<HandState>();
    //    if (handState != null)
    //    {
    //        if (handState.isGrabbing)
    //        {
    //            outline.SetActive(false);
    //        }
    //    }
    //}

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Hand") outline.SetActive(false);
    }
}
