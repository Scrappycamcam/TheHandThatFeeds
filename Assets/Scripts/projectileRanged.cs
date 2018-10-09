using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileRanged : MonoBehaviour {

    public Vector3 _direction;
    public float _speed;
    public float _damage;
    public float _maxRange;

    private float _curDist;
    private Vector3 _lastPos;

    private void Start()
    {
        _lastPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _direction, _speed);
        if(_curDist >= _maxRange)
        {
            Destroy(this.gameObject);
        }
        _curDist += Vector3.Distance(_lastPos, transform.position);
        _lastPos = transform.position;
    }
}
