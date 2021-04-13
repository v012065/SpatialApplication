using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletManager : MonoBehaviour
{
    public List<GameObject> colours;
    public Transform brush;
    //public Transform brushHolder;
    //public Transform handObj;
    public Vector3 brushOffset;
    public Quaternion brushRot;

    // Start is called before the first frame update
    void Start()
    {
        brushOffset = brush.localPosition;
        //brushRot = brush.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(brush.parent == transform)
        {
            brush.localPosition = brushOffset;
            brush.localRotation = brushRot;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (enabled)
        {
            HandState hand = collision.gameObject.GetComponent<HandState>();
            if (hand != null)
            {
            //    HandState handParent = transform.parent.GetComponent<HandState>();

            //    if (handParent != null)
            //    {
                    if (hand.isGrabbing && (hand.transform.childCount == 2))
                    {
                        brush.GetComponent<Moveable>().originalParent = brush.parent;

                        brush.parent = collision.transform;

                        brush.transform.localPosition = Vector3.zero;
                        brush.transform.localRotation = Quaternion.identity;
                        brush.transform.Rotate(new Vector3(300, 0, -180));
                    //handObj = brush.parent;
                    }
                //}
                //else
                //{
                //    handObj = null;
                //}
            }
        }
    }
}
