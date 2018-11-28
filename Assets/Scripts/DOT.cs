using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOT : MonoBehaviour {

    public float _DamagePerTick = 1f;
    public float _TotalDuration = 5f;
    public float _WindUpDuration = 1f;

    private float _dam;
    private float _DestroyTime;
    private float _FinishTime;

    private bool _doneActivating = false;

    private Color _baseColor;

    private void Start()
    {
        _DestroyTime = Time.time + _TotalDuration;
        _FinishTime = Time.time + _WindUpDuration;
        _baseColor = GetComponent<Renderer>().material.color;
        _dam = _DamagePerTick;
        _DamagePerTick = 0;
    }

    private void Update()
    {
       if(_DestroyTime <= Time.time)
        {
            Destroy(gameObject);
        }
        else if( _FinishTime <= Time.time)
        {
            _DamagePerTick = _dam;
            _doneActivating = true;
        }
        else if(!_doneActivating)
        {
            GetComponent<Renderer>().material.color = new Color(_baseColor.r, _baseColor.g, _baseColor.g, 1 + (Time.time - _FinishTime));
        }
    }
}
