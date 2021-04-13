using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class XRSceneManager : MonoBehaviour
{
    public bool enableXR = true;
    public bool enabledXR;

    // Start is called before the first frame update
    void Start()
    {
        if (enableXR)
        {
            StartXR();
        }
        else
        {
            StopXR();
        }

        enabledXR = IsXRRunning();
    }

    // Update is called once per frame
    void Update()
    {
        if(enableXR != enabledXR)
        {
            if (enableXR)
            {
                StartXR();
            }
            else
            {
                StopXR();
            }

            enabledXR = IsXRRunning();
        }
    }

    // https://forum.unity.com/threads/deprecation-nightmare.812688/#post-5394783
    List<XRDisplaySubsystemDescriptor> displaysDescs = new List<XRDisplaySubsystemDescriptor>();
    List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

    bool IsActive()
    {
        displaysDescs.Clear();
        SubsystemManager.GetSubsystemDescriptors(displaysDescs);

        // If there are registered display descriptors that is a good indication that VR is most likely "enabled"
        return displaysDescs.Count > 0;
    }

    bool IsXRRunning()
    {
        bool xrIsRunning = false;
        displays.Clear();
        SubsystemManager.GetInstances(displays);
        foreach (var displaySubsystem in displays)
        {
            if (displaySubsystem.running)
            {
                xrIsRunning = true;
                break;
            }

            //displaySubsystem.Stop();
        }

        return xrIsRunning;
    }

    void StartXR()
    {
        displays.Clear();
        SubsystemManager.GetInstances(displays);
        foreach (var displaySubsystem in displays)
        {
            if (!displaySubsystem.running)
            {
                displaySubsystem.Start();
                break;
            }

            //displaySubsystem.Stop();
        }
    }

    void StopXR()
    {
        displays.Clear();
        SubsystemManager.GetInstances(displays);
        foreach (var displaySubsystem in displays)
        {
            if (displaySubsystem.running)
            {
                displaySubsystem.Stop();
                //break;
            }

            //displaySubsystem.Stop();
        }
    }
}
