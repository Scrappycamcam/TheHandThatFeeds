﻿using System.Collections;
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
