using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingButton : MonoBehaviour
{
    public enum Type
    {
        Back,
        Next,
        Start,
        AcceptConsent,
        DeclineConsent
    }

    public Type type;
    public Onboarding onboarding;
    public bool pressed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerProc()
    {
        switch (type)
        {
            case Type.Next:
                onboarding.Next();
                break;
            case Type.Back:
                onboarding.Back();
                break;
            case Type.Start:
                onboarding.Starting();
                break;
            case Type.AcceptConsent:
                onboarding.Consent(true);
                break;
            case Type.DeclineConsent:
                onboarding.Consent(false);
                break;
            default: break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && !pressed)
        {
            TriggerProc();

            pressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HandState hand = other.gameObject.GetComponent<HandState>();
        Poke poke = other.gameObject.GetComponent<Poke>();

        if ((hand != null || poke != null) && pressed)
        {
            pressed = false;
        }
    }
}
