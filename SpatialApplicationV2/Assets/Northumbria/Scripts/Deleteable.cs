using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deleteable : MonoBehaviour
{
    public bool onDelete;
    public Transform deleteCollection;
    public bool deleted;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != null)
        {
            HandState hand = transform.parent.GetComponent<HandState>();
            if (hand == null)
            {
                if (onDelete)
                {
                    Delete();
                }
            }
            else
            {
                if (onDelete)
                {
                    ObjectOutline outline = transform.GetComponent<ObjectOutline>();
                    if (outline != null)
                    {
                        outline.outline.GetComponent<Renderer>().material = outline.deleteMat;
                        outline.outline.gameObject.SetActive(true);
                    }
                }
                else
                {
                    ObjectOutline outline = transform.GetComponent<ObjectOutline>();
                    if (outline != null)
                    {
                        outline.outline.GetComponent<Renderer>().material = outline.outlineMat;
                        outline.outline.gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (onDelete)
            {
                Delete();
            }
        }
    }

    void Delete()
    {
        deleted = true;
        gameObject.SetActive(false);
        transform.parent = deleteCollection;
    }
}
