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

    KyleplayerMove _player;

    // Use this for initialization
    void Start () {
        _player = KyleplayerMove.Instance;
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

    public void Interact()
    {
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                Debug.Log("Got hit");
                _player.GetPlayerStats.PHeal(20);
                Destroy(gameObject);
                break;
            case TypeOfObject.PUZZLE:
                break;
            default:
                break;
        }
    }

    public string ItemText { get { return _ItemText; } }
}
