using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ResetDelegate();
public class PlayerCanvas : MonoBehaviour {

    private static PlayerCanvas _instance;
    public static PlayerCanvas Instance
    {
        get
        {
            if (_instance != null)
            {

                return _instance;
            }
            else
            {
                if (FindObjectOfType<PlayerCanvas>())
                {
                    _instance = FindObjectOfType<PlayerCanvas>();
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

    [SerializeField]
    int _canvasObjectsToKeep;
    List<GameObject> _baseCanvasObjects;
    KyleplayerMove _playerRef;


    private ResetDelegate Reset;
    public ResetDelegate SetGameReset { get { return Reset; } set { Reset = value; } }

    private void Awake()
    {
        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _playerRef = KyleplayerMove.Instance;
        Reset = _playerRef.ResetPlayer;
    }

    public void WipeCanvas()
    {
        for (int i = transform.childCount - 1; i >= _canvasObjectsToKeep; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void ResetGame()
    {
        if (Reset != null)
        {
            Reset.Invoke();
        }
    }
}
