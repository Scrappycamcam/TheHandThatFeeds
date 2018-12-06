using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

public class MenuSelection_Script : MonoBehaviour {
    [SerializeField]
    private GameObject _ControlsMap;
    [SerializeField]
    private GameObject _CreditsPage;
    [SerializeField]
    private GameObject _StartBtn;
    [SerializeField]
    private GameObject _QuitBtn;
    [SerializeField]
    private GameObject _ControlsBtn;
    [SerializeField]
    private GameObject _CreditsBtn;
    private Player _player;
    private bool _buttonSelected;
    private int _MSelect;
    private float _PInput;
    private int _playerId = 0; // The Rewired player id of this character
    float _currDelayTime;
    float _startDelayTime;
    bool _menuDelay = false;
    float _menuMoveDelayDuration = 25.0f;
    List<Button> _MainMenuButtons;
    int _currButton = 0;
    int _currScene;
    bool _canMenu;

    private void Awake()
    {
        
        _player = ReInput.players.GetPlayer(_playerId);

        _MainMenuButtons = new List<Button>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Button>())
            {
                _MainMenuButtons.Add(transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }
        _currButton = 0;
        _MainMenuButtons[_currButton].Select();
        Debug.Log("init");
        _currScene = SceneManager.GetActiveScene().buildIndex;

        //_MainMenu.SetActive(false);
    }
/*
    private void Awake()
    {
        if(FindObjectOfType<KyleplayerMove>())
        {
            _playerRef = KyleplayerMove.Instance;
        }

        if(FindObjectOfType<PlayerCanvas>())
        {

        }

        if(FindObjectOfType<PlayerCamera>())
        {
            _cameraRef = PlayerCamera.Instance;
        }
    }
    */
    public void StartGame()
    {
        _MSelect = 0;
        Debug.Log("Level 1");
        //Temp Start Game Most Likely Will be Changed.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Controls()
    {
        if(_ControlsMap.activeSelf == false)
        {
            _ControlsMap.SetActive(true);
            _StartBtn.SetActive(false);
            _QuitBtn.SetActive(false);
            _CreditsBtn.SetActive(false);
        }
        else
        {
            _ControlsMap.SetActive(false);
            _StartBtn.SetActive(true);
            _QuitBtn.SetActive(true);
            _CreditsBtn.SetActive(true);
        }
    }

    public void Credits()
    {
        if (_CreditsPage.activeSelf == false)
        {
            _CreditsPage.SetActive(true);
            _StartBtn.SetActive(false);
            _QuitBtn.SetActive(false);
            _ControlsBtn.SetActive(false);
            
        }
        else
        {
            _CreditsPage.SetActive(false);
            _StartBtn.SetActive(true);
            _QuitBtn.SetActive(true);
            _ControlsBtn.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Update()
    {
        ////Take Movement Up and Down on left joystick from the controller
        ////Move cursor up/down when going through menu selection
        ////Press A to Select Button
        ////Use Switch to take input on what button was pressed
        ////Run Function Above
        ////
        ////
        ////
        ////
        ////
        ////
        ////
        ////_PInput = _player.GetAxisRaw("Move Vertical");
        ////if (_player.GetAxisRaw("Move Vertical") != 0 && _buttonSelected == false)//Fix Input
        ////{
        ////    
        ////    _buttonSelected = true;
        ////}
        ////
        ////if (_player.GetButtonDown("Interact"))
        ////{
        ////    PressedButton(_MSelect);
        ////}
        if(_player.GetAxis("Move Vertical") > 0.1)
        {
            MenuMovement(true);
        }
        else if(_player.GetAxis("Move Vertical") < -0.1)
        {
            MenuMovement(false);
        }
        else
        {

        }
        if (_player.GetButtonDown("Interact"))
        {
            ButtonPush();
        }
        if (_menuDelay)
        {
            MenuDelay();
        }
    }

    private void OnDisable()
    {
        _buttonSelected = false;
    }

    public void MenuMovement(bool UpMenu)
    {
        if (!_menuDelay)
        {
            Debug.Log("Menu Moving");
            //_canMenu = false;
            if (UpMenu)
            {
                _currButton--;
                if (_currButton < 0)
                {
                    _currButton = _MainMenuButtons.Count - 1;
                }
            }
            else if (!UpMenu)
            {
                _currButton++;
                if (_currButton > _MainMenuButtons.Count - 1)
                {
                    _currButton = 0;
                }
            }

            Debug.Log(_MainMenuButtons[_currButton].transform.name);
            _MainMenuButtons[_currButton].Select();
            _startDelayTime = 1;
            _menuDelay = true;
        }
    }

    void MenuDelay()
    {
        _currDelayTime = _startDelayTime / _menuMoveDelayDuration;

        _startDelayTime++;

        if (_currDelayTime >= 1)
        {
            _currDelayTime = 1;

            _menuDelay = false;
            _canMenu = true;
        }
    }

    public void ButtonPush()
    {
        //Time.timeScale = 1f;
        //_paused = false;
        _MainMenuButtons[_currButton].onClick.Invoke();
    }
}
