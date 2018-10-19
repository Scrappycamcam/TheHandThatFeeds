using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    KyleplayerMove _player;

    [Header("If a Puzzle")]
    [SerializeField]
    private int leverNumber;
    private Color CorrectColor = Color.green;

    private PuzzleManager pzManager;

    [Header("If Step Puzzle")]
    [SerializeField]
    bool _HasBeenStepped = false;

    [Header("UI Variables")]
    [SerializeField]
    GameObject InteractImagePrefab;
    [SerializeField]
    float _verticalOffset;
    GameObject _myImage;
    GameObject _myCanvas;
    [SerializeField]
    Vector3 _myActualPos;
    Vector3 _myPos;

    bool _active = false;

    Camera _playerCam;

    // Use this for initialization
    void Start () {
        _player = KyleplayerMove.Instance;
        pzManager = transform.GetComponentInParent<PuzzleManager>();
        _playerCam = FindObjectOfType<camera>().gameObject.GetComponent<Camera>();
        _myPos = Vector3.zero;
        _myCanvas = FindObjectOfType<Canvas>().gameObject;
        GameObject _ImageRef = Instantiate<GameObject>(InteractImagePrefab, _myPos, _myCanvas.transform.rotation, _myCanvas.transform);
        _myImage = _ImageRef;
        _myImage.SetActive(false);
    }

    private void Update()
    {
        if(_active)
        {
            _myPos = _playerCam.WorldToScreenPoint(_myActualPos) + (Vector3.up * _verticalOffset);
            _myImage.transform.position = _myPos;
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
                Debug.Log("Puzzle Type Found");

                if(pzManager.GetPzType() == PzType.LeverPz)
                {
                    Debug.Log("Lever Pulled.");
                    gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                    pzManager._OrderProg = pzManager._OrderProg + leverNumber;
                    pzManager.PzCheck();
                }
                else if (pzManager.GetPzType() == PzType.StepPz)
                {
                    //Debug.Log("Lever Pulled.");
                    if (!_HasBeenStepped)
                    {
                        gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                        pzManager._OrderProg = pzManager._OrderProg + leverNumber;
                        this._HasBeenStepped = true;
                        pzManager.PzCheck();
                        
                    }

                }
                else if(pzManager.GetPzType() == PzType.KillPz)
                {

                }
                else
                {

                }

                break;
            default:
                break;
        }
    }

    public void ShowIcon()
    {
        _myImage.SetActive(true);
        _active = true;
    }

    public void TurnOffIcon()
    {
        _myImage.SetActive(false);
        _active = false;
    }
    public bool SteppedOn { get { return _HasBeenStepped; } set { _HasBeenStepped = value; } }
}
