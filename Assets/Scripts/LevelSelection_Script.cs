using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSelection_Script : MonoBehaviour {

    public enum WhatLevel
    {
        MainMenu,
        Level1,
        Level2,
        BossRoom
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        //LoadScene(WhatLevel.Level1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(WhatLevel Level)
    {
        SceneManager.LoadScene((int)Level);
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
