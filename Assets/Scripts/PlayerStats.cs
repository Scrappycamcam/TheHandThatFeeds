using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

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

    private bool _defeated = false;
    private bool _victorious = false;

    LevelSelection_Script myscript;

    KyleplayerMove _playerRef;
    PlayerCanvas _canvasRef;

    private Player _player;
 
    private void Awake()
    {
        myscript = LevelSelection_Script.Instance;
        _playerRef = GetComponent<KyleplayerMove>();
        _canvasRef = PlayerCanvas.Instance;
        _PcurrHealth = _PmaxHealth;
        _player = ReInput.players.GetPlayer(0);

        _currentHealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        _currentHealthBar.fillAmount = 1f;

        MyReset();
        PlayerCanvas.Instance.SetGameReset += MyReset;
    }

    public void Update()
    {
        if (_defeated && _player.GetButtonDown("Dash"))
        {
            Defeat();
        }else if(_victorious && _player.GetButtonDown("Dash"))
        {
            Victory();
        }
    }

    public void MyReset()
    {
        _PcurrHealth = _PmaxHealth;
        DisplayHealth();
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

        if (_PcurrHealth <= 0 && !_defeated)
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
        if (GameObject.Find("SceneManager").GetComponent<LevelSelection_Script>()._CurLevel > 2)
        {
            WinGame();
        }
        else
        {
            GameObject.Find("SceneManager").GetComponent<LevelSelection_Script>().LoadScene();
        }
    }

    public void WinGame()
    {
        if (!_victorious)
        {
            _victorious = true;
            _VictoryDisplay.SetActive(true);
            Time.timeScale = 0f;
            _canvasRef.ResetGame();
        }
        else
        {
            _victorious = false;
            _VictoryDisplay.SetActive(false);
            Time.timeScale = 1f;

            GameObject.Find("SceneManager").GetComponent<LevelSelection_Script>()._CurLevel = 1;

            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Defeat()
    {
        if (!_defeated)
        {
            _defeated = true;
            _DefeatDisplay.SetActive(true);
            Time.timeScale = 0f;
            _canvasRef.ResetGame();
        }
        else
        {
            _defeated = false;
            _DefeatDisplay.SetActive(false);
            Time.timeScale = 1f;
            MyReset();
        }
    }

    public float GetHealth()
    {
        return _PcurrHealth;
    }
    
}
