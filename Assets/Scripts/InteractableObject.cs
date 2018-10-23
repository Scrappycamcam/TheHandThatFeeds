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
    private Color DefaultColor = Color.white;

    private GameObject _pzManagerOBJ;
    private PuzzleManager _pzManager;

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
    Vector3 _myPos;

    bool _active = false;

    Camera _playerCam;

    // Use this for initialization
    public void Init () {
        _player = KyleplayerMove.Instance;
        _pzManagerOBJ = transform.parent.transform.gameObject;
        _pzManager = _pzManagerOBJ.GetComponent<PuzzleManager>();
        _playerCam = FindObjectOfType<camera>().gameObject.GetComponent<Camera>();
        _myPos = Vector3.zero;
        _myCanvas = FindObjectOfType<Canvas>().gameObject;
        GameObject _ImageRef = Instantiate<GameObject>(InteractImagePrefab, _myPos, _myCanvas.transform.rotation, _myCanvas.transform);
        _myImage = _ImageRef;
        _myImage.SetActive(false);
        transform.parent = null;
    }

    private void Update()
    {

        if(_active)
        {
            _myPos = _playerCam.WorldToScreenPoint(transform.position) + (Vector3.up * _verticalOffset);
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
                TurnOffIcon();
                gameObject.SetActive(false);
                break;
            case TypeOfObject.PUZZLE:
                Debug.Log("Puzzle Type Found");
                Debug.Log(_pzManager.transform.name);

                if (_pzManager.GetPzType() == PzType.LeverPz)
                {
                    Debug.Log("Lever Pulled.");
                    gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                    _pzManager._OrderProg = _pzManager._OrderProg + leverNumber;
                    _pzManager.PzCheck();

                }
                else if (_pzManager.GetPzType() == PzType.StepPz)
                {
                    //Debug.Log("Lever Pulled.");
                    if (!_HasBeenStepped)
                    {
                        gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                        _pzManager._OrderProg = _pzManager._OrderProg + leverNumber;
                        this._HasBeenStepped = true;
                        _pzManager.PzCheck();
                        
                    }

                }
                else if(_pzManager.GetPzType() == PzType.KillPz)
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

    public void PuzzleReset()
    {
        GetComponent<MeshRenderer>().material.color = DefaultColor;
        _HasBeenStepped = false;
        
    }

    public PuzzleManager GetPuzzleManager { get { return _pzManager; } }
    public bool SteppedOn { get { return _HasBeenStepped; } set { _HasBeenStepped = value; } }
}
