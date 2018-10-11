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

    List<GameObject> _enemyList = new List<GameObject>();

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
        StartCoroutine(InitEnemies());
    }

    private void ResetEnemies()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            _enemyList[i].GetComponent<AIMovement>().MyReset();
        }
    }
    IEnumerator InitEnemies()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            yield return new WaitForSeconds(.0001f);
            _enemyList[i].GetComponent<AIMovement>().Init();
        }

        StopCoroutine(InitEnemies());
    }

    public GameObject AddEnemy { set { _enemyList.Add(value); } }
    public GameObject RemoveEnemy { set { _enemyList.Remove(value); } }
}
