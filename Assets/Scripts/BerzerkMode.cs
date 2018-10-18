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
    int _numKillsSoFar = 0;
    [SerializeField]
    float _newSpeed = 8f;
    [SerializeField]
    int _newLightDamage = 15;
    [SerializeField]
    int _newHeavyDamage = 20;
    [SerializeField]
    int _newCycloneDamage = 25;
    [SerializeField]
    int _newDashDamage = 25;
    [SerializeField]
    float _BerzerkLength;

    private float _origSpeed;
    private int _origLightDamage;
    private int _origHeavyDamage;
    private int _origCycloneDamage;
    private int _origDashDamage;

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
        float ratio = (float)_numKillsSoFar/_numKillsToCharge; //creates the berzerk ratio
        _currentBerzerkBar.rectTransform.localScale = new Vector3(ratio, 1, 1); // sets the scale transform for the berzerk bar
        Debug.Log(ratio);
        if (ratio == 1)
        {
            _isCharged = true;
        }
    }

    private void CheckForStart()
    {
        _berzerk = _player.GetButtonDown("Berzerk");
        if (_berzerk)
        {
            StartBerzerk();
        }
    }

    private void StartBerzerk()
    {
        _berzerking = true;
        _BerzerkTimer = Time.time + _BerzerkLength;
    }

    private void CheckForStop()
    {
        float ratio = (_BerzerkTimer - Time.time) / _BerzerkTimer;
        _currentBerzerkBar.fillAmount = ratio;
        if(ratio <= 0)
        {
            _berzerking = false;
        }
    }

    public void EnemyDied(int killWorth)
    {
        _numKillsSoFar += killWorth;
    }
}
