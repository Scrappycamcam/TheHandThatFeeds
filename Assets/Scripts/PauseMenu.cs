﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class PauseMenu : MonoBehaviour {

    private static PauseMenu _instance;
    public static PauseMenu Instance
    {
        get
        {
            if (_instance != null)
            {

                return _instance;
            }
            else
            {
                if (FindObjectOfType<PauseMenu>())
                {
                    _instance = FindObjectOfType<PauseMenu>();
                    return _instance;
                }
                else
                {
                    Debug.Log("no canvas");
                    return null;
                }
            }
        }
    }

    private int _playerId = 0; // The Rewired player id of this character
    Player _player;

    GameObject _PauseMenu;
    GameObject _ControlsMenu;

    [SerializeField]
    List<Button> _pauseMenuButtons;
    int _currButton = 0;
    
    KyleplayerMove _playerRef;
    PlayerCanvas _canvasRef;

    [SerializeField]
    float _menuMoveDelayDuration;
    bool _paused = false;
    float _pastTimeScale;

    int _currScene;
    
    float _currDelayTime;
    float _startDelayTime;
    bool _menuDelay = false;

    bool _canMenu;

    private void Awake()
    {
        if (Instance == this)
        {
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _canvasRef = PlayerCanvas.Instance;
        _player = ReInput.players.GetPlayer(_playerId);
        _playerRef = KyleplayerMove.Instance;
        
        _PauseMenu = transform.GetChild(0).gameObject;
        _ControlsMenu = transform.GetChild(1).gameObject;

        _pauseMenuButtons = new List<Button>();
        for (int i = 0; i < _PauseMenu.transform.childCount; i++)
        {
            if(_PauseMenu.transform.GetChild(i).GetComponent<Button>())
            {
                _pauseMenuButtons.Add(_PauseMenu.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }
        _currButton = 0;
        Debug.Log("init");
        _currScene = SceneManager.GetActiveScene().buildIndex;


        _pauseMenuButtons[_currButton].Select();
        _PauseMenu.SetActive(false);
        _ControlsMenu.SetActive(false);
    }

    private void Update()
    {
        if(_player.GetButtonDown("Pause"))
        {
            Pause();
        }

        if(_menuDelay)
        {
            MenuDelay();
        }
    }

    private void Pause()
    {
        if(_paused)
        {
            _PauseMenu.SetActive(false);
            _ControlsMenu.SetActive(false);
            Time.timeScale = _pastTimeScale;
            _paused = false;
            _canMenu = true;
        }
        else
        {
            _PauseMenu.SetActive(true);
            _pastTimeScale = Time.timeScale;

            Time.timeScale = 0;

            _currButton = 0;
            
            _paused = true;
            _canMenu = true;

            _pauseMenuButtons[_currButton].Select();
        }
    }

    void MenuDelay()
    {
        _currDelayTime =  _startDelayTime / _menuMoveDelayDuration;

        _startDelayTime++;

        if (_currDelayTime >= 1)
        {
            _currDelayTime = 1;

            _menuDelay = false;
            _canMenu = true;
        }
    }

    public void MenuMovement(bool UpMenu)
    {
        if(_canMenu)
        {
            Debug.Log("Menu Moving");
            _canMenu = false;
            if (UpMenu)
            {
                _currButton--;
                if (_currButton < 0)
                {
                    _currButton = _pauseMenuButtons.Count - 1;
                }
            }
            else if (!UpMenu)
            {
                _currButton++;
                if (_currButton > _pauseMenuButtons.Count - 1)
                {
                    _currButton = 0;
                }
            }

            Debug.Log(_pauseMenuButtons[_currButton].transform.name);
            _pauseMenuButtons[_currButton].Select();
            _startDelayTime = 1;
            _menuDelay = true;
        }
    }

    public void ButtonPush()
    {
        _pauseMenuButtons[_currButton].onClick.Invoke();
    }

    public void ResumeLevel()
    {
        Pause();
    }

    public void Controls()
    {
        _ControlsMenu.SetActive(true);

        _pauseMenuButtons = new List<Button>();
        for (int i = 0; i < _ControlsMenu.transform.childCount; i++)
        {
            if (_ControlsMenu.transform.GetChild(i).GetComponent<Button>())
            {
                _pauseMenuButtons.Add(_ControlsMenu.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }

        _currButton = 0;
        _pauseMenuButtons[_currButton].Select();
    }

    public void BackFromControls()
    {
        _ControlsMenu.SetActive(false);

        _pauseMenuButtons = new List<Button>();
        for (int i = 0; i < _PauseMenu.transform.childCount; i++)
        {
            if (_PauseMenu.transform.GetChild(i).GetComponent<Button>())
            {
                _pauseMenuButtons.Add(_PauseMenu.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }

        _currButton = 1;
        _pauseMenuButtons[_currButton].Select();
    }

    public void BackToMainMenu()
    {
        Destroy(_playerRef.gameObject);
        SceneManager.LoadScene(0);
    }

    public bool GameIsPaused { get { return _paused; } set { _paused = value; } }
}
