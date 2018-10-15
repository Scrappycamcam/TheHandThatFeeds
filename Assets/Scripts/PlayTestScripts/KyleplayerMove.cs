using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class KyleplayerMove : MonoBehaviour
{
    private static KyleplayerMove _instance;
    public static KyleplayerMove Instance
    {
        get
        {
            return _instance;
        }
    }

    public enum SpecialAbility
    {
        NONE,
        CYCLONE,
        THEGOODSUCC,
        DASHSTRIKE,
    }

    [Header("Base Player Variables")]
    private int _playerId = 0; // The Rewired player id of this character
    [SerializeField]
    float _playerDamage = 1;
    [SerializeField]
    float _moveSpeed = 3.0f;
    [SerializeField]
    float _dashspeed = 8f;
    [SerializeField]
    float _dashDelay = 0.8f;
    [SerializeField]
    bool _canMove = true;
    [SerializeField]
    float missDamage;
    [SerializeField]
    float _gravity;

    //public Attack _BaseAttack;
    //private Attack _LastAttack;
    //private Attack _NextAttack;

    private PlayerStats _pStats;
    private Player _player; // The Rewired Player
    private CharacterController _cc;
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _lastMove = Vector3.zero;
    private bool _dash = false;
    private bool _sprint = false;
    private bool _isDashing;
    private bool _LightAttack;
    private bool _HeavyAttack = false;
    private bool _CycloneAttack = false;
    private bool _DashStrike = false;
    private float _sprinting = 1f;
    private float _nextDash = 0f; // keeps track of when next dash can take place\
    private Vector3 _lastPos;
    //private int numFlips = 0;

    bool _attacking = false;
    bool _comboing = false;
    bool _swinging = false;
    bool _hitSomething = false;
    bool _invincible = false;
    RaycastHit hit;
    Vector3 c0, c1, c2, c3;

    [Header("Attack Variables")]
    [SerializeField]
    List<Transform> _myLightComboPos;
    [SerializeField]
    List<Transform> _myHeavyComboPos;

    GameObject _sword;
    Vector3 _swordReset;
    [SerializeField]
    float _swordDetectDistance;
    int _currComboNum = 0;
    Transform _currComboTransform;
    Transform _nextComboTransform;

    float _startComboTime;
    float _currComboTime;
    [SerializeField]
    float _lightSwingDuration;
    [SerializeField]
    float _heavySwingDuration;
    float _currSwingDuration;

    [Header("Special Ability Variables")]
    [Header("Cyclone")]
    [SerializeField]
    bool _cycloneIsUnlocked;
    [SerializeField]
    float _cycloneHealthBurden;
    [SerializeField]
    float _cycloneHeal;
    [SerializeField]
    float _cycloneSpinSpeed;
    [SerializeField]
    float _cycloneDuration;
    List<AIEnemy> _enemyHit;
    float _cycloneHits;

    [Header("Dash Strike")]
    [SerializeField]
    bool _dashIsUnlocked;
    [SerializeField]
    float _dashStrikeHealthBurden;
    [SerializeField]
    float _dashStrikeHeal;
    [SerializeField]
    float _dashStrikeBackUpDistance;
    [SerializeField]
    float _dashStrikeChargeUpDuration;
    [SerializeField]
    float _dashStrikeDistance;
    [SerializeField]
    float _dashStrikeForwardDashDuration;
    bool _chargingDashStrike;
    [SerializeField]
    GameObject _enemyIAmRamming;

    //[SerializeField]
    float stealHealthBurden;
    bool _usingSpecial = false;
    SpecialAbility _myability = SpecialAbility.NONE;

    void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _pStats = GetComponent<PlayerStats>();

        //_LastAttack = _BaseAttack;

        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        _player = ReInput.players.GetPlayer(_playerId);

        // Get the character controller
        _cc = GetComponent<CharacterController>();

        _sword = transform.GetChild(0).gameObject;
        _swordReset = _sword.transform.localPosition;
        _enemyHit = new List<AIEnemy>();
    }

    void Update()
    {

        if(!_attacking)
        {
            CheckForAttack();
        }
        else if(_usingSpecial)
        {
            SpecialAbilityActive();
        }
        else
        {
            CheckForCombo();
            Attack();
        }


        //GetAttack();
        //ProcessAttack();

        if (_canMove)
        {
            CheckFall();
            GetInput();
            ProcessMove();
            ProcessDash();
        }
        else if(_isDashing)
        {
            ProcessDash();
        }
    }

    private void CheckFall()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, .2f) && !_isDashing){ //if there is nothing below the player
            _cc.Move(Vector3.down * _gravity);//fall at rate gravity
        }
        else //if there is something below the player
        {
            if (hit.transform.tag == "Death")//if it is not meant to kill you
            {
                checkPoint(); //teleport to last position
            }
            else //it is meant to kill you
            {
                _lastPos = transform.position;   //set the last position
            }
        }
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.
        
        _moveVector.x = _player.GetAxisRaw("Move Horizontal"); // get input by name
        _moveVector.z = _player.GetAxisRaw("Move Vertical");
        _dash = _player.GetButtonDown("Dash");
        _sprint = _player.GetButtonDown("Sprint");
    }

    private void ProcessMove()
    {   // Process movement
        if (_moveVector.x != 0.0f || _moveVector.z != 0.0f) //move player
        {
            transform.rotation = Quaternion.LookRotation(_moveVector);
            _cc.Move(_moveVector.normalized * _moveSpeed * Time.deltaTime * _sprinting);
        }
        if (_moveVector == Vector3.zero) //set sprinting to 1 if not moving
        {
            _sprinting = 1f;
        }
        if (_sprint) //process sprint
        {
            _sprinting = 1.5f;
        }
    }

    private void ProcessDash()
    {
        if (_dash && Time.time > _nextDash && !_isDashing)//if you can dash and want to dash
        {
            _canMove = false;
            _isDashing = true;
            _lastMove = _moveVector;
            _nextDash = Time.time + _dashDelay;
        }
        else if (_isDashing) //if you are currently dashing
        {
            //numFlips++;
            _cc.Move(_lastMove.normalized * _dashspeed);
            _isDashing = false;
            _canMove = true;
            //transform.rotation = Quaternion.Euler(transform.right*10*numFlips);
            if (Time.time >= _nextDash)
            {
                //transform.rotation = Quaternion.Euler(0, 0, 0);
                //numFlips = 0;
                _isDashing = false;
                _canMove = true;
            }
        }
    }

    private void CheckForAttack()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");
        _CycloneAttack = _player.GetButtonDown("Ability3");
        _DashStrike = _player.GetButtonDown("Ability2");

        if (_LightAttack)
        {
            _currComboNum = 0;
            _nextComboTransform = _myLightComboPos[_currComboNum];
            //Debug.Log(_nextComboTransform);
            _attacking = true;
            _canMove = false;
        }
        else if (_HeavyAttack)
        {
            _currComboNum = 0;
            _nextComboTransform = _myHeavyComboPos[_currComboNum];
            _attacking = true;
            _canMove = false;
        }
        else if (_CycloneAttack && _cycloneIsUnlocked)
        {
            if(_pStats.GetHealth() > _cycloneHealthBurden)
            {
                Debug.Log("cyclone Active");
                _pStats.PDamage(_cycloneHealthBurden);
                _attacking = true;
                _usingSpecial = true;
                _myability = SpecialAbility.CYCLONE;
                _canMove = false;
                c0 = transform.position;
                c1 = transform.position;
                _sword.transform.localPosition = new Vector3(0, 1f, _sword.transform.localPosition.z);
                _startComboTime = Time.time;
            }
        }
        else if(_DashStrike && _dashIsUnlocked)
        {
            if (_pStats.GetHealth() > _dashStrikeHealthBurden)
            {
                Debug.Log("Dash Strike Active");
                _pStats.PDamage(_dashStrikeHealthBurden);
                _attacking = true;
                _usingSpecial = true;
                _myability = SpecialAbility.DASHSTRIKE;
                _canMove = false;
                c0 = transform.position;
                c1 = transform.position - (transform.forward * _dashStrikeBackUpDistance);
                _chargingDashStrike = true;
                _startComboTime = Time.time;
            }
        }
    }

    private void SpecialAbilityActive()
    {
        switch (_myability)
        {
            case SpecialAbility.NONE:
                _canMove = true;
                _attacking = false;
                _usingSpecial = false;
                break;
            case SpecialAbility.CYCLONE:
                UsingCyclone();
                break;
            case SpecialAbility.THEGOODSUCC:
                GiveEmTheGoodSucc();
                break;
            case SpecialAbility.DASHSTRIKE:
                if(_chargingDashStrike)
                {
                    ChargingDashStrike();
                }
                else
                {
                    UsingDashStrike();
                }
                break;
            default:
                break;
        }
    }

    public void ResetAbilities()
    {
        _myability = SpecialAbility.NONE;
        _sword.transform.localPosition = _swordReset;
        if(_enemyIAmRamming != null)
        {
            _enemyHit = new List<AIEnemy>();
            _enemyIAmRamming = null;
        }
    }

    private void UsingCyclone()
    {
        _currComboTime = (Time.time - _startComboTime) / _cycloneDuration;

        Vector3 p01;
        p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;

        transform.position = p01;

        if(_currComboTime < 1)
        {
            if (Physics.Raycast(_sword.transform.position, _sword.transform.up, out hit, _swordDetectDistance))
            {
                if (hit.collider.GetComponent<AIEnemy>())
                {
                    AIEnemy EnemyHit = hit.collider.GetComponent<AIEnemy>();

                    //Debug.Log("hit");
                    
                    if(_enemyHit.Count > 0)
                    {
                        if (!CycloneAlreadyHitEnemy(EnemyHit))
                        {
                            Debug.Log("cyclone hit");
                            _enemyHit.Add(EnemyHit);
                            EnemyHit.GotHit(_playerDamage, transform.forward);
                            if (_enemyHit.Count > 2)
                            {
                                _enemyHit = new List<AIEnemy>();
                                Debug.Log("cyclone heal");
                                _pStats.PHeal(_cycloneHeal);

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("cyclone hit");
                        _enemyHit.Add(EnemyHit);
                        EnemyHit.GotHit(_playerDamage, transform.forward);
                    }                    
                }
            }
            transform.Rotate(Vector3.up, _cycloneSpinSpeed);
        }
        else
        {
            _currComboTime = 1;

            _myability = SpecialAbility.NONE;
            _sword.transform.localPosition = _swordReset;
            _enemyHit = new List<AIEnemy>();
        }
    }

    bool CycloneAlreadyHitEnemy(AIEnemy _enemy)
    {
        for (int i = 0; i < _enemyHit.Count; i++)
        {
            if (_enemyHit[i] != null)
            {
                if (_enemyHit[i] == _enemy)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void GiveEmTheGoodSucc()
    {

    }

    private void ChargingDashStrike()
    {
        _currComboTime = (Time.time - _startComboTime) / _dashStrikeChargeUpDuration;

        Vector3 p01;
        p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;

        transform.position = p01;

        if (_currComboTime >= 1)
        {
            _currComboTime = 1;
            _sword.transform.localPosition = new Vector3(0, 1f, _sword.transform.localPosition.z);
            c0 = transform.position;
            c1 = transform.position + (transform.forward * _dashStrikeDistance);
            _startComboTime = Time.time;
            _chargingDashStrike = false;
            _invincible = true;
        }
        else
        {
            if(Physics.Raycast(transform.position + Vector3.up, -transform.forward, out hit, _swordDetectDistance))
            {
                c1 = transform.position;
            }
        }
    }

    private void UsingDashStrike()
    {
        _currComboTime = (Time.time - _startComboTime) / _dashStrikeForwardDashDuration;
        

        if (_currComboTime < 1)
        {
            if (_enemyIAmRamming == null)
            {
                Debug.DrawLine(transform.position + Vector3.up, transform.position + transform.forward + Vector3.up);
                if(Physics.Raycast(transform.position + Vector3.up, _sword.transform.up, out hit, _swordDetectDistance*2))
                {
                    GameObject thingHit = hit.collider.gameObject;
                    if (thingHit.GetComponent<AIEnemy>())
                    {
                        _enemyIAmRamming = thingHit;
                        _enemyIAmRamming.transform.parent = _sword.transform;
                        _enemyIAmRamming.GetComponent<AIEnemy>().GotDashStruck(_playerDamage);
                    }
                    else
                    {
                        _myability = SpecialAbility.NONE;
                        _sword.transform.localPosition = _swordReset;
                        _enemyHit = new List<AIEnemy>();
                    }
                }

            }
            else
            {
                for (int i = -1; i < 2; i++)
                {
                    Vector3 RayPos = new Vector3(transform.position.x + ((transform.right.x *_swordReset.x) * i), transform.position.y + 1f, transform.position.z);
                    Debug.DrawLine(RayPos, RayPos + transform.forward);
                    if (Physics.Raycast(RayPos, _sword.transform.up, out hit, _swordDetectDistance*1.3f))
                    {
                        
                        GameObject thingHit = hit.collider.gameObject;
                        if(thingHit.GetComponent<AIEnemy>())
                        {
                            Vector3 _staggerDirection;
                            if(i == -1)
                            {
                                _staggerDirection = -transform.right *3;
                            }
                            else
                            {
                                _staggerDirection = transform.right *3;
                            }
                            thingHit.GetComponent<AIEnemy>().GotHit(_playerDamage, _staggerDirection);
                        }
                        else
                        {
                            _myability = SpecialAbility.NONE;
                            _sword.transform.localPosition = _swordReset;
                            _enemyHit = new List<AIEnemy>();
                            if(_enemyIAmRamming != null)
                            {
                                _enemyIAmRamming.GetComponent<AIEnemy>().GotPinned(_playerDamage);
                                _enemyIAmRamming = null;
                            }
                        }
                    }

                }
            }
        }
        else
        {
            _currComboTime = 1;

            _myability = SpecialAbility.NONE;
            _sword.transform.localPosition = _swordReset;
            _enemyHit = new List<AIEnemy>();
            if(_enemyIAmRamming != null)
            {
                _enemyIAmRamming.GetComponent<AIEnemy>().ResetState();
                _enemyIAmRamming = null;
            }
        }

        Vector3 p01;
        p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;

        transform.position = p01;
    }

    private void CheckForCombo()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");

        if(_comboing && (_currComboNum < _myLightComboPos.Count || _currComboNum < _myHeavyComboPos.Count))
        {
            if (_LightAttack)
            {
                _nextComboTransform = _myLightComboPos[_currComboNum];
            }
            else if (_HeavyAttack)
            {
                _nextComboTransform = _myHeavyComboPos[_currComboNum];
            }
        }
    }

    private void Attack()
    {
        if(!_swinging)
        {
            if(_nextComboTransform == _myLightComboPos[_currComboNum])
            {
                _currSwingDuration = _lightSwingDuration;               
            }
            else if (_nextComboTransform == _myHeavyComboPos[_currComboNum])
            {
                _currSwingDuration = _heavySwingDuration;
            }

            //Debug.Log(_currSwingDuration);

            _currComboNum++;
            if (_nextComboTransform != null)
            {
                _currComboTransform = _nextComboTransform;
                c0 = _sword.transform.localPosition;
                c1 = ((_sword.transform.localPosition + _currComboTransform.localPosition) / 2) + transform.forward;
                c2 = _currComboTransform.localPosition;
                _nextComboTransform = null;
                //Debug.Log(_currComboTransform);
                _startComboTime = Time.time;
                _swinging = true;
            }
        }
        else
        {
            _currComboTime = (Time.time - _startComboTime) / _currSwingDuration;

            Vector3 p01, p12, p012;
            p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;
            p12 = (1 - _currComboTime) * c1 + _currComboTime * c2;

            p012 = (1 - _currComboTime) * p01 + _currComboTime * p12;

            _sword.transform.localPosition = p012;
            _sword.transform.localRotation = _currComboTransform.localRotation;

            if (_currComboTime < 1)
            {
                if(Physics.Raycast(_sword.transform.position, _sword.transform.up, out hit, _swordDetectDistance))
                {
                    if(hit.collider.GetComponent<AIEnemy>())
                    {
                        Debug.Log("hit");
                        hit.collider.GetComponent<AIEnemy>().GotHit(_playerDamage, transform.forward);
                        _hitSomething = true;
                    }
                }
            }
            else
            {
                _currComboTime = 1;
                if (!_hitSomething)
                {
                    Debug.Log("missed");
                    _pStats.PDamage(missDamage);
                }

                if (_nextComboTransform == null || _currComboNum > _myLightComboPos.Count)
                {
                    ResetSword();
                }
                _swinging = false;

                
            }
            
            if(_currComboTime >= .6f && _currComboTime <= .95f)
            {
                _comboing = true;
            }
            else
            {
                _comboing = false;
            }
        }
    }

    private void ResetSword()
    {
        //Debug.Log("Sword Reset");
        _attacking = false;
        _comboing = false;
        _swinging = false;
        _hitSomething = false;
        _sword.transform.localPosition = _swordReset;
        _currComboNum = 0;
        _nextComboTransform = null;
        _currComboTransform = null;
        _canMove = true;
    }


    /*private void GetAttack()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");
    }

    private void ProcessAttack()
    {
        if (_HeavyAttack)
        {
            if (!_LastAttack.MakeAttack(_LastMove))
            {
                _pStats.PDamage(missDamage);
            }
            _canMove = false;
        }
        else if (_LightAttack)
        {
            if (!_LastAttack.MakeAttack(_LastMove))
            {
                _pStats.PDamage(missDamage);
            }
            _canMove = false;
        }
        _canMove = true;
    }*/

    public PlayerStats GetPlayerStats
    {
        get
        {
            return _pStats;
        }
    }

    private void checkPoint()
    {
        transform.position = _lastPos;
        _pStats.PDamage(missDamage);
    }

    public SpecialAbility GetCurrAbility { get { return _myability; } }
    public bool AmIInvincible { get { return _invincible; } }
}