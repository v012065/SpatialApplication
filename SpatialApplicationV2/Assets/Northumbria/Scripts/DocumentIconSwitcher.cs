using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentIconSwitcher : MonoBehaviour
{
    public Vector3 smallScale = Vector3.one;
    public Vector3 smallCollisionScale = Vector3.one;
    public Vector3 smallOutlineScale = Vector3.one;
    public Vector3 bigScale = Vector3.one;
    public Vector3 bigCollisionScale = Vector3.one;
    public Vector3 bigOutlineScale = Vector3.one;
    public Vector3 scale = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        SetSmall();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent != null)
        {
            //if(transform.parent.GetComponent<HandInteract>().onDesk)
            //{
            //    Debug.Log("at desk");
            //    SetSmall();
            //}
            //else
            //{
            //    Debug.Log("away from desk");
            //    SetBig();
            //}

            HandState handState = transform.parent.GetComponent<HandState>();
            if (handState)
            {
                if (handState.onDesk)
                {
                    //Debug.Log("at desk");
                    SetSmall();
                }
                else
                {
                    //Debug.Log("away from desk");
                    SetBig();
                }
            }
        }
    }

    void ApplyScalor(Vector3 s)
    {
        scale = Vector3.one;
        if (transform.parent != null)
        {
            scale = transform.parent.localScale;
            scale.x = 1 / scale.x;
            scale.y = 1 / scale.y;
            scale.z = 1 / scale.z;
        }

        transform.localScale = new Vector3(s.x * scale.x, s.y * scale.y, s.z * scale.z);
    }

    void SetBig()
    {
        ApplyScalor(bigScale);
        GetComponent<DocumentInteractible>().enabled = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<BoxCollider>().size = bigCollisionScale;
        transform.Find("Sides").gameObject.SetActive(false);
        transform.Find("Marker").gameObject.SetActive(true);
        transform.Find("Outline").transform.localScale = bigOutlineScale;
    }

    void SetSmall()
    {
        ApplyScalor(smallScale);
        GetComponent<DocumentInteractible>().enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<BoxCollider>().size = smallCollisionScale;
        transform.Find("Sides").gameObject.SetActive(true);
        transform.Find("Marker").gameObject.SetActive(false);
        transform.Find("Outline").transform.localScale = Vector3.one;
        transform.Find("Outline").transform.localScale = smallOutlineScale;
    }
}