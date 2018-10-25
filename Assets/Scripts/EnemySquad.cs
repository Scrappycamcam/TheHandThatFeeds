using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : MonoBehaviour {

    [SerializeField]
    List<AIEnemy> _enemyList;

    AIOverlord _myOverlord;

    private void Awake()
    {
        _myOverlord = AIOverlord.Instance;
        _myOverlord.AddSquad = this;

        _enemyList = new List<AIEnemy>();
        for (int i = 0; i < transform.childCount; i++)
        {
            AIEnemy _enemyToAdd = transform.GetChild(i).gameObject.GetComponent<AIEnemy>();
            _enemyList.Add(_enemyToAdd);
        }
    }

    public void Init()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].Init();
        }
    }

    public void AlertSquad(KyleplayerMove _playerToAttack)
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].SetPlayer = _playerToAttack;
            _enemyList[i].GetAIState = AIState.ALERTED;
        }
    }

    public void PauseSquad()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].PauseMe();
        }
    }

    public void UnPauseSquad()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].UnPauseMe();
        }
    }

    public void ResetSquad()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].MyReset();
        }
    }
}
