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
    SACRIFICING,
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

    protected Vector3 _BossPos;

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
            if (_patrolPoints[point])
            {
                _patrolRoute.Add(_patrolPoints[point].gameObject.transform.position);
            }
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

        if (LookingForPlayer())
        {
            _sword.SetActive(true);
            _myCurrState = AIState.ALERTED;
            //_alerted = true;
        }

        if (!_enemyAgent.hasPath)
        {
            if (_currPath < _patrolRoute.Count - 1)
            {
                _currPath++;
            }
            else
            {
                _currPath = 0;
            }
            _enemyAgent.SetDestination(_patrolRoute[_currPath]);
        }
    }

    protected virtual bool LookingForPlayer()
    {
        for (int currCast = 0; currCast < _numOfCasts; currCast++)
        {
            Vector3 _castDir = transform.forward + (transform.right * ((_sightArea - ((_sightArea / ((_numOfCasts - 1) / 2)) * currCast)) / 100));
            if (_debugVision)
            {
                Debug.DrawLine(transform.position, transform.position + (_castDir * _sightDistance));
            }

            if (Physics.Raycast(transform.position, _castDir, out hit, _sightDistance))
            {
                if (hit.collider.GetComponent<KyleplayerMove>())
                {
                    _mySquad.AlertSquad(_player);
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual void CombatStrats()
    {

        if (Vector3.Distance(transform.position, _player.transform.position) <= _followDistanceThreshold)
        {
            if (!_sword.activeInHierarchy)
            {
                _sword.SetActive(true);
            }
            if (Vector3.Distance(transform.position, _player.transform.position) <= _attackDistanceThreshold)
            {
                if (!_attacking)
                {
                    AttackTell();
                }
                else
                {
                    AttackPlayer();
                }
            }
            else
            {
                FollowPlayer();
            }
        }
        else
        {
            LostSightOfPlayer();
        }
    }

    protected virtual void FollowPlayer()
    {

        if (_attacking || _waiting || _showingTheTell)
        {
            ResetState();
        }
        _enemyAgent.SetDestination(_player.transform.position);
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
        _sword.transform.localPosition = _swordPos;
        GetComponent<CapsuleCollider>().enabled = true;
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        //_stunned = false;
        //_slammed = false;
        _canTakeDamage = true;
        _enemyAgent.isStopped = false;
        _myCurrState = AIState.ALERTED;
    }

    //For Update 2
    public virtual bool GotHit(float _damageRecieved, Vector3 _knockbackdir, Vector3 _particleHitPos)
    {
        transform.parent = null;
        if (_canTakeDamage)
        {
            //Debug.Log("hit");
            _canTakeDamage = false;

            Debug.Log("gothit");
            c0 = transform.position;
            c1 = transform.position + _knockbackdir;

            _startAttackTime = Time.time;
            //_hit = true;
            _myCurrState = AIState.HIT;

            UpdateHealth(_damageRecieved);
            ShowBlood(_particleHitPos);

            if (_currEnemyHealth <= 0)
            {
                DeadActivate(_knockbackdir);
            }
            return true;
        }
        return false;
    }

    protected virtual void ShowBlood(Vector3 _bloodShowPos)
    {
        _bloodParticle.transform.position = _bloodShowPos;
        _bloodParticle.GetComponent<ParticleSystem>().Play();
    }

    protected virtual void UpdateHealth(float _damage)
    {
        _currEnemyHealth -= _damage;

        if (_currEnemyHealth < 0)
        {
            _currEnemyHealth = 0;
        }else if(_currEnemyHealth > _enemyHealth)
        {
            _currEnemyHealth = _enemyHealth;
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

        UpdateHealth(_damageRecieved);
        //_slammed = true;
        GetComponent<CapsuleCollider>().enabled = false;
        _enemyAgent.isStopped = true;
    }

    public virtual void GotPinned(float _damageRecieved)
    {
        c0 = transform.position;
        c1 = transform.position;
        ResetState();
        _enemyAgent.isStopped = true;
        _startStunTime = Time.time;
        transform.parent = null;
        _myCurrState = AIState.STUNNED;

        if (_currEnemyHealth <= 0)
        {
            DeadActivate(Vector3.zero);
        }
    }

    protected virtual void Stunned()
    {
        _currStunTime = (Time.time - _startStunTime) / _stunDuration;

        if (_currStunTime >= 1)
        {
            _currStunTime = 1;
            ResetState();
        }

        Vector3 p01;

        p01 = (1 - _currStunTime) * c0 + _currStunTime * c1;

        transform.position = p01;
    }

    protected virtual void KnockedBack()
    {

        _currentAttackTime = (Time.time - _startAttackTime) / _knockedBackDuration;

        if (_currentAttackTime >= 1)
        {
            _currentAttackTime = 1;
            ResetState();
            //_hit = false;
        }

        if (_currentAttackTime < 1)
        {
            /*if (Physics.Raycast(transform.position, -transform.forward, _damageDistance))
            {
                ResetState();
                _canTakeDamage = true;
                //_hit = false;
                _myCurrState = AIState.ALERTED;
            }*/
        }

        Vector3 p01;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

        transform.position = p01;
    }

    protected virtual void DeadActivate(Vector3 _dirToDie)
    {
        Debug.Log("activating death");
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        //_slammed = false;
        //_stunned = false;
        _enemyAgent.isStopped = false;
        _canTakeDamage = false;

        c0 = transform.position;
        c1 = transform.position + _dirToDie;
        c2 = transform.position + _dirToDie + (Vector3.down);
        _startAttackTime = Time.time;
        _enemyAgent.enabled = false;
        _myCurrState = AIState.DYING;
        Debug.Log(_myCurrState);
        Debug.Log(_init);

        //_dead = true;
    }

    protected virtual void Die()
    {
        Debug.Log("dying");
        _currentAttackTime = (Time.time - _startAttackTime) / _deathDuration;

        if (_currentAttackTime >= 1)
        {
            Debug.Log("done dying");
            _berserkRef.EnemyDied(1);
            if (gameObject.tag == "Boss" && _winRef)
            {
                _winRef.BossDied();
            }
            if (_winRef)
            {
                _winRef.EnemyDied();
            }
            _currentAttackTime = 1;

            _init = false;
            transform.parent = _mySquad.transform;
            gameObject.SetActive(false);
            _actualHealthBar.gameObject.SetActive(false);
        }

        Vector3 p01, p12, p012;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;
        p12 = (1 - _currentAttackTime) * c1 + _currentAttackTime * c2;
        p012 = (1 - _currentAttackTime) * p01 + _currentAttackTime * p12;

        transform.position = p012;
        transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
    }

    public virtual void MyReset()
    {
        _myCurrState = AIState.NOTALERTED;
        transform.parent = null;
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
    
    public virtual void Sacrifice(Vector3 Boss)
    {
        _myCurrState = AIState.SACRIFICING;
        _BossPos = Boss;
        if(gameObject.tag != "Boss")
        {
            _enemyAgent.SetDestination(_BossPos);
        }
    }

    protected virtual void Saccing()
    {
        _enemyAgent.SetDestination(_BossPos);
    }


    public bool GetDead { get { return _dead; } }
    public virtual AIState GetAIState { get{return _myCurrState;} set { _myCurrState = value; } }
    public virtual KyleplayerMove SetPlayer { set { _player = value; } }
}
