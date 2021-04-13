using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMessage : MonoBehaviour
{
    public Text screenText;
    public int currentScreen;
    public InstructionsButton next;
    public InstructionsButton back;
    public TextAsset screenFile;
    public string[] screens;

    // Start is called before the first frame update
    void Start()
    {
        screens = screenFile.text.Split('\n');

        if (currentScreen > screens.Length - 1)
        {
            currentScreen = screens.Length - 1;
            next.gameObject.SetActive(false);
        }
        else if (currentScreen <= 0)
        {
            currentScreen = 0;
            back.gameObject.SetActive(false);
        }

        UpdateScreens();
    }

    void UpdateScreens()
    {
        screenText.text = screens[currentScreen];
    }

    public void Next()
    {
        if(++currentScreen >= screens.Length - 1)
        {
            currentScreen = screens.Length - 1;
            next.gameObject.SetActive(false);
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

        next.gameObject.SetActive(true);

        UpdateScreens();
    }
}
