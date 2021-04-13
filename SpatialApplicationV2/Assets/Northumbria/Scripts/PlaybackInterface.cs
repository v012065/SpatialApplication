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
    public class PlaybackInterface : MonoBehaviour
    {
        public PlaybackManager pm;
        public Text uiText;

        // Start is called before the first frame update
        void Start()
        {
            if (pm == null) GetComponent<PlaybackManager>();

            pm.LoadEvent += LoadFinished;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Load()
        {
            pm.LoadRecording();
        }

        public void LoadWeb()
        {
            pm.LoadRecordingWeb();
        }

        void LoadFinished(object sender, PlaybackManager.LoadEventArgs e)
        {
            uiText.text = e.fileMetaData;
        }

        public void LoadEndState()
        {
            pm.LoadEndState();
        }

        public void Play()
        {
            if (pm.IsPrevState(PlaybackManager.State.Load) || pm.IsPrevState(PlaybackManager.State.Ended))
            {
                pm.Play();
            }
            else if (pm.IsState(PlaybackManager.State.Paused))
            {
                pm.SetState(PlaybackManager.State.Start);
            }
        }

        public void Pause()
        {
            if (pm.IsState(PlaybackManager.State.Playing))
            {
                pm.SetState(PlaybackManager.State.Paused);
            }
        }

        public void Stop()
        {
            if (pm.IsState(PlaybackManager.State.Playing))
            {
                pm.SetState(PlaybackManager.State.Ended);
            }
        }

        public void Reverse()
        {
            if (pm.IsState(PlaybackManager.State.Playing) || pm.IsState(PlaybackManager.State.Paused) || pm.IsPrevState(PlaybackManager.State.Ended))
            {
                pm.ReversePlayback();
            }
        }
    }
}
