using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

public class MenuSelection_Script : MonoBehaviour {
    [Header("Menu Buttons.")]
    [SerializeField] private GameObject ControlImg;
    [SerializeField] private GameObject StartBtn;
    [SerializeField] private GameObject QuitBtn;
    private Player _player;
    private bool _buttonSelected;
    private int _MSelect;
    private float _PInput;

    private KyleplayerMove _playerRef;
    private PlayerCamera _cameraRef;
    private PlayerCanvas _canvasRef;

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

    public void StartGame()
    {
        _MSelect = 0;
        Debug.Log("Level 1");
        //Temp Start Game Most Likely Will be Changed.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Controls()
    {
        if (ControlImg.activeSelf == false)
        {
            StartBtn.SetActive(false);
            QuitBtn.SetActive(false);
            ControlImg.SetActive(true);
        }
        else
        {
            StartBtn.SetActive(true);
            QuitBtn.SetActive(true);
            ControlImg.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    //public void Update()
    //{
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

    //}

    private void OnDisable()
    {
        _buttonSelected = false;
    }


    void PressedButton(int WhatWasPressed)
    {
        switch (WhatWasPressed)
        {
            default:
                StartGame();


                break;
            case 0:
                StartGame();

                break;
            case 1:
                Controls();
                break;
            case 2:
                QuitGame();
                break;
        }
    }
}
