using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public enum TypeOfObject
{
    DOOR,
    POTION,
    PUZZLE,
    Mural
}


public class InteractableObject : MonoBehaviour {
    private int _playerId = 0;
    private Player _Player;
    [Header("Type of Object")]
    [SerializeField]
    TypeOfObject _whatAmI;
    [Header("If a Mural")]
    [SerializeField]
    private GameObject _MuralObj;
    [TextArea]
    [SerializeField]
    private string MuralText;
    private Sprite _ImageToUse;
    [SerializeField]
    [Tooltip("This allows Mural Image to be Assigned. Via Int")]
    private uint _MuralNo;
    [SerializeField]
    private bool _MuralState;

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
    [SerializeField]
    GameObject _myImage;
    GameObject _myCanvas;
    Vector3 _myPos;

    bool _active = false;

    Camera _playerCam;

    public void Awake()
    {
        _Player = ReInput.players.GetPlayer(_playerId);
        _playerCam = FindObjectOfType<camera>().gameObject.GetComponent<Camera>();
        _myPos = Vector3.zero;
        _myCanvas = FindObjectOfType<Canvas>().gameObject;
        GameObject _ImageRef = Instantiate<GameObject>(InteractImagePrefab, _myPos, _myCanvas.transform.rotation, _myCanvas.transform);
        _myImage = _ImageRef;
        _myImage.SetActive(false);
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                transform.parent = null;
                break;
            case TypeOfObject.PUZZLE:
                break;
            case TypeOfObject.Mural:
                _MuralObj.SetActive(false);
                _MuralState = false;
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    public void Init () {
        _player = KyleplayerMove.Instance;
        
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                break;
            case TypeOfObject.PUZZLE:
                _pzManagerOBJ = transform.parent.transform.gameObject;
                _pzManager = _pzManagerOBJ.GetComponent<PuzzleManager>();
                break;
            case TypeOfObject.Mural:
                _MuralObj.SetActive(false);
                _MuralState = false;
                break;
            default:
                break;
        }
        transform.parent = null;
    }

    private void Update()
    {
        if(this._whatAmI == TypeOfObject.Mural)
        {

            if (_MuralState)
            {
                _MuralObj.SetActive(true);
                //Take Player Input B Button.
                if (_Player.GetButtonDown("Dash"))
                {
                    _MuralState = false;
                    _MuralObj.SetActive(false);
                }
                

                
            }

        }
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
            case TypeOfObject.Mural:

                //Pause Time.
                //Get Mural image
                //Activate Mural Object.
                _MuralState = true;
                MuralManager(this._MuralNo);
                //Activate Text Box
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

    public void MuralManager(uint _MuralNum)
    {
        _ImageToUse = Resources.Load<Sprite>("MuralImages/Mural" + _MuralNo);
        Debug.Log(_ImageToUse);
        _MuralObj.GetComponent<Image>().sprite = _ImageToUse;

    }

    public PuzzleManager GetPuzzleManager { get { return _pzManager; } }
    public bool SteppedOn { get { return _HasBeenStepped; } set { _HasBeenStepped = value; } }
}
