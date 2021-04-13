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

public class RecordManager : MonoBehaviour
{
    //[XmlRoot(ElementName = "start")]
    //public class Rec_Start
    //{
    //    [XmlAttribute(AttributeName = "time")]
    //    public string Time { get; set; }
    //    [XmlAttribute(AttributeName = "date")]
    //    public string Date { get; set; }
    //}

    //[XmlRoot(ElementName = "position")]
    //public class Position
    //{
    //    [XmlAttribute(AttributeName = "x")]
    //    public string X { get; set; }
    //    [XmlAttribute(AttributeName = "y")]
    //    public string Y { get; set; }
    //    [XmlAttribute(AttributeName = "z")]
    //    public string Z { get; set; }
    //}

    //[XmlRoot(ElementName = "rotation")]
    //public class Rotation
    //{
    //    [XmlAttribute(AttributeName = "x")]
    //    public string X { get; set; }
    //    [XmlAttribute(AttributeName = "y")]
    //    public string Y { get; set; }
    //    [XmlAttribute(AttributeName = "z")]
    //    public string Z { get; set; }
    //}

    //[XmlRoot(ElementName = "object")]
    //public class Object
    //{
    //    [XmlElement(ElementName = "position")]
    //    public Position Position { get; set; }
    //    [XmlElement(ElementName = "rotation")]
    //    public Rotation Rotation { get; set; }
    //    [XmlAttribute(AttributeName = "name")]
    //    public string Name { get; set; }
    //    [XmlAttribute(AttributeName = "tag")]
    //    public string Tag { get; set; }
    //}

    //[XmlRoot(ElementName = "update")]
    //public class Rec_Update
    //{
    //    [XmlElement(ElementName = "object")]
    //    public List<Object> Object { get; set; }
    //    [XmlAttribute(AttributeName = "seconds")]
    //    public string Seconds { get; set; }
    //}

    //[XmlRoot(ElementName = "end")]
    //public class End
    //{
    //    [XmlAttribute(AttributeName = "time")]
    //    public string Time { get; set; }
    //}

    //[XmlRoot(ElementName = "session-record")]
    //public class SessionRecord
    //{
    //    [XmlElement(ElementName = "start")]
    //    public Rec_Start Start { get; set; }
    //    [XmlElement(ElementName = "update")]
    //    public List<Rec_Update> Update { get; set; }
    //    [XmlElement(ElementName = "end")]
    //    public End End { get; set; }
    //}

    class Rec_Object
    {
        public string name;
        public string tag;
        public Vector3 position;
        public Vector3 rotation;
    }

    class Rec_Update
    {
        public float time;
        public List<Rec_Object> objList;
    }

    class Recording
    {
        public List<Rec_Update> updates;
    }

    public enum State
    {
        None,
        RecordingStart,
        Recording,
        Load,
        PlayStart,
        Playing,
        Paused,
        Ended
    }

    public List<GameObject> recordObjects;
    public GameObject proxyViewer;
    public string fileName;
    public string recordText;
    public State state = State.None;
    public State prevState = State.None;

    public float startTime;
    public float curTime;
    public float nextTick;
    public float tick = 2;

    public string loadFile;
    public float playerTime;
    public float playbackSpeed;
    public bool scrubbing;
    public float maxTime = 1f;
    public int updateCount;
    Rec_Update currentUpdate;
    Rec_Update nextUpdate;

    XmlDocument xmlDoc;
    Recording loadedRecording;
    public Text uiText;
    public Slider uiSlider;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        curTime = Time.time;

        PressButton();

