using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIMovementBoss : AIEnemy {

    [SerializeField]
    GameObject DOTObj;

    [Header("Ranged Variables")]
    public GameObject _projectile;
    public Transform _launchPos;
    
    [SerializeField]
    float _projSpeed;
    [SerializeField]
    float _maxRange;
    [SerializeField]
    float _shotSpread;

    [Header("Attack Percentages")]//make sure these add up to 100%
    [SerializeField]
    float _percentMelee;//percent chance that he will perform the melee attack
    [SerializeField]
    float _percentRanged;//percent chance that he will perform the ranged attack
    [SerializeField]
    float _percentRing;//percent chance that he will perform the ring attack
    [SerializeField]
    float _percentShield;//percent chance that he will perform the Shield move
    [SerializeField]
    float _percentHeal;//percent chance that he will perform the Heal move

    private enum whichAttack{Melee, Ranged, Ring, Shield, Heal, None};

    private whichAttack _myAtk = whichAttack.None;
    private bool _isShielded = false;

    // Update is called once per frame
    protected override void Update()
    {
        if (_init)
        {
            if (!_pauseRef.GameIsPaused)
            {
                ShowHealthBar();
                if(_enemyAgent.enabled)
                {
                    switch (_myCurrState)
                    {
                        case AIState.NOTALERTED:
                            PatrolState();
                            break;
                        case AIState.ALERTED:
                            CombatStrats();
                            break;
                        case AIState.HIT:
                            KnockedBack();
                            break;
                        case AIState.STUNNED:
                            Stunned();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Die();
                }
               
            }
        }
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

    protected override void CombatStrats()
    {

        if (Vector3.Distance(transform.position, _player.transform.position) <= _followDistanceThreshold)
        {
            if (!_sword.activeInHierarchy)
            {
                _sword.SetActive(true);
            }
            if (Vector3.Distance(transform.position, _player.transform.position) <= _attackDistanceThreshold)
            {
                if(_myAtk == whichAttack.None)
                {
                    float atk = Random.Range(0, 100);
                    float chance = _percentMelee;
                    for (int i = 0; i < 5; i++)
                    {
                        if (atk < chance)
                        {
                            _myAtk = (whichAttack)i;
                            break;
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    chance += _percentRanged;
                                    break;
                                case 1:
                                    chance += _percentRing;
                                    break;
                                case 2:
                                    chance += _percentShield;
                                    break;
                                case 3:
                                    chance += _percentHeal;
                                    break;
                                case 4:
                                    Debug.Log("Something Went Wrong With Boss Attack Selection");
                                    break;
                            }
                        }
                    }
                }
                if (!_attacking)
                {
                    AttackTell();
                }
                else
                {
                    switch (_myAtk)
                    {
                        case whichAttack.Melee:
                            Debug.Log("Melee");
                            MeleeAttack();
                            break;
                        case whichAttack.Ranged:
                            Debug.Log("Ranged");
                            RangedAttack();
                            break;
                        case whichAttack.Ring:
                            Debug.Log("Ring");
                            RingAttack();
                            break;
                        case whichAttack.Shield:
                            Debug.Log("Shield");
                            ShieldAttack();
                            break;
                        case whichAttack.Heal:
                            Debug.Log("Heal");
                            HealAttack();
                            break;
                        case whichAttack.None:
                            Debug.Log("Something Went Wrong With Boss Attack Selection");
                            break;
                    }
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

    private void MeleeAttack()
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
                _myCurrState = AIState.ALERTED;
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
                _myAtk = whichAttack.None;
            }
            else
            {
                transform.LookAt(c1);
                if (Physics.Raycast(transform.position, transform.forward, out hit, _damageDistance))
                {
                    if (hit.collider.GetComponent<PlayerStats>())
                    {
                        hit.collider.GetComponent<PlayerStats>().PDamage(_enemyDamage);
                        //Debug.Log("hit player");
                        _waiting = true;
                        c0 = transform.position;
                        c1 = transform.position;
                        _startAttackTime = Time.time;
                        _myAtk = whichAttack.None;
                    }
                }
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
    }

    private void RangedAttack()
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
                _myCurrState = AIState.ALERTED;
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
                transform.LookAt(_player.transform.position + Vector3.up);

                projectileRanged _proj = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation, null).GetComponent<projectileRanged>();
                projectileRanged _proj2 = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation * Quaternion.Euler(0, _shotSpread, 0), null).GetComponent<projectileRanged>();
                projectileRanged _proj3 = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation * Quaternion.Euler(0, -_shotSpread, 0), null).GetComponent<projectileRanged>();
                _proj._speed = _projSpeed;
                _proj._maxRange = _maxRange;
                _proj._damage = _enemyDamage;
                _proj2._speed = _projSpeed;
                _proj2._maxRange = _maxRange;
                _proj2._damage = _enemyDamage;
                _proj3._speed = _projSpeed;
                _proj3._maxRange = _maxRange;
                _proj3._damage = _enemyDamage;
                _waiting = true;
                c0 = transform.position;
                c1 = transform.position;
                _startAttackTime = Time.time;
                _myAtk = whichAttack.None;
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
    }

    private void RingAttack()
    {
        Instantiate(DOTObj, transform.position + (Vector3.down/2), DOTObj.transform.rotation, null);
        _myAtk = whichAttack.None;
    }

    private void ShieldAttack()
    {
        transform.Find("Shield").gameObject.SetActive(true);
        _isShielded = true;
        _myAtk = whichAttack.None;
    }

    private void HealAttack()
    {

    }

    public override void GotHit(float _damageRecieved, Vector3 _knockbackdir, Vector3 _particleHitPos)
    {
        transform.parent = null;
        if (_canTakeDamage && !_isShielded)
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
        }
        else if (_isShielded && _damageRecieved > 15f)
        {
            transform.Find("Shield").gameObject.SetActive(false);
            _isShielded = false;
            Debug.Log("Shield Broken");
        }
    }

    public override void GotDashStruck(float _damageRecieved)
    {
        if (_isShielded)
        {
            transform.Find("Shield").gameObject.SetActive(false);
            _isShielded = false;
            Debug.Log("Shield Broken");
        }
        UpdateHealth(_damageRecieved);
        //_slammed = true;
        GetComponent<CapsuleCollider>().enabled = false;
        _enemyAgent.isStopped = true;
    }

    public override void GotPinned(float _damageRecieved)
    {
        if (_isShielded)
        {
            transform.Find("Shield").gameObject.SetActive(false);
            _isShielded = false;
            Debug.Log("Shield Broken");
        }
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
}
