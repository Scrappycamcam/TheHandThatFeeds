using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float camTime;
    private bool _following;

    private Vector3 _newPos;
    private Vector3 velocity = Vector3.zero;

    public void Start()
    {
        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _following = true;
        _p = KyleplayerMove.Instance.transform;
    }

    private void Update()
    {
        if(_following)
        {
            FollowPlayer();
        }
        else
        {
            HitCamera();
        }
    }

    private void FollowPlayer()
    {
        _newPos = new Vector3(_p.position.x, transform.position.y, _p.position.z - _offset);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref velocity, camTime, maxSpeed);
    }

    private void HitCamera()
    {

    }
}
