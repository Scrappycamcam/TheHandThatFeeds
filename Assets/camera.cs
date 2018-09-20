using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {

    public Transform _p;
    public float _offset;
    public float maxSpeed;

    private Vector3 _newPos;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        _newPos = new Vector3(_p.position.x, transform.position.y, _p.position.z - _offset);
        transform.position = Vector3.SmoothDamp(transform.position, _newPos, ref velocity, 0.5f, maxSpeed);
    }
}
