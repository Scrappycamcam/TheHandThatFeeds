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
    float _PHealth = 100;
    [SerializeField]
    GameObject _VictoryDisplay;
    [SerializeField]
    GameObject _DefeatDisplay;
    private float _PmaxHealth = 100;
    Vector3 startPos;
 
    private void Awake()
    {
        startPos = transform.position;
        //PDamage(100);
    }

    public void ResetStartPos()
    {
        startPos = transform.position;
    }

    private void OnGUI()
    {
        DisplayHealth();
    }

    void DisplayHealth()
    {
        float ratio = _PHealth / _PmaxHealth; //creates the health ratio
        _currentHealthBar.rectTransform.localScale = new Vector3(ratio, 1, 1); // sets the scale transform for the health bar
        //_HealthDisplay.text = (ratio * 100).ToString() + '%'; //use if display text is desired
    }
    public float PDamage(float DtoTake)//function for taking damage
    {
        if(_PHealth >= DtoTake)
        {
            _PHealth = _PHealth - DtoTake;

        }
        else
        {
            _PHealth = 0;
            Defeat();
        }

        return _PHealth;
    }

    public float PHeal(float Heal)//function for healing
    {
        if(_PHealth < _PmaxHealth)
        {
            _PHealth = _PHealth + Heal;
        }
        else
        {
            _PHealth = _PmaxHealth;
        }
        return _PHealth;
    }

    public void Victory()
    {
        _VictoryDisplay.SetActive(true);
        LevelSelection_Script myscript = FindObjectOfType<LevelSelection_Script>();
        myscript.ReloadScene();
    }
    public void Defeat()
    {
        _DefeatDisplay.SetActive(true);
        _PHealth = _PmaxHealth;
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

}