        if (state == State.RecordingStart)
        {
            startTime = curTime;
            DateTime now = DateTime.Now;
            recordText = "<session-record>\n";
            recordText += "\t<start time=\"" + now.ToLongTimeString() + "\" date =\"" + now.ToShortDateString() + "\" />\n";

            prevState = state;
            state = State.Recording;
        }
        else if (state == State.Recording)
        {
            if (prevState == State.RecordingStart)
            {
                if (curTime >= nextTick)
                {
                    nextTick = curTime + tick;

                    recordText += "\t<update seconds=\"" + (curTime - startTime) + "\" >\n";

                    foreach (GameObject obj in recordObjects)
                    {
                        recordText += "\t\t<object name=\"" + obj.name + "\" tag=\"" + obj.tag + "\" >\n";
                        recordText += "\t\t\t<position x=\"" + obj.transform.position.x + "\" " + "y=\"" + obj.transform.position.y + "\" " + "z =\"" + obj.transform.position.z + "\" />\n";
                        recordText += "\t\t\t<rotation x=\"" + obj.transform.rotation.eulerAngles.x + "\" " + "y=\"" + obj.transform.rotation.eulerAngles.y + "\" " + "z =\"" + obj.transform.rotation.eulerAngles.z + "\" />\n";
                        recordText += "\t\t</object>\n";
                    }

                    recordText += "\t</update>\n";
                }
            }
            else
            {
                prevState = state;
                state = State.None;
            }
        }
        else if (state == State.Ended)
        {
            if (prevState == State.Recording)
            {
                recordText += "\t<end time=\"" + DateTime.Now.ToLongTimeString() + "\" />\n";
                recordText += "</session-record>";

                Debug.Log("Saving");

                SaveRecordingToDisk();

                Debug.Log("Saved");
            }
            if(prevState == State.Playing)
            {
                    
            }

            prevState = state;
            state = State.None;
        }
        else if (state == State.Load)
        {
            LoadRecordingFromDisk(Application.persistentDataPath + "/" + loadFile + ".xml");

            loadedRecording = new Recording();
            loadedRecording.updates = new List<Rec_Update>();

            // <start time="00:00:00" >
            uiText.text = "Start Time: " + xmlDoc.FirstChild.FirstChild.Attributes.GetNamedItem("time").Value;
            // <start date="DD/MM/YYYY" >
            uiText.text += "\nStart Date: " + xmlDoc.FirstChild.FirstChild.Attributes.GetNamedItem("date").Value;
            // <end time="00:00:00" >
            uiText.text += "\nEnd Time: " + xmlDoc.FirstChild.LastChild.Attributes.GetNamedItem("time").Value;

            for (int i = 1; i < xmlDoc.FirstChild.ChildNodes.Count - 1; ++i)
            {
                if (xmlDoc.FirstChild.ChildNodes[i].Name == "update")
                {
                    Rec_Update update = new Rec_Update();

                    // <update seconds="0.0(s)" >
                    update.time = float.Parse(xmlDoc.FirstChild.ChildNodes[i].Attributes.GetNamedItem("seconds").Value, CultureInfo.InvariantCulture.NumberFormat);
                    update.objList = new List<Rec_Object>();

                    foreach (XmlNode obj in xmlDoc.FirstChild.ChildNodes[i])
                    {
                        Rec_Object o = new Rec_Object();
                        // <object name="" >
                        o.name = obj.Attributes.GetNamedItem("name").Value;

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

            prevState = state;
            state = State.None;
        }
        else if (state == State.PlayStart)
        {
            if (prevState == State.Load || prevState == State.Ended)
            {
                currentUpdate = loadedRecording.updates[0];
                nextUpdate = loadedRecording.updates[1];

                updateCount = 1;
                playerTime = 0;
                startTime = curTime;

                prevState = state;
                state = State.Playing;
            }
            else if(prevState == State.Paused)
            {
                prevState = state;
                state = State.Playing;
            }
            else
            {
                state = State.None;
            }
        }
        else if (state == State.Playing)
        {
            if (prevState == State.PlayStart)
            {
                //UpdateSlider();

                if (playerTime >= nextUpdate.time)
                {
                    if (++updateCount >= loadedRecording.updates.Count)
                    {
                        prevState = state;
                        state = State.Ended;
                        return;
                    }

                    currentUpdate = nextUpdate;
                    nextUpdate = loadedRecording.updates[updateCount];
                }

                foreach (Rec_Object o in currentUpdate.objList)
                {
                    Transform obj = proxyViewer.transform.Find(o.name).transform;
                    Rec_Object nexto = null;

                    foreach (Rec_Object other in nextUpdate.objList)
                    {
                        if (other.name == o.name)
                        {
                            nexto = other;
                            break;
                        }
                    }

                    if ((obj != null) && (nexto != null))
                    {
                        float ratio = playerTime / nextUpdate.time;

                        obj.position = Vector3.Slerp(o.position, nexto.position, ratio);
                        //obj.rotation = Quaternion.SlerpUnclamped(Quaternion.Euler(o.rotation), Quaternion.Euler(o.rotation), ratio);
                        obj.localRotation = Quaternion.Euler(Vector3.Slerp(o.rotation, o.rotation, ratio));
                        //obj.rotation = Quaternion.Euler(o.rotation);

                        if(obj.name == "Head")
                        {
                            curPlayPos = obj.position;
                            curPlayRot = Vector3.Slerp(o.rotation, o.rotation, ratio);
                        }
                    }
                }

                playerTime += Time.deltaTime * playbackSpeed;

            }
            else
            {
                state = State.None;
            }
        }
    }

    public Vector3 curPlayPos;
    public Vector3 curPlayRot;

    private void FixedUpdate()
    {
        
    }

    //private void LateUpdate()
    //{
    //    if (state == State.Playing)
    //    {
    //        if (prevState == State.PlayStart)
    //        {
    //            UpdateSlider();
    //        }
    //    }
    //}

    public void UpdateSlider()
    {
        uiSlider.value = playerTime / maxTime;
    }

    void SaveRecordingToDisk()
    {
        Debug.Log(recordText);

        string countPath = Application.persistentDataPath + "/count.txt";
        int fileInc = 0;

        if (File.Exists(countPath))
            fileInc = int.Parse(File.ReadAllText(countPath));

        string filePath = Application.persistentDataPath + "/" + fileName + fileInc + ".xml";

        File.AppendAllText(filePath, recordText);

        File.WriteAllText(countPath, (++fileInc).ToString());

        //FileStream file;

        //if (File.Exists(filePath)) file = File.Open(filePath, FileMode.Truncate);
        //else file = File.Create(filePath);

        ////BinaryFormatter bf = new BinaryFormatter();
        ////bf.Serialize(file, recordText);

        //StreamWriter writer = new StreamWriter(file);

        //writer.Write(recordText);
        //file.Flush();
        //file.Close();
    }

    private void OnApplicationQuit()
    {

    }

    public void OnLoadRecording()
    {
        state = State.Load;
    }

    public void OnPlay()
    {
        if (scrubbing)
        {


            scrubbing = false;
        }

        if (prevState == State.Load || prevState == State.Ended)
        {
            state = State.PlayStart;
        }
        else if(state == State.Paused)
        {
            prevState = state;
            state = State.PlayStart;
        }
    }

    public void OnPause()
    {
        if (state == State.Playing)
        {
            prevState = state;
            state = State.Paused;
        }
    }

    public void OnStop()
    {
        if(state == State.Playing)
        {
            prevState = state;
            state = State.Ended;
        }
    }

    public void OnMoveTimeline(float ratio)
    {
        playerTime = ratio * maxTime;

        scrubbing = true;

        prevState = state;
        state = State.Paused;
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

    public void StartRecording()
    {
        state = State.Recording;
    }

    public void EndRecording()
    {
        if (state == State.Recording)
        {
            state = State.Ended;
        }
    }

    public bool pressed;

    void PressButton()
    {
        //if(OVRInput.GetDown(OVRInput.Button.Start))
        if(OVRInput.Get(OVRInput.Button.Start))
        {
            if (!pressed)
            {
                Debug.Log("Start Button");

                if (state == State.None)
                {
                    prevState = state;
                    state = State.RecordingStart;
                }
                if (state == State.Recording)
                {
                    prevState = state;
                    state = State.Ended;
                }

                pressed = true;
            }
        }
        else
        {
            pressed = false;
        }
    }
}
