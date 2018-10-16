using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour {

    protected NavMeshAgent _enemyAgent;

    [Header("Enemy Stats")]
    [SerializeField]
    protected float _enemyHealth;
    [SerializeField]
    protected float _enemyDamage;
    [SerializeField]
    protected float _damageDistance;
    protected Vector3 _startPoint;

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
    [SerializeField]
    protected float _stunDuration;
    

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
    public virtual void GotHit(float _damageRecieved, Vector3 _flydir)
    {

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

    public virtual void MyReset()
    {

    }



}
