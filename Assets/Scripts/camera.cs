using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {

    [SerializeField]
    private Transform _p;
    [SerializeField]
    private float _offset;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float camTime;

    private Vector3 _newPos;
    private Vector3 velocity = Vector3.zero;

    public void Awake()
    {
        _p = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        _newPos = new Vector3(_p.position.x, transform.position.y, _p.position.z - _offset);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref velocity, camTime, maxSpeed);
    }
}
