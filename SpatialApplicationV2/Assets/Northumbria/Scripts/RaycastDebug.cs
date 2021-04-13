using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebug : MonoBehaviour
{
    public Northumbria.RecordingManager rMan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray forwardRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(forwardRay, out hit, Camera.main.farClipPlane))
            {
                OnboardingButton b = hit.collider.GetComponent<OnboardingButton>();

                if (b != null)
                {
                    switch (b.type)
                    {
                        case OnboardingButton.Type.Next:
                            b.onboarding.Next();
                            break;
                        case OnboardingButton.Type.Back:
                            b.onboarding.Back();
                            break;
                        case OnboardingButton.Type.Start:
                            b.onboarding.Starting();
                            break;
                        case OnboardingButton.Type.AcceptConsent:
                            b.onboarding.Consent(true);
                            break;
                        case OnboardingButton.Type.DeclineConsent:
                            b.onboarding.Consent(false);
                            break;
                        default: break;
                    }
                }

                DiskInteractible disk = hit.collider.GetComponent<DiskInteractible>();
                if (disk != null)
                {
                    if (rMan != null)
                    {
                        rMan.EndRecording();
                    }

                    hit.collider.transform.parent = null;
                    Moveable mov = hit.collider.GetComponent<Moveable>();
                    mov.grabbed = false;
                    Destroy(mov);
                    ObjectOutline outline = hit.collider.GetComponent<ObjectOutline>();
                    outline.outline.SetActive(false);
                    Destroy(outline);
                    hit.collider.transform.position = new Vector3(18.97348f, 0.4412f, 4.425594f);
                    hit.collider.transform.rotation = Quaternion.Euler(180, 0, 0);
                    Destroy(disk);
                }
            }
        }
    }
}
