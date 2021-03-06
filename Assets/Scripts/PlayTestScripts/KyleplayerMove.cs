﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class KyleplayerMove : MonoBehaviour
{
    private static KyleplayerMove _instance;
    public static KyleplayerMove Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (FindObjectOfType<KyleplayerMove>())
                {
                    _instance = FindObjectOfType<KyleplayerMove>();
                    return _instance;
                }
                else
                {
                    Debug.Log("no Player");
                    return null;
                }
            }
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
    float _lightAttackDamage;
    [SerializeField]
    float _heavyAttackDamage;
    float _currDamage;
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
    float _sprintSpeed = 8.0f;
    [SerializeField]
    bool _canSprint;
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
    Vector3 _startPos;
    Animator _myAnimations;

    //public Attack _BaseAttack;
    //private Attack _LastAttack;
    //private Attack _NextAttack;

    private PlayerStats _pStats;
    private Player _player; // The Rewired Player
    private CharacterController _cc;
    private PlayerCanvas _canvasRef;
    private PauseMenu _pauseRef;
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _lastMove = Vector3.zero;
    private bool _dash = false;
    private bool _sprint = false;
    private bool _isDashing;
    private bool _LightAttack = false;
    private bool _HeavyAttack = false;
    private bool _CycloneAttack = false;
    private bool _DashStrike = false;
    private bool _sprinting = false;
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
    [SerializeField]
    float _TimeForComboToDecay;
    float _TimeComboStart;
    GameObject _ComboPartsParent;

    private Text _ComboText;
    private Image _DecayBar;

    GameObject _sword;
    Vector3 _swordReset;
    [SerializeField]
    float _swordDetectDistance;
    int _currComboNum = 0;
    int _currTotalCombo = 0;
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
    [SerializeField]
    bool _AbilitiesCostHealth = true;

    [Header("Cyclone")]
    [SerializeField]
    bool _cycloneIsUnlocked;
    bool _levelStartCycloneUnlock;
    [SerializeField]
    float _cycloneAttackDamage;
    [SerializeField]
    float _cycloneKnockBack;
    [SerializeField]
    float _cycloneHealthBurden;
    [SerializeField]
    float _cycloneHeal;
    [SerializeField]
    float _cycloneSpinSpeed;
    [SerializeField]
    float _cycloneDuration;
    [SerializeField]
    float _cycloneDetectionDistance;
    [SerializeField]
    bool _debugCyclone;
    List<AIEnemy> _enemyHit;
    float _cycloneHits;

    [Header("Dash Strike")]
    [SerializeField]
    bool _dashIsUnlocked;
    bool _levelStartDashUnlock;
    [SerializeField]
    float _dashStrikeAttackDamage;
    [SerializeField]
    float _dashStrikeKnockBack;
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
    [SerializeField]
    float _dashStrikeInitialDetectionDistance;
    [SerializeField]
    float _dashStrikeDetectionDistanceWhileRammingAnEnemy;
    [SerializeField]
    bool _debugDashStrike;
    bool _chargingDashStrike;
    GameObject _enemyIAmRamming;

    //[SerializeField]
    float stealHealthBurden;
    bool _usingSpecial = false;
    SpecialAbility _myability = SpecialAbility.NONE;

    void Awake()
    {
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            KyleplayerMove.Instance.GetStartPos = transform.position;
            KyleplayerMove.Instance.ResetPlayer();
            Destroy(gameObject);
        }

        _pStats = GetComponent<PlayerStats>();

        //_LastAttack = _BaseAttack;

        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        _player = ReInput.players.GetPlayer(_playerId);

        // Get the character controller
        _cc = GetComponent<CharacterController>();

        if (GetComponent<Animator>())
        {
            _myAnimations = GetComponent<Animator>();
        }

        _startPos = transform.position;
        _sword = transform.GetChild(0).gameObject;
        _swordReset = _sword.transform.localPosition;
        _enemyHit = new List<AIEnemy>();
        PlayerLevelStart();
    }

    public void PlayerLevelStart()
    {
        Debug.Log("level start called");
        transform.position = _startPos;
        _canvasRef = PlayerCanvas.Instance;
        _ComboPartsParent = _canvasRef.transform.GetChild(0).gameObject;
        _ComboText = _ComboPartsParent.transform.GetChild(0).GetComponent<Text>();
        _DecayBar = _ComboPartsParent.transform.GetChild(1).GetComponent<Image>();
        _pStats.FindHealthBar();

        _pauseRef = PauseMenu.Instance;
        _levelStartCycloneUnlock = _cycloneIsUnlocked;
        _levelStartDashUnlock = _dashIsUnlocked;

    }

    public void ResetPlayer()
    {
        Debug.Log("called reset");
        _cycloneIsUnlocked = _levelStartCycloneUnlock;
        _dashIsUnlocked = _levelStartDashUnlock;
        ResetSword();
        ResetAbilities();
        PlayerLevelStart();
    }

    void Update()
    {
        if (!_pauseRef.GameIsPaused)
        {
            if (!_attacking)
            {
                CheckForAttack();
            }
            else if (_usingSpecial)
            {
                SpecialAbilityActive();
            }
            else
            {
                CheckForCombo();
                Attack();
            }

            DecayCombo();
            //GetAttack();
            //ProcessAttack();

            if (_canMove)
            {
                CheckFall();
                GetInput();
                ProcessMove();
                ProcessDash();
            }
            else if (_isDashing)
            {
                ProcessDash();
            }
        }
        else
        {
            CheckMenuInput();
        }

    }

    void CheckMenuInput()
    {
        float _menuMove = _player.GetAxis("Move Vertical");

        if (_menuMove > 0)
        {
            _pauseRef.MenuMovement(true);
        }
        else if (_menuMove < 0)
        {
            _pauseRef.MenuMovement(false);
        }

        bool _buttonPressed = _player.GetButtonDown("Interact");

        if (_buttonPressed)
        {
            _pauseRef.ButtonPush();
        }
    }

    private void CheckFall()
    {
        RaycastHit hit;
        if (!_cc.isGrounded && !_isDashing)
        { //if there is nothing below the player
            _cc.Move(Vector3.down * _gravity); //fall at rate gravity
            //Debug.Log("I'M FREE FALLIN");
        } 
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) //if there is something below the player
        {
            //Debug.Log("Hit Thing Below Me");
            if (hit.transform.tag == "Death") //if it is not meant to kill you
            {
                //Debug.Log("Time to Reset");
                checkPoint(); //teleport to last position
            }
            else //it is meant to kill you
            {
                //Debug.Log("Set Checkpoint");
                _lastPos = transform.position; //set the last position
            }
            if (hit.transform.tag == "DOT")
            {
                //Debug.Log("Ow");
                _pStats.PDamage(hit.transform.GetComponent<DOT>()._DamagePerTick);
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
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 1f))
        {
            if (hit.collider.GetComponent<ProgressionLighting>())
            {
                hit.collider.GetComponent<ProgressionLighting>().TurnOnTorch();
            }
            else if (hit.collider.GetComponent<WinCondition>())
            {
                _pStats.Victory();
                _canvasRef.WipeCanvas();
                _canvasRef.SetGameReset = ResetPlayer;
                //PlayerLevelStart();
            }
        }
        if (Physics.Raycast(transform.position + Vector3.up, -transform.up, out hit, 1.5f))
        {
            if (hit.collider.GetComponent<InteractableObject>())
            {
                if (hit.collider.GetComponent<InteractableObject>().GetPuzzleManager.GetPzType() == PzType.StepPz)
                {
                    Debug.Log("Stepped On.");
                    hit.collider.GetComponent<InteractableObject>().Interact();
                }

                //hit.collider.GetComponent<InteractableObject>().
            }
        }
        if (_moveVector == Vector3.zero) //set sprinting to 1 if not moving
        {
            _sprinting = false;
        }
        if (_sprint && _canSprint) //process sprint
        {
            _sprinting = true;
        }
        if (_moveVector.x != 0.0f || _moveVector.z != 0.0f) //move player
        {
            float _move = _moveSpeed;
            if (_sprinting)
            {
                _move = _sprintSpeed;
            }
            transform.rotation = Quaternion.LookRotation(_moveVector);
            _cc.Move(_moveVector.normalized * _move * Time.deltaTime);
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

    //checks all inputs to see if they are attacks or special abilites
    private void CheckForAttack()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");
        _CycloneAttack = _player.GetButtonDown("Ability3");
        _DashStrike = _player.GetButtonDown("Ability2");

        if (_LightAttack)
        {
            if (_myAnimations)
            {
                _myAnimations.Play("LightAttack1", 0);
            }
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
            if (_pStats.GetHealth() > _cycloneHealthBurden || !_AbilitiesCostHealth)
            {
                Debug.Log("cyclone Active");
                if (_AbilitiesCostHealth)
                {
                    _pStats.PDamage(_cycloneHealthBurden);
                }
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
        else if (_DashStrike && _dashIsUnlocked)
        {
            if (_pStats.GetHealth() > _dashStrikeHealthBurden || !_AbilitiesCostHealth)
            {
                Debug.Log("Dash Strike Active");
                if (_AbilitiesCostHealth)
                {
                    _pStats.PDamage(_dashStrikeHealthBurden);
                }
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

    //function that dictates what ability is being used
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
                if (_chargingDashStrike)
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

    //resets everything about the player using any ability and makes the player to a basic state
    public void ResetAbilities()
    {
        _myability = SpecialAbility.NONE;
        _sword.transform.localPosition = _swordReset;
        if (_enemyIAmRamming != null)
        {
            _enemyHit = new List<AIEnemy>();
            _enemyIAmRamming = null;
        }
    }

    //called when player is actually using the cyclone ability
    //spins player around and hits all enemies around them
    //player heals for every 3 enemies they hit
    private void UsingCyclone()
    {
        _currComboTime = (Time.time - _startComboTime) / _cycloneDuration;


        if (_currComboTime < 1)
        {
            if (_debugCyclone)
            {
                Debug.DrawLine(transform.position, transform.position + (transform.forward * _cycloneDetectionDistance));
            }

            if (Physics.Raycast(_sword.transform.position, _sword.transform.up, out hit, _cycloneDetectionDistance))
            {
                if (hit.collider.GetComponent<AIEnemy>())
                {
                    AIEnemy EnemyHit = hit.collider.GetComponent<AIEnemy>();

                    //Debug.Log("hit");

                    if (_enemyHit.Count > 0)
                    {
                        if (!CycloneAlreadyHitEnemy(EnemyHit))
                        {
                            if(EnemyHit.GotHit(_cycloneAttackDamage, transform.forward * _cycloneKnockBack, hit.point))
                            {
                                Debug.Log("cyclone hit");
                                _enemyHit.Add(EnemyHit);
                                ContinueCombo();
                                if (_enemyHit.Count > 2)
                                {
                                    _enemyHit = new List<AIEnemy>();
                                    Debug.Log("cyclone heal");
                                    _pStats.PHeal(_cycloneHeal);

                                }
                            }
                        }
                    }
                    else
                    {
                        if(EnemyHit.GotHit(_cycloneAttackDamage, transform.forward * _cycloneKnockBack, hit.point))
                        {
                            Debug.Log("cyclone hit");
                            ContinueCombo();
                            _enemyHit.Add(EnemyHit);
                        }
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

    //what checks to see if the player has hit an enemy already hit by cyclone 
    //so that it wont hit them again 
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

    //start up of the charge dash ability
    //player reels back during this being called
    private void ChargingDashStrike()
    {
        if (_dashStrikeChargeUpDuration > 0)
        {
            _currComboTime = (Time.time - _startComboTime) / _dashStrikeChargeUpDuration;
        }

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
            if (Physics.Raycast(transform.position + Vector3.up, -transform.forward, out hit, _swordDetectDistance))
            {
                c1 = transform.position;
            }
        }
    }

    //actual Dash attack
    //player flings forward and grabs the first enemy
    //then pushes any other enemies out of the way
    //if the player pins an enemy to a wall it stuns the enemy and heals the player
    private void UsingDashStrike()
    {
        _currComboTime = (Time.time - _startComboTime) / _dashStrikeForwardDashDuration;

        if (_currComboTime < 1)
        {
            if (_enemyIAmRamming == null)
            {
                if (_debugDashStrike)
                {
                    Debug.DrawLine(transform.position + Vector3.up, transform.position + (transform.forward * _dashStrikeInitialDetectionDistance) + Vector3.up);
                }

                if (Physics.Raycast(transform.position + Vector3.up, _sword.transform.up, out hit, _dashStrikeInitialDetectionDistance))
                {
                    GameObject thingHit = hit.collider.gameObject;

                    if (thingHit.GetComponent<AIEnemy>())
                    {
                        _enemyIAmRamming = thingHit;
                        _enemyIAmRamming.transform.parent = _sword.transform;
                        _enemyIAmRamming.GetComponent<AIEnemy>().GotDashStruck(_dashStrikeAttackDamage);
                        ContinueCombo();
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
                    Vector3 RayPos = new Vector3(transform.position.x + ((transform.right.x * _swordReset.x) * i), transform.position.y + 1f, transform.position.z);

                    if (_debugDashStrike)
                    {
                        Debug.DrawLine(RayPos, RayPos + (transform.forward * _dashStrikeDetectionDistanceWhileRammingAnEnemy));
                    }

                    if (Physics.Raycast(RayPos, _sword.transform.up, out hit, _dashStrikeDetectionDistanceWhileRammingAnEnemy))
                    {

                        GameObject thingHit = hit.collider.gameObject;
                        if (thingHit.GetComponent<AIEnemy>())
                        {
                            Vector3 _staggerDirection;
                            if (i == -1)
                            {
                                _staggerDirection = -transform.right * _dashStrikeKnockBack;
                            }
                            else
                            {
                                _staggerDirection = transform.right * _dashStrikeKnockBack;
                            }
                            if(thingHit.GetComponent<AIEnemy>().GotHit(_dashStrikeAttackDamage, _staggerDirection, hit.point))
                            {
                                ContinueCombo();
                            }
                        }
                        else if (!thingHit.GetComponent<projectileRanged>())
                        {
                            _myability = SpecialAbility.NONE;
                            _sword.transform.localPosition = _swordReset;
                            _enemyHit = new List<AIEnemy>();
                            if (_enemyIAmRamming != null)
                            {
                                _enemyIAmRamming.GetComponent<AIEnemy>().GotPinned(_dashStrikeAttackDamage);
                                _pStats.PHeal(_dashStrikeHeal);
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
            if (_enemyIAmRamming != null)
            {
                Debug.Log("yup its not null");
                _enemyIAmRamming.GetComponent<AIEnemy>().ResetState();
                _enemyIAmRamming.GetComponent<AIEnemy>().GotHit(0, (transform.forward * _dashStrikeKnockBack), hit.point);
                ContinueCombo();
                _enemyIAmRamming = null;
            }
        }

        Vector3 p01;
        p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;

        transform.position = p01;
    }

    //checks to see if the player is comboing so that it can move the sword accordingly
    private void CheckForCombo()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");

        if (_comboing)
        {
            if (_currComboNum > _myLightComboPos.Count || _currComboNum > _myHeavyComboPos.Count)
            {
                _currComboNum = 0;
            }
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

    //processes basic attacks
    private void Attack()
    {
        if (!_swinging)
        {
            if (_nextComboTransform == _myLightComboPos[_currComboNum])
            {
                _currSwingDuration = _lightSwingDuration;
                _currDamage = _lightAttackDamage;
            }
            else if (_nextComboTransform == _myHeavyComboPos[_currComboNum])
            {
                _currSwingDuration = _heavySwingDuration;
                _currDamage = _heavyAttackDamage;
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
                if (Physics.Raycast(_sword.transform.position, _sword.transform.up, out hit, _swordDetectDistance))
                {
                    if (hit.collider.GetComponent<AIEnemy>())
                    {
                        if (hit.collider.GetComponent<AIEnemy>().GotHit(_currDamage, transform.forward, hit.point))
                        {
                            Debug.Log("hit");
                            ContinueCombo();
                            _hitSomething = true;
                        }
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

            if (_currComboTime >= .6f && _currComboTime <= .95f)
            {
                _comboing = true;
            }
            else
            {
                _comboing = false;
            }
        }
    }

    //resets all the player attack variables and puts the sword back to its basic position
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

    private void ContinueCombo()
    {
        if (!_ComboPartsParent.activeSelf)
        {
            _ComboPartsParent.SetActive(true);
        }
        _currTotalCombo++;
        Debug.Log("Current combo = " + _currTotalCombo);
        _TimeComboStart = Time.time + _TimeForComboToDecay;
        _ComboText.text = _currTotalCombo + " Hits";
    }

    private void DecayCombo()
    {
        float ratio = ((_TimeComboStart - Time.time) / _TimeForComboToDecay);
        _DecayBar.fillAmount = ratio;
        if (ratio <= 0)
        {
            Debug.Log("combo eneded");

            EndCombo();
        }
    }

    public void EndCombo()
    {
        _currTotalCombo = 0;
        _TimeComboStart = Time.time;
        _ComboText.text = "No Hits";
        _ComboPartsParent.SetActive(false);
    }

    public SpecialAbility GetCurrAbility { get { return _myability; } }
    public Vector3 GetStartPos { get { return _startPos; } set { _startPos = value; } }
    public bool AmIInvincible { get { return _invincible; } }
    public float GetLightDamage { get { return _lightAttackDamage; } set { _lightAttackDamage = value; } }
    public float GetHeavyDamage { get { return _heavyAttackDamage; } set { _heavyAttackDamage = value; } }
    public float GetMoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float GetCycloneDamage { get { return _cycloneAttackDamage; } set { _cycloneAttackDamage = value; } }
    public float GetDashDamage { get { return _dashStrikeAttackDamage; } set { _dashStrikeAttackDamage = value; } }
    public bool GetCostHealth { get { return _AbilitiesCostHealth; } set { _AbilitiesCostHealth = value; } }
    public int GetCurrentCombo { get { return _currTotalCombo; } }

    public bool GetCycloneUnlock { get { return _cycloneIsUnlocked; } set { _cycloneIsUnlocked = value; } }
    public bool GetDashStrikeUnlock { get { return _dashIsUnlocked; } set { _dashIsUnlocked = value; } }
}