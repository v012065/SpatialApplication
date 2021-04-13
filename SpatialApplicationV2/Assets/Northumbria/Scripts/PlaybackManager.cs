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
using UnityEngine.Networking;

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
    <update>
      <object name="obj.name" tag="obj.tag" colour="c.colour" >
        <position x="obj.transform.position.x" y="obj.transform.position.y" z="obj.transform.position.z" />
        <rotation x="obj.transform.rotation.x" y="obj.transform.rotation.y" z="obj.transform.rotation.z" />
      </object>
    </update>
  </end>
</session-record>
*/

namespace Northumbria
{
    public class PlaybackManager : MonoBehaviour
    {
        public enum State
        {
            None,
            Load,
            LoadWeb,
            Start,
            Playing,
            Scrubbing,
            Paused,
            Ended
        }

        // Load Event
        public event EventHandler<LoadEventArgs> LoadEvent;
        protected virtual void OnLoadEvent(LoadEventArgs e)
        {
            LoadEvent?.Invoke(this, e);
        }

        public delegate void LoadEventHandler(object sender, StartEventArgs e);

        public class LoadEventArgs : EventArgs
        {
            public LoadEventArgs()
            {

            }

            public LoadEventArgs(string s)
            {
                fileMetaData = s;
            }

            public string fileMetaData;
        }
        //

        // Start Event
        public event EventHandler StartEvent;
        protected virtual void OnStartEvent(EventArgs e)
        {
            EventHandler handler = StartEvent;
            handler?.Invoke(this, e);
        }

        public delegate void StartEventHandler(object sender, StartEventArgs e);

        public class StartEventArgs : EventArgs
        {

        }
        //

        // Playing Event
        public event EventHandler PlayingEvent;
        protected virtual void OnPlayingEvent(EventArgs e)
        {
            EventHandler handler = PlayingEvent;
            handler?.Invoke(this, e);
        }

        public delegate void PlayingEventHandler(object sender, StartEventArgs e);

        public class PlayingEventArgs : EventArgs
        {

        }
        //

        // Scrubbing Event
        public event EventHandler ScrubbingEvent;
        protected virtual void OnScrubbingEvent(EventArgs e)
        {
            EventHandler handler = ScrubbingEvent;
            handler?.Invoke(this, e);
        }

        public delegate void ScrubbingEventHandler(object sender, StartEventArgs e);

        public class ScrubbingEventArgs : EventArgs
        {

        }
        //

        // Paused Event
        public event EventHandler PausedEvent;
        protected virtual void OnPausedEvent(EventArgs e)
        {
            EventHandler handler = PausedEvent;
            handler?.Invoke(this, e);
        }

        public delegate void PausedEventHandler(object sender, StartEventArgs e);

        public class PausedEventArgs : EventArgs
        {

        }
        //

        // Ended Event
        public event EventHandler EndedEvent;
        protected virtual void OnEndedEvent(EventArgs e)
        {
            EventHandler handler = EndedEvent;
            handler?.Invoke(this, e);
        }

        public delegate void EndedEventHandler(object sender, StartEventArgs e);

        public class EndedEventArgs : EventArgs
        {

        }
        //

        public GameObject proxyViewer;
        public List<GameObject> updateList;
        public string fileName;
        public State state = State.None;
        public State prevState = State.None;

        public float playerTime;
        public float playbackSpeed = 1f;
        public float maxTime = 1f;
        public int updateCount;
        Rec_Update currentUpdate;
        Rec_Update nextUpdate;

        XmlDocument xmlDoc;
        Recording loadedRecording;
        public string recordingMetaData;
        public string loadedFile;

        public GameObject pallet;

        public NetworkManager nMan;

