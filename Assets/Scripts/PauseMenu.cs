using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class PauseMenu : MonoBehaviour {

    private int _playerId = 0; // The Rewired player id of this character
    Player _player;

    GameObject _PauseMenu;
    List<Button> _pauseMenuButtons;

    AIOverlord _AIRef;
    KyleplayerMove _playerRef;

    bool _paused = false;
    float _pastTimeScale;

    int _currScene;

    private void Awake()
    {
        _player = ReInput.players.GetPlayer(_playerId);
        _AIRef = AIOverlord.Instance;
        
        _PauseMenu = transform.GetChild(0).gameObject;

        _pauseMenuButtons = new List<Button>();
        for (int i = 0; i < _PauseMenu.transform.childCount; i++)
        {
            if(_PauseMenu.transform.GetChild(i).GetComponent<Button>())
            {
                _pauseMenuButtons.Add(_PauseMenu.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }
        _currScene = SceneManager.GetActiveScene().buildIndex;

        _PauseMenu.SetActive(false);
    }

    private void Update()
    {
        if(_player.GetButtonDown("Pause"))
        {
            Pause();
        }
    }

    private void Pause()
    {
        if(_PauseMenu.activeInHierarchy)
        {
            _PauseMenu.SetActive(false);
            Time.timeScale = _pastTimeScale;
            _paused = false;
            //_AIRef.PauseEnemies();
            //_playerRef.PausePlayer();
        }
        else
        {
            _PauseMenu.SetActive(true);
            _pastTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            _pauseMenuButtons[0].Select();
            //_pauseMenuButtons[0].
            _paused = true;
            //_AIRef.UnPauseEnemies();
            //_playerRef.UnpausePlayer()
        }
    }

    public void ResumeLevel()
    {
        Pause();
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(_currScene);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public bool GameIsPaused { get { return _paused; } set { _paused = value; } }
}
