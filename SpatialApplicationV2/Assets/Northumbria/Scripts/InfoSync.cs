using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoSync : MonoBehaviour
{
    public Text info;
    public Text sync;

    private void OnEnable()
    {
        sync.text = info.text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool pressing = OVRInput.Get(OVRInput.Button.One);

        if (pressing)
        {
            sync.text = info.text;
        }
    }
}
