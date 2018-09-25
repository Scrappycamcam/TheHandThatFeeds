using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfObject
{
    DOOR,
    POTION,
    PUZZLE
}


public class InteractableObject : MonoBehaviour {

    [SerializeField]
    TypeOfObject _whatAmI;

    string _ItemText;

    // Use this for initialization
    void Start () {
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                _ItemText = "Door";
                break;
            case TypeOfObject.POTION:
                _ItemText = "Potion";
                break;
            case TypeOfObject.PUZZLE:
                _ItemText = "Puzzle";
                break;
            default:
                break;
        }
    }

    public string ItemText { get { return _ItemText; } }
}
