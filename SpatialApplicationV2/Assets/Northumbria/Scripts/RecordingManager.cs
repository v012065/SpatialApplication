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
using System.Text;
using Unity.Jobs;
using System.Threading;

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
    class Rec_Object
    {
        public bool active;
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

    public class RecordingManager : MonoBehaviour
    {
        public enum State
        {
            None,
            Start,
            Recording,
            Paused,
            Ended
        }

        //Start Event
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

        // Recording Event
        public event EventHandler RecordingEvent;
        protected virtual void OnRecordingEvent(EventArgs e)
        {
            EventHandler handler = RecordingEvent;
            handler?.Invoke(this, e);
        }

        public delegate void RecordingEventHandler(object sender, StartEventArgs e);

        public class RecordingEventArgs : EventArgs
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

        public List<GameObject> recordObjects;
        public string fileName;
        public string recordText;
        public State state = State.None;
        public State prevState = State.None;

        public float startTime;
        public float curTime;
        public float nextTick;
        public float tick = 0.1f;

        Recording recording;
        DateTime started;
        public bool savedFile;
        XmlDocument saveDoc;

        public string userID;

        public NetworkManager nMan;
        public bool fileBackup = true;
        public bool serverBackup = true;
        public bool recUpdates = true;
        public bool recUpdateState = true;
        public bool recEndState = true;

        public Text feedbackText;
        public GameObject feedbackButton;
        public GameObject updateButton;
        public bool allowRepeat = true;

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log(Application.persistentDataPath);

            //Debug.Log(OVRManager.systemHeadsetType);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsState(State.Start))
            {
                UpdateRecordingStart();

                SetState(State.Recording);
            }
            else if (IsState(State.Recording))
            {
                if (IsPrevState(State.Start))
                {
                    if (recUpdates)
                    {
                        UpdateRecording();
                    }
                }
                else
                {
                    SetState(State.None);
                }
            }
            else if (IsState(State.Ended))
            {
                if (IsPrevState(State.Recording))
                {
                    UpdateRecordingEnded();
                }

                SetState(State.None);
            }
        }

        void UpdateRecordingStart()
        {
            startTime = Time.time;
            curTime = 0;

            started = DateTime.Now;
            //DateTime now = DateTime.Now;
            //recordText = "<session-record>\n";
            //recordText += "\t<start time=\"" + now.ToLongTimeString() + "\" date =\"" + now.ToShortDateString() + "\" />\n";

            recording = new Recording();
            recording.updates = new List<Rec_Update>();

            savedFile = false;

            if(userID == "") nMan.GetUserID(ServerIDCallback);

            Debug.Log("Recording Started");
        }

        void UpdateRecording()
        {
            if (curTime >= nextTick)
            {
                nextTick = curTime + tick;

                Rec_Update update = new Rec_Update();
                update.objList = new List<Rec_Object>();
                update.time = curTime;

                //recordText += "\t<update seconds=\"" + curTime + "\" >\n";

                foreach (GameObject obj in recordObjects)
                {
                    //recordText += "\t\t<object name=\"" + obj.name + "\" tag=\"" + obj.tag + "\" >\n";
                    //recordText += "\t\t\t<position x=\"" + obj.transform.position.x + "\" " + "y=\"" + obj.transform.position.y + "\" " + "z =\"" + obj.transform.position.z + "\" />\n";
                    //recordText += "\t\t\t<rotation x=\"" + obj.transform.rotation.eulerAngles.x + "\" " + "y=\"" + obj.transform.rotation.eulerAngles.y + "\" " + "z =\"" + obj.transform.rotation.eulerAngles.z + "\" />\n";
                    //recordText += "\t\t</object>\n";

                    Rec_Object rec_obj = new Rec_Object();
                    rec_obj.active = obj.activeInHierarchy;
                    rec_obj.name = obj.name;
                    rec_obj.tag = obj.tag;
                    rec_obj.position = obj.transform.position;
                    rec_obj.rotation = obj.transform.rotation.eulerAngles;

                    update.objList.Add(rec_obj);
                }

                recording.updates.Add(update);
                //recordText += "\t</update>\n";

                //if(OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
                //{
                //    recordText += "\t<action seconds=\"" + (curTime - startTime) + "\" type=\"r_grabbing\" />\n";
                //}
                //else if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
                //{
                //    recordText += "\t<action seconds=\"" + (curTime - startTime) + "\" type=\"l_grabbing\" />\n";
                //}

            }

            curTime += Time.deltaTime;
        }

        void UpdatePaused()
        {

        }

        private void RecEndState()
        {
            XmlElement end = (XmlElement)saveDoc.LastChild.LastChild;
            XmlElement u = saveDoc.CreateElement("update");
            end.AppendChild(u);

            foreach (GameObject obj in recordObjects)
            {
                XmlElement o = saveDoc.CreateElement("object");
                u.AppendChild(o);

                o.SetAttribute("name", obj.name);
                o.SetAttribute("tag", obj.tag);
                o.SetAttribute("active", (obj.activeInHierarchy ? "true" : "false").ToString());
                Colourable c = obj.GetComponent<Colourable>();
                if (c != null)
                {
                    o.SetAttribute("colour", c.GetColourStr());
                }

                XmlElement vector = saveDoc.CreateElement("position");
                o.AppendChild(vector);
                vector.SetAttribute("x", obj.transform.position.x.ToString());
                vector.SetAttribute("y", obj.transform.position.y.ToString());
                vector.SetAttribute("z", obj.transform.position.z.ToString());

                vector = saveDoc.CreateElement("rotation");
                o.AppendChild(vector);
                vector.SetAttribute("x", obj.transform.rotation.x.ToString());
                vector.SetAttribute("y", obj.transform.rotation.y.ToString());
                vector.SetAttribute("z", obj.transform.rotation.z.ToString());

                //Colourable c = obj.GetComponent<Colourable>();
                //ColourableTexture ct = obj.GetComponent<ColourableTexture>();
                //SpatialDocument sd = obj.GetComponent<SpatialDocument>();
                //DiskInteractible di = obj.GetComponent<DiskInteractible>();
                //if (c != null || ct != null || sd != null || di != null)
                //{
                //    Color colour = Color.white;

                //    if (c != null || ct != null)
                //    {
                //        colour = c.GetComponent<Renderer>().material.color;
                //    }
                //    else if (sd != null)
                //    {
                //        colour = sd.GetColour();
                //    }
                //    else if (di != null)
                //    {
                //        colour = di.GetColour();
                //    }

                //    XmlElement col = saveDoc.CreateElement("colour");
                //    o.AppendChild(col);
                //    col.SetAttribute("r", colour.r.ToString());
                //    col.SetAttribute("g", colour.r.ToString());
                //    col.SetAttribute("b", colour.r.ToString());
                //}
            }
        }

        void RecUpdateState()
        {
            XmlElement session = (XmlElement)saveDoc.LastChild;

            foreach (Rec_Update update in recording.updates)
            {
                XmlElement u = saveDoc.CreateElement("update");
                session.AppendChild(u);
                u.SetAttribute("seconds", update.time.ToString());

                foreach (Rec_Object obj in update.objList)
                {
                    XmlElement o = saveDoc.CreateElement("object");
                    u.AppendChild(o);

                    o.SetAttribute("name", obj.name);
                    o.SetAttribute("tag", obj.tag);
                    o.SetAttribute("active", (obj.active ? "true" : "false").ToString());

                    XmlElement vector = saveDoc.CreateElement("position");
                    o.AppendChild(vector);
                    vector.SetAttribute("x", obj.position.x.ToString());
                    vector.SetAttribute("y", obj.position.y.ToString());
                    vector.SetAttribute("z", obj.position.z.ToString());

                    vector = saveDoc.CreateElement("rotation");
                    o.AppendChild(vector);
                    vector.SetAttribute("x", obj.rotation.x.ToString());
                    vector.SetAttribute("y", obj.rotation.y.ToString());
                    vector.SetAttribute("z", obj.rotation.z.ToString());
                }
            }
        }

        void UpdateRecordingEnded()
        {
            //recordText += "\t<end time=\"" + DateTime.Now.ToLongTimeString() + "\" />\n";
            //recordText += "</session-record>";

            Debug.Log("Saving");
            feedbackText.text = "\nSaving.";

            GenerateXMLFull();
            //GenerateBinary();
            //StartCoroutine("GenerateXML");
            //SaveRecordingToDisk(Application.persistentDataPath + "/test.xml");

            //if (recEndState) RecEndState();

            if (serverBackup && nMan.serverExists) SaveRecordingToServer();

            //if (recUpdates && recUpdateState) RecUpdateState();

            //if (!nMan.serverExists)
            //{
            //    feedbackText.text += "\nCould not save data to server.";
            //    fileBackup = true;
            //}
            //else
            //{
            //    // Debug disable if upload
            //    //fileBackup = false;
            //}

            if (fileBackup) SaveFileToDisk();

            Debug.Log("Saved");
        }

        //void SaveFileToDisk()
        //{
        //    string countPath = Application.persistentDataPath + "/count.txt";
        //    int fileInc = 0;
        //    string filePath = "";

        //    if (!serverBackup || !nMan.serverExists)
        //    {
        //        if (File.Exists(countPath))
        //            fileInc = int.Parse(File.ReadAllText(countPath));

        //        filePath = Application.persistentDataPath + "/" + fileName + fileInc + ".xml";
        //    }

        //    if (userID != "")
        //    {
        //        filePath = Application.persistentDataPath + "/" + fileName + "-" + userID + ".xml";
        //    }

        //    SaveRecordingToDisk(filePath);

        //    if (!serverBackup || !nMan.serverExists || userID == "")
        //    {
        //        File.WriteAllText(countPath, (++fileInc).ToString());
        //    }

        //    feedbackText.text += "\nSaved data to local storage.\n" + filePath;
        //}

        void SaveFileToDisk()
        {
            string filePath = Application.persistentDataPath + " / " + fileName;

            //if (!serverBackup || !nMan.serverExists)
            //{
            //    filePath += ".xml";
            //}

            if (userID != "")
            {
                filePath += "-" + userID;// + ".xml";
            }

            filePath += ".xml";

            SaveRecordingToDisk(filePath);

            //feedbackText.text += "\nSaved data to local storage.\n" + filePath;
        }

        //void GenerateBinary()
        //{
        //    string countPath = Application.persistentDataPath + "/count.txt";
        //    int fileInc = 0;

        //    if (File.Exists(countPath))
        //        fileInc = int.Parse(File.ReadAllText(countPath));

        //    string filePath = Application.persistentDataPath + "/" + "binary" + fileInc + ".bin";

        //    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        //    BinaryFormatter bf = new BinaryFormatter();
        //    bf.Serialize(fs, recording);
        //    fs.Close();

        //    File.WriteAllText(countPath, (++fileInc).ToString());

        //    Debug.Log("Saved");

        //    savedFile = true;
        //}

        //IEnumerator GenerateXML()
        //{
        //    Debug.Log("XML Started");

        //    recordText = "<session-record>\n";
        //    recordText += "\t<start time=\"" + started.ToLongTimeString() + "\" date =\"" +started.ToShortDateString() + "\" />\n";

        //    foreach (Rec_Update update in recording.updates)
        //    {
        //        //Debug.Log("XML Object Loop");
        //        recordText += "\t<update seconds=\"" + update.time + "\" >\n";

        //        foreach (Rec_Object obj in update.objList)
        //        {
        //            recordText += "\t\t<object name=\"" + obj.name + "\" tag=\"" + obj.tag + "\" active=\"" + (obj.active ? "true" : "false") + "\" >\n";
        //            recordText += "\t\t\t<position x=\"" + obj.position.x + "\" " + "y=\"" + obj.position.y + "\" " + "z =\"" + obj.position.z + "\" />\n";
        //            recordText += "\t\t\t<rotation x=\"" + obj.rotation.x + "\" " + "y=\"" + obj.rotation.y + "\" " + "z =\"" + obj.rotation.z + "\" />\n";
        //            recordText += "\t\t</object>\n";
        //        }

        //        recordText += "\t</update>\n";

        //        //Debug.Log("XML PAUSED");
        //        yield return null;
        //    }

        //    Debug.Log("XML Finished");

        //    recordText += "\t<end time=\"" + DateTime.Now.ToLongTimeString() + "\" />\n";
        //    recordText += "</session-record>";

        //    SaveRecordingToDisk();
        //}

        void GenerateXML()
        {
            XmlDocument doc = new XmlDocument();

            XmlNode session = doc.CreateElement("session-record");
            doc.AppendChild(session);

            XmlElement start = doc.CreateElement("start");
            session.AppendChild(start);
            start.SetAttribute("time", started.ToLongTimeString());
            start.SetAttribute("date", started.ToShortDateString());
            //start.SetAttribute("system", OVRManager.systemHeadsetType.ToString());
            start.SetAttribute("userid", userID);

            //foreach (Rec_Update update in recording.updates)
            //{
            //    XmlElement u = doc.CreateElement("update");
            //    session.AppendChild(u);
            //    u.SetAttribute("seconds", update.time.ToString());

            //    foreach (Rec_Object obj in update.objList)
            //    {
            //        XmlElement o = doc.CreateElement("object");
            //        u.AppendChild(o);

            //        o.SetAttribute("name", obj.name);
            //        o.SetAttribute("tag", obj.tag);
            //        o.SetAttribute("active", (obj.active ? "true" : "false").ToString());

            //        XmlElement vector = doc.CreateElement("position");
            //        o.AppendChild(vector);
            //        vector.SetAttribute("x", obj.position.x.ToString());
            //        vector.SetAttribute("y", obj.position.y.ToString());
            //        vector.SetAttribute("z", obj.position.z.ToString());

            //        vector = doc.CreateElement("rotation");
            //        o.AppendChild(vector);
            //        vector.SetAttribute("x", obj.rotation.x.ToString());
            //        vector.SetAttribute("y", obj.rotation.y.ToString());
            //        vector.SetAttribute("z", obj.rotation.z.ToString());
            //    }
            //}

            XmlElement end = doc.CreateElement("end");
            session.AppendChild(end);
            end.SetAttribute("time", DateTime.Now.ToLongTimeString());
            end.SetAttribute("end_state", recEndState ? "true" : "false");

            saveDoc = doc;
        }

        void GenerateXMLFull()
        {
            saveDoc = new XmlDocument();

            XmlNode session = saveDoc.CreateElement("session-record");
            saveDoc.AppendChild(session);

            XmlElement start = saveDoc.CreateElement("start");
            session.AppendChild(start);
            start.SetAttribute("time", started.ToLongTimeString());
            start.SetAttribute("date", started.ToShortDateString());
            //start.SetAttribute("system", OVRManager.systemHeadsetType.ToString());
            start.SetAttribute("userid", userID);

            if (recUpdates && recUpdateState)
            {
                foreach (Rec_Update update in recording.updates)
                {
                    XmlElement u = saveDoc.CreateElement("update");
                    session.AppendChild(u);
                    u.SetAttribute("seconds", update.time.ToString());

                    foreach (Rec_Object obj in update.objList)
                    {
                        XmlElement o = saveDoc.CreateElement("object");
                        u.AppendChild(o);

                        o.SetAttribute("name", obj.name);
                        o.SetAttribute("tag", obj.tag);
                        o.SetAttribute("active", (obj.active ? "true" : "false").ToString());

                        XmlElement vector = saveDoc.CreateElement("position");
                        o.AppendChild(vector);
                        vector.SetAttribute("x", obj.position.x.ToString());
                        vector.SetAttribute("y", obj.position.y.ToString());
                        vector.SetAttribute("z", obj.position.z.ToString());

                        vector = saveDoc.CreateElement("rotation");
                        o.AppendChild(vector);
                        vector.SetAttribute("x", obj.rotation.x.ToString());
                        vector.SetAttribute("y", obj.rotation.y.ToString());
                        vector.SetAttribute("z", obj.rotation.z.ToString());
                    }
                }
            }

            XmlElement end = saveDoc.CreateElement("end");
            session.AppendChild(end);
            end.SetAttribute("time", DateTime.Now.ToLongTimeString());
            end.SetAttribute("end_state", recEndState ? "true" : "false");

            if (recEndState)
            {
                XmlElement u = saveDoc.CreateElement("update");
                end.AppendChild(u);

                foreach (GameObject obj in recordObjects)
                {
                    XmlElement o = saveDoc.CreateElement("object");
                    u.AppendChild(o);

                    o.SetAttribute("name", obj.name);
                    o.SetAttribute("tag", obj.tag);
                    o.SetAttribute("active", (obj.activeInHierarchy ? "true" : "false").ToString());
                    Colourable c = obj.GetComponent<Colourable>();
                    if (c != null)
                    {
                        o.SetAttribute("colour", c.GetColourStr());
                    }

                    XmlElement vector = saveDoc.CreateElement("position");
                    o.AppendChild(vector);
                    vector.SetAttribute("x", obj.transform.position.x.ToString());
                    vector.SetAttribute("y", obj.transform.position.y.ToString());
                    vector.SetAttribute("z", obj.transform.position.z.ToString());

                    vector = saveDoc.CreateElement("rotation");
                    o.AppendChild(vector);
                    vector.SetAttribute("x", obj.transform.rotation.x.ToString());
                    vector.SetAttribute("y", obj.transform.rotation.y.ToString());
                    vector.SetAttribute("z", obj.transform.rotation.z.ToString());
                }
            }
        }

        void SaveRecordingToDisk(string path)
        {
            FileStream file = File.OpenWrite(path);

            saveDoc.Save(new StreamWriter(file));

            file.Flush();
            file.Close();

            savedFile = true;
        }

        void SaveStringToDisk(string path, string message)
        {
            File.WriteAllText(path, message);
        }

        void ServerCallback(string error, string text)
        {
            if (error == "OK")
            {
                if (text == "false")
                {
                    feedbackText.text = "Server could not overwrite recording.\nUser ID: " + userID;
                }
                else if (text == "true")
                {
                    feedbackText.text = "Experiment Re-do Complete.\nUser ID: " + userID + "\nPlease visit https://tinyurl.com/y5m4b4n3 or click the survey button (left) to fill in the questionnaire.";
                    feedbackButton?.SetActive(true);
                    //updateButton?.SetActive(true);
                }
                else
                {
                    userID = text;
                    feedbackText.text = "Experiment Complete.\nUser ID: " + userID + "\nPlease visit https://tinyurl.com/y5m4b4n3 or click the survey button (left) to fill in the questionnaire.";
                    SaveStringToDisk(Application.persistentDataPath + "/userid", userID);
                    feedbackButton?.SetActive(true);
                    //updateButton?.SetActive(true);
                }
            }
        }

        void ServerIDCallback(string error, string text)
        {
            if (error == "OK")
            {
                userID = text;
                Debug.Log(userID);
            }
        }

        void SaveRecordingToServer()
        {
            if(nMan != null)
            {
                //string temp = "";

                //if (savedFile)
                //{
                //    string countPath = Application.persistentDataPath + "/count.txt";
                //    int fileInc = 0;

                //    if (File.Exists(countPath))
                //        fileInc = int.Parse(File.ReadAllText(countPath));

                //    string filePath = Application.persistentDataPath + "/" + fileName + (fileInc - 1) + ".xml";

                //    temp = File.ReadAllText(filePath);
                //}

                //nMan.SaveRecordingToServer(temp);

                StringBuilder sb = new StringBuilder();
                saveDoc.Save(new StringWriter(sb));

                //nMan.SaveRecordingToServer(CompressionHelper.CompressString(sb.ToString()));

                feedbackText.text += "\nUploading Data.\nUser ID: " + userID;

                if (allowRepeat)
                {
                    nMan.OverwriteRecordingOnServer(userID, sb.ToString(), ServerCallback);
                }
                else if (userID == "")
                {
                    nMan.SaveRecordingToServer(sb.ToString(), ServerCallback);
                }
                else
                {
                    nMan.SaveRecordingToServer(userID, sb.ToString(), ServerCallback);
                }

                //nMan.SaveRecordingToServer(saveDoc.InnerXml);
            }
        }

        void ServerUpdateCallback(string error, string text)
        {
            if (text == "false")
            {
                feedbackText.text = "Server could not upload full recording.\nUser ID: " + userID + "\nPlease visit https://tinyurl.com/y5m4b4n3 or click the survey button (left) to fill in the questionnaire.";
                feedbackButton?.SetActive(true);
            }
            else if (text == "true")
            {
                feedbackText.text = "Thank you for submitting.\nUser ID: " + userID + "\nPlease visit https://tinyurl.com/y5m4b4n3 or click the survey button (left) to fill in the questionnaire.";
                feedbackButton?.SetActive(false);
            }
        }

        public void UpdateServerRecording()
        {
            string file = File.ReadAllText(Application.persistentDataPath + "/" + fileName + "-" + userID + ".xml");

            nMan.OverwriteRecordingOnServer(userID, file, ServerCallback);
        }

        //void SaveRecordingToDisk()
        //{
        //    Debug.Log(recordText);

        //    string countPath = Application.persistentDataPath + "/count.txt";
        //    int fileInc = 0;

        //    if (File.Exists(countPath))
        //        fileInc = int.Parse(File.ReadAllText(countPath));

        //    string filePath = Application.persistentDataPath + "/" + fileName + fileInc + ".xml";

        //    File.WriteAllText(filePath, recordText);

        //    File.WriteAllText(countPath, (++fileInc).ToString());

        //    Debug.Log("Saved");

        //    savedFile = true;
        //}

        public void StartRecording()
        {
            if (IsState(State.None))
            {
                SetState(State.Start);
            }
        }

        public void EndRecording()
        {
            if (IsState(State.Recording))
            {
                SetState(State.Ended);
            }
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
