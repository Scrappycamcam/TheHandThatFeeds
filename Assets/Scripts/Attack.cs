using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public Attack[] _nextAttacks;
    public bool _isHeavy;
    public bool _hasHit;

    public bool MakeAttack(Vector3 _dir)
    {
        attack(_dir);
        return _hasHit;
    }

    private void attack(Vector3 dir)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -dir, out hit, 3f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            _hasHit = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            Debug.Log("Did not Hit");
            _hasHit = false;
        }
    }
}
