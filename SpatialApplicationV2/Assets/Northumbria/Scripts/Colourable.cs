using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colourable : MonoBehaviour
{
    public enum Type
    {
        Colourable,
        ColourableTexture,
    }

    public List<Material> originalMat;
    public string colour;
    public Type type = Type.Colourable;
    public List<Renderer> subObjects;
    public Material defaultMat;

    // Start is called before the first frame update
    void Start()
    {
        if (subObjects == null)
        {
            subObjects = new List<Renderer>();
        }

        if (subObjects.Count == 0)
        {
            subObjects.Add(GetComponent<Renderer>());
        }

        if (originalMat == null)
        {
            originalMat = new List<Material>();
        }

        if (originalMat.Count == 0)
        {
            foreach (Renderer r in subObjects)
            {
                originalMat.Add(r.material);
            }
        }

        if (colour == "")
        {
            colour = "White";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Brush>() != null)
        {
            SetMaterial(collision.collider.GetComponent<Renderer>().material, collision.collider.GetComponent<Brush>().colour);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Brush>() != null)
        {
            SetMaterial(other.GetComponent<Renderer>().material, other.GetComponent<Brush>().colour);
        }
    }

    public void ResetColour()
    {
        if (originalMat.Count > 0)
        {
            for (int i = 0; i < subObjects.Count; ++i)
            {
                if (type == Type.Colourable)
                {
                    if (originalMat[i] != null)
                    {
                        subObjects[i].material = originalMat[i];
                        colour = "Original";
                    }
                    else
                    {
                        subObjects[i].material = defaultMat;
                        colour = "White";
                    }
                }
                else if (type == Type.ColourableTexture)
                {
                    if (originalMat[i] != null)
                    {
                        if (originalMat[i].color != null)
                        {
                            subObjects[i].material.color = originalMat[i].color;
                            colour = "Original";
                        }
                        else
                        {
                            subObjects[i].material.color = defaultMat.color;
                            colour = "White";
                        }
                    }
                    else
                    {
                        subObjects[i].material = defaultMat;
                        colour = "White";
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < subObjects.Count; ++i)
            {
                if (type == Type.Colourable)
                {
                    if (originalMat[i] != null)
                    {
                        subObjects[i].material = defaultMat;
                        colour = "White";
                    }
                }
                else if (type == Type.ColourableTexture)
                {
                    if (originalMat[i] != null)
                    {
                        if (originalMat[i].color != null)
                        {
                            subObjects[i].material.color = defaultMat.color;
                            colour = "White";
                        }
                    }
                    else
                    {
                        subObjects[i].material = defaultMat;
                        colour = "White";
                    }
                }
            }
        }
    }

    public Color GetColour()
    {
        return subObjects[0].material.color;
    }

    public void SetColour(Color c, string id)
    {
        foreach (Renderer r in subObjects)
        {
            r.material.color = c;
        }

        colour = id;
    }

    public string GetColourStr()
    {
        return colour;
    }

    public Material GetMaterial()
    {
        return subObjects[0].material;
    }

    public void SetMaterial(Material m, string id)
    {
        foreach (Renderer r in subObjects)
        {
            if (type == Type.Colourable)
            {
                r.material = m;
            }
            else if(type == Type.ColourableTexture)
            {
                r.material.color = m.color;
            }
        }

        colour = id;
    }
}
