using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    [SerializeField]
    Image _currentHealthBar;
    [SerializeField]
    Text _HealthDisplay;
    [SerializeField]
    private float _PmaxHealth = 100f;
    [SerializeField]
    GameObject _VictoryDisplay;
    [SerializeField]
    GameObject _DefeatDisplay;
    float _PcurrHealth;
    Vector3 startPos;
 
    private void Awake()
    {
        
        startPos = transform.position;
        _PcurrHealth = _PmaxHealth;

        _currentHealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        //PDamage(100);
    }

    public void ResetStartPos()
    {
        startPos = transform.position;
    }

    void DisplayHealth()
    {
        float ratio = _PcurrHealth / _PmaxHealth; //creates the health ratio

        if(ratio < 0)
        {
            ratio = 0f;
        }
        else if(ratio >= 1)
        {
            ratio = 1f;
        }


        _currentHealthBar.fillAmount = ratio; // sets the scale transform for the health bar
        //_HealthDisplay.text = (ratio * 100).ToString() + '%'; //use if display text is desired
    }

    public float PDamage(float DtoTake)//function for taking damage
    {
        _PcurrHealth -= DtoTake;

        DisplayHealth();

        if (_PcurrHealth <= 0)
        {
            Defeat();
        }

        return _PcurrHealth;
    }

    public float PHeal(float Heal)//function for healing
    {
        _PcurrHealth = _PcurrHealth + Heal;
        
        if (_PcurrHealth >= _PmaxHealth)
        {
            _PcurrHealth = _PmaxHealth;
        }
        DisplayHealth();

        return _PcurrHealth;
    }

    public void Victory()
    {
        _VictoryDisplay.SetActive(true);
        LevelSelection_Script myscript = FindObjectOfType<LevelSelection_Script>();
        myscript.LoadScene(LevelSelection_Script.WhatLevel.Level2);
    }
    public void Defeat()
    {
        _DefeatDisplay.SetActive(true);
        //_PcurrHealth = _PmaxHealth;
        LevelSelection_Script myscript = FindObjectOfType<LevelSelection_Script>();
        myscript.ReloadScene();
        

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Im In the Area");
        if(other.tag == "EndOfLevel")
        {
            Debug.Log("Level Complete!");
            Victory();
        }
    }

    public float GetHealth()
    {
        return _PcurrHealth;
    }
}
