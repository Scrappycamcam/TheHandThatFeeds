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
    private GameObject _MuralSource;
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

    [Header("If Kill Puzzle")]
    [SerializeField]
    float _killWidth;
    [SerializeField]
    float _killDepth;
    [SerializeField]
    Vector3 _killAreaStart;
    [SerializeField]
    List<GameObject> _enemiesToDie;

    uint _killsLeft;

    Vector3 _boxStart;
    Vector3 _midpoint1;
    Vector3 _midpoint2;
    Vector3 _boxEnd;

    bool _active = false;

    Camera _playerCam;
    [SerializeField]
    private GameObject _CounterPopUp;


    //[Header("If Enemy Kill Puzzle")]


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
                switch (_pzManager.GetPzType())
                {
                    case PzType.KillPz:
                        _killAreaStart += transform.position;
                        _boxStart = new Vector3(_killAreaStart.x - (_killWidth / 2), _killAreaStart.y, _killAreaStart.z + (_killDepth / 2));
                        _boxEnd = new Vector3(_boxStart.x + _killWidth, _boxStart.y, _boxStart.z -_killDepth);
                        _midpoint1 = _boxStart;
                        _midpoint1.x += _killWidth;
                        _midpoint2 = _boxStart;
                        _midpoint2.z -= _killDepth;

                        Debug.Log(_boxStart);
                        Debug.Log(_boxEnd);
                        
                        for (int i = 0; i < _enemiesToDie.Count; i++)
                        {
                            _enemiesToDie[i].GetComponent<AIKillPuzzleMelee>().SetPuzzle = this;
                            _killsLeft++;
                            //Add count to kill count.
                        }
                        
                        break;
                    default:
                        break;
                }
                break;
            case TypeOfObject.Mural:
                //Load Mural Object
                _MuralSource = GameObject.Find("MuralCase");
                Debug.Log(_MuralSource);
                _MuralObj = _MuralSource.GetComponentInChildren<RectTransform>(true).gameObject;
                Debug.Log(_MuralObj);
                //Turn Mural Off
                _MuralObj.SetActive(false);
                _MuralState = false;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Entered Zone");
            StartPuzzleRoom();
            

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
                //_MuralObj = GameObject.FindGameObjectWithTag("MuralOverlay");
                _MuralObj.SetActive(false);
                _MuralState = false;
                break;
            default:
                break;
        }
        transform.parent = null;
    }

    public void StartPuzzleRoom()
    {
        Debug.Log("Started Puzzle Room.");
        switch (_pzManager.GetPzType())
        {
            case PzType.LeverPz:
                break;
            case PzType.StepPz:
                break;
            case PzType.KillPz:
                if (!_pzManager.GetPuzzleComplete())
                {
                    _pzManager.LockZone();
                    //Over Adds to Current Kills left
                    /*
                    for (int i = 0; i < _enemiesToDie.Count; i++)
                    {
                        _enemiesToDie[i].GetComponent<AIKillPuzzleMelee>().SetPuzzle = this;
                        _killsLeft++;
                        //Add count to kill count.
                    }
                    */
                    //Reset all Kill Puzzle Components HERE!

                }
                break;
            default:
                break;
        }
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
            case TypeOfObject.PUZZLE:
                switch (_pzManager.GetPzType())
                {
                    case PzType.KillPz:
                        Debug.DrawLine(_boxStart, _midpoint1);
                        Debug.DrawLine(_boxStart, _midpoint2);
                        Debug.DrawLine(_midpoint1, _boxEnd);
                        Debug.DrawLine(_midpoint2, _boxEnd);


                        //Add Ingame Visual Effects
                        //Debug.Log(KillsLeft);
                        ShowKillsLeft();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }


        
    }

    public void ShowKillsLeft()
    {
        if (KillsLeft > 0)
        {
            //Temp Disable Lock Zone.
            //_pzManager.LockZone();
            _CounterPopUp.SetActive(true);
            Debug.Log("Kill Counter Should Be Active " + KillsLeft);
            _CounterPopUp.GetComponentInChildren<Text>().text = KillsLeft.ToString();
        }
        else
        {
            //Check Enemy On Death.
            _CounterPopUp.SetActive(false);
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
                    if (_HasBeenStepped)
                    {

                        Debug.Log("Lever Pulled.");
                        if (_ParticleEffect != null)
                        {
                            _ParticleEffect.SetActive(true);

                        }
                        else
                        {
                            gameObject.GetComponent<MeshRenderer>().material.color = CorrectColor;

                        }
                    }
                    this._HasBeenStepped = true;
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
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                _myImage.SetActive(true);
                _active = true;
                break;
            case TypeOfObject.UPGRADE:
                if (_winRef.GetKilledEnemiesCount >= _NumOfEnemiesToKill)
                {
                    _killedEnough = true;
                }

                if (!_killedEnough)
                {
                    _KillsToUnlockText.SetActive(true);
                    _myImage.SetActive(false);
                }
                else
                {
                    _KillsToUnlockText.SetActive(false);
                    _myImage.SetActive(true);
                }
                break;
            case TypeOfObject.PUZZLE:
                switch (_pzManager.GetPzType())
                {
                    case PzType.LeverPz:
                        _myImage.SetActive(true);
                        _active = true;
                        break;
                    case PzType.KillPz:
                        break;
                    case PzType.StepPz:
                        break;
                    default:
                        break;
                }
                break;
            case TypeOfObject.Mural:
                break;
            default:
                break;
        }
        
    }

    public void TurnOffIcon()
    {
        switch (_whatAmI)
        {
            case TypeOfObject.DOOR:
                break;
            case TypeOfObject.POTION:
                _myImage.SetActive(false);
                _active = false;
                break;
            case TypeOfObject.UPGRADE:
                if(!_killedEnough)
                {
                    _KillsToUnlockText.SetActive(false);
                }
                _myImage.SetActive(false);
                _active = false;
                break;
            case TypeOfObject.PUZZLE:
                switch (_pzManager.GetPzType())
                {
                    case PzType.LeverPz:
                        _myImage.SetActive(false);
                        _active = false;
                        break;
                    case PzType.KillPz:
                        break;
                    case PzType.StepPz:
                        break;
                    default:
                        break;
                }
                break;
            case TypeOfObject.Mural:
                break;
            default:
                break;
        }
    }

    public bool CheckForDeathArea(GameObject _enemyToDie, Vector3 _enemyToDiePos)
    {
        
        if(_enemyToDiePos.x > _boxStart.x && _enemyToDiePos.x < _boxEnd.x && _enemyToDiePos.z < _boxStart.z && _enemyToDiePos.z > _boxEnd.z)
        {
            _enemiesToDie.Remove(_enemyToDie);
            _killsLeft--;
            CheckForAllDeadEnemies();
            return true;
        }
        else
        {
            return false;
        }
    }


    private void CheckForAllDeadEnemies()
    {
        if(_enemiesToDie.Count == 0)
        {

            _CounterPopUp.SetActive(false);
            //Turn Off Kill Counter.
            _pzManager.moveFloor();
        }
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
        if(_MuralObj == null)
        {
            //_MuralObj = GameObject.FindGameObjectWithTag("Mural")
        }
        _ImageToUse = Resources.Load<Sprite>("MuralImages/Mural" + _MuralNo);
        Debug.Log(_ImageToUse);
        _MuralObj.GetComponent<Image>().sprite = _ImageToUse;
        _MuralObj.GetComponentInChildren<Text>().text = MuralText;

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
    public uint KillsLeft{ get { return _killsLeft; } }
}
