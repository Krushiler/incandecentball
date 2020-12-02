using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    LevelPicker,
    LevelEnder
}

public class Interactable : MonoBehaviour
{
    [SerializeField] InteractableType interactableType; 
    [SerializeField] string interactableText;

    public InteractableType getType()
    {
        return interactableType;
    }

    public string getText()
    {
        return interactableText;
    }
}
