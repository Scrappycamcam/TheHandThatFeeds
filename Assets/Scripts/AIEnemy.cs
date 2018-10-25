using System.Collections;
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
    protected bool _paused = false;
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

    protected AIState _myCurrState;
    protected AIState _myPreviousState;

    public virtual void Init()
    {

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
        _screenPos = _mainCam.WorldToScreenPoint(transform.position + Vector3.up);
        if (!_actualHealthBar.gameObject.activeInHierarchy)
        {
            _actualHealthBar.gameObject.SetActive(true);
        }
        _HPBarPos = _screenPos;
        _actualHealthBar.gameObject.transform.position = _HPBarPos + (Vector3.up * _verticalOffset);
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

    }
    
    public virtual AIState GetAIState { get{return _myCurrState;} set { _myCurrState = value; } }
    public virtual KyleplayerMove SetPlayer { set { _player = value; } }
}
