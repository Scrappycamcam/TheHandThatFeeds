using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class BerzerkMode : MonoBehaviour {

    public static BerzerkMode _instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
            
    }

    [Header("Berzerk Variables")]
    [SerializeField]
    int _numKillsToCharge;
    [SerializeField]
    int _numKillsSoFar;
    [SerializeField]
    float _multiplier = 1.2f;
    [SerializeField]
    float _BerzerkLength;

    private float _BerzerkTimer;
    private Player _player;
    private bool _berzerk;
    private bool _berzerking;
    private bool _isCharged;
    private Image _currentBerzerkBar;


	// Use this for initialization
	void Awake () {
		if(_instance == null)
        {
            _instance = this;
        }else if(_instance != this)
        {
            Destroy(gameObject);
        }
        _currentBerzerkBar = GameObject.Find("BerzerkBar").GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update ()
    {
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
        float ratio = _numKillsSoFar/_numKillsToCharge; //creates the berzerk ratio
        _currentBerzerkBar.rectTransform.localScale = new Vector3(ratio, 1, 1); // sets the scale transform for the berzerk bar
        if (ratio >= 1)
        {
            ratio = 1;
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
        _currentBerzerkBar.rectTransform.localScale = new Vector3(ratio, 1, 1);
        if(ratio <= 0)
        {
            _berzerking = false;
        }
    }
}
