using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Timeline : MonoBehaviour
{
    public float value;
    public Vector2 mousePosStart;
    public GameObject handle;

    float prevValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(value != prevValue)
        {
            ValueChanged();
        }

        prevValue = value;
    }

    public void OnBeginDrag(BaseEventData e)
    {
        
    }

    public void OnDrag(BaseEventData e)
    {
        //var line = GetComponent<RectTransform>();
        //Rect handleRect = handle.GetComponent<RectTransform>().rect;
        //handleRect.x = Mathf.Clamp(handle.transform.localPosition.x+((PointerEventData)e).delta.x,0,rect.rect.width);
        //value = Mathf.Clamp(handle.transform.localPosition.x + ((PointerEventData)e).delta.x, 0, line.rect.width);
        value = handle.transform.localPosition.x + ((PointerEventData)e).delta.x;
        //if (value <= 0) value = 0;
        //else if (value >= line.rect.width) value = line.rect.width;
        //Vector2 pos = new Vector2(value, 0);
        //handle.transform.localPosition = pos;
        //handle.GetComponent<RectTransform>().rect = handleRect;
        ValueChanged();
        Vector2 pos = new Vector2(value, 0);
        handle.GetComponent<RectTransform>().anchoredPosition = pos;
        //handle.GetComponent<RectTransform>().localPosition = pos;


    }

    public void OnEndDrag(BaseEventData e)
    {
        
    }

    public void ValueChanged()
    {
        var line = GetComponent<RectTransform>();

        if (value <= 0) value = 0;
        else if (value >= line.rect.width) value = line.rect.width;
    }
}
