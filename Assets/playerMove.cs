using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class playerMove : MonoBehaviour
{

    public int _playerId = 0; // The Rewired player id of this character

    public float _moveSpeed = 3.0f;
    public float _dashspeed = 8f;
    public float _dashDelay = 0.8f;

    private Player _player; // The Rewired Player
    private CharacterController _cc;
    private Vector3 _moveVector;
    private bool _dash; 
    private float _nextDash = 0f; // keeps track of when next dash can take place

    void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        _player = ReInput.players.GetPlayer(_playerId);

        // Get the character controller
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        GetInput();
        ProcessInput();
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

        _moveVector.x = _player.GetAxis("Move Horizontal"); // get input by name
        _moveVector.z = _player.GetAxis("Move Vertical");
        _dash = _player.GetButtonDown("Dash");
    }

    private void ProcessInput()
    {   //process dash
        if (_dash && Time.time > _nextDash)
        {
            _cc.Move(_moveVector * _dashspeed);
            _nextDash = Time.time + _dashDelay;
        }
        // Process movement
        if (_moveVector.x != 0.0f || _moveVector.y != 0.0f)
        {
            _cc.Move(_moveVector.normalized * _moveSpeed * Time.deltaTime);
        }
    }
}