using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour {

    [SerializeField]
    float _vertOffsetPercentage;
    private Transform player;
    private Vector3 _playerPos;

    Camera _playerCam;

    private void Start()
    {
        _playerCam = FindObjectOfType<PlayerCamera>().gameObject.GetComponent<Camera>();
        player = KyleplayerMove.Instance.transform;
        _vertOffsetPercentage /= 100f;
    }

    // Update is called once per frame
    void Update () {

        float _actualOffset = _vertOffsetPercentage * Screen.height;

        _playerPos = _playerCam.WorldToScreenPoint(player.transform.position);
        transform.position = _playerPos + (Vector3.up * _actualOffset);
	}
}
