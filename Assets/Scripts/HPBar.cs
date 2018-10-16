using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour {

    [SerializeField]
    float _vertOffset;
    private Transform player;
    private Vector3 _playerPos;

    Camera _playerCam;

    private void Start()
    {
        _playerCam = FindObjectOfType<camera>().gameObject.GetComponent<Camera>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update () {
        _playerPos = _playerCam.WorldToScreenPoint(player.transform.position);
        transform.position = _playerPos + (Vector3.up * _vertOffset);
	}
}
