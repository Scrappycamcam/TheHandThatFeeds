using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInteraction : MonoBehaviour {
    public string _ItemText;
    public bool _ItemDisplay;
    private float _maxDist;
    public Transform _PlayerPos;
    public int _playerId = 0;

    private Player _player;

	// Use this for initialization
	void Start () {
        _ItemText = "Hello World";
        _player = ReInput.players.GetPlayer(_playerId);
    }
	
	// Update is called once per frame
	void Update () {
        if (_player.GetButtonDown("Interact"))
        {
            ShootRay();
        }
    }

    private void OnGUI()
    {
        if (_ItemDisplay)
            DisplayInfo(_ItemText);
    }



    private void OnTriggerExit(Collider other)
    {
        if (_ItemDisplay)
        {
            _ItemDisplay = false;
        }
    }

    private void ShootRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(_PlayerPos.transform.position, _PlayerPos.transform.forward, out hit, _maxDist))
        {
            Debug.Log("Hit" + hit.transform.position);
            if(hit.transform.tag == "Sign")
            {
                _ItemDisplay = true;
            }
            else
            {
                _ItemDisplay = false;
            }
        }
    }

    private void DisplayInfo(string Text)
    {
        GUI.Box(new Rect(10, 10, 400, 200), Text);
        
    }
}
