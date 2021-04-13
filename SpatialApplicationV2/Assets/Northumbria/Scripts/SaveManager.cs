using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public Northumbria.RecordingManager rMan;
    public AudioSource sound;
    public bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        //File.Create(Application.persistentDataPath + "/test.txt");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        DiskInteractible disk = other.GetComponent<DiskInteractible>();
        if (disk != null)
        {
            if (sound != null)
            {
                if (sound.clip != null)
                {
                    sound.PlayOneShot(sound.clip);
                }
            }

            if (rMan != null)
            {
                rMan.EndRecording();
            }

            other.transform.parent = null;
            Moveable mov = other.GetComponent<Moveable>();
            mov.grabbed = false;
            Destroy(mov);
            ObjectOutline outline = other.GetComponent<ObjectOutline>();
            outline.outline.SetActive(false);
            Destroy(outline);
            other.transform.position = new Vector3(18.46f, 0.9428f, 3.9864f);
            //other.transform.position = new Vector3(18.97348f, 0.4412f, 4.425594f);
            other.transform.rotation = Quaternion.Euler(180, 0, 0);
            Destroy(disk);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!triggered)
    //    {
    //        DiskInteractible disk = other.GetComponent<DiskInteractible>();
    //        if (disk != null)
    //        {
    //            if (sound != null)
    //            {
    //                if (sound.clip != null)
    //                {
    //                    sound.PlayOneShot(sound.clip);
    //                }
    //            }

    //            if (rMan != null)
    //            {
    //                rMan.EndRecording();
    //            }

    //            Destroy(disk.GetComponent<Moveable>());
    //            Destroy(disk.GetComponent<ObjectOutline>());
    //            disk.transform.parent = null;
    //            disk.transform.position = new Vector3(18.97348f, 0.4412f, 4.425594f);
    //            disk.transform.rotation = Quaternion.Euler(180, 0, 0);

    //            triggered = true;
    //        }
    //    }
    //}
}
