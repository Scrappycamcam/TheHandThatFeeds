using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

    public Attack _BaseAttack;
    private Attack _LastAttack;
    private Attack _NextAttack;

	// Use this for initialization
	void Start () {
        _LastAttack = _BaseAttack;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
