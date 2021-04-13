using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Onboarding : MonoBehaviour
{
    public Text screenText;
    //public GameObject [] screens;
    public int currentScreen;
    public OnboardingButton next;
    public OnboardingButton back;
    public OnboardingButton start;
    //public GameObject experimentScene;
    public SceneSwitcher sceneSwitcher;
    public Northumbria.RecordingManager rMan;
    public TextAsset screenFile;
    public string[] screens;
    public int specialScreenOffset;
    public GameObject consent;
    public bool consentAccepted = false;
    public GameObject surveyButton;

    void ServerCheckDoOverCallback(string error, string s)
    {
        if (error == "OK")
        {
            if (s == rMan.userID)
            {
                rMan.allowRepeat = true;
                return;
            }
        }

        Debug.Log(error + " : " + s);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/do-over") && !rMan.allowRepeat)
        {
            if(File.ReadAllText(Application.persistentDataPath + "/do-over") == "1619631")
            {
                rMan.allowRepeat = true;
                rMan.userID = File.ReadAllText(Application.persistentDataPath + "/userid");
            }
        }
        else if (File.Exists(Application.persistentDataPath + "/userid") && !rMan.allowRepeat)
        {
            rMan.userID = File.ReadAllText(Application.persistentDataPath + "/userid");
            screenText.text = "You have already participated in this experiment.\nYour UserID is " + rMan.userID + "\nPlease fill in the questionnaire if you haven't already at https://tinyurl.com/y5m4b4n3 or by clicking the survey button (left)";

            next.gameObject.SetActive(false);
            start.gameObject.SetActive(false);
            consent.gameObject.SetActive(false);
            back.gameObject.SetActive(false);

            surveyButton.SetActive(true);

            return;
        }

        if (File.Exists(Application.persistentDataPath + "/full"))
        {
            if (File.ReadAllText(Application.persistentDataPath + "/full") == "8128471")
            {
                rMan.recUpdates = true;
                rMan.recUpdateState = true;
            }
        }
        if (File.Exists(Application.persistentDataPath + "/backup") && !rMan.fileBackup)
        {
            if (File.ReadAllText(Application.persistentDataPath + "/backup") == "5467575")
            {
                rMan.fileBackup = true;
            }
        }

        //currentScreen = 0;
        screens = screenFile.text.Split('\n');

        //experimentScene.SetActive(false);
        start.gameObject.SetActive(false);
        consent.gameObject.SetActive(false);

        if (currentScreen > screens.Length - specialScreenOffset - 1)
        {
            currentScreen = screens.Length - specialScreenOffset - 1;
            next.gameObject.SetActive(false);
            start.gameObject.SetActive(true);
            consent.gameObject.SetActive(true);
        }
        else if (currentScreen <= 0)
        {
            currentScreen = 0;
            back.gameObject.SetActive(false);
        }

        UpdateScreens();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        RaycastHit hit;
    //        Ray forwardRay = Camera.main.ScreenPointToRay(Input.mousePosition);

    //        if (Physics.Raycast(forwardRay, out hit, Camera.main.farClipPlane))
    //        {
    //            OnboardingButton b = hit.collider.GetComponent<OnboardingButton>();
    //            if (b != null)
    //            {
    //                switch (b.type)
    //                {
    //                    case OnboardingButton.Type.Next:
    //                        b.onboarding.Next();
    //                        break;
    //                    case OnboardingButton.Type.Back:
    //                        b.onboarding.Back();
    //                        break;
    //                    case OnboardingButton.Type.Start:
    //                        b.onboarding.Starting();
    //                        break;
    //                    case OnboardingButton.Type.AcceptConsent:
    //                        b.onboarding.Consent(true);
    //                        break;
    //                    case OnboardingButton.Type.DeclineConsent:
    //                        b.onboarding.Consent(false);
    //                        break;
    //                    default: break;
    //                }
    //            }
    //        }
    //    }
    //}

    void UpdateScreens()
    {
        //foreach (GameObject screen in screens)
        //{
        //    screen.SetActive(false);
        //}

        //screens[currentScreen].SetActive(true);

        screenText.text = screens[currentScreen];
    }

    public void Next()
    {
        if(++currentScreen >= screens.Length - 1 - specialScreenOffset)
        {
            currentScreen = screens.Length - 1 - specialScreenOffset;
            next.gameObject.SetActive(false);
            //start.gameObject.SetActive(true);
            consent.gameObject.SetActive(true);
        }

        back.gameObject.SetActive(true);

        UpdateScreens();
    }

    public void Back()
    {
        if (--currentScreen <= 0)
        {
            currentScreen = 0;
            back.gameObject.SetActive(false);
        }
        else if(currentScreen < screens.Length - 1 - specialScreenOffset)
        {
            start.gameObject.SetActive(false);
            consent.gameObject.SetActive(false);
        }

        next.gameObject.SetActive(true);

        UpdateScreens();
    }

    public void Starting()
    {
        if (consentAccepted)
        {
            //experimentScene.SetActive(true);
            sceneSwitcher.SetActiveScene(2);
            transform.parent.gameObject.SetActive(false);

            if (rMan != null)
            {
                rMan.StartRecording();
            }
        }
    }

    public void Consent(bool accept)
    {
        consentAccepted = accept;
        consent.SetActive(false);

        if(consentAccepted)
        {
            screenText.text = screens[screens.Length - specialScreenOffset];
            start.gameObject.SetActive(true);
        }
        else
        {
            screenText.text = screens[screens.Length - 1];
        }
    }
}
