using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIMovement : AIEnemy {

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
                            Saccing();
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
            //c0 = _swordPos;
            //c1 = _swordPos + Vector3.up;
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
                c1 = _player.transform.position;
                c1.y = transform.position.y;
                _attacking = true;
                //_sword.transform.localPosition = _swordPos;
                _startAttackTime = Time.time;
            }
            else
            {
                Vector3 p01;

                p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

                Vector3 LookPos = _player.transform.position;
                LookPos.y = transform.position.y;
                transform.LookAt(LookPos);
                //_sword.transform.localPosition = p01;
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
                _myAnimations.Play("Attack", 0);
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
                    }
                }
            }

            Vector3 p01;

            p01 = (1 - _currentAttackTime) * c0 + _currentAttackTime * c1;

            transform.position = p01;
        }
    }
}
