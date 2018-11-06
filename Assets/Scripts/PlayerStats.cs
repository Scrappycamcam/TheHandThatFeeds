using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    LevelSelection_Script myscript;

    KyleplayerMove _playerRef;
    PlayerCanvas _canvasRef;
 
    private void Awake()
    {
        myscript = LevelSelection_Script.Instance;
        _playerRef = GetComponent<KyleplayerMove>();
        _canvasRef = PlayerCanvas.Instance;
        _PcurrHealth = _PmaxHealth;

        _currentHealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    public void FindHealthBar()
    {
        _currentHealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        DisplayHealth();
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
        //_VictoryDisplay.SetActive(true);
        GameObject.Find("SceneManager").GetComponent<LevelSelection_Script>().LoadScene();
    }

    public void Defeat()
    {
        _PcurrHealth = _PmaxHealth;
        DisplayHealth();
        _canvasRef.ResetGame();
    }

    public float GetHealth()
    {
        return _PcurrHealth;
    }
}
