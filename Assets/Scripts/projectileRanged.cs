using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileRanged : MonoBehaviour {

    public float _speed;
    public float _damage;
    public float _maxRange;

    private float _curDist;
    private Vector3 _startPos;
    private float _damageDistance = .5f;

    private void Awake()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _damageDistance))
        {
            //Debug.Log(hit);
            if (!hit.collider.GetComponent<AIEnemy>())
            {
                Debug.Log("Hit Something");
                if (hit.collider.GetComponent<PlayerStats>())
                {
                    Debug.Log("Hit Player");
                    hit.collider.gameObject.GetComponent<PlayerStats>().PDamage(_damage);
                    Destroy(gameObject);
                }
                if (!hit.collider.GetComponent<ProgressionLighting>() && !hit.collider.GetComponent<projectileRanged>())
                {
                    Destroy(gameObject);
                }
            }
        }
        transform.position += transform.forward * _speed * Time.deltaTime;
        if(Vector3.Distance(_startPos, transform.position) >= _maxRange)
        {
            Destroy(gameObject);
        }
    }
}
