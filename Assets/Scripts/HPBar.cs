using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour {

    private Transform player;
    private Vector3 diff;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        diff = transform.position - player.position;
    }

    // Update is called once per frame
    void Update () {
        transform.position = player.position + diff;
	}
}
