using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSelection_Script : MonoBehaviour {

    private static LevelSelection_Script _instance;
    public static LevelSelection_Script Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (FindObjectOfType<LevelSelection_Script>())
                {
                    _instance = FindObjectOfType<LevelSelection_Script>();
                    return _instance;
                }
                else
                {
                    Debug.Log("no Player");
                    return null;
                }
            }
        }
    }

    public enum WhatLevel
    {
        MainMenu,
        Level1,
        Level2,
        BossRoom
    }

    public WhatLevel _CurLevel = (WhatLevel)1;

    void Awake()
    {
        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(WhatLevel Level)
    {
        Debug.Log("level");
        _CurLevel = Level;
        SceneManager.LoadScene((int)Level);
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
