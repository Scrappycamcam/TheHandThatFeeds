using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIMovementBoss : AIEnemy {

    [SerializeField]
    GameObject DOTObj;

    private GameObject _DotGameObj;

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

    [Header("Heal Move Specifics")]
    [SerializeField]
    float _sacrificeHeal;
    [SerializeField]
    float _durationOfHealMove;
    [SerializeField]
    Transform[] _SpawnPoints;
    [SerializeField]
    GameObject _EnemyPrefab;

    private float _healTimer;

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
                        case AIState.SACRIFICING:
                            Sacrificing();
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

    public override void MyReset()
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

        transform.Find("Shield").gameObject.SetActive(false);
        _isShielded = false;

        transform.rotation = Quaternion.Euler(0,180,0);

        List<AIEnemy> MyEnemies = _mySquad.GetEnemySquad;

        foreach (AIEnemy enem in MyEnemies)
        {
            if (enem.name != this.name)
            {
                enem.GotHit(1000f, transform.forward, hit.point, BasicAttacks.HEAVY);
                //Destroy(enem.gameObject);
            }
        }
        _enemyAgent.SetDestination(_patrolRoute[_currPath]);
        _init = true;
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



                transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
                _sword.transform.localPosition = p01;
            }
        }
    }

    protected override void Die()
    {
        Debug.Log("dying");
        _currentAttackTime = (Time.time - _startAttackTime) / _deathDuration;

        if (_currentAttackTime >= 1)
        {
            Debug.Log("done dying");
            _berserkRef.EnemyDied(1);
            _winRef.BossDied();
            _currentAttackTime = 1;

            List<AIEnemy> MyEnemies = _mySquad.GetEnemySquad;

            foreach (AIEnemy enem in MyEnemies)
            {
                if (enem)
                {
                    enem.GotHit(1000f, transform.forward, hit.point, BasicAttacks.HEAVY);
                    //Destroy(enem.gameObject);
                }
            }

            _init = false;
            transform.parent = _mySquad.transform;
            gameObject.SetActive(false);
            _actualHealthBar.gameObject.SetActive(false);
            if (_BossPos != Vector3.zero)
            {
                Destroy(this.gameObject);
            }
        }

        Vector3 p01, p12, p012;

        p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;
        p12 = (1 - _currentAttackTime) * c1 + _currentAttackTime * c2;
        p012 = (1 - _currentAttackTime) * p01 + _currentAttackTime * p12;

        transform.position = p012;
        transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
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
                if(_myAtk == whichAttack.Ring && _DotGameObj)
                {
                    _myAtk = whichAttack.Ranged;
                }
                if(_myAtk == whichAttack.Shield && _isShielded)
                {
                    _myAtk = whichAttack.Melee;
                }
                if (_myAtk == whichAttack.Heal && _currEnemyHealth >= _enemyHealth)
                {
                    _myAtk = whichAttack.Ranged;
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
                transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
                if (Physics.Raycast(transform.position + Vector3.down, transform.forward, out hit, _damageDistance))
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
                transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));

                projectileRanged _proj = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation, null).GetComponent<projectileRanged>();
                projectileRanged _proj2 = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation * Quaternion.Euler(0, _shotSpread, 0), null).GetComponent<projectileRanged>();
                projectileRanged _proj3 = Instantiate<GameObject>(_projectile, _launchPos.position, transform.rotation * Quaternion.Euler(0, -_shotSpread, 0), null).GetComponent<projectileRanged>();
                _proj._speed = _projSpeed;
                _proj._maxRange = _maxRange;
                _proj._damage = _enemyDamage/3;
                _proj2._speed = _projSpeed;
                _proj2._maxRange = _maxRange;
                _proj2._damage = _enemyDamage/3;
                _proj3._speed = _projSpeed;
                _proj3._maxRange = _maxRange;
                _proj3._damage = _enemyDamage/3;
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
        if (!_DotGameObj)
        {
            _DotGameObj = Instantiate(DOTObj, transform.position + (2*Vector3.down), DOTObj.transform.rotation, null);
        }
        _waiting = false;
        _attacking = false;
        _showingTheTell = false;
        _myCurrState = AIState.ALERTED;
        _myAtk = whichAttack.None;
    }

    private void ShieldAttack()
    {
        transform.Find("Shield").gameObject.SetActive(true);
        _isShielded = true;
        _myAtk = whichAttack.None;

        _waiting = false;
        _attacking = false;
        _showingTheTell = false;
        _myCurrState = AIState.ALERTED;
    }

    private void HealAttack()
    {
        if (_mySquad.GetEnemySquad.Count < 5)
        {
            foreach (Transform t in _SpawnPoints)
            {
                Instantiate(_EnemyPrefab, t.position, _EnemyPrefab.transform.rotation, _mySquad.transform).GetComponent<AIEnemy>().Init();
            }
        }
        _mySquad.Awake();
        List<AIEnemy> MyEnemies = _mySquad.GetEnemySquad;
        if (MyEnemies.Count > 1)
        {
            foreach (AIEnemy enemy in MyEnemies)
            {
                if (enemy.isActiveAndEnabled)
                {
                    enemy.Sacrifice(transform.position);
                    enemy.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
            _healTimer = Time.time + _durationOfHealMove;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            _myCurrState = AIState.SACRIFICING;
        }
        else
        {
            ResetState();
        }
    }

    private void Sacrificing()
    {
        if(Time.time >= _healTimer || _currEnemyHealth >= _enemyHealth)
        {
            ResetSac();
        }
        else
        {
            List<AIEnemy> MyEnemies = _mySquad.GetEnemySquad;
            foreach (AIEnemy enemy in MyEnemies)
            {
                if (enemy)
                {
                    enemy.Sacrifice(transform.position);
                }
            }
        }
    }

    private void ResetSac()
    {
        ResetState();
        List<AIEnemy> MyEnemies = _mySquad.GetEnemySquad;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        foreach (AIEnemy enemy in MyEnemies)
        {
            if (enemy)
            {
                enemy.GetComponent<Rigidbody>().isKinematic = true;
                enemy.ResetState();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_myCurrState == AIState.SACRIFICING)
        {
            if (collision.gameObject.GetComponent<AIEnemy>())
            {
                collision.gameObject.GetComponent<AIEnemy>().ResetState();
                collision.gameObject.GetComponent<AIEnemy>().GotHit(1000f, transform.forward, hit.point, BasicAttacks.HEAVY);
                UpdateHealth(-_sacrificeHeal);
            }
        }
    }

    protected override bool LookingForPlayer()
    {
        for (int currCast = 0; currCast < _numOfCasts; currCast++)
        {
            Vector3 _castDir = transform.forward + (transform.right * ((_sightArea - ((_sightArea / ((_numOfCasts - 1) / 2)) * currCast)) / 100));
            if (_debugVision)
            {
                Debug.DrawLine(transform.position + Vector3.down, transform.position + (_castDir * _sightDistance));
            }

            if (Physics.Raycast(transform.position + Vector3.down, _castDir, out hit, _sightDistance))
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

    public override bool GotHit(float _damageRecieved, Vector3 _knockbackdir, Vector3 _particleHitPos, BasicAttacks _attackIGotHitBy)
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
            switch (_attackIGotHitBy)
            {
                case BasicAttacks.NONE:
                    break;
                case BasicAttacks.LIGHT:
                    ResetState();
                    break;
                case BasicAttacks.HEAVY:
                    _myCurrState = AIState.HIT;
                    break;
                default:
                    break;
            }
            if (_attackIGotHitBy == BasicAttacks.HEAVY)
            {
                _myCurrState = AIState.HIT;
            }

            UpdateHealth(_damageRecieved);
            ShowBlood(_particleHitPos);

            if (_currEnemyHealth <= 0)
            {
                ResetSac();
                DeadActivate(_knockbackdir);
            }
            return true;
        }
        else if (_isShielded && _damageRecieved > 15f)
        {
            transform.Find("Shield").gameObject.SetActive(false);
            _isShielded = false;
            Debug.Log("Shield Broken");
            return true;
        }
        return false;
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
            DeadActivate(Vector3.down);
        }
    }
}
