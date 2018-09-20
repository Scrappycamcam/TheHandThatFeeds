using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInteraction : MonoBehaviour {
    public string _ItemText;
    public bool _ItemDisplay;
    public int _playerId = 0;

    private Player _player;

	// Use this for initialization
	void Start () {
        _ItemText = "Hello World";
        _player = ReInput.players.GetPlayer(_playerId);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if (_ItemDisplay)
            DisplayInfo(_ItemText);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("In Zone");
        //if (Input.GetButton("Pickup"))
        if (_player.GetButtonDown("Interact"))
        {
            _ItemDisplay = true;
            //Pick Up or interact with Item
            Debug.Log("Picked up Item");
          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_ItemDisplay)
        {
            _ItemDisplay = false;
        }
    }

    private void DisplayInfo(string Text)
    {
        GUI.Box(new Rect(10, 10, 400, 200), Text);
        
    }
}
