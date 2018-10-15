using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour {

    [HideInInspector]
    public NavMeshAgent _enemyAgent;

    [Header("Enemy Stats")]
    public float _enemyHealth;
    public float _enemyDamage;
    public float _damageDistance;
    [HideInInspector]
    public Vector3 _startPoint;

    [Header("Vision Variables")]
    public float _sightDistance;
    public float _sightArea;
    public int _numOfCasts;
    public bool _debugVision = false;

    [Header("Player Tracking Variables")]
    public float _followDistanceThreshold;
    public float _attackDistanceThreshold;

    [HideInInspector]
    public bool _attacking = false;
    [HideInInspector]
    public bool _showingTheTell = false;
    [HideInInspector]
    public bool _waiting = false;
    [HideInInspector]
    public bool _hit = false;
    [HideInInspector]
    public bool _slammed = false;
    [HideInInspector]
    public bool _stunned = false;
    [HideInInspector]
    public bool _canTakeDamage = true;
    [HideInInspector]
    public bool _dead = false;
    [HideInInspector]
    public Vector3 _deadLook;
    [HideInInspector]
    public float _startAttackTime;
    [HideInInspector]
    public float _currentAttackTime;
    [HideInInspector]
    public Vector3 c0, c1, c2;

    [Header("Time Variables")]
    [SerializeField]
    public float _attackTellDuration;
    [SerializeField]
    public float _attackSwingDuration;
    [SerializeField]
    public float _durationBetweenAttacks;
    [SerializeField]
    public float _knockedBackDuration;
    [SerializeField]
    public float _deathDuration;
    [SerializeField]
    public float _stunDuration;

    [HideInInspector]
    public GameObject _sword;
    [HideInInspector]
    public Vector3 _swordPos;

    [Header("Patrol Variables")]
    public List<GameObject> _patrolPoints;
    [HideInInspector]
    public List<Vector3> _patrolRoute;
    [HideInInspector]
    public int _currPath;
    [HideInInspector]
    public bool _alerted = false;
    [HideInInspector]
    public bool _init = false;

    [HideInInspector]
    public RaycastHit hit;
    [HideInInspector]
    public KyleplayerMove _player;
    [HideInInspector]
    public EnemySquad _mySquad;


    public virtual void Init()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void PatrolState()
    {

    }

    public virtual bool LookingForPlayer()
    {
        return false;
    }

    public virtual void CombatStrats()
    {

    }

    public virtual void FollowPlayer()
    {

    }

    public virtual void AttackTell()
    {

    }

    public virtual void AttackPlayer()
    {

    }

    public virtual void LostSightOfPlayer()
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

    public virtual void Stunned()
    {

    }

    public virtual void KnockedBack()
    {

    }

    public virtual void DeadActivate(Vector3 _dirToDie)
    {

    }

    public virtual void Die()
    {

    }

    public virtual void MyReset()
    {

    }



}
