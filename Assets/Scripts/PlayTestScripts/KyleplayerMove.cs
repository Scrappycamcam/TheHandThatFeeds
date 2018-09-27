using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class KyleplayerMove : MonoBehaviour
{

    private int _playerId = 0; // The Rewired player id of this character

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

    public Attack _BaseAttack;
    private Attack _LastAttack;
    private Attack _NextAttack;

    private PlayerStats _pStats;
    private Player _player; // The Rewired Player
    private CharacterController _cc;
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _LastMove;
    private bool _dash;
    private bool _sprint;
    private bool _LightAttack;
    private bool _HeavyAttack;
    private float _sprinting = 1f;
    private float _nextDash = 0f; // keeps track of when next dash can take place

    bool _attacking = false;
    bool _comboing = false;
    bool _lookingForNext = false;
    Vector3 c0, c1, c2;
    [SerializeField]
    List<Transform> _myLightComboPos;
    [SerializeField]
    List<Transform> _myHeavyComboPos;

    GameObject _sword;
    Transform _swordReset;
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

    void Awake()
    {
        _pStats = GetComponent<PlayerStats>();

        _LastAttack = _BaseAttack;

        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        _player = ReInput.players.GetPlayer(_playerId);

        // Get the character controller
        _cc = GetComponent<CharacterController>();

        _sword = transform.GetChild(0).gameObject;
        _swordReset = _sword.transform;
    }

    void Update()
    {
        CheckFall();

        if(!_attacking)
        {
            CheckForAttack();
        }
        else
        {
            CheckForCombo();
        }

        Attack();

        //GetAttack();
        //ProcessAttack();

        if (_canMove)
        {
            GetInput();
            ProcessMove();
        }
    }

    private void CheckFall()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, 1f)){
            _cc.Move(Vector3.down * _gravity);
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
            _LastMove = _moveVector.normalized;
        }
        if (_moveVector == Vector3.zero) //set sprinting to 1 if not moving
        {
            _sprinting = 1f;
        }
        if (_sprint) //process sprint
        {
            _sprinting = 1.5f;
        }
        if (_dash && Time.time > _nextDash)//process dash
        {
            _cc.Move(_moveVector.normalized * _dashspeed);
            _nextDash = Time.time + _dashDelay;
        }
    }

    private void CheckForAttack()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");

        if (_LightAttack)
        {
            _currComboNum = 0;
            _nextComboTransform = _myLightComboPos[_currComboNum];
            _attacking = true;
        }
        else if (_HeavyAttack)
        {
            _currComboNum = 0;
            _nextComboTransform = _myHeavyComboPos[_currComboNum];
            _attacking = true;
        }
    }

    private void CheckForCombo()
    {
        _LightAttack = _player.GetButtonDown("LightAttack");
        _HeavyAttack = _player.GetButtonDown("HeavyAttack");

        if (_LightAttack)
        {
            _nextComboTransform = _myLightComboPos[_currComboNum];
        }
        else if (_HeavyAttack)
        {
            _nextComboTransform = _myHeavyComboPos[_currComboNum];
        }
    }

    private void Attack()
    {
        if(!_comboing && _attacking)
        {
            if(_LightAttack)
            {
                _currSwingDuration = _lightSwingDuration;
                _currComboNum++;
                if (_currComboNum >= _myLightComboPos.Count)
                {
                    _attacking = false;
                }
            }
            else if(_HeavyAttack)
            {
                _currSwingDuration = _heavySwingDuration;
                _currComboNum++;
                if (_currComboNum >= _myHeavyComboPos.Count)
                {
                    _attacking = false;
                }
            }
            _currComboTransform = _nextComboTransform;
            c0 = _sword.transform.localPosition;
            c1 = ((_sword.transform.localPosition + _currComboTransform.localPosition)/2) + transform.forward; 
            c2 = _currComboTransform.localPosition;
            _startComboTime = Time.time;
            _comboing = true;
        }
        else if(_comboing)
        {
            _currComboTime = (Time.time - _startComboTime) / _currSwingDuration;

            if(_currComboTime >= 1)
            {
                _currComboTime = 1;

                _comboing = false;

            }

            Vector3 p01, p12, p012;
            p01 = (1 - _currComboTime) * c0 + _currComboTime * c1;
            p12 = (1 - _currComboTime) * c1 + _currComboTime * c2;

            p012 = (1 - _currComboTime) * p01 + _currComboTime * p12;

            _sword.transform.localPosition = p012;
            _sword.transform.localRotation = _currComboTransform.localRotation;
        }

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
}