using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour {

    NavMeshAgent _enemyAgent;

    [Header("Enemy Stats")]
    [SerializeField]
    float _enemyHealth;
    [SerializeField]
    float _enemyDamage;
    [SerializeField]
    float _damageDistance;
    Vector3 _startPoint;

    [Header("Vision Variables")]
    [SerializeField]
    float _sightDistance;
    [SerializeField]
    float _sightArea;
    [SerializeField]
    int _numOfCasts;
    [SerializeField]
    bool _debugVision = false;

    [Header("Player Tracking Variables")]
    [SerializeField]
    float _followDistanceThreshold;
    [SerializeField]
    float _attackDistanceThreshold;
    bool _attacking = false;
    bool _showingTheTell = false;
    bool _waiting = false;
    bool _hit = false;
    bool _canTakeDamage = true;
    bool _dead = false;
    Vector3 _deadLook;
    float _startAttackTime;
    float _currentAttackTime;
    Vector3 c0, c1, c2;

    [Header("Time Variables")]
    [SerializeField]
    float _attackTellDuration;
    [SerializeField]
    float _attackSwingDuration;
    [SerializeField]
    float _durationBetweenAttacks;
    [SerializeField]
    float _knockedBackDuration;
    [SerializeField]
    float _deathDuration;

    GameObject _sword;
    Vector3 _swordPos;

    [Header("Patrol Variables")]
    [SerializeField]
    List<GameObject> _patrolPoints;
    List<Vector3> _patrolRoute;
    int _currPath;
    bool _alerted = false;
    
    RaycastHit hit;
    KyleplayerMove _player;

	// Use this for initialization
	void Start () {
        _enemyAgent = GetComponent<NavMeshAgent>();
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

        //target = TestingSightScript.tester;
	}

    // Update is called once per frame
    void Update () {
        if(!_dead)
        {
            if (!_hit)
            {
                if (!_alerted)
                {
                    PatrolState();
                }
                else
                {
                    //CombatStrats();
                }
            }
            else
            {
                KnockedBack();
            }           
        }
        else
        {
            Die();
        }
	}

    private void PatrolState()
    {
        if (LookingForPlayer())
        {
            _sword.SetActive(true);
            _alerted = true;
        }

        if (!_enemyAgent.hasPath)
        {
            _enemyAgent.SetDestination(_patrolRoute[_currPath]);
            if (_currPath < _patrolRoute.Count - 1)
            {
                _currPath++;
            }
            else
            {
                _currPath = 0;
            }
        }
    }

    private bool LookingForPlayer()
    {
        for (int currCast = 0; currCast < _numOfCasts; currCast++)
        {
            Vector3 _castDir = transform.forward + (transform.right * ((_sightArea - ((_sightArea/((_numOfCasts-1)/2)) * currCast)) / 100));
            if(_debugVision)
            {
                Debug.DrawLine(transform.position, transform.position + (_castDir * _sightDistance));
            }

            if (Physics.Raycast(transform.position, _castDir, out hit, _sightDistance))
            {
                if(hit.collider.GetComponent<KyleplayerMove>())
                {
                    _player = hit.collider.GetComponent<KyleplayerMove>();
                    return true;
                }
            }
        }
        return false;
    }

    private void CombatStrats()
    {
        if(Vector3.Distance(transform.position, _player.transform.position) <= _followDistanceThreshold)
        {
            if(Vector3.Distance(transform.position, _player.transform.position) <= _attackDistanceThreshold)
            {
                if(!_attacking)
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

    private void FollowPlayer()
    {
        if(_attacking || _waiting || _showingTheTell)
        {
            ResetState();
        }
        _enemyAgent.SetDestination(_player.transform.position);
    }

    private void AttackTell()
    {   
        //lerpingWeapon
        if(!_showingTheTell)
        {
             _enemyAgent.isStopped = true;
            c0 = _swordPos;
            c1 = _swordPos + Vector3.up;
            _startAttackTime = Time.time;
            _showingTheTell = true;
        }
        else
        {
            _currentAttackTime = (Time.time - _startAttackTime) / _attackTellDuration;

            if (_currentAttackTime > 1)
            {
                _currentAttackTime = 1;

                c0 = transform.position;
                c1 = _player.transform.position - transform.forward;
                c1.y = transform.position.y;
                _attacking = true;
                _sword.transform.localPosition = _swordPos;
                _startAttackTime = Time.time;
            }
            else
            {
                Vector3 p01;

                p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

                _sword.transform.localPosition = p01;
            }
        }
    }

    private void AttackPlayer()
    {
        if (_waiting)
        {
            //Debug.Log("waiting");
            _currentAttackTime = (Time.time - _startAttackTime) / _durationBetweenAttacks;

            if(_currentAttackTime > 1)
            {
                _currentAttackTime = 1;

                _waiting = false;
                _attacking = false;
                _showingTheTell = false;
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
        else
        {
            //Debug.Log("attacking");
            //transform.LookAt(_player.transform.position);
            _currentAttackTime = (Time.time - _startAttackTime) / _attackSwingDuration;

            if (_currentAttackTime > 1)
            {
                _currentAttackTime = 1;

                _waiting = true;
                c0 = transform.position;
                c1 = transform.position;
                _startAttackTime = Time.time;
            }
            else
            {
                transform.LookAt(_player.transform.position);
                if(Physics.Raycast(transform.position, transform.forward, out hit, _damageDistance))
                {
                    if(hit.collider.GetComponent<PlayerStats>())
                    {
                        hit.collider.GetComponent<PlayerStats>().PDamage(_enemyDamage);
                        Debug.Log("hit player");
                        _waiting = true;
                        c0 = transform.position;
                        c1 = transform.position;
                        _startAttackTime = Time.time;
                    }
                }
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
    }

    private void LostSightOfPlayer()
    {
        _alerted = false;
        if (_attacking || _waiting || _showingTheTell)
        {
            ResetState();
        }

        _sword.SetActive(false);
        _enemyAgent.SetDestination(_patrolRoute[_currPath]);
    }

    private void ResetState()
    {
        _showingTheTell = false;
        _sword.transform.localPosition = _swordPos;
        _attacking = false;
        _waiting = false;
        _enemyAgent.isStopped = false;
    }

    //For Update 2
    public void GotHit(float _damageRecieved, Vector3 _flydir)
    {
        if(_canTakeDamage)
        {
            Debug.Log("hit");
            _canTakeDamage = false;

            _startAttackTime = Time.time;
            c0 = transform.position;
            c1 = transform.position + _flydir;

            _hit = true;

            _enemyHealth -= _damageRecieved;
            if (_enemyHealth <= 0)
            {
                DeadActivate(_flydir);
            }
        }
    }

    private void KnockedBack()
    {
        _currentAttackTime = (Time.time - _startAttackTime) / _knockedBackDuration;

        if (_currentAttackTime >= 1)
        {
            _currentAttackTime = 1;

            ResetState();
            _hit = false;
        }
        else if(_currentAttackTime >= .4f)
        {
            Debug.Log("can get hit again");
            _canTakeDamage = true;
        }

        if(_currentAttackTime < 1)
        {
            if(Physics.Raycast(transform.position, -transform.forward, _damageDistance))
            {
                ResetState();
                _canTakeDamage = true;
                _hit = false;
            }
        }

        Vector3 p01;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

        transform.position = p01;
    }

    private void DeadActivate(Vector3 _dirToDie)
    {
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        _enemyAgent.isStopped = false;
        _canTakeDamage = false;
        
        c0 = transform.position;
        c1 = transform.position + _dirToDie;
        c2 = transform.position + _dirToDie + (Vector3.down);
        _startAttackTime = Time.time;
        _enemyAgent.enabled = false;
        _dead = true;
    }

    private void Die()
    {
        _currentAttackTime = (Time.time - _startAttackTime) / _deathDuration;

        if(_currentAttackTime >= 1)
        {
            _currentAttackTime = 1;

            gameObject.SetActive(false);
        }

        Vector3 p01, p12, p012;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;
        p12 = (1 - _currentAttackTime) * c1 + _currentAttackTime * c2;
        p012 = (1 - _currentAttackTime) * p01 + _currentAttackTime * p12;

        transform.position = p012;
        transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
    }
}
