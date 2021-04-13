using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class EmailLoader : MonoBehaviour
{
    //public string emailFile;
    public bool loadOnStart = true;
    public GameObject docPrefab;
    public Vector3 spawnStartPos;
    public Northumbria.RecordingManager rMan;
    //public Transform proxyViewer;
    public Northumbria.PlaybackManager pMan;
    public TextAsset emailXml;
    public string imageFolder;
    public int imageNum;

    public GameObject endButton;

    XmlDocument xmlDoc;

    // Start is called before the first frame update
    void Start()
    {
        if(loadOnStart)
        {
            LoadXML();
            //SpawnDocuments();
            SpawnDocumentsImage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // https://stackoverflow.com/questions/30224723/can-not-read-xml-document-containing-ampersand-symbol
    void LoadXML()
    {
        //if (File.Exists(Application.dataPath + "/" + emailFile))
        //{
            //var xmlContent = File.ReadAllText(Application.dataPath + "/" + emailFile);
            //FileStream file;

            //file = File.OpenRead(emailFile);

            xmlDoc = new XmlDocument();
            //xmlDoc.Load(new StreamReader(file));
            //xmlDoc.LoadXml(xmlContent.Replace("&", "&amp;"));
        xmlDoc.LoadXml(emailXml.text.Replace("&", "&amp;"));

        //file.Flush();
        //file.Close();
        //}

        //Debug.Log(xmlDoc.FirstChild.Name);
    }

    void SpawnDocuments()
    {
        Vector3 pos = spawnStartPos;
        string title = "";
        string date = "";
        string time = "";
        string from = "";
        string to = "";
        string body = "";
        string emailString = "";
        int count = 0;

        foreach (XmlNode email in xmlDoc.FirstChild)
        {
            title = email.Attributes.GetNamedItem("title").Value;

            //Debug.Log(email.ChildNodes.Count);

            foreach (XmlNode node in email.ChildNodes)
            {
                //Debug.Log(node.Name);

                if (node.Name == "received")
                {
                    date = node.Attributes.GetNamedItem("date").Value;
                    time = node.Attributes.GetNamedItem("time").Value;
                }
                else if (node.Name == "from")
                {
                    from = node.InnerText;
                }
                else if (node.Name == "to")
                {
                    to = node.InnerText;
                }
                else if (node.Name == "body")
                {
                    body = node.InnerText;
                }
            }

            /*
            <size=51>Internship for MSc student?</size>
            <size=30><color=#0B0080>
            From: eduard.donald@usoo.ac.uk

            To: computab@usoo.ac.uk</color>
            </size><size=21>

            Hello,

            Does anybody know of an MSc student or similar that would be interested in an internship at the moment? It would inolve working with IoT (Arduino dev and the like) for the construction industry (tracking heavy machine use, ect.).

            Cheers,

            Eduard
            </size>
            */
            emailString = "<size=51>" + title + "</size>\n"
                + "<size=30>" + date + " " + time + "\n<color=#0B0080>\nFrom: " + from
                + "\nTo: " + to + "</color>\n</size><size=21>\n\n"
                + body + "\n</size>";

            GameObject newEmail = Instantiate(docPrefab, transform);
            newEmail.transform.position = pos;
            newEmail.transform.Rotate(-90, 0, 0);
            newEmail.GetComponent<SpatialDocument>().documentText.text = emailString;
            newEmail.GetComponent<SpatialDocument>().spawner = this;
            newEmail.SetActive(true);
            newEmail.name = "Document" + (++count);
            rMan.recordObjects.Add(newEmail);
            //if(proxyViewer.gameObject.activeInHierarchy)
            //{
            //    newEmail.transform.parent = proxyViewer;
            //}
            if(pMan.gameObject.activeInHierarchy)
            {
                pMan.updateList.Add(newEmail);
            }

            // move along
            float x = pos.x - 0.1f;
            float z = pos.z;
            if(x <= 17.9)
            {
                x = spawnStartPos.x;
                z += 0.1f;
            }
            pos = new Vector3(x, pos.y, z);
        }
    }

    void SpawnDocumentsImage()
    {
        Vector3 pos = spawnStartPos;
        int count = 0;

        for (int i = 0; i < imageNum; ++i)
        {
            GameObject newEmail = Instantiate(docPrefab, transform);
            //newEmail.transform.position = pos;
            newEmail.transform.Rotate(-90, 0, 0);
            newEmail.GetComponent<SpatialDocument>().documentText.text = "";
            newEmail.GetComponent<SpatialDocument>().spawner = this;
            newEmail.SetActive(true);
            GameObject doc = newEmail.transform.GetChild(2).gameObject;
            doc.SetActive(true);
            Image im = doc.GetComponentInChildren<Image>();
            im.sprite = Resources.Load<Sprite>(imageFolder + "/emails-" + (i + 1));
            doc.SetActive(false);
            newEmail.name = "Document" + (++count);
            rMan.recordObjects.Add(newEmail);

            if (pMan.gameObject.activeInHierarchy)
            {
                pMan.updateList.Add(newEmail);
            }

            //// move along
            //float x = pos.x - 0.1f;
            //float z = pos.z;
            //if (x <= 17.9)
            //{
            //    x = spawnStartPos.x;
            //    z += 0.1f;
            //}
            //pos = new Vector3(x, pos.y, z);
        }

        // Randomise order of documents and place on desk
        //int[] order = new int[imageNum];
        //for (int i = 0; i < imageNum; i++)
        //{
        //    bool found = false;
        //    do
        //    {
        //        found = false;
        //        order[i] = Random.Range(0, imageNum - 1);
        //        for (int j = 0; j < i; j++)
        //        {
        //            if (order[i] == order[j])
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //    }
        //    while (found);
        //}

        // Randomise order of documents and place on desk
        // https://stackoverflow.com/questions/30014901/generating-random-numbers-without-repeating-c

        List<int> possible = Enumerable.Range(0, imageNum).ToList();
        List<int> order = new List<int>();
        for (int i = 0; i < imageNum; i++)
        {
            int index = Random.Range(0, possible.Count);
            order.Add(possible[index]);
            possible.RemoveAt(index);
        }

        for (int i = 0; i < imageNum; ++i)
        {
            GameObject email = transform.GetChild(order[i]).gameObject;
            email.transform.position = pos;

            // move along
            float x = pos.x - 0.1f;
            float z = pos.z;
            if (x <= 17.9)
            {
                x = spawnStartPos.x;
                z += 0.1f;
            }
            pos = new Vector3(x, pos.y, z);
        }
    }

    public void CheckAllInteracted()
    {
        for (int i = 0; i < imageNum; ++i)
        {
            GameObject email = transform.GetChild(i).gameObject;
            if(!email.GetComponent<SpatialDocument>().interacted)
            {
                return;
            }
        }

        endButton.SetActive(true);
    }
}
