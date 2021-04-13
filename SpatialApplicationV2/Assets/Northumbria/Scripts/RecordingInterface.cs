using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine.UI;
using System.Globalization;

// http://xmltocsharp.azurewebsites.net/
// https://answers.unity.com/questions/1248234/parse-xml-file-in-unity-c.html

/* XML Format
<session-record>
  <start time="DateTime.Now.ToLongTimeString()" date="DateTime.Now.ToShortDateString()" />
  <update seconds="Time.fixedUnscaledTime" >
    <object name="obj.name" tag="obj.tag" >
      <position x="obj.transform.position.x" y="obj.transform.position.y" z="obj.transform.position.z" />
      <rotation x="obj.transform.rotation.x" y="obj.transform.rotation.y" z="obj.transform.rotation.z" />
    </object>
    <object name="obj.name" tag="obj.tag" >
      <position x="obj.transform.position.x" y="obj.transform.position.y" z="obj.transform.position.z" />
      <rotation x="obj.transform.rotation.x" y="obj.transform.rotation.y" z="obj.transform.rotation.z" />
    </object>
  </update>
  <action seconds="" type="switchtool|spawndoc|deletedoc" />
  <end time="DateTime.Now.ToLongTimeString()" />
</session-record>
*/

namespace Northumbria
{
    public class RecordingInterface : MonoBehaviour
    {
        public RecordingManager rm;
        public bool pressed;
        public bool active;

        // Start is called before the first frame update
        void Start()
        {
            if (rm == null) GetComponent<RecordingManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //if(OVRInput.GetDown(OVRInput.Button.Start))
            if (OVRInput.Get(OVRInput.Button.Start))
            {
                Debug.Log("Start Button");

                if (!pressed && !active)
                {
                    rm.StartRecording();

                    pressed = true;
                    active = true;
                }
                else if (!pressed && active)
                {
                    rm.EndRecording();

                    pressed = true;
                    active = false;
                }
            }
            else
            {
                pressed = false;
            }
        }
    }
}
