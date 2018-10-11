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

    [Header("Type of Object")]
    [SerializeField]
    TypeOfObject _whatAmI;

    string _ItemText;

    KyleplayerMove _player;

    [Header("If a Puzzle")]
    [SerializeField]
    private int leverNumber;
    private Color CorrectColor = Color.green;

    private PuzzleManager pzManager;

    // Use this for initialization
    void Start () {
        _player = KyleplayerMove.Instance;
        pzManager = transform.GetComponentInParent<PuzzleManager>();
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                _ItemText = "Door";
                break;
            case TypeOfObject.POTION:
                _ItemText = "Potion";
                break;
            case TypeOfObject.PUZZLE:
                
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
                Debug.Log("Lever Pulled.");
                gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                pzManager._OrderProg = pzManager._OrderProg + leverNumber;
                pzManager.PzCheck();
                break;
            default:
                break;
        }
    }

    public string ItemText { get { return _ItemText; } }
}