        public void TestUpload()
        {
            if (nMan != null && xmlDoc != null)
            {
                nMan.SaveRecordingToServer(xmlDoc.InnerXml);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log(Application.persistentDataPath);
        }

        // Update is called once per frame
        void Update()
        {
            if (state == State.Load)
            {
                UpdateLoad();

                SetState(State.None);
            }
            else if(state == State.LoadWeb)
            {

            }
            else if (state == State.Start)
            {
                if (prevState == State.Ended)
                {
                    if (playbackSpeed < 0)
                    {
                        ResetPlaybackEnd();
                    }
                    else if (playbackSpeed >= 0)
                    {
                        ResetPlaybackStart();
                    }

                    SetState(State.Playing);
                }
                else if (prevState == State.Load || prevState == State.Paused)
                {
                    SetState(State.Playing);
                }
                else
                {
                    SetState(State.None);
                }
            }
            else if (state == State.Playing)
            {
                if (IsPrevState(State.Start))
                {
                    UpdatePlaying();
                }
                else
                {
                    SetState(State.None);
                }
            }
            else if (IsState(State.Ended))
            {
                if (playerTime <= 0)
                {
                    ResetPlaybackStart();
                }
                else if (playerTime >= maxTime)
                {
                    ResetPlaybackEnd();
                }
                else
                {
                    if (playbackSpeed < 0)
                    {
                        ResetPlaybackEnd();
                    }
                    else if (playbackSpeed >= 0)
                    {
                        ResetPlaybackStart();
                    }
                }

                SetState(State.None);
            }
        }

        void UpdateLoad()
        {
            //LoadRecordingFromDisk(Application.persistentDataPath + "/" + fileName + ".xml");

            loadedRecording = new Recording();
            loadedRecording.updates = new List<Rec_Update>();

            int child = 0;
            if(xmlDoc.FirstChild.Name == "xml")
            {
                child++;
            }

            // <start time="00:00:00" >
            recordingMetaData = "Start Time: " + xmlDoc.ChildNodes[child].FirstChild.Attributes.GetNamedItem("time").Value;
            // <start date="DD/MM/YYYY" >
            recordingMetaData += "\nStart Date: " + xmlDoc.ChildNodes[child].FirstChild.Attributes.GetNamedItem("date").Value;
            // <end time="00:00:00" >
            recordingMetaData += "\nEnd Time: " + xmlDoc.ChildNodes[child].LastChild.Attributes.GetNamedItem("time").Value;

            if (xmlDoc.ChildNodes[child].ChildNodes.Count > 2)
            {
                for (int i = 1; i < xmlDoc.ChildNodes[child].ChildNodes.Count - 1; ++i)
                {
                    if (xmlDoc.ChildNodes[child].ChildNodes[i].Name == "update")
                    {
                        Rec_Update update = new Rec_Update();

                        // <update seconds="0.0(s)" >
                        update.time = float.Parse(xmlDoc.ChildNodes[child].ChildNodes[i].Attributes.GetNamedItem("seconds").Value, CultureInfo.InvariantCulture.NumberFormat);
                        update.objList = new List<Rec_Object>();

                        foreach (XmlNode obj in xmlDoc.ChildNodes[child].ChildNodes[i])
                        {
                            Rec_Object o = new Rec_Object();
                            // <object name="" >
                            o.name = obj.Attributes.GetNamedItem("name").Value;
                            // tag=""
                            XmlNode node = obj.Attributes.GetNamedItem("tag");
                            o.tag = (node != null) ? node.Value : "Untagged";
                            // active="true/false"
                            node = obj.Attributes.GetNamedItem("active");
                            if (node != null)
                            {
                                bool temp = true;
                                bool success = bool.TryParse(node.Value, out temp);
                                o.active = success ? temp : true;
                                //o.active = bool.Parse(obj.Attributes.GetNamedItem("active").Value);
                            }
                            else
                            {
                                o.active = true;
                            }

                            //Debug.Log(o.name);

                            // <position x="0.0(m)" y="0.0(m)" z="0.0(m)" >
                            float x = float.Parse(obj.FirstChild.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                            float y = float.Parse(obj.FirstChild.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture.NumberFormat);
                            float z = float.Parse(obj.FirstChild.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture.NumberFormat);

                            o.position = new Vector3(x, y, z);

                            // <rotation x="0.0(degrees)" y="0.0(degrees)" z="0.0(degrees)" >
                            x = float.Parse(obj.ChildNodes[1].Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                            y = float.Parse(obj.ChildNodes[1].Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture.NumberFormat);
                            z = float.Parse(obj.ChildNodes[1].Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture.NumberFormat);

                            o.rotation = new Vector3(x, y, z);

                            //Debug.Log(o.rotation);

                            update.objList.Add(o);
                        }

                        loadedRecording.updates.Add(update);
                    }
                }

                // last recorded update time
                maxTime = loadedRecording.updates[loadedRecording.updates.Count - 1].time;

                ResetPlaybackStart();
            }

            playbackSpeed = 1;

            OnLoadEvent(new LoadEventArgs(recordingMetaData));
        }

        public void LoadEndState()
        {
            ResetScene();

            XmlElement end = (XmlElement)xmlDoc.LastChild.LastChild.FirstChild;

            if (end.HasChildNodes)
            {
                foreach (XmlElement o in end)
                {
                    Debug.Log(o);

                    foreach (GameObject uo in updateList)
                    {
                        if (uo.name == o.Attributes.GetNamedItem("name").Value)
                        {
                            XmlNode node = o.Attributes.GetNamedItem("active");
                            bool active = true;
                            if (node != null)
                            {
                                bool success = bool.TryParse(node.Value, out active);
                                active = success ? active : true;
                                //o.active = bool.Parse(obj.Attributes.GetNamedItem("active").Value);
                            }
                            else
                            {
                                active = true;
                            }

                            uo.SetActive(active);

                            Colourable c = uo.GetComponent<Colourable>();
                            if (c != null)
                            {
                                node = o.Attributes.GetNamedItem("colour");
                                string colour = "Original";
                                bool colourSet = false;
                                if (node != null)
                                {
                                    colour = node.Value;
                                    c.colour = colour;

                                    if (pallet != null)
                                    {
                                        Transform paint = pallet.transform.Find(colour);
                                        if(paint != null)
                                        {
                                            Renderer paintr = paint.GetComponent<Renderer>();
                                            if(paintr != null)
                                            {
                                                c.SetMaterial(paintr.material, colour);
                                                colourSet = true;
                                            }
                                        }
                                    }
                                }
                                
                                if(!colourSet)
                                {
                                    c.ResetColour();
                                }
                            }

                            // <position x="0.0(m)" y="0.0(m)" z="0.0(m)" >
                            float x = float.Parse(o.FirstChild.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                            float y = float.Parse(o.FirstChild.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture.NumberFormat);
                            float z = float.Parse(o.FirstChild.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture.NumberFormat);

                            uo.transform.position = new Vector3(x, y, z);

                            // <rotation x="0.0(degrees)" y="0.0(degrees)" z="0.0(degrees)" >
                            x = float.Parse(o.ChildNodes[1].Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture.NumberFormat);
                            y = float.Parse(o.ChildNodes[1].Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture.NumberFormat);
                            z = float.Parse(o.ChildNodes[1].Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture.NumberFormat);

                            uo.transform.localRotation = Quaternion.Euler(new Vector3(x, y, z));

                            //Deleteable d = uo.GetComponent<Deleteable>();
                            //if (d != null)
                            //{
                            //    if (d.deleted)
                            //    {
                            //        uo.GetComponent<Deleteable>().deleted = false;
                            //        if (sd != null)
                            //        {
                            //            uo.transform.parent = sd.spawner.transform;
                            //        }
                            //        else
                            //        {
                            //            // Not sure how to handle this condition, but currently only documents are deleteable
                            //            uo.transform.parent = null;
                            //        }
                            //    }
                            //}

                            break;
                        }
                    }
                }
            }
        }

        void ResetPlaybackStart()
        {
            currentUpdate = loadedRecording.updates[0];
            nextUpdate = loadedRecording.updates[1];

            updateCount = 1;
            playerTime = 0;

            ResetScene();
            ApplyUpdate();
        }

        void ResetPlaybackEnd()
        {
            currentUpdate = loadedRecording.updates[loadedRecording.updates.Count-1];
            nextUpdate = loadedRecording.updates[loadedRecording.updates.Count-2];

            updateCount = loadedRecording.updates.Count - 1;
            playerTime = maxTime;

            ResetScene();
            ApplyUpdate();
        }

        void ResetScene()
        {
            foreach (GameObject uo in updateList)
            {
                Colourable c = uo.GetComponent<Colourable>();
                if (c != null)
                {
                    c.ResetColour();
                }

                //ColourableTexture ct = uo.GetComponent<ColourableTexture>();
                //if (ct != null)
                //{
                //    uo.GetComponent<Renderer>().material.color = ct.originalColour;
                //}

                SpatialDocument sd = uo.GetComponent<SpatialDocument>();
                //if (sd != null)
                //{
                //    sd.ResetColour();
                //}

                //DiskInteractible di = uo.GetComponent<DiskInteractible>();
                //if (di != null)
                //{
                //    di.ResetMaterials();
                //}

                Deleteable d = uo.GetComponent<Deleteable>();
                if (d != null)
                {
                    if (d.deleted)
                    {
                        uo.GetComponent<Deleteable>().deleted = false;
                        if (sd != null)
                        {
                            uo.transform.parent = sd.spawner.transform;
                        }
                        else
                        {
                            // Not sure how to handle this condition, but currently only documents are deleteable
                            uo.transform.parent = null;
                        }
                    }
                }

                Brush b = uo.GetComponent<Brush>();
                if(b != null)
                {
                    uo.GetComponent<Renderer>().material.color = Color.white;
                }

                //t.gameObject.SetActive(true);
            }
        }

        void UpdatePlaying()
        {
            if (playbackSpeed > 0.0f)
            {
                if (playerTime >= nextUpdate.time)
                {
                    if (++updateCount >= loadedRecording.updates.Count)
                    {
                        updateCount = 0;
                        playerTime = 0;
                        SetState(State.Ended);
                        return;
                    }

                    currentUpdate = nextUpdate;
                    nextUpdate = loadedRecording.updates[updateCount];
                }
            }
            else if(playbackSpeed < 0.0f)
            {
                if (playerTime <= nextUpdate.time)
                {
                    if (--updateCount <= 0)
                    {
                        updateCount = loadedRecording.updates.Count - 1;
                        playerTime = maxTime;
                        SetState(State.Ended);
                        return;
                    }

                    currentUpdate = nextUpdate;
                    nextUpdate = loadedRecording.updates[updateCount];
                }
            }
            else
            {
                return;
            }

            ApplyUpdate();

            playerTime += Time.deltaTime * playbackSpeed;
        }

        void ApplyUpdate()
        {
            foreach (Rec_Object o in currentUpdate.objList)
            {
                //Transform obj = proxyViewer.transform.Find(o.name).transform;
                Rec_Object nexto = null;

                foreach (Rec_Object other in nextUpdate.objList)
                {
                    if (other.name == o.name)
                    {
                        nexto = other;
                        break;
                    }
                }

                if (nexto != null)
                {
                    foreach (GameObject uo in updateList)
                    {
                        if (uo.name == o.name)
                        {
                            uo.SetActive(o.active);
                            float ratio = playerTime / nextUpdate.time;

                            uo.transform.position = Vector3.Slerp(o.position, nexto.position, ratio);
                            uo.transform.localRotation = Quaternion.Euler(Vector3.Slerp(o.rotation, nexto.rotation, ratio));
                        }
                    }
                }

                //if ((obj != null) && (nexto != null))
                //{
                //    obj.gameObject.SetActive(o.active);

                //    float ratio = playerTime / nextUpdate.time;

                //    obj.position = Vector3.Slerp(o.position, nexto.position, ratio);
                //    obj.localRotation = Quaternion.Euler(Vector3.Slerp(o.rotation, nexto.rotation, ratio));
                //}
            }
        }

        public void LoadRecording()
        {
            LoadRecordingFromDisk(Application.persistentDataPath + "/" + fileName + ".xml");
            SetState(State.Load);
        }

        public void LoadRecording(string file)
        {
            fileName = file;
            LoadRecording();
        }

        public void LoadRecordingWeb()
        {
            LoadRecordingWeb(fileName);
        }

        public void LoadRecordingWeb(string uri)
        {
            if(nMan != null)
            {
                nMan.LoadRecording(fileName, OnRecordingDownloaded);
            }
        }

        void OnRecordingDownloaded(string error, string xml)
        {
            if(error == "OK")
            {
                //string path = Application.persistentDataPath + "/download.xml";
                //File.WriteAllText(path, xml);

                //LoadRecordingFromString(CompressionHelper.DecompressString(xml));
// Replace hack to get around incorrect locale xml, also currently require save object to be active to load
                LoadRecordingFromString(xml.Replace(',','.'));
                //LoadRecordingFromDisk(path);
                SetState(State.Load);
            }
        }

        public void Play()
        {
            state = State.Start;
        }

        public void ReversePlayback()
        {
            if (IsState(State.Playing) || IsState(State.Paused))
            {
                if (playbackSpeed >= 0)
                {
                    updateCount -= 2;
                    if (updateCount <= 0)
                    {
                        updateCount = 0;
                        playerTime = 0;
                        SetState(State.Ended);
                    }
                }
                else if (playbackSpeed <= 0)
                {
                    updateCount += 2;
                    if (updateCount >= loadedRecording.updates.Count)
                    {
                        updateCount = loadedRecording.updates.Count - 1;
                        playerTime = maxTime;
                        SetState(State.Ended);
                    }
                }

                playbackSpeed *= -1;

                nextUpdate = loadedRecording.updates[updateCount];
            }
            else if (IsPrevState(State.Ended))
            {
                playbackSpeed *= -1;

                if (playbackSpeed < 0)
                {
                    ResetPlaybackEnd();
                }
                else if (playbackSpeed >= 0)
                {
                    ResetPlaybackStart();
                }
            }
        }

        bool LoadRecordingFromDisk(string path)
        {
            FileStream file;

            if (File.Exists(path)) file = File.OpenRead(path);
            else return false;

            xmlDoc = new XmlDocument();
            xmlDoc.Load(new StreamReader(file));

            file.Flush();
            file.Close();

            Debug.Log(xmlDoc.ChildNodes.Count);

            return true;
        }

        void LoadRecordingFromString(string text)
        {
            xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(text);

            xmlDoc.Load(new StringReader(text));

            Debug.Log(xmlDoc.ChildNodes.Count);
        }

        public bool IsPrevState(State s)
        {
            return prevState == s;
        }

        public bool IsState(State s)
        {
            return state == s;
        }

        public void SetState(State s)
        {
            prevState = state;
            state = s;
        }

        public State GetState()
        {
            return state;
        }
    }
}
