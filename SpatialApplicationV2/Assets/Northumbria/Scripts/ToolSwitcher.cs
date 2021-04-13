using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSwitcher : MonoBehaviour
{
    public List<GameObject> tools;
    public bool pressing;
    public bool triggered;
    public int curTool;

    // Start is called before the first frame update
    void Start()
    {
        curTool = 0;
    }

    // Update is called once per frame
    void Update()
    {
        pressing = OVRInput.Get(OVRInput.Button.Three);

        if (pressing)
        {
            if (!triggered)
            {
                triggered = true;
                foreach(GameObject tool in tools)
                {
                    tool.SetActive(false);
                }
                if (++curTool >= tools.Count) curTool = 0;
                tools[curTool].SetActive(true);
            }
        }
        else
        {
            triggered = false;
        }
    }
}
