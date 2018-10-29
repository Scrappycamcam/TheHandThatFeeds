﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum AIState
{
    PAUSED,
    NOTALERTED,
    ALERTED,
    HIT,
    STUNNED,
    DYING,
}

public class AIEnemy : MonoBehaviour {

    protected NavMeshAgent _enemyAgent;

    [Header("UI Variables")]
    [SerializeField]
    protected GameObject _healthBarPrefab;
    protected GameObject _mainCanvas;
    protected Image _actualHealthBar;
    [SerializeField]
    protected float _verticalOffset;
    protected Vector3 _HPBarPos;
    protected Camera _mainCam;
    protected PauseMenu _pauseRef;

    [Header("Enemy Stats")]
    [SerializeField]
    protected GameObject _bloodParticle;
    [SerializeField]
    protected float _enemyHealth;
    protected float _currEnemyHealth;
    [SerializeField]
    protected float _enemyDamage;
    [SerializeField]
    protected float _damageDistance;
    protected Vector3 _startPoint;
    protected Vector3 _screenPos;

    [Header("Vision Variables")]
    [SerializeField]
    protected float _sightDistance;
    [SerializeField]
    protected float _sightArea;
    [SerializeField]
    protected int _numOfCasts;
    [SerializeField]
    protected bool _debugVision = false;

    [Header("Player Tracking Variables")]
    [SerializeField]
    protected float _followDistanceThreshold;
    [SerializeField]
    protected float _attackDistanceThreshold;

    protected bool _attacking = false;
    protected bool _showingTheTell = false;
    protected bool _waiting = false;
    protected bool _hit = false;
    protected bool _slammed = false;
    protected bool _stunned = false;
    protected bool _canTakeDamage = true;
    protected bool _dead = false;
    protected Vector3 _deadLook;
    protected float _startAttackTime;
    protected float _currentAttackTime;
    protected Vector3 c0, c1, c2;

    [Header("Time Variables")]
    [SerializeField]
    protected float _attackTellDuration;
    [SerializeField]
    protected float _attackSwingDuration;
    [SerializeField]
    protected float _durationBetweenAttacks;
    [SerializeField]
    protected float _knockedBackDuration;
    [SerializeField]
    protected float _deathDuration;
    protected float _startStunTime;
    protected float _currStunTime;
    [SerializeField]
    protected float _stunDuration;
    protected float _TimeOffsetDuringPause;
    protected float _UnpauseTimeOffset;

    protected GameObject _sword;
    protected Vector3 _swordPos;

    [Header("Patrol Variables")]
    [SerializeField]
    protected List<GameObject> _patrolPoints;
    protected List<Vector3> _patrolRoute;
    protected int _currPath;
    protected bool _alerted = false;
    protected bool _init = false;

    protected RaycastHit hit;
    protected KyleplayerMove _player;
    protected EnemySquad _mySquad;
    protected BerzerkMode _berserkRef;
    protected WinCondition _winRef;

    protected AIState _myCurrState;
    protected AIState _myPreviousState;

