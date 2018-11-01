using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public enum TypeOfObject
{
    DOOR,
    POTION,
    UPGRADE,
    PUZZLE,
    Mural
}

public enum WhichUpgradeAmI
{
    CYCLONE,
    DASHSTRIKE,

}

public class InteractableObject : MonoBehaviour {

    private int _playerId = 0;
    private Player _Player;

    KyleplayerMove _playerRef;

    [Header("Type of Object")]
    [SerializeField]
    TypeOfObject _whatAmI;

    [Header("UI Variables")]
    [SerializeField]
    GameObject InteractImagePrefab;
    [SerializeField]
    float _verticalOffset;
    GameObject _myImage;
    PlayerCanvas _myCanvas;
    Vector3 _myPos;
    BoxCollider _myCollider;
    MeshRenderer _myRenderer;


    [Header("If a Health Potion")]
    [SerializeField]
    float _healingAmount;

    [Header("If an Upgrade Potion")]
    [SerializeField]
    WhichUpgradeAmI _upgradeToUnlock;
    [SerializeField]
    GameObject _KillsTextPrefab;
    [SerializeField]
    float _NumOfEnemiesToKill;
    GameObject _KillsToUnlockText;
    bool _killedEnough = false;
    WinCondition _winRef;

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

    [Header("If a Puzzle")]
    [SerializeField]
    private int leverNumber;
    private Color CorrectColor = Color.green;
    private Color DefaultColor = Color.white;

    [Tooltip("This Allows a Particle effect to trigger")]
    [SerializeField]
    private GameObject _ParticleEffect;

    private GameObject _pzManagerOBJ;
    private PuzzleManager _pzManager;

    [Header("If Step Puzzle")]
    [SerializeField]
    bool _HasBeenStepped = false;
    
    bool _active = false;

    Camera _playerCam;

    public void Start()
    {
        _Player = ReInput.players.GetPlayer(_playerId);
        _playerCam = FindObjectOfType<PlayerCamera>().gameObject.GetComponent<Camera>();
        _myPos = Vector3.zero;
        _myCanvas = PlayerCanvas.Instance;
        _myCanvas.SetGameReset += InteractableReset;
        GameObject _ImageRef = Instantiate<GameObject>(InteractImagePrefab, _myPos, _myCanvas.transform.rotation, _myCanvas.transform);
        _myImage = _ImageRef;
        _myImage.SetActive(false);

        _playerRef = KyleplayerMove.Instance;
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                transform.parent = null;
                break;
            case TypeOfObject.UPGRADE:
                _winRef = FindObjectOfType<WinCondition>();
                GameObject _killtext = Instantiate<GameObject>(_KillsTextPrefab, _myPos, _myCanvas.transform.rotation, _myCanvas.transform);
                _KillsToUnlockText = _killtext;
                _KillsToUnlockText.SetActive(false);

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
        if (_active)
        {
            _myPos = _playerCam.WorldToScreenPoint(transform.position) + (Vector3.up * _verticalOffset);
            _myImage.transform.position = _myPos;
        }

        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                break;
            case TypeOfObject.UPGRADE:
                if (!_killedEnough && _active)
                {
                    _KillsToUnlockText.GetComponent<Text>().text = "" + (_NumOfEnemiesToKill - _winRef.GetKilledEnemiesCount);
                    _KillsToUnlockText.transform.position = _myPos;
                }
                break;
            case TypeOfObject.Mural:
                if (_MuralState)
                {
                    _MuralObj.SetActive(true);
                    //Take Player Input B Button.
                    if (_Player.GetButtonDown("Dash"))
                    {
                        _MuralState = false;
                        _MuralObj.SetActive(false);
                        Time.timeScale = 1;
                    }
                }
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
                _playerRef.GetPlayerStats.PHeal(_healingAmount);
                TurnOffIcon();
                gameObject.SetActive(false);
                break;
            case TypeOfObject.UPGRADE:
                if(_killedEnough)
                {
                    Debug.Log("upgrade given");
                    switch (_upgradeToUnlock)
                    {
                        case WhichUpgradeAmI.CYCLONE:
                            Debug.Log("cyclone get");
                            _playerRef.GetCycloneUnlock = true;
                            Debug.Log(_playerRef.GetCycloneUnlock);
                            TurnOffIcon();
                            gameObject.SetActive(false);
                            break;
                        case WhichUpgradeAmI.DASHSTRIKE:
                            _playerRef.GetDashStrikeUnlock = true;
                            TurnOffIcon();
                            gameObject.SetActive(false);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case TypeOfObject.PUZZLE:
                Debug.Log("Puzzle Type Found");
                Debug.Log(_pzManager.transform.name);

                if (_pzManager.GetPzType() == PzType.LeverPz)
                {
                    Debug.Log("Lever Pulled.");
                    gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                    if (_ParticleEffect != null)
                    {
                        _ParticleEffect.SetActive(true);

                    }
                    _pzManager._OrderProg = _pzManager._OrderProg + leverNumber;
                    _pzManager.PzCheck();

                }
                else if (_pzManager.GetPzType() == PzType.StepPz)
                {
                    //Debug.Log("Lever Pulled.");
                    if (!_HasBeenStepped)
                    {
                        gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;
                        if (_ParticleEffect != null)
                        {
                            _ParticleEffect.SetActive(true);

                        }
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
                Time.timeScale = 0;
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
        if(_whatAmI == TypeOfObject.UPGRADE)
        {
            if (_winRef.GetKilledEnemiesCount >= _NumOfEnemiesToKill)
            {
                _killedEnough = true;
            }

            if(!_killedEnough)
            {
                _KillsToUnlockText.SetActive(true);
                _myImage.SetActive(false);
            }
        }
        
    }

    public void TurnOffIcon()
    {
        if (_whatAmI == TypeOfObject.UPGRADE && !_killedEnough)
        {
            _KillsToUnlockText.SetActive(false);
        }

        _myImage.SetActive(false);
        _active = false;
    }

    public void PuzzleReset()
    {
        GetComponent<MeshRenderer>().material.color = DefaultColor;
        if (_ParticleEffect != null)
        {
            _ParticleEffect.SetActive(false);

        }
        
        _HasBeenStepped = false;
        
    }

    public void MuralManager(uint _MuralNum)
    {
        _ImageToUse = Resources.Load<Sprite>("MuralImages/Mural" + _MuralNo);
        Debug.Log(_ImageToUse);
        _MuralObj.GetComponent<Image>().sprite = _ImageToUse;

    }

    private void InteractableReset()
    {
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                gameObject.SetActive(true);
                break;
            case TypeOfObject.UPGRADE:
                gameObject.SetActive(true);
                break;
            case TypeOfObject.PUZZLE:
                break;
            case TypeOfObject.Mural:
                break;
            default:
                break;
        }
    }

    public PuzzleManager GetPuzzleManager { get { return _pzManager; } }
    public bool SteppedOn { get { return _HasBeenStepped; } set { _HasBeenStepped = value; } }
}
