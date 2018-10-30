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
            if (!_pauseRef.GameIsPaused)
            {
                ShowHealthBar();
                if (_enemyAgent.enabled)
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
}
