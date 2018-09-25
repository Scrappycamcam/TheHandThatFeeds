using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class playerMove : MonoBehaviour
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

    void Awake()
    {
        _pStats = GetComponent<PlayerStats>();

        _LastAttack = _BaseAttack;

        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        _player = ReInput.players.GetPlayer(_playerId);

        // Get the character controller
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckFall();
        GetAttack();
        ProcessAttack();
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

    private void GetAttack()
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
    }
}