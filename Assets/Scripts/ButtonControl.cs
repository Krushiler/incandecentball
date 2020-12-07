using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed = false;
    public bool buttonPressedDown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        buttonPressedDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        buttonPressedDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }

}
