using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : MonoBehaviour {

    [SerializeField]
    List<AIEnemy> _enemyList;

    AIOverlord _myOverlord;

    public void Awake()
    {
        _myOverlord = AIOverlord.Instance;
        _myOverlord.AddSquad = this;

        _enemyList = new List<AIEnemy>();
        for (int i = 0; i < transform.childCount; i++)
        {
            AIEnemy _enemyToAdd = transform.GetChild(i).gameObject.GetComponent<AIEnemy>();
            _enemyList.Add(_enemyToAdd);
        }

        PlayerCanvas.Instance.SetGameReset += MyReset;
    }

    public void MyReset()
    {
        foreach(AIEnemy enem in _enemyList)
        {
            if (enem)
            {
                enem.GetAIState = AIState.NOTALERTED;
            }
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
    
    public List<AIEnemy> GetEnemySquad { get { return _enemyList; } }
}
