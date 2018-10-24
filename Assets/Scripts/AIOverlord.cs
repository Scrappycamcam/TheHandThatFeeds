using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOverlord : MonoBehaviour {

    private static AIOverlord _instance;
    public static AIOverlord Instance
    {
        get
        {
            if(_instance != null)
            {
                return _instance;
            }
            else
            {
                if(FindObjectOfType<AIOverlord>())
                {
                    _instance = FindObjectOfType<AIOverlord>();
                    return _instance;
                }
                else
                {
                    Debug.Log("There is no AIOverlord");
                    return null;
                }
            }
        }
    }

    List<EnemySquad> _squadList = new List<EnemySquad>();

    private void Awake()
    {
        if(Instance == this)
        {
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(InitSquads());
    }

    private void ResetEnemies()
    {
        for (int i = 0; i < _squadList.Count; i++)
        {
            _squadList[i].GetComponent<EnemySquad>().ResetSquad();
        }
    }

    public void PauseEnemies()
    {
        for (int i = 0; i < _squadList.Count; i++)
        {
            _squadList[i].PauseSquad();
        }
    }

    public void UnPauseEnemies()
    {
        for (int i = 0; i < _squadList.Count; i++)
        {
            _squadList[i].UnPauseSquad();
        }
    }

    public void ResetMe()
    {
        _squadList = new List<EnemySquad>();
    }

    IEnumerator InitSquads()
    {
        for (int i = 0; i < _squadList.Count; i++)
        {
            yield return new WaitForSeconds(.0001f);
            _squadList[i].GetComponent<EnemySquad>().Init();
        }

        StopCoroutine(InitSquads());
    }

    public EnemySquad AddSquad { set { _squadList.Add(value); } }
}
