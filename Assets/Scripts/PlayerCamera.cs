using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraSetting
{
    NONE,
    FOLLOW,
    ACTION
}


public class PlayerCamera : MonoBehaviour {

   

    private static PlayerCamera _instance;
    public static PlayerCamera Instance
    {
        get
        {
            if (_instance != null)
            {

                return _instance;
            }
            else
            {
                if (FindObjectOfType<PlayerCamera>())
                {
                    _instance = FindObjectOfType<PlayerCamera>();
                    return _instance;
                }
                else
                {
                    Debug.Log("no Player");
                    return null;
                }
            }
        }
    }

    [SerializeField]
    private Transform _p;
    
    [SerializeField]
    private float _offset;
    private float _vertOffset;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float camTime;

    private Vector3 _newPos;
    private Vector3 velocity = Vector3.zero;
    [SerializeField]
    float _zoomInOffset;
    [SerializeField]
    float _divisibleOffsetLimit;
    float _currZoomOffset;
    [SerializeField]
    float _zoomInIncrement;
    Vector3 c0, c1;
    float _currZoomTime;
    float _startZoomTime;
    [SerializeField]
    private float _zoomDuration;
    [Tooltip("must be a float from 0 to 1, changes how fast time moves in unity during the camera zoom")]
    [SerializeField]
    private float _slowDownChange;

    CameraSetting _mySetting = CameraSetting.NONE;

    public void Start()
    {
        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            PlayerCamera.Instance.gameObject.transform.position = transform.position;
            Destroy(gameObject);
        }

        _p = KyleplayerMove.Instance.transform;
        _vertOffset = transform.position.y;
        _mySetting = CameraSetting.FOLLOW;
    }

    private void Update()
    {
        switch (_mySetting)
        {
            case CameraSetting.NONE:
                break;
            case CameraSetting.FOLLOW:
                FollowPlayer();
                break;
            case CameraSetting.ACTION:
                HitCamera();
                break;
            default:
                break;
        }
    }

    private void FollowPlayer()
    {
        _newPos = new Vector3(_p.position.x, _vertOffset, _p.position.z - _offset);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref velocity, camTime, maxSpeed);
    }

    public void StartHitCamera(Vector3 _HitPos, float _comboNum)
    {

        c0 = transform.position;
        c1 = _HitPos;
        _currZoomOffset = _zoomInOffset - (_comboNum * _zoomInIncrement);
        if (_currZoomOffset <= _zoomInOffset / _divisibleOffsetLimit) 
        {
            _currZoomOffset = _zoomInOffset / _divisibleOffsetLimit;
        }
        c1.y = _HitPos.y + _currZoomOffset;
        c1.z = _HitPos.z - _currZoomOffset;

        Time.timeScale = _slowDownChange;

        _startZoomTime = Time.time;

        _mySetting = CameraSetting.ACTION;
    }

    private void HitCamera()
    {
        _currZoomTime = (Time.time - _startZoomTime) / _zoomDuration;

        Vector3 c01;

        c01 = (1 - _currZoomTime) * c0 + _currZoomTime * c1;

        transform.position = c01;

        if (_currZoomTime >= 1)
        {
            _currZoomTime = 1f;

            Time.timeScale = 1f;
            _mySetting = CameraSetting.FOLLOW;
        }

    }
}
