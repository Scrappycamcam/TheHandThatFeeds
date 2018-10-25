using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class BerzerkMode : MonoBehaviour {

    [Header("Berzerk Variables")]
    [SerializeField]
    int _numKillsToCharge = 10;
    [SerializeField]
    float _numKillsSoFar = 0;
    [SerializeField]
    float _newSpeed = 8f;
    [SerializeField]
    float _newLightDamage = 15f;
    [SerializeField]
    float _newHeavyDamage = 20f;
    [SerializeField]
    float _newCycloneDamage = 25f;
    [SerializeField]
    float _newDashDamage = 25f;
    [SerializeField]
    float _BerzerkLength;

    private float _origSpeed;
    private float _origLightDamage;
    private float _origHeavyDamage;
    private float _origCycloneDamage;
    private float _origDashDamage;

    private float _BerzerkTimer;
    private Player _player;
    private KyleplayerMove _playerMove;
    private bool _berzerk = false;
    private bool _berzerking = false;
    private bool _isCharged = false;
    private Transform player;
    private Vector3 _playerPos;
    public Image _currentBerzerkBar;

    [SerializeField]
    float _vertOffset;

    Camera _playerCam;

    private void Awake()
    {
        _playerCam = FindObjectOfType<camera>().gameObject.GetComponent<Camera>();
        player = GameObject.Find("Player").transform;
        _player = ReInput.players.GetPlayer(0);
        _playerMove = KyleplayerMove.Instance;
    }
	
	// Update is called once per frame
	void Update ()
    {
        _playerPos = _playerCam.WorldToScreenPoint(player.transform.position);
        transform.position = _playerPos + (Vector3.up * _vertOffset);
        if (!_berzerking)
        {
            if (!_isCharged)
            {
                CheckForFull();
            }
            else
            {
                CheckForStart();
            }
        }
        else
        {
            CheckForStop(); 
        }
    }

    private void CheckForFull()
    {
        if(_numKillsSoFar > _numKillsToCharge)
        {
            _numKillsSoFar = _numKillsToCharge;
        }
        float ratio = (float)((float)_numKillsSoFar/(float)_numKillsToCharge); //creates the berzerk ratio
        _currentBerzerkBar.fillAmount = ratio; // sets the scale transform for the berzerk bar
        //Debug.Log(ratio);
        if (ratio == 1 && !_isCharged)
        {
            _isCharged = true;
        }
    }

    private void CheckForStart()
    {
        _berzerk = _player.GetButtonDown("Ability4");
        if (_berzerk)
        {
            StartBerzerk();
        }
    }

    private void StartBerzerk()
    {

        _origLightDamage = _playerMove.GetLightDamage;
        _origHeavyDamage = _playerMove.GetHeavyDamage;
        _origSpeed = _playerMove.GetMoveSpeed;
        _origCycloneDamage = _playerMove.GetCycloneDamage;
        _origDashDamage = _playerMove.GetCycloneDamage;

        _playerMove.GetCostHealth = false;
        _playerMove.GetLightDamage = _newLightDamage;
        _playerMove.GetHeavyDamage = _newHeavyDamage;
        _playerMove.GetMoveSpeed = _newSpeed;
        _playerMove.GetCycloneDamage = _newCycloneDamage;
        _playerMove.GetDashDamage = _newDashDamage;

        _berzerking = true;
        _BerzerkTimer = Time.time + _BerzerkLength;
    }

    private void CheckForStop()
    {
        float ratio = (float)((_BerzerkTimer - Time.time) / _BerzerkLength);
        _currentBerzerkBar.fillAmount = ratio;
        if(ratio <= 0)
        {
            ratio = 0;
            _numKillsSoFar = 0;
            _playerMove.GetCostHealth = true;
            _playerMove.GetLightDamage = _origLightDamage;
            _playerMove.GetHeavyDamage = _origHeavyDamage;
            _playerMove.GetMoveSpeed = _origSpeed;
            _playerMove.GetCycloneDamage = _origCycloneDamage;
            _playerMove.GetDashDamage = _origDashDamage;
            _berzerking = false;
            _isCharged = false;
        }
    }

    public void EnemyDied(int killWorth)
    {
        float combo = (float)_playerMove.GetCurrentCombo;
        Debug.Log(combo);
        float mult = Mathf.Log(combo + 10, 4);
        Debug.Log(mult);
        _numKillsSoFar += (float)(killWorth*mult);
        Debug.Log(_numKillsSoFar);
    }
}
