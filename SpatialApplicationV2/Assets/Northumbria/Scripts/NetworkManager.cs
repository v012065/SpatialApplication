using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

namespace Northumbria
{
    public class NetworkManager : MonoBehaviour
    {
        public string serverAddress;
        public string uploadEndPoint;
        public string downloadEndPoint;
        public string checkEndPoint;
        public string overwriteEndPoint;
        public string idGenEndPoint;
        public string doOverEndPoint;

        public bool serverExists = false;

        public delegate void DownloadCallback(string error, string s);
        public delegate void UploadCallback(string error, string s);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Get(serverAddress + "/" + checkEndPoint, ServerCheckCallback));
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ServerCheckCallback(string error, string s)
        {
            if(error == "OK")
            {
                if(s == "Hello")
                {
                    serverExists = true;
                    return;
                }
            }

            serverExists = false;
            Debug.Log(error + " : " + s);
        }

        public void CheckDoOver(DownloadCallback callback)
        {
            string uri = serverAddress + "/" + doOverEndPoint;

            StartCoroutine(Get(uri, callback));
        }

        public void GetUserID(DownloadCallback callback)
        {
            string uri = serverAddress + "/" + idGenEndPoint;

            StartCoroutine(Get(uri, callback));
        }

        public void SaveRecordingToServer(string xml, UploadCallback callback = null)
        {
            string uri = serverAddress + "/" + uploadEndPoint;

            StartCoroutine(PutString(uri, xml, callback));
        }

        public void SaveRecordingToServer(string userID, string xml, UploadCallback callback = null)
        {
            string uri = serverAddress + "/" + uploadEndPoint;

            string data = userID + "\n" + xml;

            Debug.Log(data);

            StartCoroutine(PutString(uri, data, callback));
        }

        public void OverwriteRecordingOnServer(string userID, string xml, UploadCallback callback = null)
        {
            string uri = serverAddress + "/" + overwriteEndPoint;

            string data = userID + "\n" + xml;

            Debug.Log(data);

            StartCoroutine(PutString(uri, data, callback));
        }

        IEnumerator PutString(string uri, string s, UploadCallback callback=null)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, s))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);

                    callback?.Invoke(webRequest.error, "");
                }
                else
                {
                    Debug.Log(pages[page] + ": Success: " + webRequest.responseCode);
                    Debug.Log(webRequest.downloadHandler.text);

                    callback?.Invoke("OK", webRequest.downloadHandler.text);
                }
            }
        }

        public void LoadRecording(string fileName, DownloadCallback callback)
        {
            string uri = serverAddress + "/" + downloadEndPoint + "/" + fileName + ".xml";

            // A correct website page.
            StartCoroutine(Get(uri, callback));
        }

        IEnumerator Get(string uri, DownloadCallback callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);

                    callback?.Invoke(webRequest.error, "");
                }
                else
                {
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    callback?.Invoke("OK", webRequest.downloadHandler.text);
                }
            }
        }
    }
}
