using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOT : MonoBehaviour {

    public float _DamagePerTick = 1;

    private float _DestroyTime;

    private void Start()
    {
        _DestroyTime = Time.time + 5f;
    }

    private void Update()
    {
       if(_DestroyTime <= Time.time)
        {
            Destroy(gameObject);
        } 
    }
}