    public virtual void Init()
    {
        _mainCanvas = PlayerCanvas.Instance.gameObject;
        _mainCanvas.GetComponent<PlayerCanvas>().SetGameReset += MyReset;
        _player = KyleplayerMove.Instance;
        _berserkRef = FindObjectOfType<BerzerkMode>();
        _winRef = FindObjectOfType<WinCondition>();
        _enemyAgent = GetComponent<NavMeshAgent>();
        _mySquad = GetComponentInParent<EnemySquad>();
        _pauseRef = PauseMenu.Instance;

        _patrolRoute = new List<Vector3>();
        for (int point = 0; point < _patrolPoints.Count; point++)
        {
            _patrolRoute.Add(_patrolPoints[point].gameObject.transform.position);
        }
        _startPoint = gameObject.transform.position;
        _patrolRoute.Add(_startPoint);
        _currPath = 0;

        _sword = transform.GetChild(0).gameObject;
        _swordPos = _sword.transform.localPosition;
        _sword.SetActive(false);
        _bloodParticle = transform.GetChild(1).gameObject;

        _mainCam = PlayerCamera.Instance.gameObject.GetComponent<Camera>();

        GameObject _healthBar = Instantiate<GameObject>(_healthBarPrefab, _mainCanvas.transform);
        _actualHealthBar = _healthBar.GetComponent<Image>();
        _actualHealthBar.gameObject.SetActive(false);
        _currEnemyHealth = _enemyHealth;

        _enemyAgent.SetDestination(_patrolRoute[_currPath]);
        _myCurrState = AIState.NOTALERTED;
        _init = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void PatrolState()
    {

    }

    protected virtual bool LookingForPlayer()
    {
        return false;
    }

    protected virtual void CombatStrats()
    {

    }

    protected virtual void FollowPlayer()
    {

    }

    protected virtual void AttackTell()
    {

    }

    protected virtual void AttackPlayer()
    {

    }

    protected virtual void LostSightOfPlayer()
    {

    }

    public virtual void ResetState()
    {

    }

    //For Update 2
    public virtual void GotHit(float _damageRecieved, Vector3 _knockbackdir, Vector3 _particleHitPos)
    {

    }

    protected virtual void ShowBlood(Vector3 _bloodShowPos)
    {

    }

    protected virtual void UpdateHealth(float _damage)
    {
        _currEnemyHealth -= _damage;

        if (_enemyHealth < 0)
        {
            _enemyHealth = 0;
        }

        _actualHealthBar.fillAmount = _currEnemyHealth / _enemyHealth;
    }

    protected virtual void ShowHealthBar()
    {
       
        if (!_actualHealthBar.gameObject.activeInHierarchy)
        {
            _actualHealthBar.gameObject.SetActive(true);
        }
        else
        {
            _screenPos = _mainCam.WorldToScreenPoint(transform.position + Vector3.up);
            _HPBarPos = _screenPos;
            _actualHealthBar.gameObject.transform.position = _HPBarPos + (Vector3.up * _verticalOffset);
        }
    }

    public virtual void GotDashStruck(float _damageRecieved)
    {

    }

    public virtual void GotPinned(float _damageRecieved)
    {

    }

    protected virtual void Stunned()
    {

    }

    protected virtual void KnockedBack()
    {

    }

    protected virtual void DeadActivate(Vector3 _dirToDie)
    {

    }

    protected virtual void Die()
    {

    }

    public virtual void PauseMe()
    {
        _TimeOffsetDuringPause = Time.time;
        _myPreviousState = _myCurrState;
        _myCurrState = AIState.PAUSED;
        _enemyAgent.enabled = false;
    }

    public virtual void UnPauseMe()
    {
        _UnpauseTimeOffset = Time.time - _TimeOffsetDuringPause;

        _myCurrState = _myPreviousState;
        switch (_myCurrState)
        {
            case AIState.ALERTED:
                _startAttackTime += _UnpauseTimeOffset;
                break;
            case AIState.HIT:
                _startAttackTime += _UnpauseTimeOffset;
                break;
            case AIState.STUNNED:
                _startStunTime += _UnpauseTimeOffset;
                break;
            case AIState.DYING:
                _startAttackTime += _UnpauseTimeOffset;
                break;
            default:
                break;
        }
        _enemyAgent.enabled = true;
    }

    public virtual void MyReset()
    {
        _myCurrState = AIState.NOTALERTED;
        gameObject.SetActive(true);
        //_dead = false;
        //_hit = false;
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        //_stunned = false;
        // _slammed = false;
        _canTakeDamage = true;

        _enemyAgent.enabled = true;
        _enemyAgent.isStopped = false;
        _currPath = 0;

        transform.position = _startPoint;
        _sword.transform.localPosition = _swordPos;
        _sword.SetActive(false);
        _actualHealthBar.fillAmount = 1;
        _currEnemyHealth = _enemyHealth;
        _actualHealthBar.gameObject.SetActive(true);

        _enemyAgent.SetDestination(_patrolRoute[_currPath]);
        _init = true;
    }
    
    public virtual AIState GetAIState { get{return _myCurrState;} set { _myCurrState = value; } }
    public virtual KyleplayerMove SetPlayer { set { _player = value; } }
}
