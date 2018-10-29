using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIMovementRanged : AIEnemy {

    [Header("Ranged Variables")]
    public GameObject _projectile;
    public Transform _launchPos;

    [SerializeField]
    float _projSpeed;
    [SerializeField]
    float _maxRange;

    // Update is called once per frame
    protected override void Update()
    {
        if (_init)
        {
            ShowHealthBar();
            if(!_slammed)
            {
                if (!_hit)
                {
                    if (!_alerted)
                    {
                        PatrolState();
                    }
                    else
                    {
                        CombatStrats();
                    }
                }
                else
                {
                    if (!_dead)
                    {
                        KnockedBack();
                    }
                    else
                    {
                        Die();
                    }
                }
            }
            else
            {
                if(_stunned)
                {
                    Stunned();
                }
            }
        }
    }

    protected override void PatrolState()
    {
        if (LookingForPlayer())
        {
            _sword.SetActive(true);
            _alerted = true;
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

    protected override bool LookingForPlayer()
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
                    _player = hit.collider.GetComponent<KyleplayerMove>();
                    _mySquad.AlertSquad(_player);
                    return true;
                }
            }
        }
        return false;
    }

    protected override void CombatStrats()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= _followDistanceThreshold)
        {
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

    protected override void FollowPlayer()
    {
        if (_attacking || _waiting || _showingTheTell)
        {
            ResetState();
        }
        _enemyAgent.SetDestination(_player.transform.position);
    }

    protected override void AttackTell()
    {
        //lerpingWeapon
        if (!_showingTheTell)
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

                transform.LookAt(_player.transform.position + Vector3.up);
                _sword.transform.localPosition = p01;
            }
        }
    }

    protected override void AttackPlayer()
    {
        if (_waiting)
        {
            //Debug.Log("waiting");
            _currentAttackTime = (Time.time - _startAttackTime) / _durationBetweenAttacks;

            if (_currentAttackTime > 1)
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
                transform.LookAt(c1);

                projectileRanged _proj = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation, null).GetComponent<projectileRanged>();
                _proj._speed = _projSpeed;
                _proj._maxRange = _maxRange;
                _proj._damage = _enemyDamage;
                _waiting = true;
                c0 = transform.position;
                c1 = transform.position;
                _startAttackTime = Time.time;
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
    }

    protected override void LostSightOfPlayer()
    {
        _alerted = false;
        if (_attacking || _waiting || _showingTheTell)
        {
            ResetState();
        }

        _sword.SetActive(false);
        _enemyAgent.SetDestination(_patrolRoute[_currPath]);
    }

    public override void ResetState()
    {
        _showingTheTell = false;
        _sword.transform.localPosition = _swordPos;
        GetComponent<CapsuleCollider>().enabled = true;
        _attacking = false;
        _waiting = false;
        _stunned = false;
        _slammed = false;
        _enemyAgent.isStopped = false;

    }

    //For Update 2
    public override void GotHit(float _damageRecieved, Vector3 _knockbackdir, Vector3 _particleHitPos)
    {
        if (_canTakeDamage)
        {
            Debug.Log("hit");
            _canTakeDamage = false;

            c0 = transform.position;
            c1 = transform.position + _knockbackdir;

            _startAttackTime = Time.time;
            _hit = true;

            UpdateHealth(_damageRecieved);
            if (_currEnemyHealth <= 0)
            {
                DeadActivate(_knockbackdir);
            }
        }
    }

    public override void GotDashStruck(float _damageRecieved)
    {
        UpdateHealth(_damageRecieved);
        _slammed = true;
        GetComponent<CapsuleCollider>().enabled = false;
        _enemyAgent.isStopped = true;
    }

    public override void GotPinned(float _damageRecieved)
    {
        c0 = transform.position;
        c1 = transform.position;
        ResetState();
        _slammed = true;
        _enemyAgent.isStopped = true;
        _startStunTime = Time.time;
        _stunned = true;
        if (_currEnemyHealth <= 0)
        {
            DeadActivate(Vector3.zero);
        }
    }

    protected override void Stunned()
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

    protected override void KnockedBack()
    {
        _currentAttackTime = (Time.time - _startAttackTime) / _knockedBackDuration;

        if (_currentAttackTime >= 1)
        {
            _currentAttackTime = 1;

            ResetState();
            _hit = false;
        }
        else if (_currentAttackTime >= .4f)
        {
            //Debug.Log("can get hit again");
            _canTakeDamage = true;
        }

        if (_currentAttackTime < 1)
        {
            if (Physics.Raycast(transform.position, -transform.forward, _damageDistance))
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

    protected override void DeadActivate(Vector3 _dirToDie)
    {
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        _slammed = false;
        _stunned = false;
        _enemyAgent.isStopped = false;
        _canTakeDamage = false;

        c0 = transform.position;
        c1 = transform.position + _dirToDie;
        c2 = transform.position + _dirToDie + (Vector3.down);
        _startAttackTime = Time.time;
        _enemyAgent.enabled = false;
        _dead = true;
    }

    protected override void Die()
    {
        _currentAttackTime = (Time.time - _startAttackTime) / _deathDuration;

        if (_currentAttackTime >= 1)
        {
            _berserkRef.EnemyDied(1);
            if (gameObject.tag == "Boss")
            {
                _winRef.BossDied();
            }
            _winRef.EnemyDied();
            _currentAttackTime = 1;

            _init = false;
            _actualHealthBar.gameObject.SetActive(false);
            gameObject.SetActive(false);

        }

        Vector3 p01, p12, p012;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;
        p12 = (1 - _currentAttackTime) * c1 + _currentAttackTime * c2;
        p012 = (1 - _currentAttackTime) * p01 + _currentAttackTime * p12;

        transform.position = p012;
        transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
    }

    public override void MyReset()
    {
        gameObject.SetActive(true);
        _dead = false;
        _hit = false;
        _showingTheTell = false;
        _attacking = false;
        _waiting = false;
        _stunned = false;
        _slammed = false;
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
}
