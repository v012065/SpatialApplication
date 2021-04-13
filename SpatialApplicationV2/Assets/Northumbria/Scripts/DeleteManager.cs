using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Deleteable del = other.gameObject.GetComponent<Deleteable>();
        if (del)
        {
            del.onDelete = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Deleteable del = other.gameObject.GetComponent<Deleteable>();
        if (del)
        {
            del.onDelete = false;
        }
    }
}
