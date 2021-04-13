using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackInteractible : Interactible
{
    public GameObject documentPrefab;
    public Vector3 rotateSpawnOffset;
    public Transform deleteCollection;

    // Start is called before the first frame update
    override public void Start()
    {
        
    }

    // Update is called once per frame
    override public void Update()
    {
        
    }

    public override void Action(GameObject obj)
    {
        GameObject doc = Instantiate(documentPrefab);
        doc.transform.position = obj.transform.position;
        doc.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        //doc.transform.Rotate(90, 0, 0);
        //doc.transform.Rotate(0, 180, 0);
        doc.transform.Rotate(rotateSpawnOffset);
        doc.GetComponent<Rigidbody>().isKinematic = true;
        doc.SetActive(true);
        Deleteable del = doc.GetComponent<Deleteable>();
        if (del.deleteCollection == null)
        {
            del.deleteCollection = deleteCollection;
        }

        HandInteract hand = obj.GetComponent<HandInteract>();
        if (hand)
        {
            hand.interactChild = doc.transform;
            //hand.originalParent = null;
        }

        HandState handState = obj.GetComponent<HandState>();
        if (hand)
        {
            doc.transform.parent = hand.transform;
            doc.GetComponent<ObjectOutline>().outline.SetActive(false);
        }
    }
}
